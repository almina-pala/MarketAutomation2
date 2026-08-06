using MarketAutomation2.API.Models.Entities;

namespace MarketAutomation2.API.Repositories.Abstract
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        Task<List<Sale>> GetAllWithItemsAsync();

        Task<Sale?> GetByIdWithItemsAsync(int id);
    }
}
