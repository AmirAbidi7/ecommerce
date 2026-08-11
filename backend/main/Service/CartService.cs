using main.Config;
using main.DTO.cart;
using main.Entity;
using main.Enum;
using main.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace main.Service;

public class CartService(AppDb db, ProducerService producerService)
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

    public async Task<CartOverView> GetCart(Guid userId)
    {
        var cart = await db.Carts.FirstOrDefaultAsync(cart =>
            cart.UserId == userId && cart.Status == CartStatus.CREATED
        );

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found");
        }

        var items = await db
            .CartItems.Where(cartItems => cartItems.CartId == cart.Id)
            .Include(item => item.Product)
            .ToListAsync();

        return new CartOverView(cart.Id, items);
    }

    public async Task<CartOverView> AddToCart(CartProductRequest cartProductRequest)
    {
        var cart =
            await db
                .Carts.Include(cart => cart.Products)
                .FirstOrDefaultAsync(cart => cart.Id == cartProductRequest.cartId)
            ?? throw new KeyNotFoundException("Cart not found!");

        ICollection<CartItem> cartItems = await db
            .CartItems.Where(ci => ci.CartId == cartProductRequest.cartId)
            .Include(ci => ci.Product)
            .ToListAsync();
        var item = await db.CartItems.FirstOrDefaultAsync(ci =>
            ci.CartId == cartProductRequest.cartId && ci.ProductId == cartProductRequest.productId
        );
        if (item is null)
        {
            item = new CartItem
            {
                CartId = cartProductRequest.cartId,
                ProductId = cartProductRequest.productId,
                ProductAmount = cartProductRequest.productAmount,
            };
            cartItems.Add(item);
            await db.CartItems.AddAsync(item);
        }
        else
        {
            item.ProductAmount += cartProductRequest.productAmount;
        }
        await db.SaveChangesAsync();
        return new CartOverView(cartProductRequest.cartId, cartItems);
    }

    public async Task<CartOverView> RemoveFromCart(CartProductRequest cartProductRequest)
    {
        var cart =
            await db
                .Carts.Include(cart => cart.Products)
                .FirstOrDefaultAsync(cart => cart.Id == cartProductRequest.cartId)
            ?? throw new KeyNotFoundException("Cart not found");

        var productExists = await db.Products.AnyAsync(product =>
            product.Id == cartProductRequest.productId
        );
        if (!productExists)
        {
            throw new KeyNotFoundException("Invalid product!");
        }

        ICollection<CartItem> cartItems = await db
            .CartItems.Where(ci => ci.CartId == cartProductRequest.cartId)
            .Include(ci => ci.Product)
            .ToListAsync();
        var item = await db.CartItems.FirstOrDefaultAsync(ci =>
            ci.CartId == cartProductRequest.cartId && ci.ProductId == cartProductRequest.productId
        );
        if (item == null)
        {
            throw new InvalidOperationException("proudct doesn't exist");
        }
        else
        {
            item.ProductAmount -= cartProductRequest.productAmount;
            if (item.ProductAmount <= 0)
            {
                db.CartItems.Remove(item);
            }
        }
        await db.SaveChangesAsync();

        return new CartOverView(cartProductRequest.cartId, cartItems);
    }

    public async Task<ICollection<CartOverView>> GetCarts(Guid userId)
    {
        ICollection<CartItem> items = await db
            .CartItems.Where(ci => ci.Cart.UserId == userId)
            .Include(ci => ci.Product)
            .ToListAsync();
        if (items == null)
        {
            return new List<CartOverView>([]);
        }
        return [.. items.GroupBy(ci => ci.CartId).Select(g => new CartOverView(g.Key, g.ToList()))];
    }

    public async Task PayCart(Guid cartId)
    {
        var cart =
            await db.Carts.FirstOrDefaultAsync(cart => cart.Id == cartId)
            ?? throw new KeyNotFoundException("Cart not found!");
        if (cart.Status == CartStatus.PAID)
        {
            throw new InvalidOperationException("Cart already paid");
        }
        cart.Status = CartStatus.PAID;
        await db.SaveChangesAsync();
        var items = await db
            .CartItems.Where(items => items.CartId == cartId)
            .Include(items => items.Product)
            .ToListAsync();
        var user =
            await db.Users.FirstOrDefaultAsync(user => user.Id == cart.UserId)
            ?? throw new KeyNotFoundException("User not found !");
        producerService.ProduceAsync(
            "payment-completed",
            new PurchaseEvent(user.Email, new CartOverView(cartId, items))
        );
    }
}
