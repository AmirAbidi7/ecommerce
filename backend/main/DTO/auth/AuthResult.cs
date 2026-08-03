using System.ComponentModel.DataAnnotations;
using main.Entity;

namespace main.dto.auth;

public class AuthResult
{
    public bool IsSuccess { get; }
    public UserInfo? UserInfo { get; }
    public string? Token { get; }
    public string? RefreshToken { get; }

    private AuthResult(bool isSuccess, UserInfo userInfo, string token, string refreshToken)
    {
        IsSuccess = isSuccess;
        UserInfo = userInfo;
        Token = token;
        RefreshToken = refreshToken;
    }

    public static AuthResult Success(UserInfo userInfo, string token, string refreshToken) =>
        new(true, userInfo, token, refreshToken);
}

public record UserInfo(Guid Id, [EmailAddress] string Email, string FirstName, string LastName)
{
    public UserInfo(AppUser user)
        : this(user.Id!.Value, user.Email, user.FirstName, user.LastName) { }
}
