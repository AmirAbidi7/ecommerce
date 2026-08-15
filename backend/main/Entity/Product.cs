using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace main.Entity;

public class Product
{
    [Key]
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required int Stock { get; set; }

    public required float Price { get; set; }
    public required string ImageUrl { get; set; }
    public required string Description { get; set; }

    public required bool IsListed { get; set; } = true;

    public Guid? CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public Guid? AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public AppUser? Author { get; set; }

    public required DateTime CreatedAt { get; set; }
}
