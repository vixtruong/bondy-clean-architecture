using Bondy.SharedKernel.Application.Commands;
using Bondy.SharedKernel.Domain.Common;

namespace Mail.Application.Services.Mail;

public interface IMailService
{
    Task<Result> SendEmail(string to, EmailPurpose purpose, Dictionary<string, string> data, string? dedupTokenId);
}