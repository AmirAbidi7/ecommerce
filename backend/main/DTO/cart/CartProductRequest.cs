namespace main.DTO.cart;

public record CartProductRequest(Guid productId, Guid cartId, int productAmount) { }
