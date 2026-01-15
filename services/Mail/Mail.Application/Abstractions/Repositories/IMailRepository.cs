using Mail.Domain.Entities;

namespace Mail.Application.Abstractions.Repositories;

public interface IMailRepository
{
    Task AddAsync(
        EmailOutbox outbox);

    Task<IReadOnlyList<EmailOutbox>> GetSendableAsync(
        int limit,
        DateTime now);

    Task<bool> TryMarkSendingAsync(
        long outboxId,
        DateTime now);

    Task MarkSentAsync(
        long outboxId,
        string providerId,
        DateTime now);

    Task MarkFailedAsync(
        long outboxId,
        string? error);
}
