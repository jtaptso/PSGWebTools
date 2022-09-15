using ProduktionAPI.DAL;
using ProduktionAPI.Models;
using SAPbobsCOM;

namespace ProduktionAPI.BLL
{
    public class MonteurBLL
    {
        public static Monteur Betriebsmitarbeiter;
        public Monteur GetMonteur(int IDNummer)
        {
            var user = new Monteur();
            
            try
            {
                if (SAPBOne.IsConnected)
                {
                    user = GetUser(IDNummer);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return user;
        }

        public Monteur GetUser(int iDNummer)
        {
            var monteur = new Monteur();
            Recordset oRecordSet;
            oRecordSet = SAPBOne.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"SELECT Code, Name, U_PG_Brand, U_PG_UserTyp FROM [@PG_BMA]" +
                           $" WHERE U_PG_SN = '{iDNummer}'";

            oRecordSet.DoQuery(query);
            if (oRecordSet.RecordCount > 0)
            {
                while (!oRecordSet.EoF)
                {
                    monteur.UserExists = true;
                    monteur.Code = int.Parse(oRecordSet.Fields.Item(0).Value.ToString());
                    monteur.KeyNummer = iDNummer;
                    monteur.Name = oRecordSet.Fields.Item(1).Value.ToString();
                    monteur.Brand = oRecordSet.Fields.Item(2).Value.ToString();
                    monteur.UserTyp = oRecordSet.Fields.Item(3).Value.ToString();
                    oRecordSet.MoveNext();
                }
            }
            Betriebsmitarbeiter = monteur;
            return monteur;
        }
    }
}
