namespace Identity.Api.Contracts.Auth;

public class RefreshTokenRequest
{
    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public string SessionId { get; set; } = null!;
}
