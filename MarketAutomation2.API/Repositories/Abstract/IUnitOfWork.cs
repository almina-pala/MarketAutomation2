namespace MarketAutomation2.API.Repositories.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }

        IProductRepository Products { get; }

        ISaleRepository Sales { get; }

        ISaleItemRepository SaleItems { get; }

        IStockMovementRepository StockMovements { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}