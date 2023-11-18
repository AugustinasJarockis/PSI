using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.FileServices;
using NOTEA.Services.LogServices;
using NOTEA.Database;
using Newtonsoft.Json;
using NOTEA.Utilities.ListManipulation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using NOTEA.Services.UserServices;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        IGenericRepository<ConspectModel> _repository;
        private readonly IUserRepository _userRepository;
        private readonly DatabaseContext _context;
        private readonly ILogsService _logsService;
        public readonly IHttpContextAccessor _contextAccessor;
        public ConspectController(IHttpContextAccessor contextAccessor, IGenericRepository<ConspectModel> repository, ILogsService logsService, DatabaseContext context, IUserRepository userRepository)
        {
            _repository = repository;
            _context = context; 
            _logsService = logsService;
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
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
        public IActionResult ConspectList()
        {
            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
            ConspectListModel<ConspectModel> conspectListModel = 
                _repository.LoadConspects(
                    _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default,
                    listManip.GetSelection()
                    );
            if(conspectListModel?.Conspects.Count() == 0)
            {
                if(listManip.FilterExists)
                    TempData["ErrorMessage"] = "No noteas match your search";
                else
                    TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
            }
            ViewData["SortStatus"] = listManip.SortStatus;
            if (listManip.FilterExists)
                ViewData["SearchValue"] = listManip.SearchValue;
            ViewData["SearchBy"] = listManip.SearchBy;
            return View(conspectListModel);
        }
        public IActionResult CancelSearch()
        {
            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
            listManip.ClearFilter();
            _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(listManip));
            return RedirectToAction(nameof(ConspectList));
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
                ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
                listManip.UpdateFilter(searchBy, searchValue);
                _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(listManip));
            }
            return RedirectToAction(nameof(ConspectList));
        }
        [HttpGet]
        public IActionResult SortConspect(SortCollumn collumn)
        {
            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
            listManip.UpdateSort(collumn);
            _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(listManip));
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

        [HttpPost]
        public IActionResult ShareConspect(ConspectModel model, string username)
        {

            if (_context.Users.Any(x => x.Username.Equals(username)))
            {
                if (username != _contextAccessor.HttpContext.Session.GetString("User"))
                {
                    int user_id = _userRepository.GetUserId(username);
                    _repository.AssignToUser(model.Id, user_id, 'e');
                    TempData["SuccessMessage"] = "Your notea has been shared successfully!";
                }
                else 
                {
                    TempData["ErrorMessage"] = "You can not share conspect with yourself";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "The username you entered does not exist";
            }
            return RedirectToAction("ViewConspect", "Conspect", new { ID = model.Id, Name = model.Name, Date = model.Date });
        }
    }
}
