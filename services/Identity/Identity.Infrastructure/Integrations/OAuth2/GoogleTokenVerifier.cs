using Google.Apis.Auth;
using Identity.Application.Abstractions.OAuth2;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Integrations.OAuth2;

public sealed class GoogleTokenVerifier : IGoogleTokenVerifier
{
    private readonly string _clientId;

    public GoogleTokenVerifier(IConfiguration configuration)
    {
        _clientId = configuration["OAuth2:Google:ClientId"]!;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _clientId }
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}
