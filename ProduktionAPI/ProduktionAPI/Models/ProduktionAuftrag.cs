namespace ProduktionAPI.Models
{
    public class ProduktionAuftrag
    {
        public int DocNum { get; set; }
        public int PaStatus { get; set; }
        public bool IsUpdated { get; set; }
        public string ItemCode { get; set; }
        public string ProdName { get; set; }
        public double PlannedQty { get; set; }
        public string Comments { get; set; }
        public string PickRmrk { get; set; }
        public List<int> UnterPAs { get; set; }
    }
}
