using main.DTO.cart;

namespace main.Events;

public record PurchaseEvent(string UserEmail, CartOverView Cart, ICollection<AuthorNotice> AuthorNotices) { }

public record AuthorNotice(
    string AuthorEmail,
    string AuthorName,
    Guid ProductId,
    string ProductName,
    int Amount,
    float UnitPricePaid,
    int? SalePercentOff
) { }