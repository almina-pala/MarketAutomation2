using MarketAutomation2.API.Models.Entities;

namespace MarketAutomation2.API.Repositories.Abstract
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
    }
}