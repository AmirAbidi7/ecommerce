using System.Reflection;
using main.Controller;
using Microsoft.AspNetCore.Authorization;
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

    [Fact]
    public void CreateProduct_ShouldBeHttpPostWithAuthorize()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.CreateProduct))!;

        Assert.NotNull(method.GetCustomAttributes(typeof(HttpPostAttribute), true).Single());
        var authorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }

    [Fact]
    public void UnlistProduct_ShouldBeHttpDeleteWithAuthorize()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.UnlistProduct))!;

        var httpDelete = method.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Cast<HttpDeleteAttribute>().Single();
        Assert.Equal("{productId:guid}", httpDelete.Template);
        var authorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }

    [Fact]
    public void GetFavoriteIds_ShouldBeHttpGetWithAuthorize()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.GetFavoriteIds))!;

        var httpGet = method.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>().Single();
        Assert.Equal("favorites", httpGet.Template);
        Assert.NotNull(method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Single());
    }

    [Fact]
    public void CreateSale_ShouldBeHttpPostWithAuthorize()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.CreateSale))!;

        var httpPost = method.GetCustomAttributes(typeof(HttpPostAttribute), true).Cast<HttpPostAttribute>().Single();
        Assert.Equal("{productId:guid}/sale", httpPost.Template);
        var authorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }

    [Fact]
    public void CancelSale_ShouldBeHttpDeleteWithAuthorize()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.CancelSale))!;

        var httpDelete = method.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Cast<HttpDeleteAttribute>().Single();
        Assert.Equal("{productId:guid}/sale", httpDelete.Template);
        var authorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }
}