namespace ProduktionAPI.Models
{
    public class BillOfMaterial
    {
        public string ItemNummer { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Father { get; set; }
        public int level { get; set; }
        public string Lagerplatz { get; set; }
        public IList<BillOfMaterial> Children { get; set; } = new List<BillOfMaterial>();
        public int PAChild { get; set; }
        public int PAFather { get; set; }
        public bool HasChildren
        {
            get
            {
                return Children.Count > 0;
            }
        }
    }
}
