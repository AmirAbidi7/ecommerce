namespace main.DTO.product;

public record CreateProductRequest(
    string Name,
    float Price,
    int Stock,
    string ImageUrl,
    string Description,
    string CategoryName
) { }