using Identity.Application.Abstractions.Security;

namespace Identity.Infrastructure.Common.Security;

public sealed class BcryptHasher : IHasher
{
    public string Hash(string raw)
        => BCrypt.Net.BCrypt.HashPassword(raw);

    public bool Verify(string raw, string hash)
        => BCrypt.Net.BCrypt.Verify(raw, hash);
}