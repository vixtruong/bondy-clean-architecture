using ApiGateway.Clients.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ApiGateway.Authentication;

public class ApiKeyAuthenticationHandler 
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyScheme = "ApiKey ";
    private readonly IIdentityClient _identityClient;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IIdentityClient identityClient) : base(options, logger, encoder, clock)
    {
        _identityClient = identityClient;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogInformation("ApiKeyAuthenticationHandler");

        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return AuthenticateResult.NoResult();

        var auth = authHeader.ToString();
        if (!auth.StartsWith(ApiKeyScheme, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        var rawKey = auth[ApiKeyScheme.Length..].Trim();
        if (string.IsNullOrWhiteSpace(rawKey))
            return AuthenticateResult.Fail("Missing Api key");

        var result = await _identityClient.ValidateApiKeyAsync(rawKey);
        if (result.IsFailure)
            return AuthenticateResult.Fail(result.Error.Message);

        var apiKey = result.Value;
        if (apiKey != null && (!apiKey.IsActive ||
                               (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt <= DateTimeOffset.UtcNow)))
            return AuthenticateResult.Fail("API key expired");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey!.Id.ToString()), 
            new Claim("auth_type", "apiKey")
        };

        foreach (var scope in apiKey.Scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
