using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAutomation2.Desktop.Models
{
    public class CreateSaleRequest
    {
        public string PaymentType { get; set; } = "";

        public List<CreateSaleItemRequest> Items { get; set; } = new();
    }
}
