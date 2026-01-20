using Bondy.Contracts.Dtos.ApiKey;
using Bondy.SharedKernel.Common;

namespace ApiGateway.Clients.Identity;

public interface IIdentityClient
{
    Task<Result<ApiKeyValidationResult>> ValidateApiKeyAsync(
        string rawApiKey,
        CancellationToken ct = default);
}