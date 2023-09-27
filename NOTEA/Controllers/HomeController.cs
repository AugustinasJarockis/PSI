using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Diagnostics;

namespace NOTEA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ConspectListModel conspectListModel = new ConspectListModel();
        public static FileHandlerModel filemodel = new FileHandlerModel();
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
            return View();
        }

        [HttpPost]
        public IActionResult CreateConspects(string name, string conspectText)
        {
            ConspectModel conspectModel = new ConspectModel(
                DateTime.Now.ToString("yyyy-MM-dd"),
                name,
                conspectText
                );
            DataService.SaveConspect(conspectModel);

            CloseWindow();

            return View();
        }
        public IActionResult CloseWindow()
        {
            TempData["SuccessMessage"] = "Your notea has been saved successfully!";
            return RedirectToAction(nameof(CreateConspects));
        }
        public IActionResult UploadConspect()
        {
            return View(filemodel);
        }

        [HttpPost]
        public IActionResult UploadConspect(IFormFile file)
        {
            if(file.ContentType == "text/plain")
            {
                String text = "";
                using (Stream stream = file.OpenReadStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        while ((text = sr.ReadLine()) != null)
                        {
                            Console.WriteLine(text);
                        }
                    }
                }
                DataService.SaveConspect(
                    new ConspectModel(
                        "Remove this later",
                        Path.GetFileNameWithoutExtension(file.FileName),
                        text
                        )
                    );
                //DataService.SaveFileName(fileNameModel, Path.GetFileNameWithoutExtension(file.FileName));
            }
            else
            {
                Console.WriteLine("Error: wrong type of file specified");
            }
            return View(filemodel);
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


        [HttpGet]
        public IActionResult ConspectList()
        {
            conspectListModel = DataService.LoadConspects("Conspects");
            return View(conspectListModel);
        }

    }
}