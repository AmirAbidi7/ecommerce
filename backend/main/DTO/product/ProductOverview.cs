using main.Entity;
using main.Service;

namespace main.DTO.product;

public record ProductOverview(
    Guid Id,
    string Name,
    float Price,
    string ImageUrl,
    string CategoryName,
    bool IsOnSale,
    int? SalePercent,
    float EffectivePrice
)
{
    public ProductOverview(Product product, Sale? activeSale)
        : this(
            product.Id,
            product.Name,
            product.Price,
            product.ImageUrl,
            product.Category?.Name ?? "",
            activeSale != null,
            activeSale?.PercentOff,
            activeSale == null ? product.Price : Pricing.EffectivePrice(product.Price, activeSale.PercentOff)
        ) { }
}