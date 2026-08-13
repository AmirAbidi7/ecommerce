using System.Reflection;
using main.Controller;
using Microsoft.AspNetCore.Mvc;

namespace main.Tests;

public class CartControllerRoutingTests
{
    [Fact]
    public void PayCart_ShouldBeHttpPostWithCartIdRoute()
    {
        var attr = typeof(CartController)
            .GetMethod(nameof(CartController.PayCart))!
            .GetCustomAttributes(typeof(HttpPostAttribute), true)
            .Cast<HttpPostAttribute>().Single();

        Assert.Equal("{cartId}", attr.Template);
    }
}