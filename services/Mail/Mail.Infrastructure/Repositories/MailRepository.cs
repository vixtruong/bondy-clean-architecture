using Mail.Application.Abstractions.Persistence;
using Mail.Application.Abstractions.Repositories;
using Mail.Domain.Entities;
using Mail.Domain.Enums;
using Mail.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Mail.Infrastructure.Repositories;

public class MailRepository : RepositoryBase, IMailRepository
{
    public MailRepository(IMailDbContext db) : base(db)
    {
    }

    public async Task AddAsync(
        EmailOutbox outbox)
    {
        _db.EmailOutboxes.Add(outbox);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<EmailOutbox>> GetSendableAsync(
        int limit,
        DateTime now)
    {
        return await _db.EmailOutboxes
            .Where(x =>
                (x.Status == MailStatus.Pending || x.Status == MailStatus.Failed) &&
                (x.LastAttemptAt == null ||
                 x.LastAttemptAt < now.AddSeconds(-30)) &&
                x.AttemptCount < 5)
            .OrderBy(x => x.CreatedAt)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<bool> TryMarkSendingAsync(long outboxId, DateTime now)
    {
        var affected = await _db.EmailOutboxes
            .Where(x =>
                x.Id == outboxId &&
                (x.Status == MailStatus.Pending || x.Status == MailStatus.Failed))
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, MailStatus.Sending)
                    .SetProperty(x => x.AttemptCount, x => x.AttemptCount + 1)
                    .SetProperty(x => x.LastAttemptAt, now)
            );

        return affected == 1;
    }

    public async Task MarkSentAsync(long outboxId, string providerId, DateTime now)
    {
        await _db.EmailOutboxes
            .Where(x => x.Id == outboxId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, MailStatus.Sent)
                    .SetProperty(x => x.SentAt, now)
            );
    }

    public async Task MarkFailedAsync(long outboxId, string? error)
    {
        await _db.EmailOutboxes
            .Where(x => x.Id == outboxId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, MailStatus.Failed)
            );
    }
}
