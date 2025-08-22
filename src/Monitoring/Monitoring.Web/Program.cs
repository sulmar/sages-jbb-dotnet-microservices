var builder = WebApplication.CreateBuilder(args);

// dotnet add package AspNetCore.HealthChecks.UI
// dotnet add package AspNetCore.HealthChecks.UI.Client
// dotnet add package AspNetCore.HealthChecks.UI.InMemory.Storage


// dotnet add package AspNetCore.HealthChecks.UI.Sqlite.Storage

builder.Services.AddHealthChecksUI(options =>
{
    options
        .AddHealthCheckEndpoint("ProductCatalog", "https://localhost:7251/hc")
        .AddHealthCheckEndpoint("Cart", "https://localhost:7298/hc");

}).AddSqliteStorage(builder.Configuration.GetConnectionString("storage"));

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();

// /healthchecks-ui
app.MapHealthChecksUI();

app.Run();
