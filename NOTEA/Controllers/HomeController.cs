using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace NOTEA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static ConspectModel model = new ConspectModel();
        public ConspectListModel conspectListModel = new ConspectListModel();
        public FileNameListModel fileNameList = new FileNameListModel();
        public FileNameModel fileNameModel = new FileNameModel();
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
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateConspects(string date, string name, string conspectText)
        {
            model.Date = date;
            model.Name = name;
            model.ConspectText = conspectText;

            ConspectModel conspectModel = new ConspectModel(date, name, conspectText);
            DataService.SaveConspect(conspectModel);
            DataService.SaveFileName(fileNameModel, name);

            CloseWindow();

            return View(model);
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
                String buffer = "";
                using (Stream stream = file.OpenReadStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        while ((buffer = sr.ReadLine()) != null)
                        {
                            Console.WriteLine(buffer);
                        }
                    }
                }
                DataService.SaveConspect(
                    new ConspectModel(
                        "Remove this later",
                        System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                        buffer
                        )
                    );
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
            fileNameList = DataService.LoadFileNames();
            foreach (FileNameModel fileName in fileNameList.fileNameList) 
            {
                model = DataService.LoadConspects(fileName.Name);
                conspectListModel.conspects.Add(model);
                Console.WriteLine(fileName.Name + " " + model.ConspectText);
            }
            return View(conspectListModel);

        }

    }
}