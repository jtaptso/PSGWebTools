using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KomWebCore.Models
{
    public class SAPB1Company
    {
        public string Server { get; set; }
        public string CompanyDB { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public bool Connected { get; set; }

        //public BoSuppLangs language { get; set; }
        //public UserTables UserTables { get; set}
        //public BoDataServerTypes DbServerType { get; set; }
    }
}
