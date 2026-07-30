using System.Net;
using BCrypt.Net;
using main.Config;
using main.Entity;
using Microsoft.EntityFrameworkCore;

public class AuthService(AppDb db)
{
    private readonly AppDb _appdb;

    public async Task<AuthResult?> RegisterAsync(RegisterUser registerUser)
    {
        var existingUser = await _appdb.Users.FirstAsync(user => user.Email == registerUser.Email);
        if (existingUser != null)
        {
            return AuthResult.Failure("Email already exists");
        }
        var userToAdd = registerUser.ToAppUser();
        userToAdd.Password = BCrypt.Net.BCrypt.HashPassword(userToAdd.Password);

        var user = await _appdb.Users.AddAsync(registerUser.ToAppUser());
        await _appdb.SaveChangesAsync();

        return AuthResult.Success(new UserInfo(user.Entity));
    }

    public async Task<AuthResult> LoginAsync(LoginUser loginUser)
    {
        var user = await _appdb.Users.FirstAsync(u => u.Email == loginUser.Email);
        if (user == null)
        {
            return AuthResult.Failure("Wrong password or Email");
        }
        if (user.Password != BCrypt.Net.BCrypt.HashPassword(loginUser.Password))
        {
            return AuthResult.Failure("Wrong password or Email");
        }

        return AuthResult.Success(new UserInfo(user));
    }
}
