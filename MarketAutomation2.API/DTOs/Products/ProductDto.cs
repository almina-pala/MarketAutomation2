namespace MarketAutomation2.API.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Barcode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int Stock { get; set; }

        public int CriticalStock { get; set; }

        public string Unit { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}