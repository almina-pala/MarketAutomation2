using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAutomation2.Desktop.Models
{
    public class CreateSaleItemRequest
    {
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }
    }
}
