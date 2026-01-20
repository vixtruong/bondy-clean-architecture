using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Constants.Authorization;
using Identity.Application.Services.ApiKey;
using Identity.Contracts.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("ap1/v1/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _service;

    public ApiKeysController(IApiKeyService service)
    {
        _service = service;
    }

    [Authorize(Policy = Scopes.AdminApiKeysCreate)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest req)
    {
        return this.ToActionResult(await _service.Create(req));
    }

    [Authorize(Policy = Scopes.AdminApiKeysRotate)]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateApiKeyRequest req)
    {
        return this.ToActionResult(await _service.Update(req));
    }

    [Authorize(Policy = Scopes.AdminApiKeysRevoke)]
    [HttpPost("{apiKeyId}/revoke")]
    public async Task<IActionResult> Revoke([FromRoute] string apiKeyId)
    {
        return this.ToActionResult(await _service.Revoke(apiKeyId));
    }
}
