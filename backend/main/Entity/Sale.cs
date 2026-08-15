using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace main.Entity;

public class Sale
{
    [Key]
    public required Guid Id { get; set; }

    public required Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public required Product Product { get; set; }

    public required int PercentOff { get; set; }

    public required DateTime StartsAt { get; set; }

    public required DateTime EndsAt { get; set; }

    public required Guid CreatedBy { get; set; }
}