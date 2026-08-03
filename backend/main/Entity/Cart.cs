using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using main.Entity;
using main.Enum;

namespace main.Entity
{
    public class Cart
    {
        [Key]
        public required Guid Id { get; set; }

        public required CartStatus Status { get; set; } = CartStatus.CREATED;

        public required ICollection<Product> Products { get; set; }
        public required Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public required AppUser User { get; set; }
    }
}
