using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation2.API.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = "Adet";

        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public decimal Stock { get; set; }

        public decimal CriticalStock { get; set; } = 5;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; } = null!;

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}