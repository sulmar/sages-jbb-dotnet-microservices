using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;

namespace ShoppingCart.Infrastructure;

public class FakeCartRepository : ICartRepository
{
    public void Add(string sessionId, Product product)
    {
    }

    public void Delete(string sessionId, int productId)
    {
        throw new NotImplementedException();
    }
}
