namespace Bondy.Contracts.Dtos.ApiKey;

public sealed record ApiKeyValidationResult(
    long Id,
    string Name,
    string Owner,
    IReadOnlyList<string> Scopes,
    string? AllowedPaths,
    bool IsActive,
    DateTimeOffset? ExpiresAt
);
