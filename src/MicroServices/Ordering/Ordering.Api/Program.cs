using Shared.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("shoppingcart", c =>
{
    c.BaseAddress = new Uri("https://localhost:7298");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("api/orders/checkout", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
{
    // TODO: pobierz zawartosc koszyka, utworz zamowienie i zapisz w bazie danych

    var client = httpClientFactory.CreateClient("shoppingcart");

    var cart = await client.GetFromJsonAsync<Cart>("api/cart", ct);




});

app.Run();

