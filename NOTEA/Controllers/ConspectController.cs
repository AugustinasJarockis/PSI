﻿using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.FileServices;
using NOTEA.Services.LogServices;
using NOTEA.Database;
using NOTEA.Models.Utilities;
using NOTEA.Models.Utilities.Comparers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        //private static ConspectListModel<ConspectModel> conspectListModel = null;
        //private static ConspectListModel<ConspectModel> tempConspectListModel = null;
        private readonly IFileService _fileService;
        private readonly DatabaseContext _context;
        private readonly ILogsService _logsService;
        public ConspectController(IFileService fileService, ILogsService logsService, DatabaseContext context)
        {
            _fileService = fileService;
            _context = context; 
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
                if (name.IsValidName())
                {
                    ConspectModel conspectModel = new ConspectModel(name: name, conspectSemester: conspectSemester, conspectText: conspectText);
                    _fileService.SaveConspect(conspectModel);
                    //conspectListModel = null;
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    return RedirectToAction(nameof(CreateConspects));
                }
                else
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty, longer than 80 symbols or contain the following characters: \\\\ / : * . ? \" < > | ";
                    throw new ArgumentNullException("file name", "File name is not valid");
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
                    string text;
                    using (StreamReader sr = new StreamReader(file.OpenReadStream()))
                    {
                        text = sr.ReadToEnd();
                    }

                    _fileService.SaveConspect(new ConspectModel(name: Path.GetFileNameWithoutExtension(file.FileName), conspectText: text));
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong type of file specified.";
                    throw new InvalidOperationException("Wrong type of file specified");
                }
                //conspectListModel = null;
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
        //[HttpGet]
        //public IActionResult ConspectList()
        //{
        //    return View(_fileService.LoadConspects());
        //}
        [HttpGet]
        public IActionResult ConspectList(string searchBy, string searchValue/*Func<DbSet<ConspectModel>, List<ConspectModel>>? selection = null*/)
        {
            //if (conspectListModel == null)
            //{
            //ConspectListModel<ConspectModel> conspectListModel = _fileService.LoadConspects();  
            //tempConspectListModel = conspectListModel;
            if (ListManipulationUtilities.selectionExists == false)
            {
                ListManipulationUtilities.searchBy = searchBy;
                ListManipulationUtilities.searchValue = searchValue;
            }
            ConspectListModel<ConspectModel> conspectListModel = null;
            if (searchValue.IsNullOrEmpty())
            {
                if(ListManipulationUtilities.selectionExists)
                {
                    conspectListModel = _fileService.LoadConspects(ListManipulationUtilities.selection);
                    ListManipulationUtilities.selectionExists = false;
                }
                else
                    conspectListModel = _fileService.LoadConspects();
                if (conspectListModel?.Conspects.Count() == 0)
                {
                    TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
                }
            }
            //}
            //if (!string.IsNullOrWhiteSpace(searchValue))
            //{
            else 
            {
                if (searchValue.Length > 80)
                {
                    TempData["ErrorMessage"] = "Search query can't be longer than 80 characters";
                }
                else
                {
                    if (searchBy.ToLower() == "name")
                    {
                        //var searchByName = conspectListModel.Conspects.Where(c => c.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                        //tempConspectListModel = new ConspectListModel<ConspectModel>(searchByName);
                        return View(_fileService.LoadConspects(list => list.Where(c => c.Name.ToLower().Contains(searchValue.ToLower())).ToList()));
                    }
                    else if (searchBy.ToLower() == "conspectsemester")
                    {
                        //var searchBySemester = conspectListModel.Conspects.Where(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())).ToList();
                        //tempConspectListModel = new ConspectListModel<ConspectModel>(searchBySemester);
                        return View(_fileService.LoadConspects(list => list.Where(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())).ToList()));
                    }
                }
                if (conspectListModel?.Conspects.Count() == 0)
                {
                    TempData["ErrorMessage"] = "No noteas match your search";
                }
            }
           // else
            //{
                //tempConspectListModel.Conspects = conspectListModel.Conspects;
            //}
            return View(conspectListModel);
        }
        //[HttpGet]
        //public IActionResult ConspectList(ConspectListModel<ConspectModel> conspectList)
        //{
        //    return View(conspectList);
        //}
        [HttpGet]
        public IActionResult SortConspect(SortCollumn collumn)
        {
            ListManipulationUtilities.collumnOrderValues[(int)collumn]++;
            if ((int)ListManipulationUtilities.collumnOrderValues[(int)collumn] == 3)
                ListManipulationUtilities.collumnOrderValues[(int)collumn] = SortPhase.None;

            ListManipulationUtilities.collumnOrderValues[((int)collumn + 1) % 3] = SortPhase.None;
            ListManipulationUtilities.collumnOrderValues[((int)collumn + 2) % 3] = SortPhase.None;

            Func<ConspectModel, bool> filter = null;
            if(ListManipulationUtilities.searchBy?.ToLower() == "name")
            {
                string searchValue = ListManipulationUtilities.searchValue;
                filter = c => c.Name.ToLower().Contains(searchValue.ToLower());
            }
            else if(ListManipulationUtilities.searchBy?.ToLower() == "conspectsemester")
            {
                string searchValue = ListManipulationUtilities.searchValue;
                filter = c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower());
            }

            Func<DbSet<ConspectModel>, List<ConspectModel>> selection = null;
            //Func<ConspectModel, string> order = null;
            //IComparer<string> comparer = null;
            switch(collumn + 3 * (int)ListManipulationUtilities.collumnOrderValues[(int)collumn])
            {
                case SortCollumn.Name + 3 * (int)SortPhase.Ascending:
                    selection = SelectionBuilder<string>.Build(filter, c => c.Name);
                    break;
                case SortCollumn.Name + 3 * (int)SortPhase.Descending:
                    selection = SelectionBuilder<string>.Build(filter : filter, order : c => c.Name, orderDescending : true);
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Ascending:
                    selection = SelectionBuilder<int>.Build(filter: filter, order: c => (int)c.ConspectSemester);
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Descending:
                    selection = SelectionBuilder<int>.Build(filter: filter, order: c => (int)c.ConspectSemester, orderDescending: true);
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Ascending:
                    selection = SelectionBuilder<DateTime>.Build(filter: filter, order: c => c.Date);
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Descending:
                    selection = SelectionBuilder<DateTime>.Build(filter: filter, order: c => c.Date, orderDescending: true);
                    break;
            }
            if(selection == null)
            {
                selection = SelectionBuilder<DateTime>.Build(filter: filter);
            }
            ListManipulationUtilities.selection = selection;
            ListManipulationUtilities.selectionExists = true;
            //ConspectListModel<ConspectModel>  conspectList = _fileService.LoadConspects(filter: filter, order: order, comparer: comparer);
            //return RedirectToAction(nameof(ConspectList), conspectList);
            return RedirectToAction(nameof(ConspectList));
        }
        [HttpGet]
        public IActionResult ViewConspect(int id)
        {
            return View(_fileService.LoadConspect(id));
        }
        [HttpPost]
        public IActionResult ViewConspect(ConspectModel model)
        {
            _fileService.SaveConspect(model);
            //conspectListModel = null;
            return View(model);
        }
        [HttpGet]
        public IActionResult EditConspect(int id)
        {
            return View(_fileService.LoadConspect(id));
        }
    }
}
