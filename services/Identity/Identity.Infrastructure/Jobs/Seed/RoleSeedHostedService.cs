using Identity.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Jobs.Seed;

public sealed class RoleSeedHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<RoleSeedHostedService> _logger;

    public RoleSeedHostedService(
        IServiceProvider sp,
        ILogger<RoleSeedHostedService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();

        await seeder.SeedAsync(ct);
        _logger.LogInformation("Role seeding completed");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}