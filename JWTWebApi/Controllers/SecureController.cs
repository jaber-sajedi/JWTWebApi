using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] // مسیر: api/secure
    public class SecureController : ControllerBase
    {
        
        [HttpGet("data")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "این اطلاعات فقط برای کاربران احراز هویت‌شده قابل دسترسی است." });
        }
    }

}
