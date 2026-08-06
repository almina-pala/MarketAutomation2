using System.ComponentModel.DataAnnotations;

namespace MarketAutomation2.API.DTOs.Sales
{
    public class CreateSaleDto
    {
        [Required]
        public string PaymentType { get; set; }="Cash";
        public List<CreateSaleItemDto> Items { get; set; }=new();
    }
}
