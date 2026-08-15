using main.Config;
using main.dto.auth;
using main.Entity;
using main.Enum;
using main.Service;
using Microsoft.EntityFrameworkCore;

namespace main.Tests;

public class AuthServiceTests : TestBase
{
    private static readonly RegisterUser RegisterData =
        new("user@example.com", "Amir", "Abidi", "testtest");

    private AuthService CreateService(AppDb db) => new(db, jwtService);

    [Fact]
    public async Task Register_ShouldCreateUserAndIssueToken()
    {
        using var db = CreateContext();
        var result = await CreateService(db).RegisterAsync(RegisterData);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.Equal(RegisterData.Email, result.UserInfo.Email);
        Assert.True(await db.Users.AnyAsync(u => u.Email == RegisterData.Email));
    }

    [Fact]
    public async Task Register_ShouldRejectDuplicateEmail()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        await service.RegisterAsync(RegisterData);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync(RegisterData));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public async Task Register_ShouldHashPassword()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        await service.RegisterAsync(RegisterData);

        var stored = await db.Users.SingleAsync(u => u.Email == RegisterData.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(RegisterData.Password, stored.Password));
        Assert.NotEqual(RegisterData.Password, stored.Password);
    }

    [Fact]
    public async Task Login_ShouldSucceedWithValidCredentials()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        await service.RegisterAsync(RegisterData);

        var result = await service.LoginAsync(new LoginUser(RegisterData.Email, RegisterData.Password));

        Assert.True(result.IsSuccess);
        Assert.Equal(RegisterData.Email, result.UserInfo.Email);
    }

    [Fact]
    public async Task Login_ShouldRejectWrongPassword()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        await service.RegisterAsync(RegisterData);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(new LoginUser(RegisterData.Email, "wrong")));
    }

    [Fact]
    public async Task Login_ShouldRejectUnknownEmail()
    {
        using var db = CreateContext();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CreateService(db).LoginAsync(new LoginUser("nobody@example.com", "x")));
    }

    [Fact]
    public async Task Refresh_ShouldRotateActiveToken()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        var first = await service.RegisterAsync(RegisterData);

        var result = await service.Refresh(first.RefreshToken!);

        Assert.True(result.IsSuccess);
        var old = await db.RefreshTokens.SingleAsync(t => t.Token == first.RefreshToken);
        Assert.NotNull(old.RevokedAt);
        Assert.True(await db.RefreshTokens.AnyAsync(t => t.Token == result.RefreshToken));
    }

    [Fact]
    public async Task Refresh_ShouldRejectRevokedToken()
    {
        using var db = CreateContext();
        var service = CreateService(db);
        var first = await service.RegisterAsync(RegisterData);
        await service.Refresh(first.RefreshToken!);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.Refresh(first.RefreshToken!));
    }

    [Fact]
    public async Task Refresh_ShouldRejectExpiredToken()
    {
        using var db = CreateContext();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "x@example.com",
            FirstName = "A",
            LastName = "B",
            Password = BCrypt.Net.BCrypt.HashPassword("p"),
        };
        db.Users.Add(user);
        db.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = user.Id.Value,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-8),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
        });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CreateService(db).Refresh("expired-token"));
    }

    [Fact]
    public void GenerateToken_ShouldIncludeRoleClaim()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "author@example.com",
            FirstName = "Amir",
            LastName = "Abidi",
            Password = "hash",
            Role = UserRole.Author,
        };

        var token = jwtService.GenerateToken(user);
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal("Author", jwt.Claims.First(c => c.Type == "Role").Value);
    }
}