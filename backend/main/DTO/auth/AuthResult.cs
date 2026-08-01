using System.ComponentModel.DataAnnotations;
using main.Entity;

public class AuthResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public UserInfo? UserInfo { get; }
    public string? Token { get; }
    public string? RefreshToken { get; }

    private AuthResult(
        bool isSuccess,
        string? errorMessage,
        UserInfo userInfo,
        string token,
        string refreshToken
    )
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        UserInfo = userInfo;
        Token = token;
        RefreshToken = refreshToken;
    }

    public static AuthResult Success(UserInfo userInfo, string token, string refreshToken) =>
        new(true, null, userInfo, token, refreshToken);

    public static AuthResult Failure(string errorMessage) =>
        new(false, errorMessage, null, null, null);
}

public record UserInfo(Guid Id, [EmailAddress] string Email, string FirstName, string LastName)
{
    public UserInfo(AppUser user)
        : this(user.Id!.Value, user.Email, user.FirstName, user.LastName) { }
}
