using MarketAutomation2.API.Data;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;

namespace MarketAutomation2.API.Repositories.Concrete
{
    public class SaleItemRepository : GenericRepository<SaleItem>, ISaleItemRepository
    {
        public SaleItemRepository(MarketDbContext context) : base(context)
        {
        }
    
    }
}
