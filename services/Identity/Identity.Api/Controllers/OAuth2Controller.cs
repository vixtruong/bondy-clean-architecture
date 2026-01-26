using Identity.Api.Contracts.Auth;
using Identity.Api.Controllers.Base;
using Identity.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Controllers;

[ApiController]
//[AllowAnonymous]
[Route("api/v1/auth")]
public class OAuth2Controller : AuthControllerBase
{
    private const string FeUrl = "http://localhost:3000";
    private readonly IAuthService _service;

    public OAuth2Controller(IAuthService service)
    {
        _service = service;
    }

    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        var state = Guid.NewGuid().ToString("N");

        // store state in cookie
        Response.Cookies.Append("oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax
        });

        var uri = _service.BuildGoogleAuthorizationUri(state);
        return Redirect(uri);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallBack([FromQuery] string code, [FromQuery] string state)
    {
        var result = await _service.HandleGoogleCallbackAsync(code, state);

        return this.AuthRedirectResponse(result, FeUrl);
    }

    [HttpGet("discord/login")]
    public IActionResult DiscordLogin()
    {
        var state = Guid.NewGuid().ToString("N");

        // store state in cookie
        Response.Cookies.Append("oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax
        });

        var uri = _service.BuildDiscordAuthorizationUri(state);
        return Redirect(uri);
    }

    [HttpGet("discord/callback")]
    public async Task<IActionResult> DiscordCallback([FromQuery][Required] string code, [FromQuery] string? error = null)
    {
        var result = await _service.HandleDiscordCallbackAsync(code, error);

        return this.AuthRedirectResponse(result, FeUrl);
    }
}
