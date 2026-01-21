using Google.Apis.Auth;

namespace Identity.Application.Abstractions.OAuth2;

public interface IGoogleTokenVerifier
{
    Task<GoogleJsonWebSignature.Payload> VerifyAsync(string idToken);
}
