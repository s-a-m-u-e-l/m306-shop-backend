using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public sealed class SamadDbContext : DbContext
    {
        public DbSet<ProductEntity> Products { get; set; }

        public DbQuery<ConsolidatedProductEntity> ProductsConsolidated { get; set; }

        public DbSet<ImageEntity> Images { get; set; }

        public DbSet<CategoryEntity> Categories { get; set; }

        public DbSet<WishlistEntity> Wishlists { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public SamadDbContext(DbContextOptions<SamadDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Products
            modelBuilder
                .Entity<ProductEntity>()
                .ToTable("products")
                .HasOne(p => p.User);

            modelBuilder
                .Query<ConsolidatedProductEntity>()
                .ToView("products_consolidated");

            modelBuilder
                .Entity<ImageEntity>()
                .ToTable("product_images");

            // Categories
            modelBuilder
                .Entity<CategoryEntity>()
                .ToTable("product_categories");

            // Wishlists
            modelBuilder
                .Entity<WishlistEntity>()
                .ToTable("wishlist_products")
                .HasKey(x => new {x.ProductId, x.UserId});
            
            // Users
            modelBuilder
                .Entity<UserEntity>()
                .ToTable("users");
        }
    }
}