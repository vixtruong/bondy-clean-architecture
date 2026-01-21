using Google.Apis.Auth;
using Identity.Application.Abstractions.OAuth2;
using Identity.Application.Exceptions;
using Identity.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Integrations.OAuth2;

public sealed class GoogleTokenVerifier : IGoogleTokenVerifier
{
    private readonly string _clientId;
    private readonly ILogger<GoogleTokenVerifier> _logger;

    public GoogleTokenVerifier(
        IConfiguration configuration,
        ILogger<GoogleTokenVerifier> logger)
    {
        _clientId = configuration["OAuth2:Google:ClientId"]!;
        _logger = logger;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            _logger.LogWarning("Invalid Google ID token");
            throw new OAuth2TokenInvalidException(AuthProvider.Google);
        }
        catch (SecurityTokenException)
        {
            _logger.LogWarning("Google token security validation failed");
            throw new OAuth2TokenInvalidException(AuthProvider.Google);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("Google OAuth2 endpoint unreachable");
            throw;
        }
    }
}