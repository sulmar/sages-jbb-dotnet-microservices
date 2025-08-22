using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;
using StackExchange.Redis;

namespace ShoppingCart.Infrastructure;

// dotnet add package StackExchange.Redis
public class RedisCartRepository(IConnectionMultiplexer connection) : ICartRepository
{
    private readonly IDatabase db = connection.GetDatabase(0);

    public void Add(string sessionId, Product product)
    {
        string key = $"cart:{sessionId}";
        string field = $"product:{product.Id}";

        db.HashIncrement(key, field); // HINCRBY key field 1
        db.KeyExpire(key, TimeSpan.FromMinutes(3)); // EXPIRE key time
    }

    public void Delete(string sessionId, int productId)
    {
        string key = $"cart:{sessionId}";
        string field = $"product:{productId}";

        var quantity = db.HashDecrement(key, field);

        if (quantity == 0)
            db.HashDelete(key, field);

        db.KeyExpire(key, TimeSpan.FromMinutes(3)); // EXPIRE key time
    }
}
