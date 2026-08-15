using main.DTO.sale;
using main.Entity;
using main.Events;
using main.Service;
using Moq;

namespace main.Tests;

public class SaleServiceTests : TestBase
{
    private Product CreateProduct(Guid authorId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = "Book",
            Stock = 5,
            Price = 100f,
            ImageUrl = "http://img/b.png",
            Description = "desc",
            CreatedAt = DateTime.UtcNow,
            IsListed = true,
            AuthorId = authorId,
        };
    }

    [Fact]
    public async Task CreateSale_ShouldPublishPromotionToFavoritingUsers()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var fan = new AppUser { Id = Guid.NewGuid(), Email = "fan@x.com", FirstName = "F", LastName = "G", Password = "p" };
        var product = CreateProduct(author.Id!.Value);
        fan.FavoriteProducts = [product];
        db.Users.AddRange(author, fan);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var producer = new Mock<IProducerService>();
        var service = new SaleService(db, producer.Object);

        await service.CreateSaleAsync(product.Id, author.Id!.Value,
            new CreateSaleRequest(25, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)));

        producer.Verify(p => p.ProduceAsync("promotion-created",
            It.Is<PromotionEvent>(e =>
                e.RecipientEmails.SequenceEqual(new[] { "fan@x.com" }) &&
                e.PercentOff == 25 &&
                e.DiscountedPrice == 75f)),
            Times.Once);
    }

    [Fact]
    public async Task CreateSale_ShouldRejectOverlappingSale()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var product = CreateProduct(author.Id!.Value);
        db.Users.Add(author);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale { Id = Guid.NewGuid(), ProductId = product.Id, PercentOff = 10,
            StartsAt = DateTime.UtcNow, EndsAt = DateTime.UtcNow.AddDays(5), CreatedBy = author.Id!.Value,
            Product = null! });
        await db.SaveChangesAsync();

        var service = new SaleService(db, new Mock<IProducerService>().Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateSaleAsync(product.Id, author.Id!.Value,
                new CreateSaleRequest(20, DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(10))));
    }

    [Fact]
    public async Task CreateSale_ShouldRejectNonOwner()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var stranger = new AppUser { Id = Guid.NewGuid(), Email = "s@x.com", FirstName = "S", LastName = "T", Password = "p" };
        var product = CreateProduct(author.Id!.Value);
        db.Users.AddRange(author, stranger);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var service = new SaleService(db, new Mock<IProducerService>().Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateSaleAsync(product.Id, stranger.Id!.Value,
                new CreateSaleRequest(20, DateTime.UtcNow, DateTime.UtcNow.AddDays(1))));
    }

    [Fact]
    public async Task CancelSale_ShouldRemoveActiveSales()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var product = CreateProduct(author.Id!.Value);
        db.Users.Add(author);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale { Id = Guid.NewGuid(), ProductId = product.Id, PercentOff = 10,
            StartsAt = DateTime.UtcNow.AddDays(-1), EndsAt = DateTime.UtcNow.AddDays(1), CreatedBy = author.Id!.Value,
            Product = null! });
        await db.SaveChangesAsync();

        var service = new SaleService(db, new Mock<IProducerService>().Object);
        await service.CancelSaleAsync(product.Id, author.Id!.Value);

        Assert.Empty(db.Sales);
    }
}