using System.ComponentModel.DataAnnotations;
using main.Entity;
using main.Enum;

namespace main.dto.auth;

public record RegisterUser(
    [EmailAddress] string Email,
    string FirstName,
    string LastName,
    string Password,
    UserRole Role = UserRole.User
)
{
    public AppUser ToAppUser()
    {
        return new AppUser
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Password = Password,
            Role = Role,
        };
    }
}
