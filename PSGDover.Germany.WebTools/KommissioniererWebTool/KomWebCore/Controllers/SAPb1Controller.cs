using KomWebCore.Models;
using KomWebCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsgSAPb1WebTools.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KomWebCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SAPb1Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static bool isConnected = false;
        private static Company psgCompany = null;
        private static SAPArtikel item;
        private static List<string> items = new List<string>();
        public SAPb1Controller(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: api/<SAPb1Controller>
        [HttpGet]
        [Route("Connect")]
        public SAPB1Company ConnectToSAPb1()
        {
            SAPB1Company compagny = null;
            try
            {
                var sapConnection = new SAPConnectionService(_configuration);

                if (sapConnection.ConnectToSAP() == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                    compagny = new SAPB1Company
                    {
                        CompanyName = psgCompany.CompanyName,
                        CompanyDB = psgCompany.CompanyDB,
                        UserName = psgCompany.UserName,
                        Connected = psgCompany.Connected,
                        Server = psgCompany.Server
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
            return compagny;
        }

        [Route("Customers")]
        [HttpGet]
        public List<PsgCustomer> GetCustomers()
        {
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }

            List<PsgCustomer> psgCustomers = new List<PsgCustomer>();

            try
            {
                if (isConnected)
                {

                    Recordset oRecordSet;
                    oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    oRecordSet.DoQuery("select DISTINCT top 10 CardCode, CardFName, CardType, [Address] from OCRD where CardCode != '@'");

                    while (!oRecordSet.EoF)
                    {
                        if (!oRecordSet.EoF)
                        {
                            var psgCustomer = new PsgCustomer();
                            psgCustomer.CardCode = oRecordSet.Fields.Item(0).Value;
                            psgCustomer.CardName = oRecordSet.Fields.Item(1).Value;
                            psgCustomer.CardType = char.Equals(oRecordSet.Fields.Item(2).Value, 'C') ? "Kunde" : "Lieferer";
                            psgCustomer.Address = oRecordSet.Fields.Item(3).Value;
                            psgCustomers.Add(psgCustomer);
                        }

                        oRecordSet.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
            return psgCustomers;
        }

        [Route("GetProdAuftrag/{PANummer}")]
        [HttpGet]
        public PsgProdAuftrag GetProdAuftrag(int PANummer)
        {
            var pa = new PsgProdAuftrag();
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
            try
            {
                pa = GetPa(PANummer);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                
            }
            return pa;
        }

        private PsgProdAuftrag GetPa(int pANummer)
        {
            var pa = new PsgProdAuftrag();
            Recordset oRecordSet;
            oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
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

        [Route("GetProdUser/{IDNummer}")]
        [HttpGet]
        public ProdUser GetProdUser(int IDNummer)
        {
            var user = new ProdUser();
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
            try
            {
                user = GetUser(IDNummer);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

            }
            return user;
        }

        private ProdUser GetUser(int iDNummer)
        {
            var prodUser = new ProdUser();
            Recordset oRecordSet;
            oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"SELECT Code, Name, U_PG_Brand FROM [@PG_MONTEUR]" +
                           $" WHERE U_PG_SN = '{iDNummer}'";

            oRecordSet.DoQuery(query);
            if (oRecordSet.RecordCount > 0)
            {
                while (!oRecordSet.EoF)
                {
                    prodUser.UserExists = true;
                    prodUser.Code = int.Parse(oRecordSet.Fields.Item(0).Value.ToString());
                    prodUser.Name = oRecordSet.Fields.Item(1).Value.ToString();
                    oRecordSet.MoveNext();
                }
            }
            return prodUser;
        }

        [Route("UpdateProdAuftrag/{DocNum}/{PaStatus}")]
        [HttpGet]
        public PsgProdAuftrag UpdateProdAuftrag(string DocNum, int PaStatus)
        {
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
            var pa = new PsgProdAuftrag();
            try
            {
                int updateRes = -1;
                ProductionOrders pOrder = psgCompany.GetBusinessObject(BoObjectTypes.oProductionOrders);
                var pOrderExist = pOrder.GetByKey(int.Parse(DocNum));

                if (pOrderExist)
                {
                    
                    pOrder.UserFields.Fields.Item("U_PG_PAST").Value = PaStatus.ToString();
                    switch (PaStatus)
                    {
                        case 2:
                            {
                                if (string.IsNullOrEmpty(pOrder.UserFields.Fields.Item("U_PG_DMoE").Value))
                                {
                                    pOrder.UserFields.Fields.Item("U_PG_DMoE").Value = DateTime.Now;
                                }
                                break;
                            }
                        case 3:
                            {
                                if (string.IsNullOrEmpty(pOrder.UserFields.Fields.Item("U_PG_DMoG").Value))
                                {
                                    pOrder.UserFields.Fields.Item("U_PG_DMoG").Value = DateTime.Now;
                                }
                                    
                                break;
                            }
                        case 4:
                            {
                                if (string.IsNullOrEmpty(pOrder.UserFields.Fields.Item("U_PG_DMoU").Value))
                                {
                                    pOrder.UserFields.Fields.Item("U_PG_DMoU").Value = DateTime.Now;
                                }
                                
                                break;
                            }
                        case 5:
                            {
                                if (string.IsNullOrEmpty(pOrder.UserFields.Fields.Item("U_PG_DMoA").Value))
                                {
                                    pOrder.UserFields.Fields.Item("U_PG_DMoA").Value = DateTime.Now;
                                }
                                
                                break;
                            }
                        case 6:
                            {
                                if (string.IsNullOrEmpty(pOrder.UserFields.Fields.Item("U_PG_DPrA").Value))
                                {
                                    pOrder.UserFields.Fields.Item("U_PG_DPrA").Value = DateTime.Now;
                                }
                                
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
            catch (Exception ex)
            {
                var msg = ex.Message;

            }
            return pa;
        }

        [Route("Lagerbestand/{ItemCode}")]
        [HttpGet]
        public List<PsgArtikelLager> Lagerbestand(string ItemCode)
        {
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
            List<PsgArtikelLager> psgLagerbestaende = new List<PsgArtikelLager>();
            try
            {
                if (isConnected)
                {

                    Recordset oRecordSet;
                    oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    oRecordSet.DoQuery($"SELECT T1.BinCode, T0.OnHandQty, T0.ItemCode, T2.ItemName " +
                                       $"FROM OIBQ T0 " +
                                       $"INNER JOIN OBIN T1 ON T0.BinAbs = T1.AbsEntry AND T0.OnHandQty <> 0 " +
                                       $"INNER JOIN OITM T2 ON T2.ItemCode = T0.ItemCode " +
                                       $"WHERE T1.WhsCode = 'L01' AND  T0.ItemCode = '{ItemCode}' ORDER BY T1.BinCode ASC ");

                    while (!oRecordSet.EoF)
                    {
                        if (!oRecordSet.EoF)
                        {
                            var psgLagerbestand = new PsgArtikelLager();

                            psgLagerbestand.Lagerplatz = oRecordSet.Fields.Item(0).Value; 
                            //psgLagerbestand.Artikelmenge = oRecordSet.Fields.Item(1).Value;
                            psgLagerbestand.Artikelnummer = oRecordSet.Fields.Item(2).Value;
                            psgLagerbestand.Artikelbeschreibung = oRecordSet.Fields.Item(3).Value;

                            psgLagerbestaende.Add(psgLagerbestand);
                        }

                        oRecordSet.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
            return psgLagerbestaende;
        }

        [Route("Lagerbestands/{ItemCode}")]
        [HttpGet]
        public List<PsgArtikelLager> Lagerbestands(string ItemCode)
        {
            var lst = ItemCode.Split(',');
            lst = lst.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var lstArray = new string[lst.Length];
            for (int i = 0; i < lst.Length; i++)
            {
                lstArray[i] = $"'{lst[i].Trim()}'"; 
            }

            var lststrg = string.Join(",", lstArray);
            List<PsgArtikelLager> psgLagerbestaende = new List<PsgArtikelLager>();
            
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
            try
            {
                if (isConnected)
                {
                    Recordset oRecordSet;
                    oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    
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
                            var psgLagerbestand = new PsgArtikelLager();

                            psgLagerbestand.Lagerplatz = oRecordSet.Fields.Item(0).Value;
                            psgLagerbestand.Artikelnummer = oRecordSet.Fields.Item(2).Value;
                            psgLagerbestand.Artikelbeschreibung = oRecordSet.Fields.Item(3).Value;

                            psgLagerbestaende.Add(psgLagerbestand);
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
            return psgLagerbestaende /*lagers*/;
        }

        [Route("BOM/{ItemCode}")]
        [HttpGet]
        public Stueck BillOfMaterials(string ItemCode)
        {
            item = new SAPArtikel();
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
                else
                {
                    
                    item.ErrorMessage = $"Connection to SAP failed";
                    return item.Parent;
                }
            }
            try
            {
                if (isConnected)
                {
                    var parent = new Stueck { ItemNummer = ItemCode, level = 1, Father = ItemCode };
                    item.StueckListe = new List<Stueck>();
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
        
        private List<Stueck> GetChildren(Stueck itemParent)
        {
            var children = new List<Stueck>();
            Recordset oRecordSet;
            oRecordSet = psgCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = $"SELECT T1.Code, T1.ItemName, T1.Quantity FROM OITT T0 INNER JOIN ITT1 T1 ON T0.Code = T1.Father" +
                           $" WHERE T0.Code = '{itemParent.ItemNummer}'";
            oRecordSet.DoQuery(query);
            var reLst = new List<Stueck>();
            while (!oRecordSet.EoF)
            {
                var itm = new Stueck();
                itm.ItemNummer = oRecordSet.Fields.Item(0).Value.ToString();
                itm.ItemName = oRecordSet.Fields.Item(1).Value.ToString();
                itm.Quantity = int.Parse(oRecordSet.Fields.Item(2).Value.ToString());
                reLst.Add(itm);
                oRecordSet.MoveNext();
            }
            if (oRecordSet.RecordCount > 0)
            {
                var myList = new List<Stueck>();
                var count = oRecordSet.RecordCount;
                for (int i = 0; i < reLst.Count; i++)
                {
                    var stk = new Stueck();
                    stk.level = itemParent.level + 1;
                    stk.ItemNummer = reLst[i].ItemNummer;
                    stk.ItemName = reLst[i].ItemName;
                    stk.Quantity = reLst[i].Quantity;
                    stk.Father = itemParent.ItemNummer;
                    
                    children.Add(stk);
                    GetChildren(stk);
                }
            }
            itemParent.Children = children;
            item.StueckListe.Add(itemParent);
            return children;
        }

        private List<PsgArtikelLager> ArrayToList(List<PsgArtikelLager>[] lagers)
        {
            List<PsgArtikelLager> totalList = new List<PsgArtikelLager>(); 
            foreach (var item in lagers)
            {
                foreach (var subitem in item)
                {
                    totalList.Add(subitem);
                }
            }
            return totalList;
        }

        void ConnectToSBO()
        {
            if (!isConnected)
            {
                var sapConnection = new SAPConnectionService(_configuration);
                var connectResult = sapConnection.ConnectToSAP();
                if (connectResult == 0)
                {
                    isConnected = true;
                    psgCompany = sapConnection.GetCompagny();
                }
            }
        }
    }
}
