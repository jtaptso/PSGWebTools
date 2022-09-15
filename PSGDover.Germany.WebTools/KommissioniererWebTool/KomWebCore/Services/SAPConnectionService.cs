using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace KomWebCore.Services
{
    public class SAPConnectionService
    {
        private SAPbobsCOM.Company Company = new SAPbobsCOM.Company();
        public int ConnectionResult { get; set; }
        private int ErrorCode = 0;
        private string ErrorMessage = "";

        private readonly IConfiguration _configuration;

        public SAPConnectionService(IConfiguration configuration)
        {
            _configuration = configuration;
            //ConnectToSAP();
        }

        public int ConnectToSAP()
        {
            Company.Server = _configuration.GetValue<string>("SAPCredentials:Server");
            Company.CompanyDB = _configuration.GetValue<string>("SAPCredentials:CompanyDB");
            Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
            Company.DbUserName = _configuration.GetValue<string>("SAPCredentials:DbUserName");
            Company.DbPassword = _configuration.GetValue<string>("SAPCredentials:DbPassword");
            Company.UserName = _configuration.GetValue<string>("SAPCredentials:UserName");
            Company.Password = _configuration.GetValue<string>("SAPCredentials:Password");
            Company.language = SAPbobsCOM.BoSuppLangs.ln_German;
            Company.UseTrusted = _configuration.GetValue<bool>("SAPCredentials:UseTrusted");
            Company.SLDServer = $"https://{Company.Server}:40000/";

            ConnectionResult = Company.Connect();

            if (ConnectionResult != 0)
            {
                Company.GetLastError(out ErrorCode, out ErrorMessage);
            }

            return ConnectionResult;
        }

        public SAPbobsCOM.Company GetCompagny()
        {
            return Company;
        }
    }
}
