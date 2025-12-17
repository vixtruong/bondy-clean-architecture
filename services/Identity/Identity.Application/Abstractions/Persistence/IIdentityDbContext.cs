using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Abstractions.Persistence;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    DbSet<PreRegistration> PreRegistrations { get; }
    DbSet<Account> Accounts { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<OtpCode> OtpCodes { get; }
    DbSet<ApiKey> ApiKeys { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}