using SAPbobsCOM;
using System.Collections.Generic;

namespace PsgSAPb1WebTools.Models
{
    public class Stueck
    {
        public string ItemNummer { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Father { get; set; }
        public int level { get; set; }
        public IList<Stueck> Children { get; set; } = new List<Stueck>();
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
