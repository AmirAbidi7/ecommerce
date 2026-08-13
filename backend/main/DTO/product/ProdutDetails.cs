using main.Entity;

namespace main.DTO.product;

public record ProdutDetails(Guid Id, string Name, float Price, string ImageUrl, string Description)
{
    public ProdutDetails(Product product)
        : this(product.Id, product.Name, product.Price, product.ImageUrl, product.Description) { }
}
