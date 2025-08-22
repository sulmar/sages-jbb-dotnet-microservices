using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;
using ShoppingCart.Infrastructure;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(connectionString));
builder.Services.AddScoped<ICartRepository, RedisCartRepository>();

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
    var session = "user:1";   // fallback

    repository.Add(session, product);
});

// DELETE api/cart
app.MapDelete("api/cart", ([FromBody] Product product, [FromServices] ICartRepository repository) =>
{

    var session = "user:1";   // fallback

    repository.Delete(session, product.Id);
});


app.UseHttpsRedirection();

app.Run();

