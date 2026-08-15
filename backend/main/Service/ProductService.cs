using main.Config;
using main.DTO.product;
using main.Entity;
using Microsoft.EntityFrameworkCore;

namespace main.Service
{
    public class ProductService(AppDb db)
    {
        public async Task<ICollection<ProductOverview>> GetProductsAsync()
        {
            var now = DateTime.UtcNow;
            var products = await db
                .Products.Include(p => p.Category)
                .Where(p => p.IsListed)
                .ToListAsync();
            var activeSales = await db
                .Sales.Where(s =>
                    s.StartsAt <= now && s.EndsAt >= now && products.Select(p => p.Id).Contains(s.ProductId)
                )
                .ToListAsync();
            return products
                .Select(p => new ProductOverview(p, activeSales.FirstOrDefault(s => s.ProductId == p.Id)))
                .ToList();
        }

        public async Task<ProductOverview> CreateProductAsync(CreateProductRequest request, Guid authorId)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == request.CategoryName);
            if (category is null)
            {
                category = new Category { Id = Guid.NewGuid(), Name = request.CategoryName };
                db.Categories.Add(category);
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = request.ImageUrl,
                Description = request.Description,
                Category = category,
                AuthorId = authorId,
                IsListed = true,
                CreatedAt = DateTime.UtcNow,
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            return new ProductOverview(product, null);
        }

        public async Task UnlistProductAsync(Guid productId, Guid authorId)
        {
            var product =
                await db.Products.FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product not found!");
            if (product.AuthorId != authorId)
            {
                throw new UnauthorizedAccessException("Not the product author");
            }
            product.IsListed = false;
            await db.SaveChangesAsync();
        }

        public async Task<ProdutDetails> GetProductAsync(Guid productId)
        {
            var product = await db
                .Products.Include(p => p.Category)
                .FirstAsync(p => p.Id == productId);
            var now = DateTime.UtcNow;
            var activeSale = await db.Sales.FirstOrDefaultAsync(s =>
                s.ProductId == productId && s.StartsAt <= now && s.EndsAt >= now
            );
            return new ProdutDetails(product, activeSale);
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
            user.FavoriteProducts ??= [];
            user.FavoriteProducts.Add(product);
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
            user.FavoriteProducts ??= [];
            user.FavoriteProducts.Remove(product);
            await db.SaveChangesAsync();
        }

        public async Task<ICollection<Guid>> GetFavoriteIdsAsync(Guid userId)
        {
            var favoriteIds = await db
                .Users.Where(u => u.Id == userId)
                .SelectMany(u => u.FavoriteProducts)
                .Select(p => p.Id)
                .ToListAsync();
            return favoriteIds;
        }
    }
}
