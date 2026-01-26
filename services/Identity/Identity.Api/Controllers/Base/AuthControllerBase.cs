using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Domain.Common;
using Identity.Api.Http;
using Identity.Application.Results.Auth;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Base;

public abstract class AuthControllerBase : ControllerBase
{
    protected IActionResult AuthResponse(Result<AuthTokensResult> result)
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

    protected IActionResult AuthRedirectResponse(Result<AuthTokensResult> result, string redirectUrl)
    {
        if (!result.IsSuccess)
        {
            // fallback: return failure JSON (controller can override)
            return this.ToActionResult(Result.Failure<AuthResult>(result.Error));
        }


        // set refresh cookie
        Response.SetRefreshInfoCookie(
            Request,
            userId: result.Value!.UserId,
            refreshTokenRaw: result.Value!.RefreshTokenRaw,
            sessionId: result.Value!.SessionId,
            days: TokenPolicy.RefreshTokenDays);


        // set access token cookie (short-lived) for convenience; optional
        Response.SetAccessTokenCookie(Request, result.Value.AccessToken, result.Value.AccessTokenMinutes);


        // redirect user to frontend
        return Redirect(redirectUrl);
    }
}
