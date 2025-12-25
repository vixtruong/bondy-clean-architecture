namespace Identity.Contracts.Auth;

public class RefreshTokenRequest
{
    public long UserId { get; set; }

    public string Token { get; set; } = null!;
}
