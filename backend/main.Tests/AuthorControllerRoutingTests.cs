using System.Reflection;
using main.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace main.Tests;

public class AuthorControllerRoutingTests
{
    [Fact]
    public void GetMyProducts_ShouldBeHttpGetWithAuthorize()
    {
        var type = typeof(AuthorController);
        var method = type.GetMethod(nameof(AuthorController.GetMyProducts))!;

        var httpGet = method.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>().Single();
        Assert.Equal("products", httpGet.Template);
        var authorize = type.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }

    [Fact]
    public void GetSales_ShouldBeHttpGetWithAuthorize()
    {
        var type = typeof(AuthorController);
        var method = type.GetMethod(nameof(AuthorController.GetSales))!;

        var httpGet = method.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>().Single();
        Assert.Equal("sales", httpGet.Template);
        var authorize = type.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>().Single();
        Assert.Equal("Author", authorize.Roles);
    }
}