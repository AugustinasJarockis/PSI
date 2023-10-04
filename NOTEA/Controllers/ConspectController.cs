﻿using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using System.Xml.Linq;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        public static ConspectListModel conspectListModel = null;
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
        public IActionResult CreateConspects(string name, ConspectType conspectType, string conspectText)
        {
            ConspectModel conspectModel = new ConspectModel(name, conspectType, conspectText);
            FileService.SaveConspect(conspectModel);
            Console.WriteLine(conspectType.ToString());

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
                    new ConspectModel(Path.GetFileNameWithoutExtension(file.FileName), ConspectType.semester1, text)
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
            if (conspectListModel == null)
            {
                conspectListModel = FileService.LoadConspects("Conspects");
            }
            return View(conspectListModel);
        }
        [HttpGet]
        public IActionResult SortConspect()
        {
            Console.WriteLine("fbvjdfvbkud");
            if(conspectListModel != null)
            {
            conspectListModel.conspects.Sort();
            }
            return RedirectToAction(nameof(ConspectList));
        }

        [HttpGet]
        public IActionResult ViewConspect(string name, ConspectType conspectType, string text)
        {
            ConspectModel conspectModel = new ConspectModel(name, conspectType, text);
            return View(conspectModel);
        }
    }
}
