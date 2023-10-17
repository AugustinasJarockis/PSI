using Microsoft.AspNetCore.Mvc;
using NOTEA.Extentions;
using NOTEA.Models.UserModels;
using NOTEA.Services.UserServices;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        public readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        public UserController(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
        }
        UserListModel userList = new UserListModel();

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password, string passwordCheck)
        {
            bool usernameTaken = false;
            if (username.IsValidName() && password.IsValidName() && passwordCheck.IsValidName())
            {
                userList = _userService.LoadUsers();
                foreach (var userModel in userList.userList)
                {
                    if(username == userModel.Username)
                    {
                        usernameTaken = true;
                    }
                }
                if (!usernameTaken)
                {
                    if (password == passwordCheck )
                    {
                        UserModel user = new UserModel(username, password);
                    
                        userList.userList.Add(user);

                        _userService.SaveUsers(userList);
                        TempData["SuccessMessage"] = "Your registration has been successful!";
                        return RedirectToAction("LogIn", "User");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The passwords you entered do not match!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "This username is already taken";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is invalid! It can't be empty, longer than 80 symbols or contain any of the following characters:\n \\\\ / : * . ? \" < > | ";
            }
            return View();
        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            UserModel user = new UserModel();
            if (username.IsValidName() && password.IsValidName())
            {
                userList = _userService.LoadUsers();
                foreach (var userModel in userList.userList)
                {
                    if(userModel.Username.Equals(username) && userModel.Password.Equals(password))
                    {
                        user = new UserModel(username, password);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is invalid! It can't be empty, longer than 80 symbols or contain any of the following characters:\n \\\\ / : * . ? \" < > | ";
            }
            if(user.Username == "" && user.Password == "")
            {
                TempData["ErrorMessage"] = "Your username or password is wrong";
            }
            else
            {
                _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult LogOut()
        {
            _contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "User");
        }
    }
}

