using System.Text.Json.Serialization;

namespace Identity.Contracts.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;

    public int AccessTokenMinutes { get; set; }
}

public sealed class AuthTokens
{
    public string AccessToken { get; init; } = default!;
    public int AccessTokenMinutes { get; init; }
    public long UserId { get; set; }
    public string RefreshTokenRaw { get; init; } = default!;
    public string SessionId { get; init; } = default!;
}

