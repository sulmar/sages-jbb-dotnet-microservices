var builder = WebApplication.CreateBuilder(args);

// dotnet add package Microsoft.Extensions.ServiceDiscovery
// dotnet add package Microsoft.Extensions.ServiceDiscovery.Yarp

builder.Services.AddServiceDiscovery();

// dotnet add package Yarp.ReverseProxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

// Middleware (warstwa poœrednia)
//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"#1 {context.Request.Method} {context.Request.Path}");

//    await next();

//    Console.WriteLine($"#1 {context.Response.StatusCode}");

//});

app.MapReverseProxy();

app.MapGet("/ping", () => "pong");


app.Run();
