
namespace Identity.Api.Contracts.Auth;

public class LogoutRequest
{
    public long UserId { get; set; }
    public string SessionId { get; set; } = null!;
}