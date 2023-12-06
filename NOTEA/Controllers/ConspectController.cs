using Microsoft.AspNetCore.Mvc;
using NOTEA.Models.ConspectModels;
using NOTEA.Services.LogServices;
using Newtonsoft.Json;
using NOTEA.Utilities.ListManipulation;
using System.Text;
using NOTEA.Models.ViewModels;
using NOTEA.Models.FileTree;
using NuGet.Protocol.Core.Types;

namespace NOTEA.Controllers
{
    public class ConspectController : Controller
    {
        private readonly ILogsService _logsService;
        public readonly IHttpContextAccessor _contextAccessor;
        public ConspectController(IHttpContextAccessor contextAccessor, ILogsService logsService)
        {
            _logsService = logsService;
            _contextAccessor = contextAccessor;
        }
        public IActionResult CreateConspects()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateConspects(ConspectModel conspectModel)
        {
            using (var client = new HttpClient())
            {
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                int current_folder_id = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync($"api/Conspect/create/{current_folder_id}/{id}", conspectModel);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    return RedirectToAction("ViewConspect", "Conspect", new { id = responseContent });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty, longer than 80 symbols or contain the following characters: \\\\ / : * . ? \" < > | ";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return View();
            }
        }
        public IActionResult UploadConspect()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadConspect(IFormFile file)
        {
            using (var client = new HttpClient())
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

                    int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                    int folder_id = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;

                    client.BaseAddress = new Uri("http://localhost:5063/");
                    var response = await client.PostAsJsonAsync($"api/Conspect/upload/{folder_id}/{id}", conspectModel);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                }
                else
                {
                    throw new InvalidOperationException("Wrong type of file specified");
                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConspectList()
        {
            using (var client = new HttpClient())
            {
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                int current_folder_id = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
                client.BaseAddress = new Uri("http://localhost:5063/");
                var requestContent = new StringContent(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"api/Conspect/list/{current_folder_id}/{id}", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var noteaAndFolders = JsonConvert.DeserializeObject<NoteaAndFolderListModel>(responseContent);
                    ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
                    ViewData["SortStatus"] = listManip.SortStatus;
                    if (listManip.FilterExists)
                        ViewData["SearchValue"] = listManip.SearchValue;
                    ViewData["SearchBy"] = listManip.SearchBy;
                    return View(noteaAndFolders);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        TempData["ErrorMessage"] = errorMessage;
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
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> CancelSearch()
        {
            using (var client = new HttpClient())
            {
                ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync("api/Conspect/cancel", listManip);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _contextAccessor.HttpContext.Session.SetString("ListManipulator", responseContent);
                    return RedirectToAction(nameof(ConspectList));
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }
                return RedirectToAction(nameof(ConspectList));
            }
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
                var responseContent = await response.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<ConspectModel>(responseContent));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ViewConspect(ConspectModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync($"api/Conspect/save", model);
                if (response.IsSuccessStatusCode)
                {
                    return View(model);
                }
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditConspect(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/view/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<ConspectModel>(responseContent));
            }
        }
        public async Task<IActionResult> DeleteConspect(int id)
        {
            using (var client = new HttpClient())
            {
                int uid = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/delete/{uid}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ConspectList", "Conspect");
                }
                return RedirectToAction("Error", "Home");
            }
        }
        //         [HttpGet]
        //         public async Task<IActionResult> ViewConspect(int id)
        //         {
        //             using (var client = new HttpClient())
        //             {
        //                 client.BaseAddress = new Uri("http://localhost:5063/");
        //                 var response = await client.GetAsync($"api/Conspect/view/{id}");

        //                 if (response.IsSuccessStatusCode)
        //                 {
        //                     var responseContent = await response.Content.ReadAsStringAsync();
        //                     var conspectModel = JsonConvert.DeserializeObject<ConspectModel>(responseContent);
        //                     return View(conspectModel);
        //                 }
        //                 else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //                 {
        //                     TempData["ErrorMessage"] = "You do not have access to this conspect";
        //                 }
        //                 else
        //                 {
        //                     TempData["ErrorMessage"] = "An error occurred while processing your request";
        //                 }
        //                 return RedirectToAction("ConspectList", "Conspect");
        //             }
        //         }
        //         [HttpPost]
        //         public async Task<IActionResult> ViewConspect(ConspectModel model)
        //         {
        //             using (var client = new HttpClient())
        //             {
        //                 client.BaseAddress = new Uri("http://localhost:5063/");
        //                 var response = await client.PostAsJsonAsync("api/Conspect/view", model);
        //                 if (response.IsSuccessStatusCode)
        //                 {
        //                     TempData["SuccessMessage"] = "Conspect saved successfully";            
        //                 }
        //                 else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //                 {
        //                     TempData["ErrorMessage"] = "You do not have access to this conspect";
        //                 }
        //                 else
        //                 {
        //                     TempData["ErrorMessage"] = "An error occurred while processing your request";
        //                 }
        //                 return View(model);
        //             };
        //         }
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
                        if (errorMessage.Contains("already shared"))
                        {
                            TempData["ErrorMessage"] = "Conspect is already shared with this user";
                        }
                        else if (errorMessage.Contains("yourself"))
                        {
                            TempData["ErrorMessage"] = "You can not share conspect with yourself";
                        }
                        else if (errorMessage.Contains("does not exist"))
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
        [HttpGet]
        public async Task<IActionResult> AddFolder(string foldername)
        {
            using (var client = new HttpClient())
            {
                int currentFolder = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
                int currentUser = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/folder/add/{currentFolder}/{currentUser}/{foldername}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ConspectList", "Conspect");
                }
                return RedirectToAction("Error", "Home");
            }
        }
        public IActionResult OpenFolder(int id)
        {
            _contextAccessor.HttpContext.Session.SetInt32("CurrentFolderID", id);
            return RedirectToAction(nameof(ConspectList));
        }
        public async Task<IActionResult> DeleteFolder(int id)
        {
            using (var client = new HttpClient())
            {
                int userId = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;

                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/folder/delete/{userId}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ConspectList", "Conspect");
                }
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> GoBack()
        {
            using (var client = new HttpClient())
            {
                int currentFolder = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
                int currentUser = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;

                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync($"api/Conspect/folder/back/{currentUser}/{currentFolder}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _contextAccessor.HttpContext.Session.SetInt32("CurrentFolderID", Int32.Parse(responseContent));
                    return RedirectToAction("ConspectList", "Conspect");
                }
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
