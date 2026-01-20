using Bondy.Contracts.Dtos.ApiKey;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;

namespace ApiGateway.Clients.Identity;

public sealed class IdentityClient : IIdentityClient
{
    private readonly HttpClient _http;

    public IdentityClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<Result<ApiKeyValidationResult>> ValidateApiKeyAsync(
        string rawApiKey,
        CancellationToken ct = default)
    {
        var request = new ValidateApiKeyRequest
        {
            ApiKey = rawApiKey
        };

        var response = await _http.PostAsJsonAsync(
            "/internal/apikeys/validate",
            request,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, "ApiKey Invalid"));
        }

        var result = await response
            .Content
            .ReadFromJsonAsync<Result<ApiKeyValidationResult>>(ct);

        return result ?? Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, "ApiKey Invalid"));
    }
}