namespace main.Entity;

public class CartItem
{
    public Guid ProductId { get; set; }
    public int ProductAmount { get; set; }
    public Guid CartId { get; set; }
}
