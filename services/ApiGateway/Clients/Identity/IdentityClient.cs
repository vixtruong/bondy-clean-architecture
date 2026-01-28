using ApiGateway.Clients.Dtos;
using Bondy.Contracts.Dtos.ApiKey;

namespace ApiGateway.Clients.Identity;

public sealed class IdentityClient : IIdentityClient
{
    private readonly HttpClient _http;
    private readonly ILogger<IdentityClient> _logger;

    public IdentityClient(HttpClient http, ILogger<IdentityClient> logger)
    {
        _http = http;
        _logger = logger;
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
            "/api/v1/internal/apikeys/validate",
            request,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);

            // log raw reason (server-side only)
            _logger.LogWarning(
                "ApiKey validation failed. Status={StatusCode}, Response={Body}",
                response.StatusCode,
                body);

            return Result<ApiKeyValidationResult>.Failure(
                Error.Unauthorized(
                    ErrorCodes.Auth.ApiKeyInvalid,
                    $"ApiKey validation failed ({response.StatusCode})"));
        }

        var envelope = await response.Content
            .ReadFromJsonAsync<ApiResponse<ApiKeyValidationResult>>(ct);

        if (envelope is null || !envelope.Success || envelope.Data is null)
        {
            return Result<ApiKeyValidationResult>.Failure(
                Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, envelope?.Message ?? "ApiKey Invalid"));
        }

        return Result.Success(envelope.Data);

    }
}