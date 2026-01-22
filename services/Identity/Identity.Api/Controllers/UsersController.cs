using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Application.Authorization.Scopes;
using Identity.Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [Authorize(Policy = ProfileScopes.Read)]
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetProfile()
        => this.ToActionResult(await _service.GetProfile());

    [Authorize(Policy = ProfileScopes.Update)]
    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateProfile()
        => this.ToActionResult(await _service.UpdateProfile());

    [Authorize(Policy = ProfileScopes.AvatarUpload)]
    [HttpPost("me/avatar")]
    public async Task<IActionResult> UploadAvatar()
        => this.ToActionResult(await _service.UploadAvatar());
}

