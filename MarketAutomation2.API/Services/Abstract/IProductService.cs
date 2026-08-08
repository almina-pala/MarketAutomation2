using MarketAutomation2.API.DTOs.Products;

namespace MarketAutomation2.API.Services.Abstract
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto?> GetByBarcodeAsync(string barcode);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);

        
    }
}