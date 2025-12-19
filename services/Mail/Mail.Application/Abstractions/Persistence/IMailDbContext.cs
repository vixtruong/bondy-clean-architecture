using Mail.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mail.Application.Abstractions.Persistence
{
    public interface IMailDbContext
    {
        DbSet<EmailLog> EmailLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
