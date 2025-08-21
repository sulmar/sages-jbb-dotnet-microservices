using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;
using ShoppingCart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICartRepository, FakeCartRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/ping", () => "pong");

app.MapPost("api/cart", (Product product, ICartRepository repository, HttpContext context) =>
{
    //string? session = context.Session?.Id;

    //if (string.IsNullOrEmpty(session))
    var session = "default-session-001";   // fallback

    repository.Add(session, product);
});

app.UseHttpsRedirection();

app.Run();

