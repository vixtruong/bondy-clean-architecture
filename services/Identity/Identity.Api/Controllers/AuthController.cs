using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
            => this.ToActionResult(await _service.LoginAsync(request, ct));
    }
}
