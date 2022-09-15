using System.Collections.Generic;

namespace PsgSAPb1WebTools.Models
{
    public class Order
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public string Status { get; set; } = "Success";
        public string Message { get; set; }

        public Order()
        {
            OrderLines = new List<OrderLine>();
        }
    }
}
