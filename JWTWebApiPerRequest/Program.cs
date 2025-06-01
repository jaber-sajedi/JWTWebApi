using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using JWTWebApiPerRequest.Services;
using Microsoft.AspNetCore.Authorization; // در صورت نیاز به namespace

var builder = WebApplication.CreateBuilder(args);

// Add Swagger + JWT auth
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "توکن را وارد کنید به صورت: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

// حافظه
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 500;
});

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateTokenReplay = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Authorization + JTI Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JtiPolicy", policy =>
        policy.Requirements.Add(new JtiRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, JtiAuthorizationHandler>();

builder.Services.AddSingleton<JwtService>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint: دریافت توکن
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

// Endpoint: محافظت‌شده
app.MapGet("/secure", () => "شما مجاز هستید!")
   .RequireAuthorization("JtiPolicy");

app.Run();
