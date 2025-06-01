using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTWebApiRSA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("secure")]
        public IActionResult Secure()
        {
            return Ok($"دسترسی امن برای {User.Identity?.Name} فعال شد!");
        }
    }
}
