using IdentityProvider.Api.Abstractions;
using IdentityProvider.Api.Infrastructures;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

var app = builder.Build();

app.MapGet("/", () => "Hello IdentityProvider.Api!");

app.MapPost("api/login", (LoginRequest request, IAuthService authService, ITokenService tokenService) =>
{
    var result = authService.Authorize(request.Username, request.Password);

    if (result.IsAuthenticated)
    {
        var accessToken = tokenService.GenerateAccessToken(result.identity);

        return Results.Ok(accessToken);
    }

    return Results.Unauthorized();
    
});

app.Run();

record LoginRequest(string Username, string Password);