var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("api/orders/checkout", () =>
{
    // TODO: pobierz zawartosc koszyka, utworz zamowienie i zapisz w bazie danych
});

app.Run();

