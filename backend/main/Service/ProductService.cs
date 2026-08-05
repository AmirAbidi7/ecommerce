using main.Config;
using main.DTO.product;
using Microsoft.EntityFrameworkCore;

namespace main.Service
{
    public class ProductService(AppDb db)
    {
        public async Task<ICollection<ProductOverview>> GetProductsAsync()
        {
            return await db.Products.Select(p => new ProductOverview(p)).ToListAsync();
        }

        public async Task<ProdutDetails> GetProductAsync(Guid productId)
        {
            return await db
                .Products.Select(p => new ProdutDetails(p))
                .FirstAsync(p => p.Id == productId);
        }

        public async Task FavoriteProduct(Guid productId, Guid userId)
        {
            var exists = await db
                .Users.Where(u => u.Id == userId)
                .SelectMany(u => u.FavoriteProducts)
                .AnyAsync(p => p.Id == productId);

            if (exists)
            {
                throw new InvalidOperationException("Product already favorited");
            }
            var user =
                await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found!");
            var product =
                await db.Products.FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product does not exist!");
            user.FavoriteProducts!.Add(product);
            await db.SaveChangesAsync();
        }

        public async Task UnfavoriteProduct(Guid productId, Guid userId)
        {
            var exists = await db
                .Users.Where(u => u.Id == userId)
                .SelectMany(u => u.FavoriteProducts)
                .AnyAsync(p => p.Id == productId);
            if (!exists)
            {
                throw new InvalidOperationException("Product already not favorited");
            }
            var user =
                await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");
            var product =
                await db.Products.FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product not found");
            user.FavoriteProducts!.Remove(product);
            await db.SaveChangesAsync();
        }
    }
}
