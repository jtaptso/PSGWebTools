using System.Collections.Generic;

namespace PsgSAPb1WebTools.Models
{
    public class SAPArtikel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ErrorMessage { get; set; }
        public List<Stueck> StueckListe { get; set; }
        public List<List<Stueck>> GroupedList { get; set; }
        public IEnumerable<Stueck> hierarchy { get; set; }
        public Dictionary<string, Stueck> StkList { get; set; } = new Dictionary<string, Stueck>();
        public Stueck Parent { get; set; }

        public IEnumerable<Stueck> FlatToHierarchy(List<Stueck> list)
        {
            // hashtable lookup that allows us to grab references to containers based on id
            var lookup = new Dictionary<string, Stueck>();
            // actual nested collection to return
            var nested = new List<Stueck>();

            foreach (Stueck item in list)
            {
                if (lookup.ContainsKey(item.Father))
                {
                    // add to the parent's child list 
                    lookup[item.Father].Children.Add(item);
                }
                else
                {
                    // no parent added yet (or this is the first time)
                    nested.Add(item);
                }
                lookup.Add(item.ItemNummer, item);
            }
            StkList = lookup;
            return nested;
        }

        public IEnumerable<Stueck> PAFlatToHierarchy(List<Stueck> list)
        {
            // hashtable lookup that allows us to grab references to containers based on id
            var lookup = new Dictionary<string, Stueck>();
            // actual nested collection to return
            var nested = new List<Stueck>();

            foreach (Stueck item in list)
            {
                if (lookup.ContainsKey(item.Father))
                {
                    // add to the parent's child list 
                    lookup[item.Father].Children.Add(item);
                }
                else
                {
                    // no parent added yet (or this is the first time)
                    nested.Add(item);
                }
                lookup.Add(item.ItemNummer, item);
            }
            StkList = lookup;
            return nested;
        }
    }

    
}
