using NOTEA.Models.ExceptionModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NOTEA.Repositories.UserRepositories
{//DELTE THE FILE
    public class UserRepository<UserType> : IUserRepository<UserType> where UserType : class, IUserModel
    {
        private readonly ILogsService _logsService;
        private DbSet<UserType> _userTypes;
        public UserRepository(ILogsService logsService)
        {
            _logsService = logsService;
        }
        public bool CheckLogIn(UserType user)
        {
            return _userTypes.Where(u =>
                u.Username.Equals(user.Username) && u.Password.Equals(user.Password)
                ).ToList().Count() == 1;
        }
        public int GetUserId(string username)
        {
            return _userTypes.Where(u => u.Username.Equals(username)).First().Id;
        }
    }
}