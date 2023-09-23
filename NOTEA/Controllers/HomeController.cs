using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Diagnostics;

namespace NOTEA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static ConspectModel model = new ConspectModel();
        private readonly IDataService DataService; 

        public HomeController(ILogger<HomeController> logger, IDataService dataService)
        {
            _logger = logger;
            DataService = dataService;
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
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateConspects(string date, string name, string conspectText)
        {
            Console.WriteLine(model.Name);
            model.Date = date;
            model.Name = name;
            model.ConspectText = conspectText;

            ConspectModel conspectModel = new ConspectModel(date, name, conspectText);
            DataService.SaveConspects(conspectModel);

            CloseWindow();

            return View(model);
        }
       
        public IActionResult CloseWindow()
        {
            TempData["SuccessMessage"] = "Your notea has been saved successfully!";
            return RedirectToAction(nameof(CreateConspects));
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