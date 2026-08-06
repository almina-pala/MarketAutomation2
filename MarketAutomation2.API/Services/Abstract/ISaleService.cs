using MarketAutomation2.API.DTOs.Sales;

namespace MarketAutomation2.API.Services.Abstract
{
    public interface ISaleService
    {
        Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);

        Task<List<SaleDto>> GetAllSalesAsync();

        Task<SaleDto?> GetSaleByIdAsync(int id);
    }
}
