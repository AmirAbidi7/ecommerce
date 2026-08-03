using System.ComponentModel.DataAnnotations;

namespace main.Entity;

public class Product
{
    [Key]
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required int Stock { get; set; }

    public required float Price { get; set; }
    public required string imageUrl { get; set; }
}
