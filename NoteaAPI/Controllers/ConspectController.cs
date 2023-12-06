using Microsoft.AspNetCore.Http;
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
using Azure;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConspectController : ControllerBase
    {
        IGenericRepository<ConspectModel> _repository;
        private readonly IUserRepository<UserModel> _userRepository;
        private readonly ILogsService _logsService;
        public ConspectController(IGenericRepository<ConspectModel> repository, ILogsService logsService, IUserRepository<UserModel> userRepository)
        {
            _repository = repository;
            _logsService = logsService;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("create/{id}")]
        public IActionResult CreateConspects(int id, [FromForm] ConspectModel conspectModel)
        {
            try
            {
                if (conspectModel.Name.IsValidName())
                {
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToUser(conspectModel.Id, id);
                    return Ok();
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
        [HttpGet]
        [Route("delete/{uid}/{id}")]
        public IActionResult SaveConspect(int uid, int id)
        {
            _repository.DeleteConspect(id, uid);
            return Ok();
        }
    }
}
