namespace PsgSAPb1WebTools.Models
{
    public class OrderHeader
    {
        public string PartnerEmail { get; set; } // Find in OCPR the internal Code 
        public string CardCode { get; set; } // Customer_Number
        public string ShipToCode { get; set; } // ShipTocode
        public string External_Order_Num { get; set; } //U_PG_ISOrdNo varchar(16)
        public string Shipping_Method { get; set; }
        public string Frieght_Term { get; set; } // U_OZV_FrgtTerm
        public string Carrier_Acctno { get; set; } //varchar(20) - U_PG_CAcNo
        public bool Batch_Shipment { get; set; } = true;
        public string BillToCode { get; set; } // PayToCode
        public string Ship_To_Cust_Pickup_Name { get; set; }
        public string Ship_To_Cust_Pickup_Phone { get; set; }
        public string Ship_To_Cust_Pickup_Email { get; set; }
        public string NumAtCard { get; set; } // PO Number
        public string dataPath { get; set; } // filename entnehmen
        public int OwnerCode { get; set; } // von OCRD using Customer_Number
        public string RequestedChipDate { get; set; }
        public string Status { get; set; } = "Success";
        public string Message { get; set; }
    }
}
