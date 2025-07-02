using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class LoginModelExampleFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(JWTWebApi.Models.LoginModel))
        {
            schema.Example = new OpenApiObject
            {
                ["username"] = new OpenApiString("admin"),
                ["password"] = new OpenApiString("1234")
            };
        }
    }
}
