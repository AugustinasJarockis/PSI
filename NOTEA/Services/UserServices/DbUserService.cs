using NOTEA.Database;
using NOTEA.Models.ExceptionModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
        public bool CheckLogIn(UserModel user)
        {
            return _database.Users.Where(u =>
                u.Username.Equals(user.Username) && u.Password.Equals(user.Password)
                ).ToList().Count() == 1;
        }
        public void SaveUser(UserModel user)
        {
            try
            {
                _database.Users.Add(user);
                _database.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.GetType() == typeof(SqlException) && ((SqlException)ex.InnerException).Number == 2627)
                {
                    throw new UsernameTakenException();
                }
                else throw;
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
        }
    }
}