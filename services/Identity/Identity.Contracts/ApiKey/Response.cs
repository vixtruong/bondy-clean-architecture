namespace Identity.Contracts.ApiKey;


/// <summary>
/// Returned ONLY ONCE when creating or rotating an API key.
/// </summary>
public sealed record ApiKeyCreatedResponse(
    long Id,
    string Name,
    string RawApiKey,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

/// <summary>
/// API key metadata for listing / management.
/// </summary>
public sealed record ApiKeyResponse(
    long Id,
    string Name,
    string KeyPrefix,
    bool IsActive,
    IReadOnlyList<string> Scopes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? LastUsedAt
);