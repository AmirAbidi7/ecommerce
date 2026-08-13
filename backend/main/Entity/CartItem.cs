using System.ComponentModel.DataAnnotations.Schema;

namespace main.Entity;

public class CartItem
{
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
    public int ProductAmount { get; set; }
    public Guid CartId { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart Cart { get; set; }
}
