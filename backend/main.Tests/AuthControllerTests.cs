using main.Controller;
using main.dto.auth;
using main.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace main.Tests;

public class AuthControllerTests : TestBase
{
    private static readonly RegisterUser RegisterData =
        new("user@example.com", "Amir", "Abidi", "testtest");

    private (AuthController Controller, DefaultHttpContext Http) CreateController()
    {
        var context = CreateContext();
        var http = new DefaultHttpContext();
        http.Features.Set<IResponseCookiesFeature>(new ResponseCookiesFeature(http.Features));
        var controller = new AuthController(new AuthService(context, jwtService))
        {
            ControllerContext = new ControllerContext { HttpContext = http },
        };
        return (controller, http);
    }

    [Fact]
    public async Task Register_ShouldSetRefreshCookie()
    {
        var (controller, http) = CreateController();
        var result = await controller.Register(RegisterData);

        Assert.IsType<OkObjectResult>(result.Result);
        var setCookie = http.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refreshToken=", setCookie);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/api/auth/refresh", setCookie);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorizedWithoutCookie()
    {
        var (controller, _) = CreateController();
        var result = await controller.Refresh();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Token not set", unauthorized.Value);
    }

    [Fact]
    public async Task Refresh_ShouldRotateTokenWithCookie()
    {
        var (controller, http) = CreateController();
        var registered = await controller.Register(RegisterData);
        var refreshToken = ((AuthResult)((OkObjectResult)registered.Result!).Value).RefreshToken!;
        http.Request.Headers["Cookie"] = $"refreshToken={refreshToken}";

        var result = await controller.Refresh();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Logout_ShouldDeleteCookie()
    {
        var (controller, http) = CreateController();
        await controller.Logout();

        var setCookie = http.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refreshToken=", setCookie);
        Assert.Contains("expires=", setCookie);
    }
}