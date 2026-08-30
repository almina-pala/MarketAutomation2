using MarketAutomation2.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly MarketDbContext _context;

        public ReportsController(MarketDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                // -----------------------------
                // TOPLAM AKTİF ÜRÜN
                // -----------------------------

                var totalProducts = await _context.Products
                    .CountAsync(x => x.IsActive);

                // -----------------------------
                // KRİTİK STOK
                // -----------------------------

                var criticalStock = await _context.Products
                    .CountAsync(x =>
                        x.IsActive &&
                        x.Stock <= x.CriticalStock);

                // -----------------------------
                // BUGÜNKÜ SATIŞLAR
                // -----------------------------

                var todaySales = await _context.Sales
                    .Where(x =>
                        x.SaleDate >= today &&
                        x.SaleDate < tomorrow)
                    .ToListAsync();

                var todaySalesCount = todaySales.Count;

                var todayTotalAmount = todaySales
                    .Sum(x => x.TotalAmount);

                // -----------------------------
                // NAKİT
                // -----------------------------

                var cashTotal = todaySales
                    .Where(x =>
                        x.PaymentType == "Cash" ||
                        x.PaymentType == "Nakit")
                    .Sum(x => x.TotalAmount);

                // -----------------------------
                // KART
                // -----------------------------

                var cardTotal = todaySales
                    .Where(x =>
                        x.PaymentType == "Card" ||
                        x.PaymentType == "Kart")
                    .Sum(x => x.TotalAmount);

                // -----------------------------
                // EN ÇOK SATAN ÜRÜNLER
                // -----------------------------
                //
                // SQLite decimal SUM problemi nedeniyle
                // önce verileri çekiyoruz, ardından C# tarafında
                // GroupBy ve Sum yapıyoruz.
                //

                var saleItems = await _context.SaleItems
                    .Where(x =>
                        x.Sale.SaleDate >= today &&
                        x.Sale.SaleDate < tomorrow)
                    .Select(x => new
                    {
                        x.ProductId,
                        ProductName = x.Product.Name,
                        x.Quantity,
                        x.UnitPrice
                    })
                    .ToListAsync();

                var bestSellingProducts = saleItems
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        x.ProductName
                    })
                    .Select(x => new
                    {
                        ProductId = x.Key.ProductId,
                        ProductName = x.Key.ProductName,
                        Quantity = x.Sum(y => y.Quantity),
                        TotalAmount = x.Sum(y =>
                            y.Quantity * y.UnitPrice)
                    })
                    .OrderByDescending(x => x.Quantity)
                    .Take(5)
                    .ToList();

                // -----------------------------
                // SON 10 SATIŞ
                // -----------------------------

                var recentSales = await _context.Sales
                    .OrderByDescending(x => x.SaleDate)
                    .Take(10)
                    .Select(x => new
                    {
                        SaleId = x.Id,
                        SaleDate = x.SaleDate,
                        TotalAmount = x.TotalAmount,
                        PaymentType = x.PaymentType
                    })
                    .ToListAsync();

                // -----------------------------
                // RAPOR
                // -----------------------------

                return Ok(new
                {
                    TotalProducts = totalProducts,
                    CriticalStock = criticalStock,

                    TodaySalesCount = todaySalesCount,
                    TodayTotalAmount = todayTotalAmount,

                    CashTotal = cashTotal,
                    CardTotal = cardTotal,

                    BestSellingProducts = bestSellingProducts,
                    RecentSales = recentSales
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "Rapor oluşturulurken hata oluştu.",
                        Error = ex.Message
                    });
            }
        }
    }
}