using main.DTO.cart;
using main.Entity;
using Microsoft.EntityFrameworkCore;

namespace main.Config
{
    public class AppDb(DbContextOptions<AppDb> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AppUser>()
                .HasMany(p => p.Carts)
                .WithOne(u => u.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>().HasMany(u => u.FavoriteProducts).WithMany();

            modelBuilder
                .Entity<RefreshToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();
            modelBuilder
                .Entity<Product>()
                .Property(p => p.IsListed)
                .HasDefaultValue(true);
            modelBuilder
                .Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });
            modelBuilder
                .Entity<Cart>()
                .HasMany(cart => cart.Products)
                .WithMany()
                .UsingEntity<CartItem>();

            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder
                .Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
