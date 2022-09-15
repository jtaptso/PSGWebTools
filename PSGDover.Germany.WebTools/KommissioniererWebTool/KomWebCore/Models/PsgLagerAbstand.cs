using KomWebCore.Models;
using System.Collections.Generic;

namespace PsgSAPb1WebTools.Models
{
    public class PsgLagerAbstand
    {
        public string ItemCode { get; set; }
        public List<PsgArtikelLager> LagerAbstandList { get; set; }
    }
}
