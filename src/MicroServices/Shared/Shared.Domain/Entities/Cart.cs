namespace Shared.Domain.Entities;

public class Cart
{
    public string SessionId { get; set; }
    public List<CartItem> Items { get; set; }
    public decimal Total => Items.Sum(i => i.Amount);
}

public class CartItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount => Price * Quantity;
}

