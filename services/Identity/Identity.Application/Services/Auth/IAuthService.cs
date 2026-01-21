
using Bondy.SharedKernel.Domain.Common;
using Identity.Contracts.Auth;

namespace Identity.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthTokens>> LoginAsync(LoginRequest req);
    Task<Result<AuthTokens>> GoogleLoginAsync(GoogleLoginRequest req);
    Task<Result> RegisterInit(RegisterRequest req);
    Task<Result> RegisterVerify(VerifyOtpRequest req);
    Task<Result<AuthTokens>> RefreshToken(RefreshTokenRequest req);
    Task<Result> Logout(LogoutRequest req);
}