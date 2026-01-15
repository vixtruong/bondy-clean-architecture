using Identity.Domain.ValueObjects;
using Bondy.SharedKernel.Common;

namespace Identity.Domain.Entities;

public sealed class ApiKey : AggregateRoot
{
    public string KeyId { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public HashedValue KeyHash { get; private set; } = default!;

    public string Owner { get; private set; } = default!;

    public string? AllowedPaths { get; private set; }

    public int? RateLimitPlanId { get; private set; }

    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Scope> _scopes = new();
    public IReadOnlyCollection<Scope> Scopes => _scopes;

    private ApiKey() { } // EF
    public ApiKey(
        string keyId,
        string name,
        HashedValue keyHash,
        string owner,
        IEnumerable<Scope> scopes,
        string? allowedPaths,
        int? rateLimitPlanId,
        DateTime? expiresAt,
        DateTime createdAt)
    {
        KeyId = keyId;
        Name = name;
        KeyHash = keyHash;
        Owner = owner;
        AllowedPaths = allowedPaths;
        RateLimitPlanId = rateLimitPlanId;
        ExpiresAt = expiresAt;
        IsActive = true;
        CreatedAt = createdAt;

        _scopes.AddRange(scopes);
    }

    public bool IsExpired(DateTime now)
        => ExpiresAt.HasValue && now >= ExpiresAt.Value;

    public bool HasScope(string scope)
        => _scopes.Any(s => s.Value == scope);

    public bool IsPathAllowed(string path)
    {
        if (string.IsNullOrWhiteSpace(AllowedPaths)) return true;

        // ví dụ: regex list ngăn cách bởi ;
        return AllowedPaths
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Any(p => System.Text.RegularExpressions.Regex.IsMatch(path, p));
    }

    public void Disable(DateTime utcNow)
    {
        IsActive = false;
        UpdatedAt = utcNow;
    }

    public void Touch(DateTime utcNow)
    {
        UpdatedAt = utcNow; // dùng làm LastUsed
    }
}
