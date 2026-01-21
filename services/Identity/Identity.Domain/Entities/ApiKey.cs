using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Domain.Common;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public sealed class ApiKey : AggregateRoot
{
    public string KeyId { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string KeyPrefix { get; private set; } = default!;

    public HashedValue KeyHash { get; private set; } = default!;

    public string Owner { get; private set; } = default!;

    public Email OwnerEmail { get; private set; } = default!;

    public string? AllowedPaths { get; private set; }

    public int? RateLimitPlanId { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Scope> _scopes = new();
    public IReadOnlyCollection<Scope> Scopes => _scopes;
    
    public DateTimeOffset? RotateAt { get; private set; }
    public DateTimeOffset? RevokeAt { get; private set; }

    public DateTime? LastUsed => UpdatedAt;

    public ApiKeyRevokeReason? RevokeReason { get; private set; }

    private ApiKey() { } // EF
    public ApiKey(
        string keyId,
        string name,
        string keyPrefix,
        HashedValue keyHash,
        string owner,
        Email ownerEmail,
        IEnumerable<Scope> scopes,
        string? allowedPaths,
        int? rateLimitPlanId,
        DateTimeOffset? expiresAt,
        DateTime createdAt)
    {
        KeyId = keyId;
        Name = name;
        KeyPrefix = keyPrefix;
        KeyHash = keyHash;
        Owner = owner;
        OwnerEmail = ownerEmail;
        AllowedPaths = allowedPaths;
        RateLimitPlanId = rateLimitPlanId;
        ExpiresAt = expiresAt;
        IsActive = true;
        CreatedAt = createdAt;

        _scopes.AddRange(scopes);
    }

    public bool IsExpired(DateTimeOffset now)
        => ExpiresAt.HasValue && now >= ExpiresAt.Value;

    public bool HasScope(string scope)
        => _scopes.Any(s => s.Value == scope);

    public bool IsPathAllowed(string path)
    {
        if (string.IsNullOrWhiteSpace(AllowedPaths)) return true;

        return AllowedPaths
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Any(p => System.Text.RegularExpressions.Regex.IsMatch(path, p));
    }

    public void Disable(DateTime utcNow)
    {
        IsActive = false;
        UpdatedAt = utcNow;
    }

    public void Touch(DateTimeOffset utcNow)
    {
        UpdatedAt = utcNow.DateTime;
    }

    public void Revoke(ApiKeyRevokeReason reason, DateTimeOffset utcNow)
    {
        if (!IsActive)
            return;

        IsActive = false;
        RevokeAt = utcNow;
        RevokeReason = reason;
        UpdatedAt = utcNow.DateTime;
    }

    public void Rotate(DateTimeOffset utcNow, TimeSpan gracePeriod)
    {
        RotateAt = utcNow.Add(gracePeriod);
        UpdatedAt = utcNow.DateTime;
    }
}
