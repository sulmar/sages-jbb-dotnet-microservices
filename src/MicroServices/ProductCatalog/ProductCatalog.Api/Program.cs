using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Infrastructure;
using Shared.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<Context>(sp =>
{
    List<Product> products =
    [
        new Product { Id = 1, Name = "Popular Product", Price = 80.00m,  },
        new Product { Id = 2, Name = "Special Product", Price = 100.00m },
        new Product { Id = 3, Name = "Extra Product", Price = 120.00m, DiscountedPrice = 1 },
        new Product { Id = 4, Name = "Smart Product", Price = 40.00m },
        new Product { Id = 5, Name = "Oldschool Product", Price = 10.00m  },
    ];

    return new Context { Products = products.ToDictionary(p => p.Id) };
});

builder.Services.AddHealthChecks()
    .AddCheck("Ping", () => HealthCheckResult.Healthy("Pong"))
    .AddCheck("Random", () =>
    {
        if (DateTime.Now.Minute % 2 == 0)
            return HealthCheckResult.Healthy("Even minute");
        else
            return HealthCheckResult.Unhealthy("Odd minute");
    })
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Minimal Api

// Endpoints
app.MapGet("/", () => "Hello Products.Api!");


// GET api/products
app.MapGet("api/products", (IProductRepository repository) => repository.GetAll());
app.MapGet("api/products/{id}", (int id, IProductRepository repository) => repository.Get(id));

// app.MapPost("api/products", ([FromBody] Product product) => "Created.");

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        await context.Response.WriteAsJsonAsync(report);
    }
});

// Middleware
app.UseHttpsRedirection();

app.Run();

