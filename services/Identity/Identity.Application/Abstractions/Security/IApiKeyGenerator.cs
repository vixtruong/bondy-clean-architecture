namespace Identity.Application.Abstractions.Security;

public interface IApiKeyGenerator
{
    public ApiKeyGeneratedResult Generate(DateTimeOffset now);
}

public sealed record ApiKeyGeneratedResult(string rawKey, string keyHash, string prefix);