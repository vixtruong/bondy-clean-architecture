using Mail.Application.Abstractions.Persistence.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mail.Infrastructure.Persistence.Migrations;

public sealed class MigrationHostedService : IHostedService
{
    private readonly IServiceProvider _sp;

    public MigrationHostedService(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();
        await migrator.MigrateAsync(ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}