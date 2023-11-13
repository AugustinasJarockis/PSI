using Microsoft.AspNetCore.Mvc;
using NOTEA.Exceptions;
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

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password, string passwordCheck, string email) //Make return UserModel
        {
            if (username.IsValidName() && password.IsValidName() && passwordCheck.IsValidName() && email.IsValidEmail())
            {
                try
                {
                    if (password == passwordCheck )
                    {
                        UserModel user = new UserModel(username, password, "brr");//_mapper.Map<UserModel>(SignInModel)
                        _userService.SaveUser(user);
                        TempData["SuccessMessage"] = "Your registration has been successful!";
                        return RedirectToAction("LogIn", "User");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The passwords you entered do not match!";
                    }
                }
                catch (UsernameTakenException)
                {
                    TempData["ErrorMessage"] = "This username is already taken";
                }
            }
            else if (!email.IsValidEmail())
            {
                TempData["ErrorMessage"] = "Your email is not valid. It should follow the \"user\\@example.com\" pattern";

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
            UserModel user = new UserModel(username, password, "");
            //PAVOJUS KAS CIA REIKIA EMAILO ALIO NORIU KAD NEREIKETU
            if (username.IsValidName() && password.IsValidName() && _userService.CheckLogIn(user))
            {
                _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                _contextAccessor.HttpContext.Session.SetInt32("Id", user.Id);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is wrong";
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

