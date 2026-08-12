using main.Config;
using main.dto.auth;
using main.Entity;
using main.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace main.Tests;

public class AuthTest : TestBase
{
    [Fact]
    public async Task ShouldRegisterCorrectly()
    {
        RegisterUser registerUser = new("user@example.com", "Amir", "Abidi", "testtest");

        using var context = CreateContext();
        AuthService authService = new(context, config);
        var result = await authService.RegisterAsync(registerUser);
        Assert.NotNull(result);
    }
}
