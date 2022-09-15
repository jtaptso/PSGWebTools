using ProduktionAPI.DAL;
using ProduktionAPI.Models;
using SAPbobsCOM;

namespace ProduktionAPI.BLL
{
    public class BillOfMaterialBLL
    {
        private static Artikel item;

        //private readonly IConfiguration configuration;
        //public BillOfMaterialBLL(IConfiguration _configuration)
        //{
        //    configuration = _configuration;
        //}
        public BillOfMaterial BillOfMaterials(string ItemCode)
        {
            item = new Artikel();
            try
            {
                if (SAPBOne.IsConnected)
                {
                    var parent = new BillOfMaterial { ItemNummer = ItemCode, level = 1, Father = ItemCode };
                    item.StueckListe = new List<BillOfMaterial>();
                    var tChil = GetChildren(parent);
                    item.Parent = parent;

                    parent.Children = tChil;

                    var groupedCustomerList = item.StueckListe
                        .GroupBy(u => u.level)
                        .Select(grp => grp.ToList())
                        .ToList();

                    item.GroupedList = groupedCustomerList;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                item.ErrorMessage = $"something wrong happened: {msg}";
                return item.Parent;
            }
            return item.Parent;
        }

        private List<BillOfMaterial> GetChildren(BillOfMaterial itemParent)
        {
            var children = new List<BillOfMaterial>();
            Recordset oRecordSet;
            oRecordSet = SAPBOne.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"SELECT T1.Code, T1.ItemName, T1.Quantity,T2.[U_PG_QFLP] FROM OITT T0 INNER JOIN ITT1 T1 ON T0.Code = T1.Father " +
                           $"INNER JOIN OITM T2 ON T1.[Code] = T2.[ItemCode] " +
                           $"WHERE T0.Code = '{itemParent.ItemNummer}'";
            oRecordSet.DoQuery(query);
            var reLst = new List<BillOfMaterial>();
            while (!oRecordSet.EoF)
            {
                var itm = new BillOfMaterial();
                itm.ItemNummer = oRecordSet.Fields.Item(0).Value.ToString();
                itm.ItemName = oRecordSet.Fields.Item(1).Value.ToString();
                itm.Quantity = int.Parse(oRecordSet.Fields.Item(2).Value.ToString());
                string lgPlatz = oRecordSet.Fields.Item(3).Value;
                itm.Lagerplatz = string.IsNullOrEmpty(lgPlatz) ? "n.a" : lgPlatz;
                reLst.Add(itm);
                oRecordSet.MoveNext();
            }
            if (oRecordSet.RecordCount > 0)
            {
                var myList = new List<BillOfMaterial>();
                var count = oRecordSet.RecordCount;
                for (int i = 0; i < reLst.Count; i++)
                {
                    var stk = new BillOfMaterial();
                    stk.level = itemParent.level + 1;
                    stk.ItemNummer = reLst[i].ItemNummer;
                    stk.ItemName = reLst[i].ItemName;
                    stk.Quantity = reLst[i].Quantity;
                    stk.Lagerplatz = reLst[i].Lagerplatz;
                    stk.Father = itemParent.ItemNummer;

                    children.Add(stk);
                    GetChildren(stk);
                }
            }
            itemParent.Children = children;
            item.StueckListe.Add(itemParent);
            return children;
        }
    }
}
