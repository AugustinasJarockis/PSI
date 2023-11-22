using Microsoft.AspNetCore.Mvc;
using NOTEA.Exceptions;
using NOTEA.Extentions;
using NOTEA.Models.UserModels;
using NOTEA.Utilities.ListManipulation;
using Newtonsoft.Json;
using NOTEA.Repositories.UserRepositories;
using NOTEA.Models.OnlineUserListModels;

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
            if (user.Username.IsValidName() && user.Password.IsValidName() && user.PasswordCheck.IsValidName() && user.Email.IsValidEmail())
            {
                try
                {
                    if (user.Password == user.PasswordCheck)
                    {
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
            else if (!user.Email.IsValidEmail())
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
        public IActionResult LogIn(UserModel user)
        {
            if (user.Username.IsValidName() && user.Password.IsValidName() && _userRepository.CheckLogIn(user))
            {
                user.Id = _userRepository.GetUserId(user.Username);
                bool addSuccess = _onlineUserList.OnlineUsers.TryAdd(user.Id, user);
                if (!addSuccess)
                {
                    TempData["ErrorMessage"] = "You are already online on another device";
                    return View();
                }
                else
                {
                    _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                    _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(new ListManipulator()));
                    _contextAccessor.HttpContext.Session.SetInt32("Id", user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is wrong";
            }
            return View();
        }
        public IActionResult LogOut()
        {
            UserModel temp = null;
            bool removeSuccess = _onlineUserList.OnlineUsers.TryRemove((int)_contextAccessor.HttpContext.Session.GetInt32("Id"), out temp);
            if (!removeSuccess)
            {
                throw new Exception();
            }
            _contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "User");
        }

        public IActionResult UserList()
        {
            return View(_onlineUserList);
        }
    }
}

