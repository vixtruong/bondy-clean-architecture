using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class Account : AggregateRoot
{
    public long UserId { get; private set; }
    public AuthProvider Provider { get; private set; } = AuthProvider.Local;

    // social login có thể null
    public HashedValue? PasswordHash { get; private set; }

    public User User { get; private set; } = default!;

    private Account() { }

    public Account(AuthProvider provider, HashedValue? passwordHash, DateTime createdAtUtc)
    {
        Provider = provider;
        PasswordHash = passwordHash;
        CreatedAt = createdAtUtc;
    }

    public void SetPasswordHash(HashedValue? passwordHash, DateTime utcNow)
    {
        PasswordHash = passwordHash;
        UpdatedAt = utcNow;
    }
}