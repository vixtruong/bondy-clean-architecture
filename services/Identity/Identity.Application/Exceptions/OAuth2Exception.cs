using Identity.Domain.Enums;

namespace Identity.Application.Exceptions;

public sealed class OAuth2Exception : Exception
{
    public AuthProvider Provider { get; }

    public OAuth2Exception(AuthProvider provider)
        : base($"{provider} OAuth2 Failed")
    {
        Provider = provider;
    }
}