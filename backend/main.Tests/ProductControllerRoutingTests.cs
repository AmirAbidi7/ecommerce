using System.Reflection;
using main.Controller;
using Microsoft.AspNetCore.Mvc;

namespace main.Tests;

public class ProductControllerRoutingTests
{
    [Fact]
    public void GetProduct_ShouldUseRouteParameter()
    {
        var template = typeof(ProductController)
            .GetMethod(nameof(ProductController.GetProduct))!
            .GetCustomAttributes(typeof(HttpGetAttribute), true)
            .Cast<HttpGetAttribute>().Single().Template;

        Assert.Contains("{", template);
    }

    [Fact]
    public void FavoriteAndUnfavorite_ShouldHaveDistinctRoutes()
    {
        var favorite = typeof(ProductController)
            .GetMethod(nameof(ProductController.FavoriteProduct))!
            .GetCustomAttributes(typeof(HttpPutAttribute), true)
            .Cast<HttpPutAttribute>().Single().Template;
        var unfavorite = typeof(ProductController)
            .GetMethod(nameof(ProductController.UnfavoriteProduct))!
            .GetCustomAttributes(typeof(HttpPutAttribute), true)
            .Cast<HttpPutAttribute>().Single().Template;

        Assert.Equal("favorite", favorite);
        Assert.Equal("unfavorite", unfavorite);
    }
}