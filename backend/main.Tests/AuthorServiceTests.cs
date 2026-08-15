using main.DTO.author;
using main.Entity;
using main.Enum;
using main.Service;

namespace main.Tests;

public class AuthorServiceTests : TestBase
{
    private Product CreateBook(Guid authorId, string name = "Book", float price = 100f)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Stock = 5,
            Price = price,
            ImageUrl = "http://img/b.png",
            Description = "desc",
            CreatedAt = DateTime.UtcNow,
            IsListed = true,
            AuthorId = authorId,
            Category = new Category { Id = Guid.NewGuid(), Name = "Fiction" },
        };
    }

    [Fact]
    public async Task GetMyBooks_ShouldReturnOnlyOwnListedAndUnlisted()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p", Role = UserRole.Author };
        var other = new AppUser { Id = Guid.NewGuid(), Email = "o@x.com", FirstName = "O", LastName = "P", Password = "p", Role = UserRole.Author };
        var mine = CreateBook(author.Id!.Value);
        var unlisted = CreateBook(author.Id!.Value, "Old", 50f);
        unlisted.IsListed = false;
        var theirs = CreateBook(other.Id!.Value, "Their", 10f);
        db.Users.AddRange(author, other);
        db.Products.AddRange(mine, unlisted, theirs);
        await db.SaveChangesAsync();

        var result = await new AuthorService(db).GetMyBooksAsync(author.Id!.Value);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b.Id == mine.Id && b.CategoryName == "Fiction");
        Assert.Contains(result, b => b.Id == unlisted.Id && !b.IsListed);
    }

    [Fact]
    public async Task GetMyBooks_ShouldIncludeActiveSale()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var book = CreateBook(author.Id!.Value);
        db.Users.Add(author);
        db.Products.Add(book);
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale { Id = Guid.NewGuid(), ProductId = book.Id, PercentOff = 25,
            StartsAt = DateTime.UtcNow.AddDays(-1), EndsAt = DateTime.UtcNow.AddDays(1),
            CreatedBy = author.Id!.Value, Product = null! });
        await db.SaveChangesAsync();

        var result = await new AuthorService(db).GetMyBooksAsync(author.Id!.Value);

        var dto = Assert.Single(result);
        Assert.True(dto.IsOnSale);
        Assert.Equal(75f, dto.EffectivePrice);
    }

    [Fact]
    public async Task GetSales_ShouldSumPaidQuantitiesAndRevenue()
    {
        using var db = CreateContext();
        var author = new AppUser { Id = Guid.NewGuid(), Email = "a@x.com", FirstName = "A", LastName = "B", Password = "p" };
        var book = CreateBook(author.Id!.Value);
        var buyer = new AppUser { Id = Guid.NewGuid(), Email = "b@x.com", FirstName = "B", LastName = "U", Password = "p" };
        var cart = new Cart { Id = Guid.NewGuid(), Status = CartStatus.PAID, Products = [], UserId = buyer.Id!.Value, User = buyer };
        db.Users.AddRange(author, buyer);
        db.Products.Add(book);
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
        db.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = book.Id, ProductAmount = 3 });
        await db.SaveChangesAsync();

        var result = await new AuthorService(db).GetSalesAsync(author.Id!.Value);

        var stat = Assert.Single(result);
        Assert.Equal(3, stat.UnitsSold);
        Assert.Equal(300f, stat.Revenue);
    }
}