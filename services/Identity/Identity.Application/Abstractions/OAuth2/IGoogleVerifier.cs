using Google.Apis.Auth;

namespace Identity.Application.Abstractions.OAuth2;

public interface IGoogleVerifier
{
    Task<GoogleJsonWebSignature.Payload> VerifyTokenAsync(string idToken);

    /// <summary>
    /// Build Google OAuth2 authorization URL
    /// </summary>
    string BuildAuthorizationUrl(string state);


    /// <summary>
    /// Handle callback: exchange code + verify id_token
    /// </summary>
    Task<GoogleJsonWebSignature.Payload> AuthenticateAsync(string code);
}