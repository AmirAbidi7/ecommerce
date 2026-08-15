using System.Security.Claims;
using main.Config;
using main.Controller;
using main.Entity;
using main.Enum;
using main.Events;
using main.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace main.Tests;

public class CartControllerTests : TestBase
{
    private (CartController Controller, Mock<IProducerService> Producer) CreateController(
        AppDb db, Guid userId)
    {
        var http = new DefaultHttpContext();
        http.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()) }, "Test"));
        var producer = new Mock<IProducerService>();
        producer
            .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<PurchaseEvent>()))
            .Returns(Task.CompletedTask);
        var controller = new CartController(new CartService(db, producer.Object))
        {
            ControllerContext = new ControllerContext { HttpContext = http },
        };
        return (controller, producer);
    }

    private async Task<AppUser> SeedUser(AppDb db, Guid userId)
    {
        var user = new AppUser
        {
            Id = userId,
            Email = "u@example.com",
            FirstName = "A",
            LastName = "B",
            Password = "h",
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task GetCart_ShouldReturnActiveCart()
    {
        using var db = CreateSqliteContext();
        var userId = Guid.NewGuid();
        await SeedUser(db, userId);
        var (controller, _) = CreateController(db, userId);

        await controller.CreateCart();
        var result = await controller.GetCart();

        Assert.NotNull(result.Value);
        Assert.Equal(userId, (await db.Carts.SingleAsync()).UserId);
    }

    [Fact]
    public async Task PayCart_ShouldMarkPaid()
    {
        using var db = CreateSqliteContext();
        var userId = Guid.NewGuid();
        var user = await SeedUser(db, userId);
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            Status = CartStatus.CREATED,
            Products = [],
            UserId = userId,
            User = user,
        };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
        var (controller, producer) = CreateController(db, userId);

        var result = await controller.PayCart(cart.Id);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(CartStatus.PAID, (await db.Carts.SingleAsync()).Status);
        producer.Verify(p => p.ProduceAsync("payment-completed", It.IsAny<PurchaseEvent>()), Times.Once);
    }
}