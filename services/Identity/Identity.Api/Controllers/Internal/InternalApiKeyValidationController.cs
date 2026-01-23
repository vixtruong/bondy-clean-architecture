using Bondy.ServiceDefaults.Http;
using Identity.Api.Contracts.ApiKey;
using Identity.Application.Services.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Internal;

[ApiController]
[Route("api/v1/internal/apikeys")]
[AllowAnonymous]
public class InternalApiKeyValidationController : ControllerBase
{
    private readonly IApiKeyService _service;

    public InternalApiKeyValidationController(IApiKeyService service)
    {
        _service = service;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateApiKeyRequest req)
    {
        return this.ToActionResult(await _service.Validate(req.ApiKey));
    }
}
