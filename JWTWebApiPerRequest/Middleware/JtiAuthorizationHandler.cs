using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

public class JtiAuthorizationHandler : AuthorizationHandler<JtiRequirement>
{
    private readonly IMemoryCache _cache;

    public JtiAuthorizationHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JtiRequirement requirement)
    {
        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (!string.IsNullOrEmpty(jti) && _cache.TryGetValue(jti, out _))
        {
            _cache.Remove(jti); // 👈 بعد از استفاده، حذف از کش
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

}
