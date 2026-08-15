namespace main.Events;

public record PromotionEvent(
    List<string> RecipientEmails,
    Guid ProductId,
    string ProductName,
    float OriginalPrice,
    float DiscountedPrice,
    int PercentOff,
    string CategoryName
) { }