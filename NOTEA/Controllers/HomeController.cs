using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NOTEA.Models.ErrorModels;

namespace NOTEA.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var httpClient = new HttpClient();
            var response = (await httpClient.GetAsync("http://localhost:5063/index"));
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Privacy()
        {
            var httpClient = new HttpClient();
            var response = (await httpClient.GetAsync("http://localhost:5063/privacy"));
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}