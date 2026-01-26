using Bondy.SharedKernel.Application.Authorization.Role;
using Bondy.SharedKernel.Application.Base;
using Bondy.SharedKernel.Application.Commands;
using Bondy.SharedKernel.Domain.Abstractions;
using Bondy.SharedKernel.Domain.Common;
using Google.Apis.Auth;
using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.OAuth2;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Application.Exceptions;
using Identity.Application.Results.Auth;
using Identity.Application.Results.Otp;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
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
    private readonly IGoogleVerifier _googleVerifier;
    private readonly ITempPasswordGenerator _tempPasswordGenerator;
    private readonly IRoleRepository _roles;
    private readonly IDiscordVerifier _discordVerifier;

    public AuthService(ILogger<AuthService> logger, 
        IClock clock, 
        IUserRepository users, 
        IRefreshTokenRepository refreshTokens, 
        IHasher hasher, ITokenGenerator jwt, 
        IPreRegistrationRepository preRegistrations, 
        IMailClient mailClient, 
        IOtpCodeRepository otpCodes, 
        IOtpGenerator otpGenerator, 
        IGoogleVerifier googleVerifier, 
        ITempPasswordGenerator tempPasswordGenerator, 
        IRoleRepository roles, 
        IDiscordVerifier discordVerifier) : base(logger, clock)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
        _jwt = jwt;
        _preRegistrations = preRegistrations;
        _mailClient = mailClient;
        _otpCodes = otpCodes;
        _otpGenerator = otpGenerator;
        _googleVerifier = googleVerifier;
        _tempPasswordGenerator = tempPasswordGenerator;
        _roles = roles;
        _discordVerifier = discordVerifier;
    }

    #endregion

    #region Main Methods

    public async Task<Result<AuthTokensResult>> LoginAsync(string email, string password)
    {
        Domain.Entities.User? user = await _users.GetByEmailAsync(Email.FromPersisted(email));

        if (user is null)
            return Result.Failure<AuthTokensResult>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!user.Active)
            return Result.Failure<AuthTokensResult>(
                Error.Forbidden(ErrorCodes.Auth.UserInactive, "User is inactive"));

        Account? local = user.Accounts.FirstOrDefault(a => a.Provider == AuthProvider.Local);
        if (local?.PasswordHash is null)
            return Result.Failure<AuthTokensResult>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        if (!_hasher.Verify(password, local.PasswordHash.Value))
            return Result.Failure<AuthTokensResult>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidCredentials, "Invalid credentials"));

        var userForToken = await _users.GetByIdForTokenAsync(user.Id);

        var accessTokenResult = _jwt.GenerateAccessToken(userForToken!);

        string newSessionId = Guid.NewGuid().ToString("N");
        string refreshToken = await GenerateRefreshToken(user.Id, newSessionId);

        return Result.Success(
            new AuthTokensResult
            {
                UserId = user.Id,
                AccessToken = accessTokenResult.AccessToken,
                RefreshTokenRaw = refreshToken,
                AccessTokenMinutes = accessTokenResult.AccessTokenMinutes,
                SessionId = newSessionId
            },
            successCode: SuccessCodes.Auth.LoginSuccess);
    }

    #region OAuth2

    //public async Task<Result<AuthTokensResult>> GoogleLoginAsync(string idToken)
    //{
    //    var now = _clock.Now;
    //    var provider = AuthProvider.Google;

    //    GoogleJsonWebSignature.Payload payload;

    //    try
    //    {
    //        payload = await _googleVerifier.VerifyTokenAsync(idToken);
    //    }
    //    catch (OAuth2Exception ex)
    //    {
    //        return Result.Failure<AuthTokensResult>(
    //            Error.Unauthorized(ErrorCodes.Auth.InvalidOAuth2Token, ex.Message));
    //    }

    //    var email = Email.FromPersisted(payload.Email);

    //    return await ProcessOAuthLoginAsync(provider, email, payload.GivenName, payload.FamilyName, payload.Picture, now);
    //}

    public string BuildGoogleAuthorizationUri(string state)
    {
        return _googleVerifier.BuildAuthorizationUrl(state);
    }

    public string BuildDiscordAuthorizationUri(string? state = null)
    {
        return _discordVerifier.BuildAuthorizationUrl(state);
    }


    public async Task<Result<AuthTokensResult>> HandleGoogleCallbackAsync(string code, string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return Result.Failure<AuthTokensResult>(Error.Validation(ErrorCodes.Validation.Argument, "State must not null or empty."));

        var now = _clock.Now;
        var provider = AuthProvider.Google;

        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await _googleVerifier.AuthenticateAsync(code);
        }
        catch (OAuth2Exception ex)
        {
            return Result.Failure<AuthTokensResult>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidOAuth2Token, ex.Message));
        }

        var email = Email.FromPersisted(payload.Email);

        return await ProcessOAuthLoginAsync(provider, email, payload.GivenName, payload.FamilyName, payload.Picture, now);
    }

    public async Task<Result<AuthTokensResult>> HandleDiscordCallbackAsync(string code, string? error)
    {
        if (!string.IsNullOrWhiteSpace(error))
            return Result.Failure<AuthTokensResult>(Error.BadRequest(ErrorCodes.Auth.OAuth2DiscordError, error));

        var provider = AuthProvider.Discord;
        var now = _clock.Now;

        DiscordUser user;

        try
        {
            user = await _discordVerifier.AuthenticateAsync(code);
        }
        catch (OAuth2Exception ex)
        {
            return Result.Failure<AuthTokensResult>(
                Error.Unauthorized(ErrorCodes.Auth.InvalidOAuth2Token, ex.Message));
        }

        var email = Email.FromPersisted(user.Email);

        return await ProcessOAuthLoginAsync(provider, email, user.Username, null, user.AvatarUrl, now);
    }

    #endregion

    public async Task<Result> RegisterInit(
        string email,
        string firstName,
        string? middleName,
        string? lastName,
        DateTime dob,
        string password)
    {
        var existed = await _users.ExistByEmailAsync(Email.FromPersisted(email));

        if (existed)
            return Result.Failure(Error.Conflict(ErrorCodes.User.EmailAlreadyExist, "Email already exists."));

        var preReg = await _preRegistrations.GetByEmailAsync(Email.FromPersisted(email));

        if (preReg == null)
        {
            preReg = new PreRegistration(
                Email.FromPersisted(email),
                PersonName.FromPersisted(firstName, middleName, lastName),
                dob,
                null,
                HashedValue.FromPersisted(_hasher.Hash(password)),
                _clock.Now
            );

            await _preRegistrations.AddAsync(preReg);
        }

        var otp = await GenerateOtp(preReg.Id, OtpSubjectType.PreRegistration, OtpPurpose.VerifyEmail);

        // send mail
        try
        {
            await _mailClient.SendEmailAsync(
                new SendEmailCommand
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

    public async Task<Result> RegisterVerify(string email, string otp)
    {
        var now = _clock.Now;
        var emailValue = Email.FromPersisted(email);

        var preReg = await _preRegistrations.GetByEmailAsync(emailValue);
        if (preReg is null)
            return Result.Failure(
                Error.NotFound(
                    ErrorCodes.PreRegistration.NotFound,
                    "Pre-registration not found. Please register first."
                ));

        var otpResult = await ValidateAndConsumeOtp(
            subjectId: preReg.Id,
            purpose: OtpPurpose.VerifyEmail,
            rawOtp: otp,
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

        var newUser = new Domain.Entities.User(
            preReg.Email,
            preReg.Name,
            now,
            preReg.Dob);

        var userRole = await _roles.GetByCodeAsync(RoleCodes.User);
        if (userRole is null)
            return Result.Failure<AuthTokensResult>(Error.Failure(ErrorCodes.Server.Error, "Something wrong roles."));

        newUser.AssignRole(userRole);

        newUser.AddLocalAccount(preReg.PasswordHash, now);

        await _users.AddAsync(newUser);
        await _preRegistrations.RemoveAsync(preReg);

        await SendEmail(new SendEmailCommand
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

        return Result.Success(
            value: new { message = "Email verified successfully." },
            successCode: SuccessCodes.User.RegisterVerify
        );
    }

    public async Task<Result<AuthTokensResult>> RefreshToken(long userId, string sessionId, string token)
    {
        var now = _clock.Now;

        var tokens = await _refreshTokens.GetActiveTokensByUserIdAndSessionId(userId, sessionId, now);

        var match = tokens.FirstOrDefault(r => _hasher.Verify(token, r.TokenHash.Value));

        if (match is null)
            return Result<AuthTokensResult>.Failure(Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid refresh info"));

        var userForToken = await _users.GetByIdForTokenAsync(match.UserId);

        var accessTokenResult = _jwt.GenerateAccessToken(userForToken!);
        var refreshToken = await GenerateRefreshToken(match.UserId, sessionId);

        return Result.Success(
            new AuthTokensResult
            {
                UserId = match.UserId,
                AccessToken = accessTokenResult.AccessToken,
                RefreshTokenRaw = refreshToken,
                AccessTokenMinutes = accessTokenResult.AccessTokenMinutes,
                SessionId = sessionId
            },
            successCode: SuccessCodes.Auth.LoginSuccess);
    }

    public async Task<Result> Logout(long userId, string sessionId)
    {
        await _refreshTokens.RevokeTokens(userId, sessionId, _clock.Now);

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

    private async Task SendEmail(SendEmailCommand command)
    {
        try
        {
            await _mailClient.SendEmailAsync(command);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Welcome mail failed for {Email}", command.To);
        }
    }

    /// <summary>
    /// Handles OAuth2 login for users authenticated via third-party providers (Google, Discord, etc.).
    /// 
    /// - Looks up the user by verified email:
    ///   • If the user does not exist, creates a new user with a temporary local account,
    ///     assigns the default role, links the social provider account,
    ///     and marks the user for a welcome email.
    ///   • If the user exists but is not linked to the provider, links the social account
    ///     and marks the user for a notification email.
    /// 
    /// - Generates access token, refresh token, and session information.
    /// - Sends a welcome or account-linking email when applicable.
    /// </summary>
    /// <param name="provider">The OAuth2 provider used for authentication (e.g., Google, Discord).</param>
    /// <param name="email">The verified email address returned by the provider.</param>
    /// <param name="givenName">The user's given name from the provider (optional).</param>
    /// <param name="familyName">The user's family name from the provider (optional).</param>
    /// <param name="avatarUrl">The user's avatar URL from the provider (optional).</param>
    /// <param name="now">The current timestamp used for auditing and token generation.</param>
    /// <returns>
    /// A result containing access and refresh tokens with session details if successful;
    /// otherwise, a failure result describing the authentication error.
    /// </returns>
    private async Task<Result<AuthTokensResult>> ProcessOAuthLoginAsync(
        AuthProvider provider,
        Email email,
        string givenName,
        string? familyName,
        string? avatarUrl,
        DateTime now)
    {
        bool shouldSendWelcomeEmail = false;
        string? tempPassword = null;

        var user = await _users.GetByEmailAsync(email);

        if (user is null)
        {
            user = new Domain.Entities.User(
                email,
                PersonName.FromPersisted(givenName, "", familyName),
                now,
                avatarUrl: avatarUrl);

            tempPassword = _tempPasswordGenerator.Generate();
            var passwordHash = _hasher.Hash(tempPassword);

            user.AddLocalAccount(
                HashedValue.FromPersisted(passwordHash), now);

            var userRole = await _roles.GetByCodeAsync(RoleCodes.User);
            if (userRole is null)
                return Result.Failure<AuthTokensResult>(Error.Failure(ErrorCodes.Server.Error, "Something wrong roles."));

            user.AssignRole(userRole);

            user.AddSocialAccount(provider, now);

            shouldSendWelcomeEmail = true;

            await _users.AddAsync(user);
        }
        else if (!user.HasAccount(provider))
        {
            user.AddSocialAccount(provider, now);
            await _users.UpdateAsync(user);
            shouldSendWelcomeEmail = true;
        }

        var userForToken = await _users.GetByIdForTokenAsync(user.Id);

        var accessTokenResult = _jwt.GenerateAccessToken(userForToken!);

        var sessionId = Guid.NewGuid().ToString("N");
        var refreshToken = await GenerateRefreshToken(user.Id, sessionId);

        if (shouldSendWelcomeEmail)
        {
            var data = new Dictionary<string, string>
            {
                ["firstName"] = user.Name.FirstName,
                ["provider"] = provider.ToString(),
                ["email"] = user.Email.Value,
                ["hasPassword"] = (tempPassword != null).ToString().ToLower(),
                ["password"] = tempPassword != null
                    ? tempPassword
                    : $"Your account already exists. We have successfully linked your {provider.ToString()} login to it."
            };

            await SendEmail(new SendEmailCommand
            {
                To = user.Email.Value,
                Purpose = EmailPurpose.OAuth2Welcome,
                Data = data,
                DedupTokenId = $"{user.Id.ToString()}:{provider}"
            });
        }

        return Result.Success(
            new AuthTokensResult
            {
                UserId = user.Id,
                AccessToken = accessTokenResult.AccessToken,
                RefreshTokenRaw = refreshToken,
                AccessTokenMinutes = accessTokenResult.AccessTokenMinutes,
                SessionId = sessionId,
            },
            successCode: SuccessCodes.Auth.LoginSuccess);
    }
    #endregion

}
