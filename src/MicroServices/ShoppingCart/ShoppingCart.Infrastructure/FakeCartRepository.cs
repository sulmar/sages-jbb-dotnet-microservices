using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;

namespace ShoppingCart.Infrastructure;

public class FakeCartRepository : ICartRepository
{
    public void Add(string sessionId, Product product)
    {
    }
}
