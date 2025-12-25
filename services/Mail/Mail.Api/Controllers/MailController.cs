using Bondy.Contracts.Dtos.Mail;
using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Mail.Application.Services.Mail;
using Microsoft.AspNetCore.Mvc;

namespace Mail.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MailController : ControllerBase
{
    private readonly IMailService _service;

    public MailController(IMailService service)
    {
        _service = service;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailDto dto)
        => this.ToActionResult(await _service.SendEmail(dto));
}
