using MarketAutomation2.API.Models.Entities;

namespace MarketAutomation2.API.Repositories.Abstract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetByBarcodeAsync(string barcode);

        Task<bool> BarcodeExistsAsync(string barcode);
        Task<List<Product>> GetAllWithCategoryAsync();

        Task<Product?> GetByIdWithCategoryAsync(int id);

        Task<Product?> GetForSaleAsync(int id);
    }
}