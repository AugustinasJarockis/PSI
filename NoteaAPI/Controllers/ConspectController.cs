using Microsoft.AspNetCore.Mvc;
using NoteaAPI.Extentions;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.ExceptionModels;
using NoteaAPI.Services.LogServices;
using Newtonsoft.Json;
using NoteaAPI.Utilities.ListManipulation;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Database;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConspectController : ControllerBase
    {
        IGenericRepository<ConspectModel> _repository;
        private readonly IUserRepository<UserModel> _userRepository;
        private readonly ILogsService _logsService;
        private readonly IDatabaseContext _database;

        public ConspectController(IGenericRepository<ConspectModel> repository, ILogsService logsService, IUserRepository<UserModel> userRepository, IDatabaseContext database)
        {
            _repository = repository;
            _logsService = logsService;
            _userRepository = userRepository;
            _database = database;
        }
        
        [HttpPost]
        [Route("create/{id}")]
        public IActionResult CreateConspects(int id, [FromBody] ConspectModel conspectModel)
        {
            try
            {
                if (conspectModel.Name.IsValidName())
                {
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToUser(conspectModel.Id, id);
                    return Ok(conspectModel.Id);
                }
                else
                {
                    return Conflict();
                }
            }
            catch (ArgumentNullException ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logsService.SaveExceptionInfo(new ExceptionModel(ex));
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("upload/{id}")]
        public IActionResult UploadConspect(int id, [FromBody] ConspectModel conspectModel)
        {

            _repository.SaveConspect(conspectModel, conspectModel.Id);
            _repository.AssignToUser(conspectModel.Id, id);
            return Ok();
        }
        [HttpPost]
        [Route("list/{id}")]
        public IActionResult ConspectList(int id, [FromBody] ListManipulator listManip)
        {
            ConspectListModel<ConspectModel> conspectListModel = _repository.LoadConspects(id, listManip.GetSelection());

            if (conspectListModel?.Conspects.Count() == 0)
            {
                if (listManip.FilterExists)
                    return BadRequest("No noteas match your search");
                else
                    return BadRequest("There are 0 noteas. Write one!");
            }
            return Ok(JsonConvert.SerializeObject(conspectListModel));
        }
        [HttpPost]
        [Route("cancel")]
        public IActionResult CancelSearch([FromBody] ListManipulator listManip)
        {
            listManip.ClearFilter();
            return Ok(JsonConvert.SerializeObject(listManip));
        }
        [HttpGet]
        [Route("view/{id}")]
        public ActionResult<ConspectModel> GetConspect (int id)
        {
            return _repository.LoadConspect(id);
        }
        [HttpPost]
        [Route("save")]
        public IActionResult SaveConspect([FromBody] ConspectModel model)
        {
            _repository.SaveConspect(model, model.Id);
            return Ok();
        }
        [HttpPost]
        [Route("share/{current_username}/{username}")]
        public IActionResult ShareConspect(string current_username, string username, ConspectModel model)
        {
            if (_database.Users.Any(x => x.Username.Equals(username)))
            {
                if (username != current_username)
                {
                    int user_id = _userRepository.GetUserId(username);
                    if (!_database.UserConspects.Any(x => x.User_Id.Equals(user_id) && x.Conspect_Id.Equals(model.Id)))
                    {
                        _repository.AssignToUser(model.Id, user_id, 'e');
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Conspect is already shared with this user");
                    }
                }
                else
                {
                    return BadRequest("You can not share conspect with yourself");
                }
            }
            else
            {
               return BadRequest("The username you entered does not exist");
            }
        }
        [HttpGet]
        [Route("delete/{uid}/{id}")]
        public IActionResult DeleteConspect(int uid, int id)
        {
            _repository.DeleteConspect(id, uid);
            return Ok();
        }
    }
}
