using MarketAutomation2.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation2.API.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleItem> SaleItems { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(x => x.PurchasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(x => x.SalePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(x => x.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleItem>()
                .HasOne(x => x.Sale)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.SaleId);

            modelBuilder.Entity<SaleItem>()
                .HasOne(x => x.Product)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<StockMovement>()
                .HasOne(x => x.Product)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
