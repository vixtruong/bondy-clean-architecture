using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Contracts.ApiKey;

public class ValidateApiKeyRequest
{
    [Required]
    public string ApiKey { get; set; } = null!;
}
