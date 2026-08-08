using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.Models.Entities
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }

        public Sale Sale { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}