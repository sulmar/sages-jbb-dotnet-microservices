using Shared.Domain.Entities;
using ShoppingCart.Domain.Abstractions;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure;

public class FakeCartRepository : ICartRepository
{
    public void Add(string sessionId, Product product)
    {
        throw new NotImplementedException();
    }

    public void Delete(string sessionId, int productId)
    {
        throw new NotImplementedException();
    }

    public Cart? Get(string sessionId)
    {
        throw new NotImplementedException();
    }
}
