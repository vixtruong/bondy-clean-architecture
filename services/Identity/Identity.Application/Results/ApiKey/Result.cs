namespace Identity.Application.Results.ApiKey;

/// <summary>
/// Returned ONLY ONCE when creating or rotating an API key.
/// </summary>
public sealed record ApiKeyCreatedResult(
    long Id,
    string Name,
    string RawApiKey,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

/// <summary>
/// API key metadata for listing / management.
/// </summary>
public sealed record ApiKeyResult(
    long Id,
    string Name,
    string KeyPrefix,
    bool IsActive,
    IReadOnlyList<string> Scopes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? LastUsedAt
);

public sealed record ApiKeyValidationResult(
    long Id,
    string Name,
    string Owner,
    IReadOnlyList<string> Scopes,
    string? AllowedPaths,
    bool IsActive,
    DateTimeOffset? ExpiresAt
);
