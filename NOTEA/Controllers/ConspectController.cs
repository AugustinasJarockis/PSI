using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using NOTEA.Services;
using System.Xml.Linq;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        private static ConspectListModel<ConspectModel> conspectListModel = null;
        private static FileHandlerModel filemodel = new FileHandlerModel();
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
        public IActionResult CreateConspects(string name, ConspectSemester conspectSemester, string conspectText)
        {
            ConspectModel conspectModel = new ConspectModel(name : name, conspectSemester : conspectSemester, conspectText : conspectText) ;
            FileService.SaveConspect(conspectModel);
            conspectListModel = null;
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
                using (Stream stream = file.OpenReadStream())
                {
                    using StreamReader sr = new StreamReader(stream);
                    text = sr.ReadToEnd();
                }
                FileService.SaveConspect(
                    new ConspectModel(name : Path.GetFileNameWithoutExtension(file.FileName),
                                      conspectText : text)
                    );
            }
            else
            {
                Console.WriteLine("Error: wrong type of file specified");
            }
            conspectListModel = null;
            return View(filemodel);
        }

        [HttpGet]
        public IActionResult ConspectList()
        {
            if (conspectListModel == null)
            {
                conspectListModel = FileService.LoadConspects<ConspectModel>("Conspects");
            }
            return View(conspectListModel);
        }
        [HttpGet]
        public IActionResult SortConspect()
        {
            if (conspectListModel != null)
            {
                conspectListModel.conspects.Sort();
            }
            return RedirectToAction(nameof(ConspectList));
        }

        [HttpGet]
        public IActionResult ViewConspect(string name, ConspectSemester conspectSemester, string text)
        {
            ConspectModel conspectModel = new ConspectModel(name : name, conspectSemester: conspectSemester, conspectText : text);
            return View(conspectModel);
        }
    }
}
