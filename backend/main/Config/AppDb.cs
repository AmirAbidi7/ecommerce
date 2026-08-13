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
                .Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });
            modelBuilder
                .Entity<Cart>()
                .HasMany(cart => cart.Products)
                .WithMany()
                .UsingEntity<CartItem>();
        }
    }
}
