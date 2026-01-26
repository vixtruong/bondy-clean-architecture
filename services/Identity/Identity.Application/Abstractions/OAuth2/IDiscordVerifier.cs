using Identity.Application.Results.Auth;

namespace Identity.Application.Abstractions.OAuth2;

public interface IDiscordVerifier
{
    string BuildAuthorizationUrl(string? state = null);
    Task<DiscordUser> AuthenticateAsync(string code);
}