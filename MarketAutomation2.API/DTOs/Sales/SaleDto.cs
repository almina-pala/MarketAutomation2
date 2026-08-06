namespace MarketAutomation2.API.DTOs.Sales
{
    public class SaleDto
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentType { get; set; }=string.Empty;
    }
}
