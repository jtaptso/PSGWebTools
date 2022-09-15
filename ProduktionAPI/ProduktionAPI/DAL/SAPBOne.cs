using SAPbobsCOM;

namespace ProduktionAPI.DAL
{
    public class SAPBOne
    {
        private readonly IConfiguration _configuration;
        private int ErrorCode = 0;
        private string ErrorMessage = "";
        public static Company Company;
        public static int ConnectionResult { get; set; }
        public static bool IsConnected { get; set; }
        public SAPBOne(IConfiguration configuration)
        {
            _configuration = configuration;
            if (!IsConnected) ConnectionResult = Connect();
        }
        public int Connect()
        {
            Company = new Company();
            Company.Server = _configuration.GetSection("SAPCredentials:Server").Value;
            Company.CompanyDB = _configuration.GetSection("SAPCredentials:CompanyDB").Value;
            Company.DbServerType = BoDataServerTypes.dst_MSSQL2016;
            Company.DbUserName = _configuration.GetSection("SAPCredentials:DbUserName").Value;
            Company.DbPassword = _configuration.GetSection("SAPCredentials:DbPassword").Value;
            Company.UserName = _configuration.GetSection("SAPCredentials:UserName").Value;
            Company.Password = _configuration.GetSection("SAPCredentials:Password").Value;
            Company.language = BoSuppLangs.ln_English;

            ConnectionResult = Company.Connect();

            if (ConnectionResult != 0) Company.GetLastError(out ErrorCode, out ErrorMessage);
            else IsConnected = true;


            return ConnectionResult;
        }

    }
}
