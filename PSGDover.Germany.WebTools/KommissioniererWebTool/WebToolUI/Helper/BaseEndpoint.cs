namespace WebToolUI.Helper
{
    public class BaseEndpoint
    {
        public HttpClient Api { get; set; }
        public HttpClient Initialize()
        {
            var baseUrl = "http://psg-ger-sap:8082/";
            var devUrl = "https://localhost:5001/";
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            return client;
        }
    }
}
