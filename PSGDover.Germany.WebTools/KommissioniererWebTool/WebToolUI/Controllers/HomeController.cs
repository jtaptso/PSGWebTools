using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebToolUI.Helper;
using WebToolUI.Models;

namespace WebToolUI.Controllers
{
    public class HomeController : Controller
    {
        Endpoints endpointApi = new Endpoints();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ConnectViewModel connect = new ConnectViewModel();
            connect = await endpointApi.Connect2SAP();
            
            return View(connect);
        }

        [Route("Lagerbestand/{itemcode}")]
        public async Task<IActionResult> GetLagerbestand(string itemcode)
        {
            var lagerbestaende = new List<LagerbestandViewModel>();
            lagerbestaende = await endpointApi.Lagerbestaende(itemcode);

            return View(lagerbestaende);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}