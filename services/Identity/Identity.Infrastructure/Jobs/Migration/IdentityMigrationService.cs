using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Jobs.Migration;

public class IdentityMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentityMigrationService> _logger;

    public IdentityMigrationService(
        IServiceProvider serviceProvider,
        ILogger<IdentityMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Identity database migration...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

            _logger.LogInformation("Applying pending migrations for IdentityDbContext...");
            await dbContext.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Identity database migration completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Identity database migration failed.");
            throw; // cho app crash để k8s / docker restart
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("IdentityMigrationService stopped.");
        return Task.CompletedTask;
    }
}