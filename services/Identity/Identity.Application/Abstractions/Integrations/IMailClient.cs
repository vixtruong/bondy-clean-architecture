using Bondy.SharedKernel.Application.Commands;
using Identity.Application.Results.Mail;

namespace Identity.Application.Abstractions.Integrations;

public interface IMailClient
{
    Task<MailSendResult> SendEmailAsync(SendEmailCommand command);
}
