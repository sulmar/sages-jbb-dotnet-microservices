using Shared.Domain.Entities;

namespace ShoppingCart.Domain.Abstractions;

public interface ICartRepository
{
    void Add(string sessionId, Product product);
}
