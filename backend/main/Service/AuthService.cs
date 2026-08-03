using main.Config;
using main.dto.auth;
using main.Entity;
using Microsoft.EntityFrameworkCore;

namespace main.Service;

public class AuthService(AppDb db, JwtService jwtService)
{
    public async Task<AuthResult?> RegisterAsync(RegisterUser registerUser)
    {
        var existingUser = await db.Users.AnyAsync(user => user.Email == registerUser.Email);
        if (existingUser)
        {
            throw new InvalidOperationException("User with that email already exists!");
        }
        var userToAdd = registerUser.ToAppUser();
        userToAdd.Password = BCrypt.Net.BCrypt.HashPassword(userToAdd.Password);

        var user = await db.Users.AddAsync(userToAdd);
        await db.SaveChangesAsync();

        return await IssueAuthResultAsync(user.Entity);
    }

    public async Task<AuthResult> LoginAsync(LoginUser loginUser)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        return await IssueAuthResultAsync(user);
    }

    public async Task<AuthResult> IssueAuthResultAsync(AppUser user)
    {
        var token = jwtService.GenerateToken(user);
        var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(token =>
            token.UserId == user.Id
        );

        if (refreshToken == null || !refreshToken.IsActive)
        {
            refreshToken = new RefreshToken
            {
                Token = jwtService.GenerateRefreshToken(),
                UserId = user.Id!.Value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await db.RefreshTokens.AddAsync(refreshToken);
            await db.SaveChangesAsync();
        }
        return AuthResult.Success(new UserInfo(user), token, refreshToken!.Token);
    }

    public async Task<AuthResult> Refresh(string refreshToken)
    {
        var token = await db.RefreshTokens.FirstAsync(token => token.Token == refreshToken);

        var user = await db.Users.FirstAsync(user => user.Id == token.UserId);

        return await IssueAuthResultAsync(user);
    }
}
