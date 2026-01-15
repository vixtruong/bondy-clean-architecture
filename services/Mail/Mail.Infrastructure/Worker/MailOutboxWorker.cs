using Mail.Application.Services.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mail.Infrastructure.Worker;

public sealed class MailOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MailOutboxWorker> _logger;

    public MailOutboxWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<MailOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Mail outbox worker started");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dispatcher = scope.ServiceProvider
                    .GetRequiredService<MailDispatchService>();

                await dispatcher.DispatchAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mail outbox worker crashed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }
}