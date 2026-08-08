namespace MarketAutomation2.Desktop.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Barcode { get; set; } = "";

        public string Name { get; set; } = "";

        public decimal SalePrice { get; set; }

        public decimal Stock { get; set; }
    }
}