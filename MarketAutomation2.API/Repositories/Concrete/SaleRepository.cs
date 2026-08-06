using MarketAutomation2.API.Data;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation2.API.Repositories.Concrete
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        private readonly MarketDbContext _context;

        public SaleRepository(MarketDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Sale>> GetAllWithItemsAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}