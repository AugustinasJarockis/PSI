﻿using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Helpers;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.FileServices;
using NOTEA.Services.LogServices;
using System.Text.Json.Serialization;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        private static ConspectListModel<ConspectModel> conspectListModel = null;
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
                    return RedirectToAction(nameof(CreateConspects));
                }
                else
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty, longer than 80 symbols or contain the following characters: \\\\ / : * . ? \" < > | ";
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
            return View();
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

            return View();
        }
        [HttpGet]
        public IActionResult ConspectList(string searchBy, string searchValue = "")
        {
            if (conspectListModel == null)
            {
                conspectListModel = _fileService.LoadConspects<ConspectModel>("Conspects");
                ConspectListModel<ConspectModel> tempConspectListModel = new ConspectListModel<ConspectModel>(conspectListModel.Conspects);
                if (string.IsNullOrEmpty(searchValue))
                {
                    return View(tempConspectListModel);
                }
            }
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                if (conspectListModel == null)
                {
                    TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
                }
                else if (searchValue.Length > 80)
                {
                    TempData["ErrorMessage"] = "Search query can't be longer than 80 characters";
                }
                else
                {
                    if (searchBy.ToLower() == "name")
                    {
                        var searchByName = conspectListModel.Conspects.Where(c => c.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                        ConspectListModel<ConspectModel> tempConspectListModel = new ConspectListModel<ConspectModel>(searchByName);
                        return View(tempConspectListModel);
                    }
                    else if (searchBy.ToLower() == "conspectsemester")
                    {
                        var searchBySemester = conspectListModel.Conspects.Where(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())).ToList();
                        ConspectListModel<ConspectModel> tempConspectListModel = new ConspectListModel<ConspectModel>(searchBySemester);
                        return View(tempConspectListModel);
                    }
                }
                return View(conspectListModel);
            }

             return View(conspectListModel);
        }
        [HttpGet]
        public IActionResult SortConspect()
        {
            if (conspectListModel != null)
            {
                conspectListModel.Conspects.Sort();
            }
            else
            {
                TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
            }
            return RedirectToAction(nameof(ConspectList));
        }

        [HttpGet]
        public IActionResult ViewConspect(int Index)
        {
            return View(conspectListModel.Conspects[Index]);
        }
    }
}
