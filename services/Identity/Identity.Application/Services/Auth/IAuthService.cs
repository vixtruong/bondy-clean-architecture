
using Bondy.SharedKernel.Common;
using Identity.Contracts.Auth;

namespace Identity.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest req);
}