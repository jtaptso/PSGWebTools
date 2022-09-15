namespace ProduktionAPI.Models
{
    public class Artikel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ErrorMessage { get; set; }
        public List<BillOfMaterial> StueckListe { get; set; }
        public List<List<BillOfMaterial>> GroupedList { get; set; }
        public IEnumerable<BillOfMaterial> hierarchy { get; set; }
        public Dictionary<string, BillOfMaterial> StkList { get; set; } = new Dictionary<string, BillOfMaterial>();
        public BillOfMaterial Parent { get; set; }
    }
}
