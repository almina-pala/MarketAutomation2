using MarketAutomation2.API.Data;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation2.API.Repositories.Concrete
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(MarketDbContext context)
            : base(context)
        {
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<bool> BarcodeExistsAsync(string barcode)
        {
            return await _context.Products
                .AnyAsync(p => p.Barcode == barcode);
        }

        public async Task<List<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetForSaleAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }
    }
}