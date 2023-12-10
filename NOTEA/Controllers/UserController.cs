using Microsoft.AspNetCore.Mvc;
using NOTEA.Models.UserModels;
using NOTEA.Utilities.ListManipulation;
using Newtonsoft.Json;
using NOTEA.Models.OnlineUserListModels;
using System.Text;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignInAsync(SignInUserModel user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var response = await client.PostAsJsonAsync("api/User/signin", user);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Your registration has been successful!";
                    return RedirectToAction("LogIn", "User");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "The passwords you entered do not match!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "This username is already taken";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Invalid username or email";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                }

                return View();
            }
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogInAsync(UserModel user)
        {
            using (var client = new HttpClient())
            {
                user.Email = "";
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var requestContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/User/login", requestContent);
                
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
        }
        [HttpGet]
        public async Task<IActionResult> AccountSettings()
        {
            using (var client = new HttpClient())
            {
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var response = await client.GetAsync($"api/User/getuser/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    return View(userModel);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser (String username, String email)
        {

            using (var client = new HttpClient())
            {
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                UserModel user = new UserModel(username, "", email);
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var response = await client.PostAsJsonAsync($"api/User/updateuser/{id}", user);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    _contextAccessor.HttpContext.Session.SetString("User", userModel.Username);
                    return RedirectToAction("AccountSettings", "User", userModel);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "This username is already taken";
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(errorMessage) && errorMessage.Contains("email"))
                    {
                        TempData["ErrorMessage"] = "Your provided email is invalid";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                }
                return RedirectToAction("AccountSettings", "User");
            }
        }
        public async Task<IActionResult> LogOut()
        {
            using (var client = new HttpClient())
            {
                int id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var response = await client.GetAsync($"api/User/logout/{id}");

                if(response.IsSuccessStatusCode) 
                {
                    _contextAccessor.HttpContext.Session.Clear();
                    return RedirectToAction("LogIn", "User");
                }
                else 
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public async Task<IActionResult> UserList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _configuration.GetValue<Uri>("BaseUri");
                var response = await client.GetAsync("api/User/users/online");
                var responseContent = await response.Content.ReadAsStringAsync();
                var userList = JsonConvert.DeserializeObject<OnlineUserList>(responseContent);
                return View(userList);
            }
        }
    }
}

