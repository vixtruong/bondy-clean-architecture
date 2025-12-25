using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Microsoft.AspNetCore.Mvc;

namespace Mail.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MailController : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail()
        => this.ToActionResult(Result.Success());
}
