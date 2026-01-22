
using Bondy.SharedKernel.Domain.Common;
using Identity.Application.Results.Auth;

namespace Identity.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthTokensResult>> LoginAsync(string email, string password);
    Task<Result<AuthTokensResult>> GoogleLoginAsync(string idToken);
    Task<Result> RegisterInit(
        string email,
        string firstName,
        string? middleName,
        string? lastName,
        DateTime dob,
        string password);
    Task<Result> RegisterVerify(string email, string otp);
    Task<Result<AuthTokensResult>> RefreshToken(long userId, string sessionId, string token);
    Task<Result> Logout(long userId, string sessionId);
}