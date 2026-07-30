using System.ComponentModel.DataAnnotations;
using main.entity;

namespace main.Entity
{
    public class AppUser
    {
        [Key]
        public Guid? Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }

        public ICollection<Product>? FavoriteProducts { get; set; }

        public ICollection<Cart>? Carts { get; set; }
    }
}
