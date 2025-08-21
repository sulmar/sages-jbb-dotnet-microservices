using ProductCatalog.Domain.Entities;
using Shared.Domain.Entities;

namespace ProductCatalog.Domain.Abstractions;

// Szablon - interfejs generyczny
public interface IEntityRepository<T>
{
    List<T> GetAll();
    T Get(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}


public interface IProductRepository : IEntityRepository<Product>
{
}

public interface ICategoryRepository : IEntityRepository<Category>
{
}