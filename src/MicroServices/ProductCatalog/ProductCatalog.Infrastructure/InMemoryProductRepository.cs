using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure;

public class Context
{
    public IDictionary<int, Product> Products { get; set; } = new Dictionary<int, Product>();
}

// Primary Constructor
public class InMemoryProductRepository(Context _context) : IProductRepository
{
    public void Add(Product entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Product Get(int id)
    {
        _context.Products.TryGetValue(id, out var product);

        return product;
    }

    public List<Product> GetAll() => _context.Products.Values.ToList();

    public void Update(Product entity)
    {
        throw new NotImplementedException();
    }
}
