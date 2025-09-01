using Grpc.Core;
using PaymentService.Grcp;
using Shared.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("shoppingcart", c =>
{
    c.BaseAddress = new Uri("https://localhost:7298");
});

builder.Services.AddGrpcClient<PaymentService.Grcp.Payment.PaymentClient>(c =>
{
    c.Address = new Uri("https://localhost:7211");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("api/orders/checkout", async (
    IHttpClientFactory httpClientFactory, 
    CancellationToken ct,
    PaymentService.Grcp.Payment.PaymentClient paymentClient
    ) =>
{
    // TODO: pobierz zawartosc koszyka, utworz zamowienie i zapisz w bazie danych

    var client = httpClientFactory.CreateClient("shoppingcart");

    var cart = await client.GetFromJsonAsync<Cart>("api/cart", ct);


    var request = new PaymentService.Grcp.PaymentRequest
    {
        OrderId = Random.Shared.Next(),
        Amount = (double)cart.Total,
        Currency = "PLN"
    };

    // var response = await paymentClient.AuthorizePaymentAsync(request);

    var call = paymentClient.AuthorizePaymentStream(request);

    await foreach (var stage in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{stage.Stage}");

        var response = stage.Response;

        if (response.Status == PaymentStatus.Declined)
            return Results.BadRequest(new { message = response.Reason });
    }


    return Results.Accepted();

});

app.Run();

