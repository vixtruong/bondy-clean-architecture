using Bondy.Contracts.Dtos.ApiKey;
using Bondy.SharedKernel.Common;
using Identity.Contracts.ApiKey;

namespace Identity.Application.Services.ApiKey;

public interface IApiKeyService
{
    Task<Result<ApiKeyCreatedResponse>> Create(CreateApiKeyRequest req);

    Task<Result<ApiKeyResponse>> Update(UpdateApiKeyRequest req);

    Task<Result<ApiKeyCreatedResponse>> Rotate(RotateApiKeyRequest req);

    Task<Result> Revoke(string apiKeyId);

    Task<Result<ApiKeyValidationResult>> Validate(ValidateApiKeyRequest req);
}
