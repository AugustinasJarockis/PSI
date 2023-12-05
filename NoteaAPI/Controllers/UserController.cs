using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Utilities.ListManipulation;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Models.OnlineUserListModels;
using NoteaAPI.Extentions;
using NoteaAPI.Exceptions;
using Newtonsoft.Json;
using Microsoft.Identity.Client;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
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
        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInUserModel user)
        {
            if (user.Username.IsValidName() && user.Password.IsValidName() && user.PasswordCheck.IsValidName() && user.Email.IsValidEmail())
            {
                try
                {
                    if (user.Password == user.PasswordCheck)
                    {
                        await _userRepository.SaveUserAsync(user);
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                catch (UsernameTakenException)
                {
                    return Conflict();
                }
            }
            else if (!user.Email.IsValidEmail())
            {
                return BadRequest();

            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("login")]
        public IActionResult LogIn([FromBody] UserModel user)
        {
            if (user.Username.IsValidName() && user.Password.IsValidName() && _userRepository.CheckLogIn(user))
            {
                user.Id = _userRepository.GetUserId(user.Username);
                bool addSuccess = _onlineUserList.OnlineUsers.TryAdd(user.Id, user);
                if (!addSuccess)
                {
                    //return BadRequest();
                    if (_onlineUserList.OnlineUsers.ContainsKey(user.Id))
                    {
                        return BadRequest("User is already online on another device.");
                    }
                    else
                    {
                        return BadRequest("An error occurred while adding the user to the online user list.");
                    }
                }
                else
                {
                    _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                    _contextAccessor.HttpContext.Session.SetString("ListManipulator", JsonConvert.SerializeObject(new ListManipulator()));
                    _contextAccessor.HttpContext.Session.SetInt32("Id", user.Id);
                    return Ok();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet]
        [Route("getuser")]
        public UserModel GetUser()
        {
            //return _userRepository.GetUser(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default); <- sita grazinti i veikima, kai pagaliau loginas bus sutvarkytas
            return _userRepository.GetUser(2002);//<-pakeisti priklausomai nuo userio id, kad butu galima bent patestuot
        }
        [HttpPut]
        [Route("updateuser")]
        public IActionResult UpdateUser ([FromBody] UserModel user)
        {
            if (user.Username.IsValidName() && user.Email.IsValidEmail())
            {
                try
                {
                    _userRepository.UpdateUser(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default, user.Username, user.Email);
                    _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                    return Ok(_userRepository.GetUser(_contextAccessor.HttpContext.Session.GetInt32("Id") ?? default));
                }
                catch (UsernameTakenException)
                {
                    return Conflict();
                }
            }
            else if (!user.Email.IsValidEmail())
            {
                return BadRequest("Email is not valid");

            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete]
        [Route("logout")]
        public IActionResult LogOut()
        {
            UserModel temp = null;
            bool removeSuccess = _onlineUserList.OnlineUsers.TryRemove((int)_contextAccessor.HttpContext.Session.GetInt32("Id"), out temp);
            if (!removeSuccess)
            {
                throw new Exception();
            }
            _contextAccessor.HttpContext.Session.Clear();
            return Ok();
        }
        [HttpGet]
        [Route("users/online")]
        public IOnlineUserList GetOnlineUserList()
        {
            return _onlineUserList;
        }
    }
}
