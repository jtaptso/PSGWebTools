using ProduktionAPI.DAL;
using ProduktionAPI.Models;
using SAPbobsCOM;

namespace ProduktionAPI.BLL
{
    public class LagerbestandBLL
    {
        private readonly IConfiguration configuration;
        public LagerbestandBLL(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public List<Lagerbestand> Lagerbestands(string ItemCode)
        {
            var lgBestands = new List<Lagerbestand>();

            try
            {
                var lst = ItemCode.Split(',');
                lst = lst.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var lstArray = new string[lst.Length];
                for (int i = 0; i < lst.Length; i++)
                {
                    lstArray[i] = $"'{lst[i].Trim()}'";
                }

                var lststrg = string.Join(",", lstArray);

                if (SAPBOne.IsConnected)
                {
                    Recordset oRecordSet;
                    oRecordSet = SAPBOne.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                    string query = "";
                    query = $"SELECT T1.BinCode, T0.OnHandQty, T0.ItemCode, T2.ItemName " +
                                       $"FROM OIBQ T0 " +
                                       $"INNER JOIN OBIN T1 ON T0.BinAbs = T1.AbsEntry AND T0.OnHandQty <> 0 " +
                                       $"INNER JOIN OITM T2 ON T2.ItemCode = T0.ItemCode " +
                                       $"WHERE T1.WhsCode = 'L01' AND  T0.ItemCode in ({lststrg}) ORDER BY T1.BinCode ASC ";
                    oRecordSet.DoQuery(query);
                    while (!oRecordSet.EoF)
                    {
                        if (!oRecordSet.EoF)
                        {
                            var psgLagerbestand = new Lagerbestand();

                            psgLagerbestand.Lagerplatz = oRecordSet.Fields.Item(0).Value;
                            psgLagerbestand.Artikelnummer = oRecordSet.Fields.Item(2).Value;
                            psgLagerbestand.Artikelbeschreibung = oRecordSet.Fields.Item(3).Value;

                            lgBestands.Add(psgLagerbestand);
                        }
                        oRecordSet.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
            return lgBestands;


        }
    }
}
