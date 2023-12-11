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
using NoteaAPI.Models.FileTree;
using NoteaAPI.Services.FolderService;
using NoteaAPI.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http.Extensions;

namespace NoteaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConspectController : ControllerBase
    {
        IGenericRepository<ConspectModel> _repository;
        private readonly IUserRepository<UserModel> _userRepository;
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        private readonly IFolderService _folderService;

        public ConspectController(IGenericRepository<ConspectModel> repository, ILogsService logsService, IFolderService folderService, IUserRepository<UserModel> userRepository, DatabaseContext database)
        {
            _repository = repository;
            _logsService = logsService;
            _userRepository = userRepository;
            _folderService = folderService;
            _database = database;
        }
        
        [HttpPost]
        [Route("create/{folder_id}/{id}")]
        public IActionResult CreateConspects(int folder_id, int id, [FromBody] ConspectModel conspectModel)
        {
            try
            {
                if (conspectModel.Name != null && conspectModel.Name.Length <= 80)
                {
                    _repository.SaveConspect(conspectModel, conspectModel.Id);
                    _repository.AssignToFolder(new TreeNodeModel(NodeType.File, conspectModel.Id, id, folder_id));
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
        [Route("upload/{folder_id}/{id}")]
        public IActionResult UploadConspect(int folder_id, int id, [FromBody] ConspectModel conspectModel)
        {
            _repository.SaveConspect(conspectModel, conspectModel.Id);
            _repository.AssignToFolder(new TreeNodeModel(NodeType.File, conspectModel.Id, id, folder_id));
            _repository.AssignToUser(conspectModel.Id, id);
            return Ok();
        }
        [HttpGet]
        [Route("conspectlist/{folder_id}/{id}/{manipulator}")]
        public IActionResult GetConspectList(int folder_id, int id, string manipulator)
        {
            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(manipulator);
            return Ok(JsonConvert.SerializeObject(_repository.LoadConspects(id, listManip.GetSelection(), folder_id)));
        }
        [HttpGet]
        [Route("folderlist/{folder_id}/{id}/{manipulator}")]
        public IActionResult FolderList (int folder_id, int id, string manipulator)
        {
            ListManipulator listManip = JsonConvert.DeserializeObject<ListManipulator>(manipulator);
            return Ok(JsonConvert.SerializeObject(_folderService.GetFolderList(id, folder_id, listManip.GetFolderSelection())));
        }
        [HttpGet]
        [Route("view/{id}/{user_id}")]
        public ActionResult<ConspectModel> GetConspect (int id, int user_id)
        {
            List<int> ints = _database.UserConspects.Where(x => x.User_Id == user_id).Select(x => x.Conspect_Id).ToList();
            int a = 0;
            foreach (int conspects_id in ints)
            {
                if (id == conspects_id)
                {
                    a++;
                }
            }
            if (a == 0)
            {
                return Unauthorized();
            }
            else 
            {
                return _repository.LoadConspect(id);
            }
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
        public IActionResult DeleteConspect(int uid, int id)
        {
            List<int> ints = _database.UserConspects.Where(x => x.User_Id == uid).Select(x => x.Conspect_Id).ToList();
            int a = 0;
            foreach (int conspects_id in ints)
            {
                if (id == conspects_id)
                {
                    a++;
                }
            }
            if (a == 0)
            {
                return Unauthorized();
            }
            else
            {
                _repository.DeleteConspect(id, uid);
                return Ok();
            }
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
                        _repository.AssignToFolder(new TreeNodeModel(NodeType.File, model.Id, user_id));
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
        [Route("folder/add/{folder_id}/{user_id}/{foldername}")]
        public IActionResult AddFolder(int folder_id, int user_id, string foldername)
        {
            int id = _folderService.AddFolder(foldername);
            _repository.AssignToFolder(new TreeNodeModel(NodeType.Folder, id, user_id, folder_id));
            return Ok();
        }

        [HttpGet]
        [Route("folder/delete/{user_id}/{folder_id}")]
        public IActionResult DeleteFolder(int user_id, int folder_id)
        {
            _folderService.DeleteFolder(user_id, folder_id);
            return Ok();
        }
        [HttpGet]
        [Route("folder/back/{user_id}/{folder_id}")]
        public IActionResult GoBack(int user_id, int folder_id)
        {
            return Ok(_folderService.GetPreviousFolderID(user_id, folder_id));
        }
    }
}
