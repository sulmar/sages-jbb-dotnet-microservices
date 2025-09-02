using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ProductCatalog.Api.Authorization;
using ProductCatalog.Api.Models;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Infrastructure;
using Shared.Domain.Entities;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<Context>(sp =>
{
    List<Product> products =
    [
        new Product { Id = 1, Name = "Popular Product", Price = 80.00m,  },
        new Product { Id = 2, Name = "Special Product", Price = 100.00m },
        new Product { Id = 3, Name = "Extra Product", Price = 120.00m, DiscountedPrice = 1 },
        new Product { Id = 4, Name = "Smart Product", Price = 40.00m },
        new Product { Id = 5, Name = "Oldschool Product", Price = 10.00m  },
    ];

    return new Context { Products = products.ToDictionary(p => p.Id) };
});

builder.Services.AddHealthChecks()
    .AddCheck("Ping", () => HealthCheckResult.Healthy("Pong"))
    .AddCheck("Random", () =>
    {
        if (DateTime.Now.Minute % 2 == 0)
            return HealthCheckResult.Healthy("Even minute");
        else
            return HealthCheckResult.Unhealthy("Odd minute");
    })
    ;

// dotnet add package StackExchange.Redis
var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(connectionString));

string secretKey = "a-string-secret-at-least-256-bits-long";

var key = System.Text.Encoding.UTF8.GetBytes(secretKey);

// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)    
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://www.sages.pl",
            ValidateAudience = true,
            ValidAudience = "https://jbb.pl",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };        

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // Dodatkowo: token z query ?access_token= (np. SSE/WS/SignalR)
                if (string.IsNullOrEmpty(ctx.Token) &&
                    ctx.Request.Query.TryGetValue("access_token", out var t))
                {
                    ctx.Token = t;
                }
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtDiag");

                var reason = ctx.Exception switch
                {
                    SecurityTokenExpiredException => "token_expired",
                    SecurityTokenInvalidAudienceException => "invalid_audience",
                    SecurityTokenInvalidIssuerException => "invalid_issuer",
                    SecurityTokenInvalidSignatureException => "invalid_signature",
                    SecurityTokenNoExpirationException => "no_exp_claim",
                    SecurityTokenInvalidLifetimeException => "invalid_lifetime",
                    SecurityTokenInvalidAlgorithmException => "invalid_alg",
                    _ => "auth_failed"
                };

                logger.LogError(ctx.Exception, "JWT auth failed: {Reason}", reason);

                // Uzupe³nij WWW-Authenticate dla 401
                ctx.Response.Headers["WWW-Authenticate"] =
                    $"Bearer error=\"invalid_token\", error_description=\"{reason}: {ctx.Exception.Message}\"";

                return Task.CompletedTask;
            },

            OnTokenValidated = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtDiag");

                var jwt = ctx.SecurityToken;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Adult", policy =>
    {
        policy.RequireRole("developer").RequireClaim(ClaimTypes.Email);
        policy.AddRequirements(new MinimumAge(18));
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<IAuthorizationHandler, MinimumAgeHandler>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Minimal Api

// Endpoints
app.MapGet("/", () => "Hello Products.Api!");


// GET api/products
app.MapGet("api/products", (IProductRepository repository) => repository.GetAll())
    .RequireAuthorization(); // [Authorize]


app.MapGet("api/products/{id}", (int id, IProductRepository repository) => repository.Get(id))
    .RequireAuthorization("Adult"); // [Authorize(Roles = "developer")]

// [Authorize(Policies="CanPrint")]


// app.MapPost("api/products", ([FromBody] Product product) => "Created.");

// PUT

// PATCH

const string PubSubChannel = "events:product-updated";

app.MapPatch("api/products/{id}/price", (int id, PriceDto dto, IConnectionMultiplexer connection) =>
{
    Console.WriteLine(dto.ToString());

    var sub = connection.GetSubscriber();

    var @event = new
    {
        type = "ProductUpdated",
        productId = id,
        newPrice = dto.Price,
    };

    sub.Publish(PubSubChannel, JsonSerializer.Serialize(@event));

    return Results.Ok();
  
});

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        await context.Response.WriteAsJsonAsync(report);
    }
});

// Middleware
app.UseHttpsRedirection();

app.Run();

