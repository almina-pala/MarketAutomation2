namespace MarketAutomation2.Desktop.Models
{
    public class UpdateProductRequest
    {
        public string Barcode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public decimal Stock { get; set; }

        public decimal CriticalStock { get; set; }

        public string Unit { get; set; } = "Adet";

        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}