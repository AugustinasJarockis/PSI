using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.LogServices;
using Newtonsoft.Json;
using NOTEA.Utilities.ListManipulation;
using System.Text;

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
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync($"api/Conspect/create/{id}", conspectModel);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    TempData["SuccessMessage"] = "Your notea has been saved successfully!";
                    return RedirectToAction("ViewConspect", "Conspect", new { id = responseContent });
    //======= //Movr API
                        //_repository.AssignToFolder(new TreeNodeModel(
                        //    NodeType.File,
                        //    conspectModel.Id,
                        //    _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default,
                        //    _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default));
    //>>>>>>> main
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "Your conspect name is invalid! It can't be empty, longer than 80 symbols or contain the following characters: \\\\ / : * . ? \" < > | ";
    //======= //Move to API
       
                    //Move load folder
                    //ConspectListModel<ConspectModel> conspectListModel = 
                    //    _repository.LoadConspects(
                    //        _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default,
                    //        listManip.GetSelection(),
                    //        _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default
                    //        );
                    //List<FolderModel> folders = _folderService.GetFolderList(
                    //    _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default,
                    //    _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default,
                    //    listManip.GetFolderSelection()
                    //    );
                    //if(conspectListModel?.Conspects.Count + folders.Count == 0)
                    //{
                    //    if(listManip.FilterExists)
                    //        TempData["ErrorMessage"] = "No noteas match your search";
                    //    else if(_contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") == 0)
                    //        TempData["ErrorMessage"] = "There are 0 noteas. Write one!";
                    //}
                    //return View(new CombinedNoteaAndFolderListModel(folders, conspectListModel.Conspects));
    //>>>>>>> main
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
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;

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

                    client.BaseAddress = new Uri("http://localhost:5063/");
                    var response = await client.PostAsJsonAsync($"api/Conspect/upload/{id}", conspectModel);

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
                client.BaseAddress = new Uri("http://localhost:5063/");
                var requestContent = new StringContent(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"api/Conspect/list/{id}", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var conspectListModel = JsonConvert.DeserializeObject<ConspectListModel<ConspectModel>>(responseContent);
                    ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
                    ViewData["SortStatus"] = listManip.SortStatus;
                    if (listManip.FilterExists)
                        ViewData["SearchValue"] = listManip.SearchValue;
                    ViewData["SearchBy"] = listManip.SearchBy;
                    return View(conspectListModel);
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
        //    [HttpGet]
        //    public IActionResult FilterConspect(string searchBy, string searchValue)
        //    {
        //        if (searchValue.Length > 80)
        //        {
        //            TempData["ErrorMessage"] = "Search query can't be longer than 80 characters";
        //        }
        //        else
        //        {
        //            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
        //            listManip.UpdateFilter(searchBy, searchValue);
        //            _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(listManip));
        //        }
        //        return RedirectToAction(nameof(ConspectList));
        //    }
        //    [HttpGet]
        //    public IActionResult SortConspect(SortCollumn collumn)
        //    {
        //        ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(_contextAccessor.HttpContext.Session.GetString("ListManipulator") ?? default);
        //        listManip.UpdateSort(collumn);
        //        _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(listManip));
        //        return RedirectToAction(nameof(ConspectList));
        //    }
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
    //======= //Add to API
            //[HttpGet]
            //public IActionResult AddFolder(string foldername)
            //{
            //    int currentFolder = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
            //    int currentUser = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
            //    int id = _folderService.AddFolder(foldername);
            //    _repository.AssignToFolder(new TreeNodeModel(NodeType.Folder, id, currentUser, currentFolder));
            //    return RedirectToAction(nameof(ConspectList));
            //}

            //public IActionResult OpenFolder(int id)
            //{
            //    _contextAccessor.HttpContext.Session.SetInt32("CurrentFolderID", id);
            //    return RedirectToAction(nameof(ConspectList));
            //}

            //public IActionResult DeleteFolder(int id)
            //{
            //    _folderService.DeleteFolder(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, id);
            //    return RedirectToAction(nameof(ConspectList));
            //}

            //public IActionResult GoBack()
            //{
            //    int currentFolder = _contextAccessor.HttpContext.Session.GetInt32("CurrentFolderID") ?? default;
            //    int currentUser = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
            //    _contextAccessor.HttpContext.Session.SetInt32("CurrentFolderID", _folderService.GetPreviousFolderID(currentUser, currentFolder)); 
            //    return RedirectToAction(nameof(ConspectList));
            //}
    //>>>>>>> main
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
    //======= //Move to API
                   
                       //     _repository.AssignToFolder(new TreeNodeModel(NodeType.File, model.Id, user_id));
                    
    //>>>>>>> main
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
    }
}
