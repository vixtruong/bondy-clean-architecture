using Bondy.Contracts.Dtos.Mail;
using Bondy.SharedKernel.Domain.Common;

namespace Mail.Application.Services.Mail;

public interface IMailService
{
    Task<Result> SendEmail(SendEmailDto dto);
}
