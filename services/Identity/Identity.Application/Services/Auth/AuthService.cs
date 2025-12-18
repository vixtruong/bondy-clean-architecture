using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Contracts.Auth;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IHasher _hasher;
    private readonly ITokenGenerator _jwt;
    private readonly IClock _clock;

    public AuthService(
        IUserRepository users,
        IHasher hasher,
        ITokenGenerator jwt,
        IClock clock,
        IRefreshTokenRepository refreshTokens)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _clock = clock;
        _refreshTokens = refreshTokens;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!user.Active)
            return Result.Failure<LoginResponse>(
                Error.Forbidden(ErrorCodes.Auth.UserInactive, "User is inactive"));

        var local = user.Accounts.FirstOrDefault(a => a.Provider == AuthProvider.Local);
        if (local?.PasswordHash is null)
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!_hasher.Verify(request.Password, local.PasswordHash.Value))
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user.Id);

        return Result.Success(
            new LoginResponse(accessToken, refreshToken),
            successCode: SuccessCodes.Auth.LoginSuccess);
    }


    #region private

    private async Task<string> GenerateRefreshToken(long userId)
    {
        var now = _clock.UtcNow;

        var token = Guid.NewGuid().ToString("N");
        var tokenHash = _hasher.Hash(token);

        var newRefreshToken = new RefreshToken(
            userId,
            HashedValue.Create(tokenHash),
            now.AddDays(AppConstant.RefreshTokenDays),
            now);

        await _refreshTokens.AddAsync(newRefreshToken);
        await _refreshTokens.RevokeTokens(userId, now);

        return token;
    }

    #endregion

}
