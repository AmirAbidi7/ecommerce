using main.Config;
using main.DTO.author;
using main.Enum;
using Microsoft.EntityFrameworkCore;

namespace main.Service;

public class AuthorService(AppDb db)
{
    public async Task<ICollection<AuthorBook>> GetMyBooksAsync(Guid authorId)
    {
        var now = DateTime.UtcNow;
        var books = await db
            .Products.Include(p => p.Category)
            .Where(p => p.AuthorId == authorId)
            .ToListAsync();
        var ids = books.Select(b => b.Id).ToList();
        var sales = await db
            .Sales.Where(s => ids.Contains(s.ProductId) && s.StartsAt <= now && s.EndsAt >= now)
            .ToListAsync();

        return books
            .Select(b =>
            {
                var sale = sales.FirstOrDefault(s => s.ProductId == b.Id);
                return new AuthorBook(
                    b.Id,
                    b.Name,
                    b.Price,
                    sale == null ? b.Price : Pricing.EffectivePrice(b.Price, sale.PercentOff),
                    sale != null,
                    sale?.PercentOff,
                    b.IsListed,
                    b.Stock,
                    b.Category?.Name ?? ""
                );
            })
            .ToList();
    }

    public async Task<ICollection<AuthorSaleStat>> GetSalesAsync(Guid authorId)
    {
        var items = await db
            .CartItems.Where(ci =>
                ci.Product.AuthorId == authorId && ci.Cart.Status == CartStatus.PAID
            )
            .Include(ci => ci.Product)
            .ToListAsync();

        return items
            .GroupBy(ci => ci.ProductId)
            .Select(g => new AuthorSaleStat(
                g.Key,
                g.First().Product.Name,
                g.Sum(ci => ci.ProductAmount),
                g.Sum(ci => ci.UnitPricePaid * ci.ProductAmount)
            ))
            .ToList();
    }
}