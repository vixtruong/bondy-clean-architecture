using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.ApiKey;

public sealed record CreateApiKeyRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = default!;

    [Required]
    [StringLength(100)]
    public string Owner { get; init; } = default!;

    [Required]
    [EmailAddress]
    public string OwnerEmail { get; init; } = default!;

    [Required]
    [MinLength(1)]
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();

    [DataType(DataType.DateTime)]
    public DateTimeOffset? ExpiresAt { get; init; }
}

public sealed record RotateApiKeyRequest
{
    [Range(1, long.MaxValue)]
    public long ApiKeyId { get; init; }
}

public sealed record UpdateApiKeyRequest
{
    [Required]
    public long ApiKeyId { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = default!;

    [Required]
    [MinLength(1)]
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();

    [DataType(DataType.DateTime)]
    public DateTimeOffset? ExpiresAt { get; init; }

    public bool? IsActive { get; init; }
}