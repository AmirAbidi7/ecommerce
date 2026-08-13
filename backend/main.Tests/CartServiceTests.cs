using main.Config;
using main.DTO.cart;
using main.Entity;
using main.Enum;
using main.Events;
using main.Service;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace main.Tests;

public class CartServiceTests : TestBase
{
    private Product CreateProduct(string name = "Laptop")
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Stock = 5,
            Price = 999.99f,
            ImageUrl = "http://img/laptop.png",
            Description = "A laptop",
            CreatedAt = DateTime.UtcNow,
        };
    }

    private AppUser CreateUser(Guid id)
    {
        return new AppUser
        {
            Id = id,
            Email = "user@example.com",
            FirstName = "Amir",
            LastName = "Abidi",
            Password = "hash",
        };
    }

    private (CartService Service, Mock<IProducerService> Producer) CreateService(AppDb db)
    {
        var producer = new Mock<IProducerService>();
        producer
            .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<PurchaseEvent>()))
            .Returns(Task.CompletedTask);
        return (new CartService(db, producer.Object), producer);
    }

    private async Task<(Cart Cart, AppUser User)> SeedUserWithCart(AppDb db)
    {
        var user = CreateUser(Guid.NewGuid());
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            Status = CartStatus.CREATED,
            Products = [],
            UserId = user.Id!.Value,
            User = user,
        };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
        return (cart, user);
    }

    [Fact]
    public async Task CreateCart_ShouldCreateCreatedCart()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.CreateCart(user.Id!.Value);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Empty(result.Items);
        var cart = await db.Carts.SingleAsync(c => c.Id == result.Id);
        Assert.Equal(CartStatus.CREATED, cart.Status);
    }

    [Fact]
    public async Task CreateCart_ShouldThrowWhenUserMissing()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService(db).Service.CreateCart(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCart_ShouldReturnActiveCartWithItems()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        db.CartItems.Add(new CartItem
        {
            CartId = cart.Id,
            ProductId = product.Id,
            ProductAmount = 2,
        });
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.GetCart(cart.UserId);

        Assert.Equal(cart.Id, result.Id);
        var item = Assert.Single(result.Items);
        Assert.Equal(product.Name, item.Product.Name);
        Assert.Equal(2, item.Amount);
    }

    [Fact]
    public async Task GetCart_ShouldThrowWhenNoActiveCart()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        db.Users.Add(user);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService(db).Service.GetCart(user.Id!.Value));
    }

    [Fact]
    public async Task AddToCart_ShouldCreateNewItem()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.AddToCart(
            new CartProductRequest(product.Id, cart.Id, 3));

        Assert.Equal(cart.Id, result.Id);
        var item = Assert.Single(result.Items);
        Assert.Equal(3, item.Amount);
    }

    [Fact]
    public async Task AddToCart_ShouldIncrementExistingItem()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        db.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = product.Id, ProductAmount = 2 });
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.AddToCart(
            new CartProductRequest(product.Id, cart.Id, 3));

        Assert.Equal(5, Assert.Single(result.Items).Amount);
    }

    [Fact]
    public async Task AddToCart_ShouldThrowWhenCartMissing()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateService(db).Service.AddToCart(new CartProductRequest(Guid.NewGuid(), Guid.NewGuid(), 1)));
    }

    [Fact]
    public async Task RemoveFromCart_ShouldDecrement()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        db.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = product.Id, ProductAmount = 5 });
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.RemoveFromCart(
            new CartProductRequest(product.Id, cart.Id, 2));

        Assert.Equal(3, Assert.Single(result.Items).Amount);
    }

    [Fact]
    public async Task RemoveFromCart_ShouldDeleteItemAtZero()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        db.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = product.Id, ProductAmount = 2 });
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.RemoveFromCart(
            new CartProductRequest(product.Id, cart.Id, 5));

        Assert.Empty(result.Items);
        Assert.Equal(0, await db.CartItems.CountAsync());
    }

    [Fact]
    public async Task RemoveFromCart_ShouldThrowWhenCartMissing()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateService(db).Service.RemoveFromCart(new CartProductRequest(Guid.NewGuid(), Guid.NewGuid(), 1)));
    }

    [Fact]
    public async Task RemoveFromCart_ShouldThrowWhenProductMissing()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            CreateService(db).Service.RemoveFromCart(new CartProductRequest(Guid.NewGuid(), cart.Id, 1)));
    }

    [Fact]
    public async Task RemoveFromCart_ShouldThrowWhenItemMissing()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateService(db).Service.RemoveFromCart(new CartProductRequest(product.Id, cart.Id, 1)));
    }

    [Fact]
    public async Task GetCarts_ShouldGroupItemsByCart()
    {
        using var db = CreateContext();
        var (cart1, _) = await SeedUserWithCart(db);
        var cart2 = new Cart
        {
            Id = Guid.NewGuid(),
            Status = CartStatus.PAID,
            Products = [],
            UserId = cart1.UserId,
            User = cart1.User,
        };
        db.Carts.Add(cart2);
        var product = CreateProduct();
        db.Products.Add(product);
        db.CartItems.AddRange(
            new CartItem { CartId = cart1.Id, ProductId = product.Id, ProductAmount = 1 },
            new CartItem { CartId = cart2.Id, ProductId = product.Id, ProductAmount = 2 });
        await db.SaveChangesAsync();

        var result = await CreateService(db).Service.GetCarts(cart1.UserId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == cart1.Id);
        Assert.Contains(result, c => c.Id == cart2.Id);
    }

    [Fact]
    public async Task PayCart_ShouldMarkPaidAndProduceEvent()
    {
        using var db = CreateContext();
        var (cart, user) = await SeedUserWithCart(db);
        var (service, producer) = CreateService(db);

        await service.PayCart(cart.Id);

        Assert.Equal(CartStatus.PAID, (await db.Carts.SingleAsync(c => c.Id == cart.Id)).Status);
        producer.Verify(p => p.ProduceAsync(
            "payment-completed",
            It.Is<PurchaseEvent>(e => e.UserEmail == user.Email && e.Cart.Id == cart.Id)),
            Times.Once);
    }

    [Fact]
    public async Task PayCart_ShouldThrowWhenAlreadyPaid()
    {
        using var db = CreateContext();
        var (cart, _) = await SeedUserWithCart(db);
        cart.Status = CartStatus.PAID;
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService(db).Service.PayCart(cart.Id));
    }

    [Fact]
    public async Task PayCart_ShouldThrowWhenCartMissing()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService(db).Service.PayCart(Guid.NewGuid()));
    }
}