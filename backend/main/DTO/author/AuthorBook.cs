namespace main.DTO.author;

public record AuthorBook(
    Guid Id,
    string Name,
    float Price,
    float EffectivePrice,
    bool IsOnSale,
    int? SalePercent,
    bool IsListed,
    int Stock,
    string CategoryName
) { }