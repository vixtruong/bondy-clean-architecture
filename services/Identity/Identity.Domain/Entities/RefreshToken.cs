using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class RefreshToken : AggregateRoot
{
    public long UserId { get; private set; }

    public HashedValue TokenHash { get; private set; } = default!;

    public bool Revoked { get; private set; } = false;

    public DateTime? RevokedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public User User { get; private set; } = default!;

    public string SessionId { get; private set; } = default!;

    private RefreshToken() { }

    public RefreshToken(long userId, string sessionId, HashedValue tokenHash, DateTime expiresAtUtc, DateTime createdAtUtc)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAtUtc;
        CreatedAt = createdAtUtc;
        Revoked = false;
        SessionId = sessionId;
    }

    public bool IsActive(DateTime utcNow) =>
        !Revoked && RevokedAt == null && ExpiresAt > utcNow;

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public void Revoke(DateTime now)
    {
        if (Revoked) return;

        Revoked = true;
        RevokedAt = now;
    }
}