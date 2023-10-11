using Microsoft.AspNetCore.Mvc;
using NOTEA.Models;
using NOTEA.Services;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        private static ConspectListModel<ConspectModel> conspectListModel = null;
        private static FileHandlerModel filemodel = new FileHandlerModel();
        private readonly IFileService _fileService;
        private readonly ILogsService _logsService;
        public ConspectController(IFileService fileService, ILogsService logsService)
        {
            _fileService = fileService;
            _logsService = logsService;
        }

        public IActionResult CreateConspects()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateConspects(string name, ConspectSemester conspectSemester, string conspectText)
        {
            try
            {
                if (name.IsValidFilename())
                {
                    ConspectModel conspectModel = new ConspectModel(name: name, conspectSemester: conspectSemester, conspectText: conspectText);
                    _fileService.SaveConspect(conspectModel);
                    conspectListModel = null;
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty or contain any of the following characters: \\\\ / : * . ? \" < > | ";
                    throw new ArgumentNullException("file name", "File name is null");
                }
      
            }
            catch (ArgumentNullException ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
                

            }
            return View();
        }
        public IActionResult UploadConspect()
        {
            return View(filemodel);
        }

        [HttpPost]
        public IActionResult UploadConspect(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    throw new ArgumentNullException("file", "File is null");
                }

                if (file.ContentType == "text/plain")
                {
                    string text = "";
                    using (Stream stream = file.OpenReadStream())
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        text = sr.ReadToEnd();
                    }

                    _fileService.SaveConspect(
                        new ConspectModel(name: Path.GetFileNameWithoutExtension(file.FileName),
                                          conspectText: text, ConspectSemester.Unknown)
                    );
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong type of file specified.";
                    throw new InvalidOperationException("Wrong type of file specified");
                }

                conspectListModel = null;
            }
            catch (ArgumentNullException ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
                
            }
            catch (InvalidOperationException ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }

            return View(filemodel);
        }
        [HttpGet]
        public IActionResult ConspectList(string searchBy, string searchValue)
        {
            if (conspectListModel == null)
            {
                conspectListModel = _fileService.LoadConspects<ConspectModel>("Conspects");
            }
            if (string.IsNullOrEmpty(searchValue))
            {
                //TempData["ErrorMessage"] = "Please provide search value.";
                return View(conspectListModel);
            }
            else
            {
                if (searchBy.ToLower() == "name")
                {

                    var searchByName = conspectListModel.conspects.Where(c => c.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                    ConspectListModel<ConspectModel> tempConspectListModel = new ConspectListModel<ConspectModel>(searchByName);
                    return View(tempConspectListModel);
                }
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
            ConspectModel conspectModel = new ConspectModel(name: name, conspectSemester: conspectSemester, conspectText: text);
            return View(conspectModel);
        }
    }
}
