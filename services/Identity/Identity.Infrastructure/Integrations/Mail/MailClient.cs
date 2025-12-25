using System.Net.Http.Json;
using Bondy.Contracts.Dtos.Mail;
using Identity.Application.Abstractions.Integrations;
using Identity.Contracts.Mail;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Integrations.Mail;

public sealed class MailClient : IMailClient
{
    private readonly HttpClient _http;
    private readonly ILogger<MailClient> _logger;

    public MailClient(HttpClient http, ILogger<MailClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<MailSendResult> SendEmailAsync(SendEmailDto dto)
    {
        try
        {
            using var resp = await _http.PostAsJsonAsync("/api/v1/mail/send", dto);
            var body = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
                return new MailSendResult(true, (int)resp.StatusCode, body);

            _logger.LogWarning("Mail send failed. Status={Status} Body={Body}", (int)resp.StatusCode, body);
            return new MailSendResult(false, (int)resp.StatusCode, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail send exception");
            return new MailSendResult(false, 0, ex.Message);
        }
    }
}