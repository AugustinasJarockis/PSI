using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.LogServices;
using Newtonsoft.Json;
using NOTEA.Utilities.ListManipulation;
using NOTEA.Repositories.GenericRepositories;
using NOTEA.Repositories.UserRepositories;
using NOTEA.Models.UserModels;
using System.Text;
using NuGet.Protocol.Core.Types;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        //IGenericRepository<ConspectModel> _repository;
        //private readonly IUserRepository<UserModel> _userRepository;
        private readonly ILogsService _logsService;
        public readonly IHttpContextAccessor _contextAccessor;
        public ConspectController(IHttpContextAccessor contextAccessor, /*IGenericRepository<ConspectModel> repository,*/ ILogsService logsService/*, IUserRepository<UserModel> userRepository*/)
        {
            //_repository = repository;
            _logsService = logsService;
            _contextAccessor = contextAccessor;
            //_userRepository = userRepository;
        }
        public IActionResult CreateConspects()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateConspects(ConspectModel conspectModel)
        {
            try
            {
                if (conspectModel.Name.IsValidName())
                {
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToUser(conspectModel.Id, _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default);
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    return RedirectToAction("ViewConspect", "Conspect", new { id = conspectModel.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty, longer than 80 symbols or contain the following characters: \\\\ / : * . ? \" < > | ";
                    throw new ArgumentNullException("file name", "File name is not valid");
                }
            }
            catch (ArgumentNullException ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
            }
            catch (Exception ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
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
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));

            }
            catch (InvalidOperationException ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
            }
            catch (Exception ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
            }
            return View();
        }

        [HttpGet]
        public IActionResult ConspectList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var requestContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("conspect/list/{id}", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    _contextAccessor.HttpContext.Session.SetString("User", userModel.Username);
                    _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(new ListManipulator()));
                    _contextAccessor.HttpContext.Session.SetInt32("Id", userModel.Id);
                    _contextAccessor.HttpContext.Session.SetInt32("CurrentFolderID", 0);
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(errorMessage) && errorMessage.Contains("already online"))
                    {
                        TempData["ErrorMessage"] = "You are already online on another device";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Your username or password is wrong";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return View();
            }

            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
            ConspectListModel<ConspectModel> conspectListModel =
                _repository.LoadConspects(
                    _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default,
                    listManip.GetSelection()
                    );
            if (conspectListModel?.Conspects.Count() == 0)
            {
                if (listManip.FilterExists)
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
        public async Task<IActionResult> ViewConspect(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/view/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var conspectModel = JsonConvert.DeserializeObject<ConspectModel>(responseContent);
                    return View(conspectModel);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "You do not have access to this conspect";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return RedirectToAction("ConspectList", "Conspect");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ViewConspect(ConspectModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync("api/Conspect/view", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Conspect saved successfully";            
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "You do not have access to this conspect";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return View(model);
            };
        }
        [HttpGet]
        public IActionResult EditConspect(int id)
        {
            return View(_repository.LoadConspect(id));
        }
        public IActionResult DeleteConspect(int id)
        {
            _repository.DeleteConspect(id, _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default);
            return RedirectToAction("ConspectList", "Conspect");
        }

        [HttpPost]
        public async Task<IActionResult> ShareConspect(ConspectModel model, string username)
        {
            using (var client = new HttpClient())
            {
                string currentUsername = _contextAccessor.HttpContext.Session.GetString("User") ?? default;
                client.BaseAddress = new Uri("http://localhost:5063/");
                var requestContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"api/Conspect/share/{currentUsername}/{username}", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Your notea has been shared successfully!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        if(errorMessage.Contains("already shared"))
                        {
                            TempData["ErrorMessage"] = "Conspect is already shared with this user";
                        }
                        else if(errorMessage.Contains("yourself"))
                        {
                            TempData["ErrorMessage"] = "You can not share conspect with yourself";
                        }
                        else if(errorMessage.Contains("does not exist"))
                        {
                            TempData["ErrorMessage"] = "The username you entered does not exist";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error occurred while processing your request";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Your do not have permission to execute this action";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return RedirectToAction("ViewConspect", "Conspect", new { ID = model.Id, Name = model.Name, Date = model.Date }); 
            }
        }
    }
}
