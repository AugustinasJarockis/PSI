using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using NOTEA.Exceptions;
using NOTEA.Extentions;
using NOTEA.Models.UserModels;
using NOTEA.Services.UserServices;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        public readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepository _userRepository;
        public UserController(IHttpContextAccessor contextAccessor, IUserRepository userRepository)
        {
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignInAsync(string username, string password, string passwordCheck, string email) //Make return UserModel
        {
            if (username.IsValidName() && password.IsValidName() && passwordCheck.IsValidName() && email.IsValidEmail())
            {
                try
                {
                    if (password == passwordCheck)
                    {
                        UserModel user = new UserModel(username, password, email);//_mapper.Map<UserModel>(SignInModel)
                        await _userRepository.SaveUserAsync(user);
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
            UserModel user = new UserModel(username, password);
            if (username.IsValidName() && password.IsValidName() && _userService.CheckLogIn(user))
            {
                _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                user.Id = _userRepository.GetUserId(username);
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

