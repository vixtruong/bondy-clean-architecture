using Identity.Application.Abstractions.Security;
using Identity.Domain.Constants;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Common.Security;

public class ApiKeyGenerator : IApiKeyGenerator
{
    private readonly IApiKeyHasher _hasher;

    public ApiKeyGenerator(IApiKeyHasher hasher)
    {
        _hasher = hasher;
    }

    public ApiKeyGeneratedResult Generate(string env, DateTimeOffset now)
    {
        var shortId = RandomNumberGenerator
            .GetBytes(4);

        var shortIdHex = Convert.ToHexString(shortId).ToLowerInvariant();

        var keyPrefix = ApiKeyPrefix.Build(env, ApiKeyPrefix.App, shortIdHex);

        var secretBytes = RandomNumberGenerator.GetBytes(32);
        var secret = Convert.ToBase64String(secretBytes);

        var rawApiKey = $"{keyPrefix}_{secret}";

        var keyHash = _hasher.Hash(rawApiKey);

        return new ApiKeyGeneratedResult(rawApiKey, keyHash, keyPrefix);
    }
}
