using System;

namespace PsgSAPb1WebTools.Models
{
    public class OrderLine
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double Discount { get; set; }
        public DateTime RequestedShipDate { get; set; }
    }
}
