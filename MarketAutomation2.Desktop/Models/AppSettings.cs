namespace MarketAutomation2.Desktop.Models
{
    public class AppSettings
    {
        public string MarketName { get; set; } = "Market Automation";

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string CashierName { get; set; } = "";

        public string ApiUrl { get; set; } = "https://localhost:7116/";
    }
}