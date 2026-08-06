using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.DTOs.Products
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(13)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Range(0, 999999)]
        public decimal PurchasePrice { get; set; }

        [Range(0, 999999)]
        public decimal SalePrice { get; set; }

        [Range(0, 999999)]
        public int Stock { get; set; }

        [Range(0, 999999)]
        public int CriticalStock { get; set; }

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = "Adet";

        [Required]
        public int CategoryId { get; set; }
    }
}