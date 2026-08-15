using main.Config;
using main.DTO.sale;
using main.Entity;
using main.Events;
using Microsoft.EntityFrameworkCore;

namespace main.Service;

public class SaleService(AppDb db, IProducerService producerService)
{
    public async Task CreateSaleAsync(Guid productId, Guid authorId, CreateSaleRequest request)
    {
        if (request.PercentOff is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PercentOff), "PercentOff must be between 1 and 100");
        }
        if (request.StartsAt >= request.EndsAt)
        {
            throw new ArgumentException("StartsAt must be before EndsAt");
        }

        var product =
            await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException("Product not found!");
        if (product.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Not the product author");
        }
        if (!product.IsListed)
        {
            throw new ArgumentException("Cannot create a sale for an unlisted product");
        }

        var overlaps = await db.Sales.AnyAsync(s =>
            s.ProductId == productId && s.StartsAt < request.EndsAt && s.EndsAt > request.StartsAt
        );
        if (overlaps)
        {
            throw new InvalidOperationException("Product already has a sale in that period");
        }

        db.Sales.Add(new Sale
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            PercentOff = request.PercentOff,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            CreatedBy = authorId,
            Product = null!,
        });
        await db.SaveChangesAsync();

        var emails = await db
            .Users.Where(u => u.FavoriteProducts.Any(p => p.Id == productId))
            .Select(u => u.Email)
            .ToListAsync();

        await producerService.ProduceAsync(
            "promotion-created",
            new PromotionEvent(
                emails,
                product.Id,
                product.Name,
                product.Price,
                Pricing.EffectivePrice(product.Price, request.PercentOff),
                request.PercentOff,
                product.Category?.Name ?? ""
            )
        );
    }

    public async Task CancelSaleAsync(Guid productId, Guid authorId)
    {
        var product =
            await db.Products.FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException("Product not found!");
        if (product.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Not the product author");
        }

        var sales = await db.Sales.Where(s => s.ProductId == productId).ToListAsync();
        db.Sales.RemoveRange(sales);
        await db.SaveChangesAsync();
    }
}