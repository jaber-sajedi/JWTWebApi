using Microsoft.Extensions.Caching.Memory;

namespace JWTWebApiPerRequest.Controllers
{
    public static class MinimalApiEndpoints
    {
        public static void Register(WebApplication app)
        {
            app.MapPost("/token", (JwtService jwtService, IMemoryCache cache) =>
            {
                var token = jwtService.GenerateToken("user1", out var jti);

                cache.Set(jti, true, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(6),
                    Size = 1
                });

                return Results.Ok(new { token });
            });

            app.MapGet("/secure", () => "شما مجاز هستید!")
               .RequireAuthorization("JtiPolicy");
        }
    }

}
