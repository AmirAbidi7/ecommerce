using main.Entity;

namespace main.DTO.product;

public record ProductOverview(Guid Id, string Name, float Price, string ImageUrl)
{
    public ProductOverview(Product product)
        : this(product.Id, product.Name, product.Price, product.ImageUrl) { }
}
