using Microsoft.AspNetCore.Mvc;
using NOTEA.Exceptions;
using NOTEA.Extentions;
using NOTEA.Models.UserModels;
using NOTEA.Utilities.ListManipulation;
using Newtonsoft.Json;
using NOTEA.Repositories.UserRepositories;
using NOTEA.Models.OnlineUserListModels;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        public readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepository<UserModel> _userRepository;
        private readonly IOnlineUserList _onlineUserList;
        public UserController(IHttpContextAccessor contextAccessor, IUserRepository<UserModel> userRepository, IOnlineUserList onlineUserList)
        {
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
            _onlineUserList = onlineUserList;
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
                client.BaseAddress = new Uri("http://localhost:5063/");
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
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync("api/User/login", user);

                if (response.IsSuccessStatusCode)
                {
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

        //[HttpPost]
        //public async Task<IActionResult> LogIn(UserModel user)
        //{
        //    if (user.Username.IsValidName() && user.Password.IsValidName() && _userRepository.CheckLogIn(user))
        //    {
        //        user.Id = _userRepository.GetUserId(user.Username);
        //        bool addSuccess = _onlineUserList.OnlineUsers.TryAdd(user.Id, user);
        //        if (!addSuccess)
        //        {
        //            TempData["ErrorMessage"] = "You are already online on another device";
        //            return View();
        //        }
        //        else
        //        {
        //            _contextAccessor.HttpContext.Session.SetString("User", user.Username);
        //            _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(new ListManipulator()));
        //            _contextAccessor.HttpContext.Session.SetInt32("Id", user.Id);
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Your username or password is wrong";
        //    }
        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> AccountSettings()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync("api/User/getuser");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    return View(userModel);
                }
                else
                {
                    return View("Error");
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser (String username, String email)
        {

            using (var client = new HttpClient())
            {
                UserModel user = new UserModel(username, "", email);
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.PostAsJsonAsync("api/User/getuser", user);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    return View(userModel);
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
                return RedirectToAction("User", "AccountSettings");
            }
        }
        public async Task<IActionResult> LogOut()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync("api/User/logout");

                if(response.IsSuccessStatusCode) 
                {
                    return RedirectToAction("LogIn", "User");
                }
                else 
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your request";
                    return RedirectToAction("Home", "Index");
                }
            }
        }
        //public IActionResult LogOut()
        //{
        //    UserModel temp = null;
        //    bool removeSuccess = _onlineUserList.OnlineUsers.TryRemove((int)_contextAccessor.HttpContext.Session.GetInt32("Id"), out temp);
        //    if (!removeSuccess)
        //    {
        //        throw new Exception();
        //    }
        //    _contextAccessor.HttpContext.Session.Clear();
        //    return RedirectToAction("LogIn", "User");
        //}
        public async Task<IActionResult> UserList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5063/");
                var response = await client.GetAsync("api/User/users/online");
                var responseContent = await response.Content.ReadAsStringAsync();
                var userList = JsonConvert.DeserializeObject<OnlineUserList>(responseContent);
                return View(userList);
            }
        }
        //public IActionResult UserList()
        //{
        //    return View(_onlineUserList);
        //}
    }
}

