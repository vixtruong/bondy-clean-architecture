using System.ComponentModel.DataAnnotations;

namespace Bondy.Contracts.Dtos.ApiKey;

public sealed record ValidateApiKeyRequest
{
    [Required]
    public required string ApiKey { get; init; }
}