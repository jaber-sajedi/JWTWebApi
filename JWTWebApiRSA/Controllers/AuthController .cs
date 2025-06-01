

using JWTWebApiRSA.Models.JwtRsaAuthExample.Models;
using JWTWebApiRSA.Services;
using Microsoft.AspNetCore.Mvc;

namespace JWTWebApiRSA.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // در اینجا شما می‌توانید بررسی واقعی نام‌کاربری/رمزعبور را انجام دهید
        if (request.Username == "admin" && request.Password == "1234")
        {
            var token = _jwtTokenService.GenerateToken(request.Username);
            return Ok(new { token });
        }

        return Unauthorized("Invalid credentials");
    }
}
