namespace Identity.Application.Results.Auth;

public class AuthResult
{
    public string AccessToken { get; set; } = null!;

    public int AccessTokenMinutes { get; set; }
}

public sealed class AuthTokensResult
{
    public string AccessToken { get; init; } = default!;
    public int AccessTokenMinutes { get; init; }
    public long UserId { get; set; }
    public string RefreshTokenRaw { get; init; } = default!;
    public string SessionId { get; init; } = default!;
    public string? RedirectUrl { get; init; }
}