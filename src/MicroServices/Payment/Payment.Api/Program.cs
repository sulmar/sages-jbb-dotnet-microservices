using Payment.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGet("/", () => "Hello Payment.Api!");

app.MapGrpcService<PaymentImplementation>();

app.Run();
