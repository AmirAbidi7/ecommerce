using main.Entity;

namespace main.DTO.product;

public record ProductList(Guid Id, string Name, float Price, string ImageUrl)
{
    public ProductList(Product product)
        : this(product.Id, product.Name, product.Price, product.ImageUrl) { }
};
