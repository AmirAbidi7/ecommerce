using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using main.Entity;
using Microsoft.IdentityModel.Tokens;

namespace main.Service;

public class JwtService(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public string GenerateToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.Id!.Value.ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("Email", user.Email),
            new Claim("Role", user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumer = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumer);
        return Convert.ToBase64String(randomNumer);
    }
}
