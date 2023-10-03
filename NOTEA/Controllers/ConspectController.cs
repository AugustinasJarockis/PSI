using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        public ConspectListModel conspectListModel = new ConspectListModel();
        public static FileHandlerModel filemodel = new FileHandlerModel();
        private readonly IFileService FileService;
        public ConspectController(IFileService fileService)
        {
            FileService = fileService;
        }

        public IActionResult CreateConspects()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateConspects(string name, string conspectText)
        {
            ConspectModel conspectModel = new ConspectModel(name, conspectText);
            FileService.SaveConspect(conspectModel);

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
            if (file.ContentType == "text/plain")
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
                FileService.SaveConspect(
                    new ConspectModel(Path.GetFileNameWithoutExtension(file.FileName), text)
                    );
                //DataService.SaveFileName(fileNameModel, Path.GetFileNameWithoutExtension(file.FileName));
            }
            else
            {
                Console.WriteLine("Error: wrong type of file specified");
            }
            return View(filemodel);
        }

        [HttpGet]
        public IActionResult ConspectList()
        {
            conspectListModel = FileService.LoadConspects("Conspects");
            return View(conspectListModel);
        }
    }
}
