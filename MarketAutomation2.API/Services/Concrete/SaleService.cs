using MarketAutomation2.API.DTOs.Sales;
using MarketAutomation2.API.Models.Entities;
using MarketAutomation2.API.Repositories.Abstract;
using MarketAutomation2.API.Services.Abstract;

namespace MarketAutomation2.API.Services.Concrete
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _unitOfWork.Sales.GetAllWithItemsAsync();

            return sales.Select(sale => new SaleDto
            {
                SaleId = sale.Id,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                PaymentType = sale.PaymentType
            }).ToList();
        }

        public async Task<SaleDto?> GetSaleByIdAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetByIdWithItemsAsync(id);

            if (sale == null)
                return null;

            return new SaleDto
            {
                SaleId = sale.Id,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                PaymentType = sale.PaymentType
            };
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("Satışta en az bir ürün bulunmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.PaymentType))
                throw new Exception("Ödeme tipi zorunludur.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                decimal totalAmount = 0;

                // Satışı oluştur
                var sale = new Sale
                {
                    SaleDate = DateTime.Now,
                    PaymentType = dto.PaymentType,
                    TotalAmount = 0
                };

                await _unitOfWork.Sales.AddAsync(sale);
                await _unitOfWork.SaveChangesAsync();

                // Satış kalemlerini işle
                foreach (var item in dto.Items)
                {
                    var product = await _unitOfWork.Products.GetForSaleAsync(item.ProductId);

                    if (product == null)
                        throw new Exception($"ID'si {item.ProductId} olan ürün bulunamadı.");

                    if (product.Stock < item.Quantity)
                        throw new Exception($"{product.Name} ürününde yeterli stok bulunmamaktadır.");

                    // Satış kalemi
                    var saleItem = new SaleItem
                    {
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.SalePrice
                    };

                    await _unitOfWork.SaleItems.AddAsync(saleItem);

                    // Toplam tutarı hesapla
                    totalAmount += product.SalePrice * item.Quantity;

                    // Stok düş
                    product.Stock -= item.Quantity;
                    _unitOfWork.Products.Update(product);

                    // Stok hareketi oluştur
                    var stockMovement = new StockMovement
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        MovementType = "Sale",
                        MovementDate = DateTime.Now
                    };

                    await _unitOfWork.StockMovements.AddAsync(stockMovement);
                }

                // Toplam tutarı güncelle
                sale.TotalAmount = totalAmount;
                _unitOfWork.Sales.Update(sale);

                // Tüm değişiklikleri kaydet
                await _unitOfWork.SaveChangesAsync();

                // Transaction'ı tamamla
                await _unitOfWork.CommitAsync();

                return new SaleDto
                {
                    SaleId = sale.Id,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount,
                    PaymentType = sale.PaymentType
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}