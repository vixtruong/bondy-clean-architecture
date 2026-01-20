using Identity.Application.Abstractions.Security;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Common.Security;

public sealed class ApiKeyHasher : IApiKeyHasher
{
    public string Hash(string raw)
        => Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

    public bool Verify(string raw, string hash)
        => CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(raw)),
            Convert.FromHexString(hash));
}

