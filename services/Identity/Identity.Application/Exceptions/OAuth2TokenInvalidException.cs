using Identity.Domain.Enums;

namespace Identity.Application.Exceptions;

public sealed class OAuth2TokenInvalidException : Exception
{
    public AuthProvider Provider { get; }

    public OAuth2TokenInvalidException(AuthProvider provider)
        : base($"Invalid {provider} OAuth2 token")
    {
        Provider = provider;
    }
}