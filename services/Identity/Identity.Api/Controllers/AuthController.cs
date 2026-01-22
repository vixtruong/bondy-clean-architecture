using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Domain.Common;
using Identity.Api.Contracts.Auth;
using Identity.Api.Http;
using Identity.Application.Results.Auth;
using Identity.Application.Services.Auth;
using Identity.Domain.Constants;
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
            var result = await _service.LoginAsync(request.Email, request.Password);

            return this.AuthResponse(result);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _service.GoogleLoginAsync(request.IdToken);

            return this.AuthResponse(result);
        }


        [HttpPost("register/init")]
        public async Task<IActionResult> RegisterInit([FromBody] RegisterRequest request)
                => this.ToActionResult(
                    await _service.RegisterInit(
                        request.Email,
                        request.FirstName,
                        request.MiddleName,
                        request.LastName,
                        request.Dob,
                        request.Password)
                    );

        [HttpPost("register/verify")]
        public async Task<IActionResult> RegisterVerify([FromBody] VerifyOtpRequest request)
            => this.ToActionResult(await _service.RegisterVerify(request.Email, request.Otp));

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var rt = Request.Cookies["rt"];
            var uid = Request.Cookies["uid"];
            var sessionId = Request.Cookies["sessionId"];

            if (string.IsNullOrWhiteSpace(rt) || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(sessionId))
                return this.ToActionResult(Result.Failure<AuthResult>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid refresh info")));

            if (!long.TryParse(uid, out var userId))
            {
                return this.ToActionResult(Result.Failure<AuthResult>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid user id format")));
            }

            var result = await _service.RefreshToken(userId, sessionId, rt);

            return this.AuthResponse(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var uid = Request.Cookies["uid"];
            var sessionId = Request.Cookies["sessionId"];

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(sessionId))
                return this.ToActionResult(Result.Failure<AuthResult>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid logout info")));

            if (!long.TryParse(uid, out var userId))
            {
                return this.ToActionResult(Result.Failure<AuthResult>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid user id format")));
            }

            var result = await _service.Logout(userId, sessionId);

            if (result.IsSuccess)
                Response.ClearRefreshInfoCookies(Request);

            return this.ToActionResult(result);
        }


        #endregion

        #region Support Methods

        private IActionResult AuthResponse(Result<AuthTokensResult> result)
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
                    new AuthResult
                    {
                        AccessToken = result.Value.AccessToken,
                        AccessTokenMinutes = result.Value.AccessTokenMinutes
                    },
                    successCode: SuccessCodes.Auth.LoginSuccess));
            }

            return this.ToActionResult(Result.Failure<AuthResult>(result.Error));
        }

        #endregion
    }
}
