using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Application.Authorization.Scopes;
using Identity.Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Internal;

[ApiController]
[Route("api/v1/internal/user-profiles")]
[Authorize(Policy = ProfileScopes.Read)]
public class UserProfilesInternalController : ControllerBase
{
    private readonly IUserService _service;

    public UserProfilesInternalController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBasicProfile(long userId)
        => this.ToActionResult(await _service.GetBasicProfile(userId));

    [HttpPost("batch")]
    public async Task<IActionResult> GetBasicProfiles(
        [FromBody] IReadOnlyCollection<long> userIds)
        => this.ToActionResult(await _service.GetBasicProfiles(userIds));
}

