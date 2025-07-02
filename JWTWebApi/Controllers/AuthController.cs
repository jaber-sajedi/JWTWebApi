using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // مسیر API مثل: api/auth
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] Models.LoginModel model) // متدی برای ورود (Login)
        {
            // بررسی اطلاعات کاربر (اینجا به صورت ثابت برای تست استفاده شده)
            if (model.Username == "admin" && model.Password == "1234")
            {
                // تعریف اطلاعات کاربر در قالب Claims (ادعاها)
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, model.Username), // نام کاربر
                new Claim(ClaimTypes.Role, "Admin")         // نقش کاربر (برای کنترل دسترسی)
            };

                // تعریف کلید سری برای امضای توکن
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyUltraStrongSecretKeyWithEnoughLength123!"));

                // تعریف الگوریتم امضا
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // ساخت توکن
                var token = new JwtSecurityToken(
                     issuer: "https://localhost:7265",
                          audience: "https://localhost",
                    claims: claims,              // ادعاهای کاربر
                    expires: DateTime.UtcNow.AddHours(1), // مدت اعتبار توکن (۱ ساعت)
                    signingCredentials: creds    // امضای دیجیتال توکن
                );

                // بازگرداندن توکن به کلاینت
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return Unauthorized(); // اگر اطلاعات ورود نادرست باشد
        }
    }

}
