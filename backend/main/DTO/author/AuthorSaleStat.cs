namespace main.DTO.author;

public record AuthorSaleStat(Guid ProductId, string Name, int UnitsSold, float Revenue) { }