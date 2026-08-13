using main.DTO.product;
using main.Entity;

namespace main.DTO.cart;

public record CartOverView(Guid Id, ICollection<CartProduct> Items)
{
    public CartOverView(Guid cartId, ICollection<CartItem> cartItems)
        : this(
            cartId,
            cartItems
                .Select(ci => new CartProduct(
                    ci.ProductId,
                    new ProductOverview(ci.Product),
                    ci.ProductAmount
                ))
                .ToList()
        ) { }
}

public record CartProduct(Guid Id, ProductOverview Product, int Amount);
