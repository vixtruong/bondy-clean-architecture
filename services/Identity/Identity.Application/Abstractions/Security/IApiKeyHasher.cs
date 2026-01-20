namespace Identity.Application.Abstractions.Security;

public interface IApiKeyHasher
{
    string Hash(string rawApiKey);
    bool Verify(string rawApiKey, string hash);
}
