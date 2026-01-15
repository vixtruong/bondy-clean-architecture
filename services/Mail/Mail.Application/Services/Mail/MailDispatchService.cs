using Bondy.SharedKernel.Abstractions;
using Mail.Application.Abstractions.Repositories;
using Mail.Application.Abstractions.Templating;
using Mail.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Mail.Application.Services.Mail;

public sealed class MailDispatchService
{
    private readonly IMailRepository _mail;
    private readonly IEmailSender _sender;
    private readonly IClock _clock;
    private readonly ILogger<MailDispatchService> _logger;

    public MailDispatchService(
        IMailRepository mail,
        IEmailSender sender,
        IClock clock,
        ILogger<MailDispatchService> logger)
    {
        _mail = mail;
        _sender = sender;
        _clock = clock;
        _logger = logger;
    }

    public async Task DispatchAsync(CancellationToken ct)
    {
        var now = _clock.Now;

        var emails = await _mail.GetSendableAsync(
            limit: 50,
            now: now);

        foreach (var email in emails)
        {
            if (ct.IsCancellationRequested)
                break;

            await ProcessOneAsync(email, ct);
        }
    }

    private async Task ProcessOneAsync(
        EmailOutbox email,
        CancellationToken ct)
    {
        var now = _clock.Now;

        var locked = await _mail.TryMarkSendingAsync(
            email.Id,
            now);

        if (!locked)
        {
            _logger.LogDebug(
                "Email {Id} skipped (picked by another worker)",
                email.Id);
            return;
        }

        try
        {
            await _sender.SendHtmlAsync(
                email.To.Value,
                email.Subject,
                email.Html!);

            await _mail.MarkSentAsync(
                email.Id,
                providerId: "smtp",
                now: _clock.Now);

            _logger.LogInformation(
                "Email sent. OutboxId={Id}, To={To}",
                email.Id,
                email.To.Value);
        }
        catch (Exception ex)
        {
            await _mail.MarkFailedAsync(
                email.Id,
                ex.Message);

            _logger.LogError(
                ex,
                "Send email failed. OutboxId={Id}, To={To}",
                email.Id,
                email.To.Value);
        }
    }
}
