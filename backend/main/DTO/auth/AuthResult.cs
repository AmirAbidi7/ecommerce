using System.ComponentModel.DataAnnotations;
using main.Entity;

public class AuthResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public UserInfo? UserInfo { get; }

    private AuthResult(bool isSuccess, string? errorMessage, UserInfo userInfo)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        UserInfo = userInfo;
    }

    public static AuthResult Success(UserInfo userInfo) => new(true, null, userInfo);

    public static AuthResult Failure(string errorMessage) => new(false, errorMessage, null);
}

public record UserInfo(Guid Id, [EmailAddress] string Email, string FirstName, string LastName)
{
    public UserInfo(AppUser user)
        : this(user.Id!.Value, user.Email, user.FirstName, user.LastName) { }
}
