using Bondy.SharedKernel.Common;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Contracts.Auth;
using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return Result.Failure<LoginResponse>(Error.Unauthorized("auth.invalid_credentials", "Invalid credentials"));

        if (!user.Active)
            return Result.Failure<LoginResponse>(Error.Forbidden("auth.user_inactive", "User is inactive"));

        var local = user.Accounts.FirstOrDefault(a => a.Provider == AuthProvider.Local);
        if (local?.PasswordHash is null)
            return Result.Failure<LoginResponse>(Error.Unauthorized("auth.invalid_credentials", "Invalid credentials"));

        if (!_hasher.Verify(request.Password, local.PasswordHash.Value))
            return Result.Failure<LoginResponse>(Error.Unauthorized("auth.invalid_credentials", "Invalid credentials"));

        var accessToken = _jwt.GenerateAccessToken(user);

        var refreshToken = Guid.NewGuid().ToString("N");

        return Result.Success(new LoginResponse(accessToken, refreshToken), successCode: "auth.login.success");
    }
}

