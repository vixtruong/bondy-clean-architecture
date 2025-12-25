using Mail.Application.Abstractions.Templating;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Mail.Infrastructure.Templating;

public sealed class EmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<SmtpOptions> opt, ILogger<EmailSender> logger)
    {
        _opt = opt.Value;
        _logger = logger;
    }

    public async Task SendHtmlAsync(string to, string subject, string html)
    {
        if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("To is required.", nameof(to));
        if (string.IsNullOrWhiteSpace(subject)) subject = "(No subject)";
        if (html is null) html = string.Empty;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_opt.FromName, _opt.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var client = new SmtpClient();

        // (optional) nếu server dùng cert tự ký dev
        // client.ServerCertificateValidationCallback = (_, _, _, _) => true;

        var socket = _opt.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : (_opt.UseStartTls ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None);

        await client.ConnectAsync(_opt.Host, _opt.Port, socket);

        if (!string.IsNullOrWhiteSpace(_opt.Username))
            await client.AuthenticateAsync(_opt.Username, _opt.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("SMTP sent email to {To} subject {Subject}", to, subject);
    }
}