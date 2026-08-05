using main.Config;
using main.DTO.cart;
using main.Entity;
using main.Enum;
using Microsoft.EntityFrameworkCore;

namespace main.Service;

public class CartService(AppDb db)
{
    public async Task<CartOverView> CreateCart(Guid userId)
    {
        var user =
            await db.Users.FirstOrDefaultAsync(user => user.Id == userId)
            ?? throw new KeyNotFoundException("User not found!");
        var cart = new Cart
        {
            Status = CartStatus.CREATED,
            Products = [],
            UserId = userId,
            User = user,
        };

        await db.Carts.AddAsync(cart);
        await db.SaveChangesAsync();
        return new CartOverView(cart.Id, new List<CartProduct>());
    }

    public async Task<CartOverView> AddToCart(Guid productId, Guid cartId, int productAmount)
    {
        var cart =
            await db
                .Carts.Include(cart => cart.Products)
                .FirstOrDefaultAsync(cart => cart.Id == cartId)
            ?? throw new KeyNotFoundException("Cart not found!");

        ICollection<CartItem> cartItems = await db
            .CartItems.Where(ci => ci.CartId == cartId)
            .Include(ci => ci.Product)
            .ToListAsync();
        var item = await db.CartItems.FirstOrDefaultAsync(ci =>
            ci.CartId == cartId && ci.ProductId == productId
        );
        if (item is null)
        {
            item = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                ProductAmount = productAmount,
            };
            cartItems.Add(item);
            await db.CartItems.AddAsync(item);
        }
        else
        {
            item.ProductAmount += productAmount;
        }
        await db.SaveChangesAsync();
        return new CartOverView(cartId, cartItems);
    }
}
