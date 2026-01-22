using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Application.Authorization.Scopes;
using Identity.Api.Contracts.ApiKey;
using Identity.Application.Services.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("ap1/v1/[controller]")]
[Authorize]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _service;

    public ApiKeysController(IApiKeyService service)
    {
        _service = service;
    }

    [Authorize(Policy = AdminApiKeyScopes.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest req)
    {
        return this.ToActionResult(
            await _service.Create(
                req.Name,
                req.Owner,
                req.OwnerEmail,
                req.Scopes,
                req.ExpiresAt));
    }

    [Authorize(Policy = AdminApiKeyScopes.Update)]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateApiKeyRequest req)
    {
        return this.ToActionResult(
            await _service.Update(
                req.ApiKeyId,
                req.Name,
                req.Scopes,
                req.ExpiresAt,
                req.IsActive));
    }

    [Authorize(Policy = AdminApiKeyScopes.Revoke)]
    [HttpPost("{apiKeyId}/revoke")]
    public async Task<IActionResult> Revoke([FromRoute] string apiKeyId)
    {
        return this.ToActionResult(await _service.Revoke(apiKeyId));
    }
}
