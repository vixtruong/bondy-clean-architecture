using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Identity.Api.Http;
using Identity.Application.Services.Auth;
using Identity.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _service.LoginAsync(request);

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

            if (string.IsNullOrWhiteSpace(rt) || string.IsNullOrWhiteSpace(uid))
                return this.ToActionResult(Result.Failure<AuthResponse>(
                    Error.Unauthorized(ErrorCodes.Auth.Unauthorized, "Invalid refresh info")));

            var result = await _service.RefreshToken(new RefreshTokenRequest { Token = rt, UserId = long.Parse(uid) }
            );

            return this.AuthResponse(result);
        }

        #region private

        private IActionResult AuthResponse(Result<AuthTokens> result)
        {
            if (result.IsSuccess)
            {
                Response.SetRefreshInfoCookie(
                    Request,
                    userId: result.Value!.UserId,
                    refreshTokenRaw: result.Value!.RefreshTokenRaw,
                    days: AppConstant.RefreshTokenDays,
                    path: "/identity/api/v1/auth/refresh"
                );

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
