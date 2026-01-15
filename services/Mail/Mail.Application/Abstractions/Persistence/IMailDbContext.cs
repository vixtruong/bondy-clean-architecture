using Mail.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mail.Application.Abstractions.Persistence
{
    public interface IMailDbContext
    {
        DbSet<EmailOutbox> EmailOutboxes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
