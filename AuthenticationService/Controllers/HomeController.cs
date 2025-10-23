using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("/health")]
        public async Task<IActionResult> Health()
        {
            return Ok("Authentication Service is running...");
        }
    }
}