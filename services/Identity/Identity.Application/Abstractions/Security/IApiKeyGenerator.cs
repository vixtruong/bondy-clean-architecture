namespace Identity.Application.Abstractions.Security;

public interface IApiKeyGenerator
{
    public ApiKeyGeneratedResult Generate(string env, DateTimeOffset now);
}

public sealed record ApiKeyGeneratedResult(string rawKey, string keyHash, string prefix);