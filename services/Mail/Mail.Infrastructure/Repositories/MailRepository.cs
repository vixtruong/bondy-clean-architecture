using Mail.Application.Abstractions.Persistence;
using Mail.Application.Abstractions.Repositories;
using Mail.Domain.Entities;
using Mail.Infrastructure.Repositories.Base;

namespace Mail.Infrastructure.Repositories;

public class MailRepository : RepositoryBase, IMailRepository
{
    public MailRepository(IMailDbContext db) : base(db)
    {
    }

    public async Task AddAsync(EmailLog log)
    {
        _db.EmailLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmailLog log)
    {
        _db.EmailLogs.Update(log);
        await _db.SaveChangesAsync();
    }

    public async Task<EmailLog?> GetByIdAsync(long id)
    {
        return await _db.EmailLogs.FindAsync(id);
    }
}
