using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.Models.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(20)]
        public string MovementType { get; set; } = string.Empty;
    }
}