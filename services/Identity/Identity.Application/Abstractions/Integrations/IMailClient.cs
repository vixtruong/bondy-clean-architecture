using Bondy.Contracts.Dtos.Mail;
using Identity.Contracts.Mail;

namespace Identity.Application.Abstractions.Integrations;

public interface IMailClient
{
    Task<MailSendResult> SendEmailAsync(SendEmailDto dto);
}
