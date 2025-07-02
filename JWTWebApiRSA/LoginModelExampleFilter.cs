using JWTWebApiRSA.Models.JwtRsaAuthExample.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JWTWebApiRSA
{
    public class LoginModelExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginRequest))
            {
                schema.Example = new OpenApiObject
                {
                    ["username"] = new OpenApiString("admin"),
                    ["password"] = new OpenApiString("1234")
                };
            }
        }
    }
}
