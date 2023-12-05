using NOTEA.Database;
using NOTEA.Models.ExceptionModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NOTEA.Repositories.UserRepositories
{
    public class UserRepository<UserType> : IUserRepository<UserType> where UserType : class, IUserModel
    {
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        private DbSet<UserType> _userTypes;
        public UserRepository(ILogsService logsService, DatabaseContext database)
        {
            _logsService = logsService;
            _database = database;
            _userTypes = _database.Set<UserType>();
        }
        public bool CheckLogIn(UserType user)
        {
            return _userTypes.Where(u =>
                u.Username.Equals(user.Username) && u.Password.Equals(user.Password)
                ).ToList().Count() == 1;
        }
        public async Task SaveUserAsync(UserType user)
        {
            try
            {
                _userTypes.Add(user);
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
        public UserType GetUser (int user_id)
        { 
            return _userTypes.Where(u => u.Id == user_id).First();
        }
        public void UpdateUser(int user_id, String username, String email)
        { 
            try
            {
                var user = _userTypes.Where(u => u.Id == user_id).FirstOrDefault();
                user.Email = email;
                user.Username = username;
                _database.SaveChanges();
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
            return _userTypes.Where(u => u.Username.Equals(username)).First().Id;
        }
    }
}