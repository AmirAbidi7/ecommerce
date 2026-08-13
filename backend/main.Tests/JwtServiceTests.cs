using System.IdentityModel.Tokens.Jwt;
using main.Entity;
using main.Service;

namespace main.Tests;

public class JwtServiceTests : TestBase
{
    private AppUser CreateUser()
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            FirstName = "Amir",
            LastName = "Abidi",
            Password = "hashed",
        };
    }

    [Fact]
    public void ShouldIncludeUserClaimsInToken()
    {
        var user = CreateUser();
        var token = jwtService.GenerateToken(user);
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .ToDictionary(c => c.Type, c => c.Value);

        Assert.Equal(user.Id.Value.ToString(), claims["UserId"]);
        Assert.Equal(user.FirstName, claims["FirstName"]);
        Assert.Equal(user.Email, claims["Email"]);
    }

    [Fact]
    public void ShouldSetIssuerAudienceAndExpiry()
    {
        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(jwtService.GenerateToken(CreateUser()));

        Assert.Equal("Ecommerce", token.Issuer);
        Assert.Equal("EcommerceUsers", token.Audiences.Single());
        Assert.Equal(DateTime.Now.AddMinutes(1), token.ValidTo.ToLocalTime(), TimeSpan.FromMinutes(2));
    }

    [Fact]
    public void ShouldGenerateDistinctRefreshTokens()
    {
        var first = jwtService.GenerateRefreshToken();
        var second = jwtService.GenerateRefreshToken();

        Assert.NotEqual(first, second);
        Assert.Equal(44, first.Length);
    }
}