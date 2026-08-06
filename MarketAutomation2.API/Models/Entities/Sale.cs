using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.Models.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentType { get; set; } = "Cash";

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}