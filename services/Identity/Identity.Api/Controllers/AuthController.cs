using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("hello")]
        public IActionResult test()
        {
            return Ok("abc");
        }
    }
}
