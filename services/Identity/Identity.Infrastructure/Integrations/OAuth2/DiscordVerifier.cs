using Identity.Application.Abstractions.OAuth2;
using Identity.Application.Exceptions;
using Identity.Application.Results.Auth;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Identity.Infrastructure.Integrations.OAuth2;

public class DiscordVerifier : IDiscordVerifier
{
    private const string TokenEndpoint = "https://discord.com/api/oauth2/token";
    private const string UserEndpoint = "https://discord.com/api/users/@me";
    private const string AuthorizeBase = "https://discord.com/api/oauth2/authorize";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly ILogger<DiscordVerifier> _logger;

    public DiscordVerifier(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DiscordVerifier> logger)
    {
        _clientId = configuration["OAuth2:Discord:ClientId"]!;
        _clientSecret = configuration["OAuth2:Discord:ClientSecret"]!;
        _redirectUri = configuration["OAuth2:Discord:RedirectUri"]!;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public string BuildAuthorizationUrl(string? state = null)
    {
        // base authorize endpoint

        var query = new Dictionary<string, string?>
        {
            ["client_id"] = _clientId,
            ["redirect_uri"] = _redirectUri,
            ["response_type"] = "code",
            ["scope"] = "identify email",
            // optional prompt / include_granted_scopes if you want:
            ["prompt"] = "consent"
        };

        if (!string.IsNullOrEmpty(state))
        {
            query["state"] = state;
        }

        // Build query string
        var qs = QueryHelpers.AddQueryString(AuthorizeBase, query!);
        _logger.LogDebug("Discord authorization URL generated: {Url}", qs);
        return qs;
    }

    public async Task<DiscordUser> AuthenticateAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Discord authorization code is empty.");
            throw new OAuth2Exception(AuthProvider.Discord);
        }

        var client = _httpClientFactory.CreateClient();

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", _redirectUri)
        };

        var req = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var res = await client.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("DiscordVerifier get token failed: {data}", res.Content.ToString());
            throw new OAuth2Exception(AuthProvider.Discord);
        }

        var json = await res.Content.ReadAsStringAsync();
        var tokenResp = JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (tokenResp == null)
        {
            _logger.LogWarning("DiscordTokenVerifier token reps null");
            throw new OAuth2Exception(AuthProvider.Discord);
        }

        var user = await GetDiscordUserAsync(tokenResp.Access_Token);
        if (user == null)
        {
            _logger.LogWarning("DiscordTokenVerifier discord user reps null");
            throw new OAuth2Exception(AuthProvider.Discord);
        }

        return user;
    }

    private async Task<DiscordUser?> GetDiscordUserAsync(string discordAccessToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", discordAccessToken);
        var res = await client.GetAsync(UserEndpoint);
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DiscordUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
