using System.ComponentModel.DataAnnotations;
using main.Entity;

namespace main.dto.auth;

public record RegisterUser(
    [EmailAddress] string Email,
    string FirstName,
    string LastName,
    string Password
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
        };
    }
}
