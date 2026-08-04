using MarketAutomation2.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation.API.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
