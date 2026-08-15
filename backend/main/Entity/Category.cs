using System.ComponentModel.DataAnnotations;

namespace main.Entity;

public class Category
{
    [Key]
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Product>? Products { get; set; }
}