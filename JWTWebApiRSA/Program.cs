using JWTWebApiRSA;
using JWTWebApiRSA.Models;
using JWTWebApiRSA.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

// ساخت builder برنامه
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "JWT API", Version = "v1" });
    c.SchemaFilter<LoginModelExampleFilter>();
    // تعریف امنیت JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_**"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// بارگذاری تنظیمات JWT از appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

var privateKeyPath = Path.Combine(AppContext.BaseDirectory, "Keys", "private.pem");
var publicKeyPath = Path.Combine(AppContext.BaseDirectory, "Keys", "public.pem");

// بارگذاری کلید عمومی برای اعتبارسنجی توکن
var publicKeyText = File.ReadAllText(publicKeyPath);
var publicRsa = RSA.Create();
publicRsa.ImportFromPem(publicKeyText.ToCharArray());
var rsaPublicKey = new RsaSecurityKey(publicRsa);

// بارگذاری کلید خصوصی برای صدور توکن (در سرویس تولید توکن استفاده می‌شود)
var privateKeyText = File.ReadAllText(privateKeyPath);
var privateRsa = RSA.Create();
privateRsa.ImportFromPem(privateKeyText.ToCharArray());
var rsaPrivateKey = new RsaSecurityKey(privateRsa); // این خط برای نگهداری کلید خصوصی

// ثبت سرویس تولید توکن با کلید خصوصی
builder.Services.AddSingleton(new JwtTokenService(rsaPrivateKey, jwtSettings));

// پیکربندی احراز هویت JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = rsaPublicKey
        };
    });

// افزودن authorization و controller ها
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// فعال‌سازی Swagger در محیط توسعه
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
