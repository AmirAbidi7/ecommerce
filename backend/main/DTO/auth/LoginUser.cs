using System.ComponentModel.DataAnnotations;

namespace main.dto.auth;

public record LoginUser([EmailAddress] string Email, string Password) { }
