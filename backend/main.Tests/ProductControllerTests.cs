using System.Security.Claims;
using main.Config;
using main.Controller;
using main.Entity;
using main.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace main.Tests;

public class ProductControllerTests : TestBase
{
    private ProductController CreateController(AppDb db, Guid? userId = null)
    {
        var http = new DefaultHttpContext();
        if (userId is not null)
        {
            http.User = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()!) }, "Test"));
        }
        return new ProductController(new ProductService(db))
        {
            ControllerContext = new ControllerContext { HttpContext = http },
        };
    }

    [Fact]
    public async Task GetProducts_ShouldReturnList()
    {
        using var db = CreateContext();
        db.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Stock = 5,
            Price = 1,
            ImageUrl = "u",
            Description = "d",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        var result = await CreateController(db).GetProducts();

        Assert.Single(Assert.IsType<OkObjectResult>(result.Result).Value as System.Collections.ICollection);
    }

    [Fact]
    public async Task Favorite_ShouldWorkWithAuthenticatedUser()
    {
        using var db = CreateContext();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Stock = 5,
            Price = 1,
            ImageUrl = "u",
            Description = "d",
            CreatedAt = DateTime.UtcNow,
        };
        db.Users.Add(new AppUser
        {
            Id = userId,
            Email = "u@example.com",
            FirstName = "A",
            LastName = "B",
            Password = "h",
        });
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var controller = CreateController(db, userId);
        var result = await controller.FavoriteProduct(product.Id);
        Assert.IsType<OkResult>(result);

        var count = await db.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.FavoriteProducts).CountAsync();
        Assert.Equal(1, count);

        var unfavorite = await controller.UnfavoriteProduct(product.Id);
        Assert.IsType<OkResult>(unfavorite);
    }
}