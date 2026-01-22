using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Results.ApiKey;

namespace Identity.Application.Services.ApiKey;

public interface IApiKeyService
{
    Task<Result<ApiKeyCreatedResult>> Create(string name, string owner, string ownerEmail, IReadOnlyList<string> scopes, DateTimeOffset? expiresAt);

    Task<Result<ApiKeyResult>> Update(long apiKeyId, string name, IReadOnlyList<string> scopes, DateTimeOffset? expiresAt, bool? isActive);

    Task<Result<ApiKeyCreatedResult>> Rotate(long apiKeyId);

    Task<Result> Revoke(string apiKeyId);

    Task<Result<ApiKeyValidationResult>> Validate(string apiKey);
}