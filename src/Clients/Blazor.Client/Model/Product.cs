namespace BlazorApp.Model;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice  { get; set; }
    public bool IsSale => Price > DiscountedPrice;

    public Product(int id, string name, decimal price, decimal? discountedPrice = null)
    {
        Id = id;
        Name = name;
        Price = price;

        if (discountedPrice.HasValue)
            DiscountedPrice = discountedPrice.Value;
        else
            DiscountedPrice = Price;
    }
}