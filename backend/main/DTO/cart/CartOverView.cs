using main.DTO.product;

namespace main.DTO.cart;

public record CartOverView(Guid Id, ICollection<CartProduct> Items) { }

public record CartProduct(Guid Id, ProductOverview Product) { }
