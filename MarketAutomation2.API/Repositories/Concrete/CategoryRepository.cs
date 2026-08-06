using MarketAutomation2.API.Data;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation2.API.Repositories.Concrete
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MarketDbContext context)
            : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}