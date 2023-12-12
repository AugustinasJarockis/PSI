using Microsoft.AspNetCore.Mvc;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Models.OnlineUserListModels;
using NoteaAPI.Extentions;
using NoteaAPI.Exceptions;
using Castle.DynamicProxy;
using NOTEA.Controllers;
using NoteaAPI.Interceptor;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository<UserModel> _userRepository;
        private readonly IOnlineUserList _onlineUserList;

        public UserController(IUserRepository<UserModel> userRepository, IOnlineUserList onlineUserList)
        {
            _userRepository = userRepository;
            _onlineUserList = onlineUserList;
        }
        [HttpPost]
        [Route("signin")]
        public virtual async Task<IActionResult> SignInAsync([FromBody] SignInUserModel user)
        {
            if (user.Username.IsValidName() && user.Email.IsValidEmail())
            {
                var generator = new ProxyGenerator();
                var interceptor = new PasswordValidationInterceptor();

                var service = generator.CreateClassProxy<Validator>(interceptor);

                try
                {
                    service.ValidatePassword(user.Password);
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
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
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
                    return Ok(user);
                }
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet]
        [Route("getuser/{id}")]
        public ActionResult<UserModel> GetUser(int id)
        {
            var userModel = _userRepository.GetUser(id);
            if (userModel == null)
                return NotFound();
            return _userRepository.GetUser(id);
        }
        [HttpPost]
        [Route("updateuser/{id}")]
        public IActionResult UpdateUser (int id, [FromBody] UserModel user)
        {
            if (user.Username.IsValidName() && user.Email.IsValidEmail())
            {
                try
                {
                    _userRepository.UpdateUser(id, user.Username, user.Email);
                    return Ok(_userRepository.GetUser(id));
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
        [HttpGet]
        [Route("logout/{id}")]
        public IActionResult LogOut(int id)
        {
            UserModel temp = null;
            bool removeSuccess = _onlineUserList.OnlineUsers.TryRemove(id, out temp);
            if (!removeSuccess)
            {
                throw new Exception();
            }
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
