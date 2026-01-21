using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Bondy.SharedKernel.Constants.Authorization;
using Identity.Application.Services.Auth;
using Identity.Contracts.Auth;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    //[AllowAnonymous]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Constructor

        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        #endregion

        #region Api Actions

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _service.LoginAsync(request);

            return this.AuthResponse(result);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _service.GoogleLoginAsync(request);

            return this.AuthResponse(result);
        }


        [HttpPost("register/init")]
        public async Task<IActionResult> RegisterInit([FromBody] RegisterRequest request)
                => this.ToActionResult(await _service.RegisterInit(request));

        [HttpPost("register/verify")]
        public async Task<IActionResult> RegisterVerify([FromBody] VerifyOtpRequest request)
            => this.ToActionResult(await _service.RegisterVerify(request));

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var rt = Request.Cookies["rt"];
            var uid = Request.Cookies["uid"];
            var sessionId = Request.Cookies["sessionId"];

            if (string.IsNullOrWhiteSpace(rt) || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(sessionId))
                return this.ToActionResult(Result.Failure<AuthResponse>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid refresh info")));

            if (!long.TryParse(uid, out var userId))
            {
                return this.ToActionResult(Result.Failure<AuthResponse>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid user id format")));
            }

            var result = await _service.RefreshToken(
                new RefreshTokenRequest
                {
                    Token = rt,
                    UserId = userId,
                    SessionId = sessionId,
                });

            return this.AuthResponse(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = Scopes.AuthLogout)]
        public async Task<IActionResult> Logout()
        {
            var uid = Request.Cookies["uid"];
            var sessionId = Request.Cookies["sessionId"];

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(sessionId))
                return this.ToActionResult(Result.Failure<AuthResponse>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid logout info")));

            if (!long.TryParse(uid, out var userId))
            {
                return this.ToActionResult(Result.Failure<AuthResponse>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid user id format")));
            }

            var result = await _service.Logout(
                new LogoutRequest
                {
                    UserId = userId,
                    SessionId = sessionId
                });

            if (result.IsSuccess)
                Response.ClearRefreshInfoCookies(Request);

            return this.ToActionResult(result);
        }


        #endregion

        #region Support Methods

        private IActionResult AuthResponse(Result<AuthTokens> result)
        {
            if (result.IsSuccess)
            {
                Response.SetRefreshInfoCookie(
                    Request,
                    userId: result.Value!.UserId,
                    refreshTokenRaw: result.Value!.RefreshTokenRaw,
                    sessionId: result.Value!.SessionId,
                    days: TokenPolicy.RefreshTokenDays);

                return this.ToActionResult(Result.Success(
                    new AuthResponse
                    {
                        AccessToken = result.Value.AccessToken,
                        AccessTokenMinutes = result.Value.AccessTokenMinutes
                    },
                    successCode: SuccessCodes.Auth.LoginSuccess));
            }

            return this.ToActionResult(Result.Failure<AuthResponse>(result.Error));
        }

        #endregion
    }
}
