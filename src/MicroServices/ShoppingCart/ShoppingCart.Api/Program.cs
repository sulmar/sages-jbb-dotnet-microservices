using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Domain.Entities;
using ShoppingCart.Api.BackgroundServices;
using ShoppingCart.Api.Hubs;
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

// dotnet add package AspNetCore.HealthChecks.Redis
builder.Services.AddHealthChecks()
    .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>());

builder.Services.AddSignalR();


// builder.Services.AddHostedService<MyWorker>();
builder.Services.AddHostedService<NotificationWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/ping", () => "pong");

app.MapPost("api/cart", async (Product product, ICartRepository repository, HttpContext context, IHubContext<CartHub> hubContext) =>
{
    //string? session = context.Session?.Id;

    //if (string.IsNullOrEmpty(session))
    var session = "user:1";   // fallback

    repository.Add(session, product);

    // await hubContext.Clients.All.SendAsync("CartChanged", 10);

    await hubContext.Clients.Group(session).SendAsync("CartChanged", 10);

});

// DELETE api/cart
app.MapDelete("api/cart", async ([FromBody] Product product, [FromServices] ICartRepository repository, IHubContext<CartHub> hubContext) =>
{

    var session = "user:1";   // fallback

    repository.Delete(session, product.Id);


    await hubContext.Clients.Group(session).SendAsync("CartChanged", 0);
});

// GET api/cart

app.MapGet("api/cart", (ICartRepository repository) =>
{
    var session = "user:1";

    var cart = repository.Get(session);

    return Results.Ok(cart);

});


app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        await context.Response.WriteAsJsonAsync(report);
    }
});


app.MapHub<CartHub>("signalr/cart");


app.Run();

