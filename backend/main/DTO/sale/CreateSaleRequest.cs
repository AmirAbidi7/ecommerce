namespace main.DTO.sale;

public record CreateSaleRequest(int PercentOff, DateTime StartsAt, DateTime EndsAt) { }