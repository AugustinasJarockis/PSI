using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.FileServices;
using NOTEA.Services.LogServices;
using NOTEA.Database;
using NOTEA.Models.Utilities;
using Microsoft.IdentityModel.Tokens;
using NOTEA.Services.ListManipulation;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        IGenericRepository<ConspectModel> _repository;
        //private readonly IGenericRepository _fileService;
        private readonly DatabaseContext _context;
        private readonly ILogsService _logsService;
        private readonly IListManipulationService _listManipulationService;
        public readonly IHttpContextAccessor _contextAccessor;
        public ConspectController(IHttpContextAccessor contextAccessor, IGenericRepository<ConspectModel> repository, ILogsService logsService, DatabaseContext context, IListManipulationService listManipulationService)
        {
            _repository = repository;
            _context = context; 
            _logsService = logsService;
            _contextAccessor = contextAccessor;
            _listManipulationService = listManipulationService;
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
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToUser(conspectModel.Id, _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default);
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
                    var conspectModel = new ConspectModel(name: Path.GetFileNameWithoutExtension(file.FileName), conspectText: text);
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToUser(conspectModel.Id, _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default);
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Wrong type of file specified.";
                    throw new InvalidOperationException("Wrong type of file specified");
                }
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
        public IActionResult ConspectList(/*string searchBy, string searchValue*/)
        {
            //    if (ListManipulationUtilities.selectionExists == false)
            //    {
            //        ListManipulationUtilities.searchBy = searchBy;
            //        ListManipulationUtilities.searchValue = searchValue;
            //    }
            //ConspectListModel<ConspectModel> conspectListModel = null;
            
            ConspectListModel<ConspectModel> conspectListModel = _repository.LoadConspects(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, _listManipulationService.GetSelection());
            if(conspectListModel?.Conspects.Count() == 0)
            {
                if(_listManipulationService.FilterExists)
                    TempData["ErrorMessage"] = "No noteas match your search";
                else
                    TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
            }
            //if (searchValue.IsNullOrEmpty())
            //{
            //    if(ListManipulationUtilities.selectionExists)
            //    {
            //        conspectListModel = _repository.LoadConspects(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, ListManipulationUtilities.selection);
            //        ListManipulationUtilities.selectionExists = false;
            //    }
            //    else
            //        conspectListModel = _repository.LoadConspects(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default);
            //    //if (conspectListModel?.Conspects.Count() == 0)
            //    //{
            //    //    TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
            //    //}
            //}
            //else 
            //{
                //if (searchValue.Length > 80)
                //{
                //    TempData["ErrorMessage"] = "Search query can't be longer than 80 characters";
                //}
                //else
                //{
                //    if (searchBy.ToLower() == "name")
                //    {
                //        conspectListModel = _repository.LoadConspects(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, list => list.Where(c => c.Name.ToLower().Contains(searchValue.ToLower())).ToList());
                //    }
                //    else if (searchBy.ToLower() == "conspectsemester")
                //    {
                //        conspectListModel = _repository.LoadConspects(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, list => list.Where((Func<ConspectModel, bool>)(c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower()))).ToList());
                //    }
                //}
                if (conspectListModel?.Conspects.Count() == 0)
                {
                    
                }
            //}
            return View(conspectListModel);
        }
        [HttpGet]
        public IActionResult FilterConspect(string searchBy, string searchValue)
        {
            if (searchValue.Length > 80)
            {
                TempData["ErrorMessage"] = "Search query can't be longer than 80 characters";
            }
            else
            {
                _listManipulationService.UpdateFilter(searchBy, searchValue);
            }
            return RedirectToAction(nameof(ConspectList));
        }
        [HttpGet]
        public IActionResult SortConspect(SortCollumn collumn)
        {
            _listManipulationService.UpdateSort(collumn);
            //ListManipulationUtilities.collumnOrderValues[(int)collumn]++;
            //if ((int)ListManipulationUtilities.collumnOrderValues[(int)collumn] == 3)
            //    ListManipulationUtilities.collumnOrderValues[(int)collumn] = SortPhase.None;

            //ListManipulationUtilities.collumnOrderValues[((int)collumn + 1) % 3] = SortPhase.None;
            //ListManipulationUtilities.collumnOrderValues[((int)collumn + 2) % 3] = SortPhase.None;

            //Func<ConspectModel, bool> filter = null;
            //if(ListManipulationUtilities.searchBy?.ToLower() == "name")
            //{
            //    string searchValue = ListManipulationUtilities.searchValue;
            //    filter = c => c.Name.ToLower().Contains(searchValue.ToLower());
            //}
            //else if(ListManipulationUtilities.searchBy?.ToLower() == "conspectsemester")
            //{
            //    string searchValue = ListManipulationUtilities.searchValue;
            //    filter = c => c.ConspectSemester.GetDisplayName().ToLower().Contains(searchValue.ToLower());
            //}

            //Func<IQueryable<ConspectModel>, List<ConspectModel>> selection = null;
            //switch(collumn + 3 * (int)ListManipulationUtilities.collumnOrderValues[(int)collumn])
            //{
            //    case SortCollumn.Name + 3 * (int)SortPhase.Ascending:
            //        selection = SelectionBuilder.Build<string>(filter, c => c.Name);
            //        break;
            //    case SortCollumn.Name + 3 * (int)SortPhase.Descending:
            //        selection = SelectionBuilder.Build<string>(filter : filter, order : c => c.Name, orderDescending : true);
            //        break;
            //    case SortCollumn.Semester + 3 * (int)SortPhase.Ascending:
            //        selection = SelectionBuilder.Build<int>(filter: filter, order: c => (int)c.ConspectSemester);
            //        break;
            //    case SortCollumn.Semester + 3 * (int)SortPhase.Descending:
            //        selection = SelectionBuilder.Build<int>(filter: filter, order: c => (int)c.ConspectSemester, orderDescending: true);
            //        break;
            //    case SortCollumn.Date + 3 * (int)SortPhase.Ascending:
            //        selection = SelectionBuilder.Build<DateTime>(filter: filter, order: c => c.Date);
            //        break;
            //    case SortCollumn.Date + 3 * (int)SortPhase.Descending:
            //        selection = SelectionBuilder.Build<DateTime>(filter: filter, order: c => c.Date, orderDescending: true);
            //        break;
            //}
            //if(selection == null)
            //{
            //    selection = SelectionBuilder.Build<int>(filter: filter);
            //}
            //ListManipulationUtilities.selection = selection;
            //ListManipulationUtilities.selectionExists = true;
            return RedirectToAction(nameof(ConspectList));
        }
        [HttpGet]
        public IActionResult ViewConspect(int id)
        {
            return View(_repository.LoadConspect(id));
        }
        [HttpPost]
        public IActionResult ViewConspect(ConspectModel model)
        {
            _repository.SaveConspect(model, model.Id);
            return View(model);
        }
        [HttpGet]
        public IActionResult EditConspect(int id)
        {
            return View(_repository.LoadConspect(id));
        }
        public IActionResult DeleteConspect(int id)
        {
            _repository.DeleteConspect(id);
            return RedirectToAction("ConspectList", "Conspect");
        }
    }
}
