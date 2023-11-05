using Newtonsoft.Json;
using NOTEA.Database;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;

namespace NOTEA.Services.UserServices
{
    public class DbUserService : IUserService
    {
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        public DbUserService(ILogsService logsService, DatabaseContext database)
        {
            _logsService = logsService;
            _database = database;
        }
        public UserListModel LoadUsers()
        {
            var users = _database.Users.ToList();
            UserListModel userList = new UserListModel();
            userList.userList = users;
            return userList;
        }
        public void SaveUser(UserModel user)
        {
            try
            {
                _database.Users.Add(user);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
        }
    }
}