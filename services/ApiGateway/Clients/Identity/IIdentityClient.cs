using Bondy.Contracts.Dtos.ApiKey;

namespace ApiGateway.Clients.Identity;

public interface IIdentityClient
{
    Task<Result<ApiKeyValidationResult>> ValidateApiKeyAsync(
        string rawApiKey,
        CancellationToken ct = default);
}