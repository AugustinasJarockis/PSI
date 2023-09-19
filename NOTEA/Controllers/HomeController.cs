using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Diagnostics;

namespace NOTEA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static ConspectModel model = new ConspectModel(); 

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateConspects()
        {
            Console.WriteLine(model.Name);
            model.Name = "Default name";
            return View(model);
        }

        public string Save()
        {
            return "Stringy";
        }
        public IActionResult HowToUse()
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