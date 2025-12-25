using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class ApiKey : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public HashedValue KeyHash { get; private set; } = default!;
    public string Prefix { get; private set; } = default!;

    public DateTime? ExpiresAt { get; private set; }
    public bool Active { get; private set; } = true;

    private ApiKey() { }

    public ApiKey(
        string name,
        HashedValue keyHash,
        string prefix,
        DateTime? expiresAt,
        DateTime createdAt)
    {
        Name = name;
        KeyHash = keyHash;
        Prefix = prefix;
        ExpiresAt = expiresAt;
        Active = true;

        // base dùng DateTime -> lưu UTC
        CreatedAt = createdAt;
    }

    public bool IsExpired(DateTime now)
        => ExpiresAt.HasValue && now >= ExpiresAt.Value;

    public void Disable(DateTime now)
    {
        Active = false;
    }
}