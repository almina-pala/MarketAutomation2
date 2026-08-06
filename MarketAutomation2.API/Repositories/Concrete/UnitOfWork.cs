using MarketAutomation2.API.Data;
using MarketAutomation2.API.Repositories.Abstract;
using Microsoft.EntityFrameworkCore.Storage;

namespace MarketAutomation2.API.Repositories.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MarketDbContext _context;

        public ICategoryRepository Categories { get; }

        public IProductRepository Products { get; }

        public ISaleRepository Sales { get; }

        public ISaleItemRepository SaleItems { get; }

        public IStockMovementRepository StockMovements { get; }

        public UnitOfWork(
            MarketDbContext context,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ISaleRepository saleRepository,
            ISaleItemRepository saleItemRepository,
            IStockMovementRepository stockMovementRepository)
        {
            _context = context;

            Categories = categoryRepository;
            Products = productRepository;
            Sales = saleRepository;
            SaleItems = saleItemRepository;
            StockMovements = stockMovementRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        private IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction!.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
    }
}