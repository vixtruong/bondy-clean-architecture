using Bondy.SharedKernel.Application.Base;
using Bondy.SharedKernel.Domain.Abstractions;
using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Application.Results.ApiKey;
using Identity.Domain.Constants;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services.ApiKey;

public sealed class ApiKeyService : ApplicationServiceBase, IApiKeyService
{
    private readonly IApiKeyRepository _apiKeys;
    private readonly IApiKeyHasher _apiKeyHasher;
    private readonly IApiKeyGenerator _apiKeyGenerator;

    public ApiKeyService(
        ILogger<ApiKeyService> logger, 
        IClock clock, 
        IApiKeyRepository apiKeys, 
        IApiKeyHasher apiKeyHasher, IApiKeyGenerator apiKeyGenerator) : base(logger, clock)
    {
        _apiKeys = apiKeys;
        _apiKeyHasher = apiKeyHasher;
        _apiKeyGenerator = apiKeyGenerator;
    }

    public async Task<Result<ApiKeyCreatedResult>> Create(string name, string owner, string ownerEmail, IReadOnlyList<string> scopes, DateTimeOffset? expiresAt)
    {
        var now = _clock.Now;

        var apiKeyGen = _apiKeyGenerator.Generate(now);
        var rawApiKey = apiKeyGen.rawKey;
        var keyPrefix = apiKeyGen.prefix;
        var keyHash = apiKeyGen.keyHash;

        var apiKey = new Domain.Entities.ApiKey(
            keyId: Guid.NewGuid().ToString("N"),
            name: name,
            keyPrefix: keyPrefix,
            keyHash: HashedValue.FromPersisted(keyHash),
            owner: owner,
            ownerEmail: Email.FromPersisted(ownerEmail),
            scopes: scopes.Select(scope => new Scope(scope)),
            allowedPaths: null,
            rateLimitPlanId: null,
            expiresAt: expiresAt,
            createdAt: now
        );

        await _apiKeys.AddAsync(apiKey);

        return Result.Success(new ApiKeyCreatedResult(
            Id: apiKey.Id,
            Name: apiKey.Name,
            RawApiKey: rawApiKey,
            CreatedAt: now,
            ExpiresAt: expiresAt
        ));
    }

    public async Task<Result<ApiKeyResult>> Update(long apiKeyId, string name, IReadOnlyList<string> scopes, DateTimeOffset? expiresAt, bool? isActive)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ApiKeyCreatedResult>> Rotate(long apiKeyId)
    {
        var now = _clock.NowOffset;

        var oldKey = await _apiKeys.GetByIdAsync(apiKeyId);
        if (oldKey is null)
            return Result<ApiKeyCreatedResult>.Failure(Error.BadRequest(ErrorCodes.Common.NotFound, "Api key not found."));

        if (!oldKey.IsActive || oldKey.RevokeAt != null)
            return Result<ApiKeyCreatedResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyRevoked, "Api key revoked"));

        if (oldKey.IsExpired(now) || (oldKey.RotateAt != null && oldKey.RotateAt < now))
            return Result<ApiKeyCreatedResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyExpired, "Api key expired or rotated"));

        oldKey.Rotate(now, ApiKeyPolicy.DefaultGracePeriod);

        var apiKeyGen = _apiKeyGenerator.Generate(now);
        var rawApiKey = apiKeyGen.rawKey;
        var keyPrefix = apiKeyGen.prefix;
        var keyHash = apiKeyGen.keyHash;

        var newKey = new Domain.Entities.ApiKey(
            keyId: Guid.NewGuid().ToString("N"),
            name: oldKey.Name,
            keyPrefix: keyPrefix,
            keyHash: HashedValue.FromPersisted(keyHash),
            owner: oldKey.Owner,
            ownerEmail: oldKey.OwnerEmail,
            scopes: oldKey.Scopes,
            allowedPaths: null,
            rateLimitPlanId: null,
            expiresAt: oldKey.ExpiresAt,
            createdAt: now.DateTime
        );

        await _apiKeys.AddAsync(newKey);
        await _apiKeys.UpdateAsync(oldKey);

        return Result.Success(new ApiKeyCreatedResult(
            Id: newKey.Id,
            Name: newKey.Name,
            RawApiKey: rawApiKey,
            CreatedAt: now,
            ExpiresAt: newKey.ExpiresAt
        ));
    }

    public async Task<Result> Revoke(string apiKeyId)
    {
        var now = _clock.NowOffset;

        if (!long.TryParse(apiKeyId, out var validId))
            return Result.Failure(Error.Validation(ErrorCodes.Validation.Argument, "Api key must be long type."));

        var apiKey = await _apiKeys.GetByIdAsync(validId);
        if (apiKey == null)
            return Result.Failure(Error.BadRequest(ErrorCodes.Common.NotFound, "Api key not found."));

        apiKey.Revoke(ApiKeyRevokeReason.UserAction, now);

        await _apiKeys.RemoveAsync(apiKey);

        return Result.Success(SuccessCodes.Common.Ok);
    }

    public async Task<Result<ApiKeyValidationResult>> Validate(string apiKeyReq)
    {
        var now = _clock.NowOffset;

        var lastUnderscoreIndex = apiKeyReq.LastIndexOf('_');
        if (lastUnderscoreIndex <= 0 ||
            lastUnderscoreIndex == apiKeyReq.Length - 1)
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, "Api key invalid"));

        var keyPrefix = apiKeyReq[..lastUnderscoreIndex];
        var rawKey = apiKeyReq;

        var apiKey = await _apiKeys.GetByKeyPrefitAsync(keyPrefix);
        if (apiKey is null)
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, "Api key invalid"));

        var hashed = HashedValue.FromPersisted(
            _apiKeyHasher.Hash(rawKey));

        if (!apiKey.KeyHash.Equals(hashed))
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyInvalid, "Api key invalid"));

        if (!apiKey.IsActive || apiKey.RevokeAt != null)
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyRevoked, "Api key revoked"));

        if (apiKey.IsExpired(now) || (apiKey.RotateAt != null && apiKey.RotateAt < now))
            return Result<ApiKeyValidationResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.ApiKeyExpired, "Api key expired or rotated"));

        apiKey.Touch(now);
        await _apiKeys.TouchAsync(apiKey);

        return Result.Success(new ApiKeyValidationResult(
            Id: apiKey.Id,
            Name: apiKey.Name,
            Owner: apiKey.Owner,
            Scopes: apiKey.Scopes.Select(s => s.Value).ToArray(),
            AllowedPaths: apiKey.AllowedPaths,
            IsActive: apiKey.IsActive,
            ExpiresAt: apiKey.ExpiresAt
        ));
    }

}
