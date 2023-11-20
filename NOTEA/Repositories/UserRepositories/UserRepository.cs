using NOTEA.Database;
using NOTEA.Models.ExceptionModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NOTEA.Repositories.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        public UserRepository(ILogsService logsService, DatabaseContext database)
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
        public async Task SaveUserAsync(UserModel user)
        {
            try
            {
                _database.Users.Add(user);
                await _database.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.GetType() == typeof(SqlException) && ((SqlException)ex.InnerException).Number == 2601)
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
        public int GetUserId(string username)
        {
            return _database.Users.Where(u => u.Username.Equals(username)).First().Id;
        }
    }
}