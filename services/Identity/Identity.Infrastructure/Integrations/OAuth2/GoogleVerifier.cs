using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Identity.Application.Abstractions.OAuth2;
using Identity.Application.Exceptions;
using Identity.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Identity.Infrastructure.Integrations.OAuth2;

public sealed class GoogleVerifier : IGoogleVerifier
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly ILogger<GoogleVerifier> _logger;
    private readonly HttpClient _httpClient;


    public GoogleVerifier(
        IConfiguration configuration,
        ILogger<GoogleVerifier> logger,
        HttpClient httpClient)
    {
        _clientId = configuration["OAuth2:Google:ClientId"]!;
        _clientSecret = configuration["OAuth2:Google:ClientSecret"]!;
        _redirectUri = configuration["OAuth2:Google:RedirectUri"]!;
        
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyTokenAsync(string idToken)
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
            throw new OAuth2Exception(AuthProvider.Google);
        }
        catch (SecurityTokenException)
        {
            _logger.LogWarning("Google token security validation failed");
            throw new OAuth2Exception(AuthProvider.Google);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("Google OAuth2 endpoint unreachable");
            throw;
        }
    }

    public string BuildAuthorizationUrl(string state)
    {
        var url = new GoogleAuthorizationCodeRequestUrl(
            new Uri("https://accounts.google.com/o/oauth2/v2/auth"))
        {
            ClientId = _clientId,
            RedirectUri = _redirectUri,
            Scope = "openid email profile",
            ResponseType = "code",
            AccessType = "offline",
            Prompt = "consent",
            State = state
        }.Build();

        _logger.LogDebug("Google OAuth authorization URL generated");

        return url.ToString();
    }

    public async Task<GoogleJsonWebSignature.Payload> AuthenticateAsync(string code)
    {
        try
        {
            var tokenUrl = "https://oauth2.googleapis.com/token";
            var values = new List<KeyValuePair<string, string>>
        {
            new("code", code),
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("redirect_uri", _redirectUri),
            new("grant_type", "authorization_code")
        };

            using var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(values)
            };
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await _httpClient.SendAsync(req);
            var txt = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google token endpoint failed: {Status} {Body}", res.StatusCode, txt);
                throw new OAuth2Exception(AuthProvider.Google);
            }

            // Deserialize token response
            using var doc = JsonDocument.Parse(txt);
            var root = doc.RootElement;
            if (!root.TryGetProperty("id_token", out var idTokenEl))
            {
                _logger.LogWarning("Google token exchange returned no id_token: {Body}", txt);
                throw new OAuth2Exception(AuthProvider.Google);
            }
            var idToken = idTokenEl.GetString()!;

            // Validate id_token using Google.Apis.Auth
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            });

            _logger.LogInformation("Google OAuth success for {Email}", payload.Email);
            return payload;
        }
        catch (InvalidJwtException)
        {
            _logger.LogWarning("Invalid Google ID token");
            throw new OAuth2Exception(AuthProvider.Google);
        }
        catch (SecurityTokenException)
        {
            _logger.LogWarning("Google token security validation failed");
            throw new OAuth2Exception(AuthProvider.Google);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("Google OAuth2 endpoint unreachable");
            throw;
        }
    }
}