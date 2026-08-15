using main.Entity;
using main.Service;
using Microsoft.EntityFrameworkCore;

namespace main.Tests;

public class ProductServiceTests : TestBase
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
            IsListed = true,
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

    [Fact]
    public async Task GetProducts_ShouldReturnAll()
    {
        using var db = CreateContext();
        db.Products.AddRange(CreateProduct(), CreateProduct("Phone"));
        await db.SaveChangesAsync();

        var result = await new ProductService(db).GetProductsAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.False(string.IsNullOrEmpty(p.Name)));
    }

    [Fact]
    public async Task GetProduct_ShouldReturnDetails()
    {
        using var db = CreateContext();
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var result = await new ProductService(db).GetProductAsync(product.Id);

        Assert.Equal(product.Name, result.Name);
        Assert.Equal("A laptop", result.Description);
    }

    [Fact]
    public async Task GetProduct_ShouldThrowWhenMissing()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => new ProductService(db).GetProductAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Favorite_ShouldAddProduct()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        var product = CreateProduct();
        db.Users.Add(user);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await new ProductService(db).FavoriteProduct(product.Id, user.Id!.Value);

        var count = await db.Users.Where(u => u.Id == user.Id)
            .SelectMany(u => u.FavoriteProducts).CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Favorite_ShouldRejectDuplicate()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        var product = CreateProduct();
        user.FavoriteProducts = [product];
        db.Users.Add(user);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => new ProductService(db).FavoriteProduct(product.Id, user.Id!.Value));
    }

    [Fact]
    public async Task Favorite_ShouldThrowWhenUserMissing()
    {
        using var db = CreateContext();
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => new ProductService(db).FavoriteProduct(product.Id, Guid.NewGuid()));
    }

    [Fact]
    public async Task Favorite_ShouldThrowWhenProductMissing()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        db.Users.Add(user);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => new ProductService(db).FavoriteProduct(Guid.NewGuid(), user.Id!.Value));
    }

    [Fact]
    public async Task Unfavorite_ShouldRemoveProduct()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        var product = CreateProduct();
        user.FavoriteProducts = [product];
        db.Users.Add(user);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await new ProductService(db).UnfavoriteProduct(product.Id, user.Id!.Value);

        var count = await db.Users.Where(u => u.Id == user.Id)
            .SelectMany(u => u.FavoriteProducts).CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Unfavorite_ShouldRejectWhenNotFavorited()
    {
        using var db = CreateContext();
        var user = CreateUser(Guid.NewGuid());
        var product = CreateProduct();
        db.Users.Add(user);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => new ProductService(db).UnfavoriteProduct(product.Id, user.Id!.Value));
    }

    [Fact]
    public async Task GetProducts_ShouldIncludeActiveSale()
    {
        using var db = CreateContext();
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale
        {
            Id = Guid.NewGuid(), ProductId = product.Id, PercentOff = 25,
            StartsAt = DateTime.UtcNow.AddDays(-1), EndsAt = DateTime.UtcNow.AddDays(1),
            CreatedBy = Guid.NewGuid(), Product = null!
        });
        await db.SaveChangesAsync();

        var result = await new ProductService(db).GetProductsAsync();

        var dto = Assert.Single(result);
        Assert.True(dto.IsOnSale);
        Assert.Equal(25, dto.SalePercent);
        Assert.Equal(Pricing.EffectivePrice(product.Price, 25), dto.EffectivePrice);
    }

    [Fact]
    public async Task GetProduct_ShouldNotApplyExpiredSale()
    {
        using var db = CreateContext();
        var product = CreateProduct();
        db.Products.Add(product);
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale
        {
            Id = Guid.NewGuid(), ProductId = product.Id, PercentOff = 25,
            StartsAt = DateTime.UtcNow.AddDays(-2), EndsAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = Guid.NewGuid(), Product = null!
        });
        await db.SaveChangesAsync();

        var result = await new ProductService(db).GetProductAsync(product.Id);

        Assert.False(result.IsOnSale);
        Assert.Equal(product.Price, result.EffectivePrice);
    }
}