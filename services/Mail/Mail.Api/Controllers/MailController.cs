using Bondy.ServiceDefaults.Http;
using Mail.Api.Contracts;
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
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        => this.ToActionResult(await _service.SendEmail(request.To, request.Purpose, request.Data, request.DedupTokenId));
}
