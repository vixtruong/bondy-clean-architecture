using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using Mail.Application.Abstractions.Persistence.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mail.Infrastructure.Persistence.Migrations;

public sealed class SqlFileMigrator : IDbMigrator
{
    private readonly DbConnection _connection;
    private readonly ILogger<SqlFileMigrator> _logger;
    private readonly string _migrationsPath;

    public SqlFileMigrator(DbConnection connection,
        IConfiguration config,
        ILogger<SqlFileMigrator> logger)
    {
        _connection = connection;
        _logger = logger;
        _migrationsPath = config["Migrations:SqlPath"]
                          ?? throw new InvalidOperationException("Missing config Migrations:SqlPath");
    }

    public async Task MigrateAsync(CancellationToken ct)
    {
        var files = Directory.GetFiles(_migrationsPath, "V*__*.sql")
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!files.Any())
        {
            _logger.LogInformation("No SQL migration files found at {Path}", _migrationsPath);
            return;
        }

        await _connection.OpenAsync(ct);

        await EnsureMigrationsTableAsync(ct);

        var applied = await GetAppliedAsync(ct); // version -> checksum

        foreach (var file in files)
        {
            var name = Path.GetFileName(file)!;
            var (version, desc) = Parse(name);

            var sql = await File.ReadAllTextAsync(file, ct);
            var checksum = Sha256(sql);

            if (applied.TryGetValue(version, out var oldChecksum))
            {
                // Đã chạy rồi: nếu checksum khác => có người sửa file cũ (nguy hiểm)
                if (!string.Equals(oldChecksum, checksum, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(
                        $"Migration '{name}' was already applied but checksum changed. Do NOT edit applied migrations.");

                _logger.LogInformation("Skip migration {Version} ({File}) - already applied", version, name);
                continue;
            }

            _logger.LogInformation("Applying migration {Version} ({File})", version, name);

            // Chạy trong transaction để fail là rollback
            await using var tx = await _connection.BeginTransactionAsync(ct);
            try
            {
                await ExecuteSqlAsync(sql, tx, ct);
                await InsertAppliedAsync(version, desc, checksum, tx, ct);

                await tx.CommitAsync(ct);
                _logger.LogInformation("Applied migration {Version}", version);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }

    private async Task EnsureMigrationsTableAsync(CancellationToken ct)
    {
        const string create = @"
CREATE TABLE IF NOT EXISTS schema_migrations (
  version        VARCHAR(50) PRIMARY KEY,
  description    TEXT NOT NULL,
  checksum       VARCHAR(64) NOT NULL,
  applied_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);";
        await ExecuteSqlAsync(create, transaction: null, ct);
    }

    private async Task<Dictionary<string, string>> GetAppliedAsync(CancellationToken ct)
    {
        const string q = "SELECT version, checksum FROM schema_migrations;";
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = q;

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result[reader.GetString(0)] = reader.GetString(1);
        }
        return result;
    }

    private async Task InsertAppliedAsync(string version, string desc, string checksum, DbTransaction tx, CancellationToken ct)
    {
        const string ins = @"
INSERT INTO schema_migrations(version, description, checksum, applied_at)
VALUES (@v, @d, @c, now());";

        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = ins;

        var p1 = cmd.CreateParameter(); p1.ParameterName = "@v"; p1.Value = version;
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@d"; p2.Value = desc;
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@c"; p3.Value = checksum;
        cmd.Parameters.Add(p1); cmd.Parameters.Add(p2); cmd.Parameters.Add(p3);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task ExecuteSqlAsync(string sql, DbTransaction? transaction, CancellationToken ct)
    {
        // nếu bạn muốn support GO (SQL Server) thì cần split theo batch; Postgres không cần
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sql;
        cmd.CommandTimeout = 120;
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static (string version, string desc) Parse(string fileName)
    {
        // V0001__init.sql
        var parts = fileName.Split("__", 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) throw new ArgumentException($"Invalid migration file name: {fileName}");
        var version = parts[0]; // V0001
        var desc = Path.GetFileNameWithoutExtension(parts[1]).Replace('_', ' ');
        return (version, desc);
    }

    private static string Sha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}