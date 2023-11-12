using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.FileServices;
using NOTEA.Services.LogServices;
using NOTEA.Database;
using NOTEA.Models.Utilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System.Diagnostics.Metrics;

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
        [HttpGet]
        public IActionResult ConspectList(string searchBy, string searchValue)
        {
            //if (conspectListModel == null)
            //{
            //ConspectListModel<ConspectModel> conspectListModel = _fileService.LoadConspects();  
            //tempConspectListModel = conspectListModel;
            ConspectListModel<ConspectModel> conspectListModel = null;
            if (searchValue.IsNullOrEmpty())
            {
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
                        return View(_fileService.LoadConspects(filter : c => c.Name.ToLower().Contains(searchValue.ToLower())));
                    }
                    else if (searchBy.ToLower() == "conspectsemester")
                    {
                        //var searchBySemester = conspectListModel.Conspects.Where(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())).ToList();
                        //tempConspectListModel = new ConspectListModel<ConspectModel>(searchBySemester);
                        return View(_fileService.LoadConspects(filter: c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower())));
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
        public IActionResult ConspectList(ConspectListModel<ConspectModel> sortedConspect)
        {
            return View(sortedConspect);
        }
        [HttpGet]
        public IActionResult SortConspect(SortCollumn collumn)
        {
            ListManipulationUtilities.collumnOrderValues[(int)collumn]++;
            if ((int)ListManipulationUtilities.collumnOrderValues[(int)collumn] == 3)
                ListManipulationUtilities.collumnOrderValues[(int)collumn] = SortPhase.None;

            Func<ConspectModel, bool> filter = null;
            if(ListManipulationUtilities.searchBy.ToLower() == "name")
            {
                filter = c => c.Name.ToLower().Contains(ListManipulationUtilities.searchValue.ToLower());
            }
            else if(ListManipulationUtilities.searchBy.ToLower() == "conspectsemester")
            {
                filter = c => c.ConspectSemester.GetDisplayName().ToLower().Contains(ListManipulationUtilities.searchValue.ToLower());
            }

            Func<ConspectModel, string> order = null;
            IComparer<string> comparer = null;
            switch(collumn + 3 * (int)ListManipulationUtilities.collumnOrderValues[(int)collumn])
            {
                case SortCollumn.Name + 3 * (int)SortPhase.Ascending:
                    order = c => c.Name;
                    break;
                case SortCollumn.Name + 3 * (int)SortPhase.Descending:
                    order = c => c.Name;
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Ascending:
                    order = c => ((int)c.ConspectSemester).ToString();
                    break;
                case SortCollumn.Semester + 3 * (int)SortPhase.Descending:
                    order = c => (-(int)c.ConspectSemester).ToString();
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Ascending:
                    order = c => c.Date.ToFileTime().ToString();
                    break;
                case SortCollumn.Date + 3 * (int)SortPhase.Descending:
                    order = c => (-c.Date.ToFileTime()).ToString();
                    break;
            }
            if (ListManipulationUtilities.collumnOrderValues[(int)collumn] = SortPhase.None)
            {
                filter = c => c.Name.ToLower().Contains(ListManipulationUtilities.searchValue.ToLower());
            }
            else if (ListManipulationUtilities.searchBy.ToLower() == "conspectsemester")
            {
                filter = c => c.ConspectSemester.GetDisplayName().ToLower().Contains(ListManipulationUtilities.searchValue.ToLower());
            }
            ConspectListModel<ConspectModel>  conspectList = _fileService.LoadConspects(filter: filter, order: order, comparer: );
            return RedirectToAction(nameof(ConspectList), conspectList);
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
