using ProduktionAPI.DAL;
using ProduktionAPI.Models;
using SAPbobsCOM;

namespace ProduktionAPI.BLL
{
    public class ProduktionAuftragBLL
    {
        public ProduktionAuftrag GetProdAuftrag(int PANummer)
        {
            ProduktionAuftrag pa = new ProduktionAuftrag();
            
            try
            {
                if (SAPBOne.IsConnected)
                {
                    pa = GetSelectedPa(PANummer);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

            }
            return pa;
        }

        private ProduktionAuftrag GetSelectedPa(int pANummer)
        {
            var pa = new ProduktionAuftrag();
            Recordset oRecordSet;
            oRecordSet = SAPBOne.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"SELECT U_PG_PAST FROM OWOR" +
                           $" WHERE DocNum = {pANummer}";

            oRecordSet.DoQuery(query);
            if (oRecordSet.RecordCount > 0)
            {
                while (!oRecordSet.EoF)
                {
                    pa.DocNum = pANummer;
                    pa.PaStatus = int.Parse(oRecordSet.Fields.Item(0).Value.ToString());
                    oRecordSet.MoveNext();
                }
            }
            return pa;
        }

        public ProduktionAuftrag UpdateProdAuftrag(string DocNum, int PaStatus, int MonteurCode)
        {
            
            var pa = new ProduktionAuftrag();
            try
            {
                if (SAPBOne.IsConnected)
                {
                    int updateRes = -1;
                    ProductionOrders pOrder = SAPBOne.Company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                    var pOrderExist = pOrder.GetByKey(int.Parse(DocNum));

                    if (pOrderExist)
                    {
                        var loc = DateTime.Now.ToLocalTime();

                        pOrder.UserFields.Fields.Item("U_PG_PAST").Value = PaStatus.ToString();
                        if (MonteurBLL.Betriebsmitarbeiter.UserTyp == "M")
                        {
                            pOrder.UserFields.Fields.Item("U_PG_MO").Value = MonteurCode.ToString();
                        }
                        else
                        {
                            pOrder.UserFields.Fields.Item("U_PG_PR").Value = MonteurCode.ToString();
                        }
                        var minDt = 0;
                        switch (PaStatus)
                        {
                            case 2:
                                {
                                    //var dt = pOrder.UserFields.Fields.Item("U_PG_DMoE").Value;
                                    //minDt = dt.Year;
                                    //if (minDt < 1990)
                                    //{
                                    //    pOrder.UserFields.Fields.Item("U_PG_DMoE").Value = DateTime.Now.ToLocalTime();
                                    //}
                                    pOrder.UserFields.Fields.Item("U_PG_DMoE").Value = DateTime.Now.ToLocalTime();
                                    break;
                                }
                            case 3:
                                {
                                    //var dt = pOrder.UserFields.Fields.Item("U_PG_DMoG").Value;

                                    //if(!string.IsNullOrEmpty(dt))
                                    //    minDt = Convert.ToDateTime(dt).Year;
                                    //if (minDt < 1990)
                                    //{
                                    //    pOrder.UserFields.Fields.Item("U_PG_DMoG").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    //}
                                    pOrder.UserFields.Fields.Item("U_PG_DMoG").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    break;
                                }
                            case 4:
                                {
                                    //var dt = pOrder.UserFields.Fields.Item("U_PG_DMoU").Value;
                                    //if (!string.IsNullOrEmpty(dt))
                                    //    minDt = Convert.ToDateTime(dt).Year;
                                    //if (minDt < 1990)
                                    //{
                                    //    pOrder.UserFields.Fields.Item("U_PG_DMoU").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    //}
                                    pOrder.UserFields.Fields.Item("U_PG_DMoU").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    break;
                                }
                            case 5:
                                {
                                    //var dt = pOrder.UserFields.Fields.Item("U_PG_DMoA").Value;
                                    //if (!string.IsNullOrEmpty(dt))
                                    //    minDt = Convert.ToDateTime(dt).Year;
                                    //if (minDt < 1990)
                                    //{
                                    //    pOrder.UserFields.Fields.Item("U_PG_DMoA").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    //}
                                    pOrder.UserFields.Fields.Item("U_PG_DMoA").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    break;
                                }
                            case 6:
                                {
                                    //var dt = pOrder.UserFields.Fields.Item("U_PG_DPrA").Value;
                                    //if (!string.IsNullOrEmpty(dt))
                                    //    minDt = Convert.ToDateTime(dt).Year;
                                    //if (minDt < 1990)
                                    //{
                                    //    pOrder.UserFields.Fields.Item("U_PG_DPrA").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    //}
                                    pOrder.UserFields.Fields.Item("U_PG_DPrA").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    break;
                                }
                            default:
                                break;
                        }
                        updateRes = pOrder.Update();
                        if (updateRes == 0)
                        {
                            pa.PaStatus = PaStatus;
                            pa.DocNum = int.Parse(DocNum);
                            pa.IsUpdated = true;
                        }
                        // Logik to increment or decrement the new UDV for the kommissionierer
                    }
                }
                
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

            }
            return pa;
        }
    }
}
