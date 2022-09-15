using Newtonsoft.Json;
using WebToolUI.Models;

namespace WebToolUI.Helper
{
    public class Endpoints : BaseEndpoint
    {
        public Endpoints()
        {
            Api = Initialize();
        }

        //Connect
        public async Task<ConnectViewModel> Connect2SAP()
        {
            ConnectViewModel connect = new ConnectViewModel();
            HttpResponseMessage result = await Api.GetAsync("SAPb1/Connect");
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                connect = JsonConvert.DeserializeObject<ConnectViewModel>(res);
            }

            return connect;
        }
        //Get customer by Id
        public async Task<CustomerViewModel> Customer(int id)
        {
            var uri = $"SAPb1/Customer/{id}";
            CustomerViewModel customer = new CustomerViewModel();
            HttpResponseMessage result = await Api.GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                customer = JsonConvert.DeserializeObject<CustomerViewModel>(res);
            }
            return customer;
        }

        //Get list of customers
        public async Task<List<CustomerViewModel>> Customers()
        {
            var customers = new List<CustomerViewModel>();
            HttpResponseMessage result = await Api.GetAsync("SAPb1/Customers");
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(res);
            }
            return customers;

        }
        
        //Get Produktionsauftrag by auftragNr
        public async Task<ProduktionsauftragViewModel> Prodauftrag(int auftragNr)
        {
            var prodauftrag = new ProduktionsauftragViewModel();
            var uri = $"SAPb1/ProduktionAuftrag/{auftragNr}";
            HttpResponseMessage result = await Api.GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                prodauftrag = JsonConvert.DeserializeObject<ProduktionsauftragViewModel>(res);
            }
            return prodauftrag;
        }

        //Get list of Produktionsaufträge
        //public async Task<List<ProduktionsauftragViewModel>> Prodauftraege()
        //{

        //}
        //Get GetLagerbestaende by itemcode
        public async Task<List<LagerbestandViewModel>> Lagerbestaende(string itemcode)
        {
            var lagerbestaende = new List<LagerbestandViewModel>();
            var uri = $"SAPb1/Lagerbestand/{itemcode}";
            HttpResponseMessage result = await Api.GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                lagerbestaende = JsonConvert.DeserializeObject<List<LagerbestandViewModel>>(res);
            }
            return lagerbestaende;
        }
    }
}
