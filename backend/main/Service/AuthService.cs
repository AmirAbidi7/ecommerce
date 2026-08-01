using main.Config;
using main.Entity;
using Microsoft.EntityFrameworkCore;

public class AuthService(AppDb db, JwtService jwtService)
{
    public async Task<AuthResult?> RegisterAsync(RegisterUser registerUser)
    {
        var existingUser = await db.Users.FirstAsync(user => user.Email == registerUser.Email);
        if (existingUser != null)
        {
            return AuthResult.Failure("Email already exists");
        }
        var userToAdd = registerUser.ToAppUser();
        userToAdd.Password = BCrypt.Net.BCrypt.HashPassword(userToAdd.Password);

        var user = await db.Users.AddAsync(userToAdd);
        await db.SaveChangesAsync();

        return await IssueAuthResultAsync(user.Entity);
    }

    public async Task<AuthResult> LoginAsync(LoginUser loginUser)
    {
        var user = await db.Users.FirstAsync(u => u.Email == loginUser.Email);
        if (user == null)
        {
            return AuthResult.Failure("Wrong password or Email");
        }
        if (BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
        {
            return AuthResult.Failure("Wrong password or Email");
        }
        return await IssueAuthResultAsync(user);
    }

    public async Task<AuthResult> IssueAuthResultAsync(AppUser user)
    {
        var token = jwtService.GenerateToken(user.FirstName);
        var refreshToken = await db.RefreshTokens.FirstAsync(token => token.UserId == user.Id);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            refreshToken = new RefreshToken
            {
                Token = jwtService.GenerateRefreshToken(),
                UserId = user.Id!.Value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await db.RefreshTokens.AddAsync(
                new RefreshToken
                {
                    Token = jwtService.GenerateRefreshToken(),
                    UserId = user.Id!.Value,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                }
            );
            await db.SaveChangesAsync();
        }
        return AuthResult.Success(new UserInfo(user), token, refreshToken!.Token);
    }
}
