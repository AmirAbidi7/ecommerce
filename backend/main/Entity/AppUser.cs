using System.ComponentModel.DataAnnotations;
using main.entity;

namespace main.Entity
{
    public class AppUser
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string password { get; set; }

        public ICollection<Product>? FavoriteProducts { get; set; }

        public ICollection<Cart>? Carts { get; set; }
    }
}
