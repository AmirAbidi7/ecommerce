using System.ComponentModel.DataAnnotations;
using main.Entity;

public record LoginUser([EmailAddress] string Email, string Password) { }
