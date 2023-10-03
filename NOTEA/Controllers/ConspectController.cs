using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Xml.Linq;

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
            //TODO: Use stream to save a file, not just text
            if (file.ContentType == "text/plain")
            {
                String text = "";
                String lineText = "";
                using (Stream stream = file.OpenReadStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        while ((lineText = sr.ReadLine()) != null)
                        {
                            text += lineText;
                            //Console.WriteLine(text);
                        }
                    }
                }
                FileService.SaveConspect(
                    new ConspectModel(Path.GetFileNameWithoutExtension(file.FileName), text)
                    );
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
        [HttpGet]
        public IActionResult ViewConspect(string name, string text)
        {
            ConspectModel conspectModel = new ConspectModel(name, text);
            return View(conspectModel);
        }
    }
}
