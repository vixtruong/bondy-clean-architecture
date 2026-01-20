using Bondy.Contracts.Dtos.Mail;
using Bondy.Contracts.Enums.Mail;
using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Application;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Configuration;
using Bondy.SharedKernel.Constants;
using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Contracts.Auth;
using Identity.Contracts.Otp;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Identity.Application.Services.Auth;

public sealed class AuthService : ApplicationServiceBase, IAuthService
{
    #region Constructor

    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IHasher _hasher;
    private readonly ITokenGenerator _jwt;
    private readonly IPreRegistrationRepository _preRegistrations;
    private readonly IMailClient _mailClient;
    private readonly IOtpCodeRepository _otpCodes;
    private readonly IOtpGenerator _otpGenerator;

    public AuthService(ILogger<AuthService> logger, IClock clock, IOptions<AppConfigOptions> options, IUserRepository users, IRefreshTokenRepository refreshTokens, IHasher hasher, ITokenGenerator jwt, IPreRegistrationRepository preRegistrations, IMailClient mailClient, IOtpCodeRepository otpCodes, IOtpGenerator otpGenerator) : base(logger, clock, options.Value)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
        _jwt = jwt;
        _preRegistrations = preRegistrations;
        _mailClient = mailClient;
        _otpCodes = otpCodes;
        _otpGenerator = otpGenerator;
    }

    #endregion

    #region Main Methods

    public async Task<Result<AuthTokens>> LoginAsync(LoginRequest request)
    {
        User? user = await _users.GetByEmailAsync(Email.FromPersisted(request.Email));

        if (user is null)
            return Result.Failure<AuthTokens>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!user.Active)
            return Result.Failure<AuthTokens>(
                Error.Forbidden(ErrorCodes.Auth.UserInactive, "User is inactive"));

        Account? local = user.Accounts.FirstOrDefault(a => a.Provider == AuthProvider.Local);
        if (local?.PasswordHash is null)
            return Result.Failure<AuthTokens>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!_hasher.Verify(request.Password, local.PasswordHash.Value))
            return Result.Failure<AuthTokens>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        var accessTokenResult = _jwt.GenerateAccessToken(user);

        string newSessionId = Guid.NewGuid().ToString("N");
        string refreshToken = await GenerateRefreshToken(user.Id, newSessionId);

        return Result.Success(
            new AuthTokens
            {
                UserId = user.Id,
                AccessToken = accessTokenResult.AccessToken,
                RefreshTokenRaw = refreshToken,
                AccessTokenMinutes = accessTokenResult.AccessTokenMinutes,
                SessionId = newSessionId
            },
            successCode: SuccessCodes.Auth.LoginSuccess);
    }

    public async Task<Result> RegisterInit(RegisterRequest req)
    {
        var existed = await _users.ExistByEmailAsync(Email.FromPersisted(req.Email));

        if (existed)
            return Result.Failure(Error.Conflict(ErrorCodes.User.EmailAlreadyExist, "Email already exists."));

        var preReg = await _preRegistrations.GetByEmailAsync(Email.FromPersisted(req.Email));

        if (preReg == null)
        {
            preReg = new PreRegistration(
                Email.FromPersisted(req.Email),
                PersonName.FromPersisted(req.FirstName, req.MiddleName, req.LastName),
                req.Dob,
                null,
                HashedValue.FromPersisted(_hasher.Hash(req.Password)),
                _clock.Now
            );

            await _preRegistrations.AddAsync(preReg);
        }

        var otp = await GenerateOtp(preReg.Id, OtpSubjectType.PreRegistration, OtpPurpose.VerifyEmail);

        // send mail
        try
        {
            await _mailClient.SendEmailAsync(
                new SendEmailDto
                {
                    To = preReg.Email.Value,
                    Purpose = EmailPurpose.Registration,
                    Data = new Dictionary<string, string>
                    {
                        ["otp"] = otp.CodeRaw,
                        ["firstName"] = preReg.Name.FirstName,
                        ["expiresMinutes"] = OtpPolicy.ExpiryMinutes.ToString()
                    },
                    DedupTokenId = otp.Id.ToString()
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Register init send mail failed for {Email}", preReg.Email.Value);
        }

        return Result.Success(
            value: new { message = "OTP Code sent to your email." }, successCode: SuccessCodes.User.RegisterInit);
    }

    public async Task<Result> RegisterVerify(VerifyOtpRequest req)
    {
        var now = _clock.Now;
        var email = Email.FromPersisted(req.Email);

        var preReg = await _preRegistrations.GetByEmailAsync(email);
        if (preReg is null)
            return Result.Failure(
                Error.NotFound(
                    ErrorCodes.PreRegistration.NotFound,
                    "Pre-registration not found. Please register first."
                ));

        var otpResult = await ValidateAndConsumeOtp(
            subjectId: preReg.Id,
            purpose: OtpPurpose.VerifyEmail,
            rawOtp: req.Otp,
            now: now);

        if (otpResult.IsFailure)
            return Result.Failure(otpResult.Error);

        // TODO: create User from PreRegistration + delete preReg
        if (await _users.ExistByEmailAsync(preReg.Email))
            return Result.Failure(
                Error.Conflict(
                    ErrorCodes.User.EmailAlreadyExist,
                    "User already exists."
                ));

        var newUser = new User(
            preReg.Email,
            preReg.Name,
            ScopeSet.UserScopes,
            now,
            preReg.Dob);

        await _users.AddAsync(newUser);
        await _preRegistrations.RemoveAsync(preReg);

        try
        {
            await _mailClient.SendEmailAsync(new SendEmailDto
            {
                To = newUser.Email.Value,
                Purpose = EmailPurpose.Welcome,
                Data = new Dictionary<string, string>
                {
                    ["firstName"] = newUser.Name.FirstName,
                    ["email"] = newUser.Email.Value
                },
                DedupTokenId = newUser.Id.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Welcome mail failed for {Email}", newUser.Email.Value);
        }


        return Result.Success(
            value: new { message = "Email verified successfully." },
            successCode: SuccessCodes.User.RegisterVerify
        );
    }

    public async Task<Result<AuthTokens>> RefreshToken(RefreshTokenRequest req)
    {
        var now = _clock.Now;

        var tokens = await _refreshTokens.GetActiveTokensByUserIdAndSessionId(req.UserId, req.SessionId, now);

        var match = tokens.FirstOrDefault(r => _hasher.Verify(req.Token, r.TokenHash.Value));

        if (match is null)
            return Result<AuthTokens>.Failure(Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid refresh info"));

        var accessTokenResult = _jwt.GenerateAccessToken(match.User);
        var refreshToken = await GenerateRefreshToken(match.UserId, req.SessionId);

        return Result.Success(
            new AuthTokens
            {
                UserId = match.UserId,
                AccessToken = accessTokenResult.AccessToken,
                RefreshTokenRaw = refreshToken,
                AccessTokenMinutes = accessTokenResult.AccessTokenMinutes,
                SessionId = req.SessionId
            },
            successCode: SuccessCodes.Auth.LoginSuccess);
    }

    public async Task<Result> Logout(LogoutRequest req)
    {
        await _refreshTokens.RevokeTokens(req.UserId, req.SessionId, _clock.Now);

        return Result.Success(SuccessCodes.Auth.LogoutSuccess);
    }

    #endregion

    #region Support Methods

    private async Task<string> GenerateRefreshToken(long userId, string sessionId)
    {
        var now = _clock.Now;

        var bytes = RandomNumberGenerator.GetBytes(TokenPolicy.RefreshTokenByteLength);

        var tokenRaw = Convert.ToHexString(bytes).ToLowerInvariant();
        var tokenHash = _hasher.Hash(tokenRaw);

        var newRefreshToken = new RefreshToken(
            userId,
            sessionId,
            HashedValue.FromPersisted(tokenHash),
            now.AddDays(TokenPolicy.RefreshTokenDays),
            now);

        await _refreshTokens.RevokeTokens(userId, sessionId, now);

        await _refreshTokens.AddAsync(newRefreshToken);

        return tokenRaw;
    }

    private async Task<OtpCreatedResult> GenerateOtp(long subjectId, OtpSubjectType subjectType, OtpPurpose purpose)
    {
        var now = _clock.Now;

        await _otpCodes.DeactivateActiveOtp(subjectId, purpose, now);

        var otpRaw = _otpGenerator.Generate(OtpPolicy.Length);

        var otp = new OtpCode(
            subjectType,
            subjectId,
            purpose,
            HashedValue.FromPersisted(_hasher.Hash(otpRaw)),
            now.AddMinutes(OtpPolicy.ExpiryMinutes),
            now);

        await _otpCodes.AddAsync(otp);
        
        return new OtpCreatedResult(otp.Id, otpRaw);
    }

    private async Task<Result<OtpCode>> ValidateAndConsumeOtp(
        long subjectId,
        OtpPurpose purpose,
        string rawOtp,
        DateTime now)
    {
        var otp = await _otpCodes.GetActiveBySubject(subjectId, purpose);

        if (otp is null)
            return Result.Failure<OtpCode>(
                Error.NotFound(
                    ErrorCodes.PreRegistration.OtpNotFound,
                    "OTP does not exist for this request."
                ));

        if (!otp.Active)
            return Result.Failure<OtpCode>(
                Error.Conflict(
                    ErrorCodes.PreRegistration.OtpInactive,
                    "OTP is inactive. Please request a new code."
                ));

        if (otp.IsExpired(now))
        {
            otp.Deactivate(now);
            await _otpCodes.UpdateAsync(otp);

            return Result.Failure<OtpCode>(
                Error.Conflict(
                    ErrorCodes.PreRegistration.OtpExpired,
                    "OTP has expired. Please request a new code."
                ));
        }

        if (otp.Attempts >= OtpPolicy.MaxAttempts)
        {
            otp.Deactivate(now);
            await _otpCodes.UpdateAsync(otp);

            return Result.Failure<OtpCode>(
                Error.Conflict(
                    ErrorCodes.PreRegistration.OtpLocked,
                    "Too many failed attempts. OTP has been locked."
                ));
        }

        if (!_hasher.Verify(rawOtp, otp.CodeHash.Value))
        {
            otp.IncreaseAttempts(now);

            if (otp.Attempts >= OtpPolicy.MaxAttempts)
                otp.Deactivate(now);

            await _otpCodes.UpdateAsync(otp);

            var remaining = Math.Max(0, OtpPolicy.MaxAttempts - otp.Attempts);

            var msg = remaining > 0
                ? $"Incorrect OTP. Attempts remaining: {remaining}."
                : "Too many failed attempts. OTP has been locked.";

            return Result.Failure<OtpCode>(
                Error.Validation(
                    ErrorCodes.PreRegistration.OtpInvalid,
                    msg,
                    new Dictionary<string, object?>
                    {
                        ["remainingAttempts"] = remaining
                    }
                ));
        }

        // OTP đúng => consume (deactivate)
        otp.Deactivate(now);
        await _otpCodes.UpdateAsync(otp);

        return Result.Success(otp);
    }


    #endregion

}
