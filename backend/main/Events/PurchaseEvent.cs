using main.DTO.cart;

namespace main.Events;

public record PurchaseEvent(string UserEmail, CartOverView Cart) { }
