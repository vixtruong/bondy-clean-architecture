using Bondy.SharedKernel.Application.Authorization.Role;
using Bondy.SharedKernel.Domain.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence.Seed;

public sealed class RoleSeeder
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<RoleSeeder> _logger;
    private readonly IClock _clock;

    public RoleSeeder(
        IdentityDbContext db,
        ILogger<RoleSeeder> logger, IClock clock)
    {
        _db = db;
        _logger = logger;
        _clock = clock;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        foreach (var def in RoleDefinitions.All)
        {
            await SeedRoleAsync(def, ct);
        }
    }

    private async Task SeedRoleAsync(RoleDefinition def, CancellationToken ct)
    {
        var now = _clock.Now;

        var role = await _db.Roles
            .Include(r => r.Scopes)
            .FirstOrDefaultAsync(r => r.Code == def.Code, ct);

        if (role == null)
        {
            role = new Role(def.Code,def.Name, createdAt: now);

            foreach (var scope in def.Scopes.Distinct())
            {
                role.AddScope(new Scope(scope));
            }

            _db.Roles.Add(role);
            await SaveSafeAsync(def.Code, ct);

            _logger.LogInformation("Created role {Role}", def.Code);
            return;
        }

        // role exists → add missing scopes only
        var existingScopes = role.Scopes
            .Select(s => s.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var added = 0;
        foreach (var scope in def.Scopes.Distinct())
        {
            if (existingScopes.Add(scope))
            {
                role.AddScope(new Scope(scope));
                added++;
            }
        }

        if (added > 0)
        {
            await SaveSafeAsync(def.Code, ct);
            _logger.LogInformation(
                "Updated role {Role}, added {Count} scopes",
                def.Code, added);
        }
    }

    private async Task SaveSafeAsync(string roleCode, CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            // race-condition safe (multi-instance)
            _logger.LogWarning(
                ex,
                "Seeding role {Role} failed (possible concurrent insert)",
                roleCode);
        }
    }
}
