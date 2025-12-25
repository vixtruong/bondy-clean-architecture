using Mail.Domain.Entities;

namespace Mail.Application.Abstractions.Repositories;

public interface IMailRepository
{
    Task AddAsync(EmailLog log);
    Task UpdateAsync(EmailLog log);

    Task<EmailLog?> GetByIdAsync(long id);
}