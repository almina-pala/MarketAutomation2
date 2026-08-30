namespace MarketAutomation2.Desktop.Models
{
    public class ReportDashboard
    {
        public int TotalProducts { get; set; }

        public int CriticalStock { get; set; }

        public int TodaySalesCount { get; set; }

        public decimal TodayTotalAmount { get; set; }

        public decimal CashTotal { get; set; }

        public decimal CardTotal { get; set; }

        public List<BestSellingProduct> BestSellingProducts { get; set; }
            = new();

        public List<RecentSale> RecentSales { get; set; }
            = new();
    }

    public class BestSellingProduct
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal TotalAmount { get; set; }
    }

    public class RecentSale
    {
        public int SaleId { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentType { get; set; } = string.Empty;
    }
}