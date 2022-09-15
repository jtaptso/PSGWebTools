using ProduktionAPI.DAL;
using ProduktionAPI.Models;

namespace ProduktionAPI.BLL
{
    public class ConnectBLL
    {
        public SAPB1User Connect()
        {
            SAPB1User sapUser = null;
            try
            {
                if (SAPBOne.IsConnected)
                {
                    sapUser = new SAPB1User
                    {
                        CompanyName = SAPBOne.Company.CompanyName,
                        CompanyDB = SAPBOne.Company.CompanyDB,
                        UserName = SAPBOne.Company.UserName,
                        Connected = SAPBOne.Company.Connected,
                        Server = SAPBOne.Company.Server
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }


            return sapUser;
        }
    }
}
