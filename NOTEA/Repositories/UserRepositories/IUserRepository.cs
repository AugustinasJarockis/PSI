using NOTEA.Models.UserModels;

namespace NOTEA.Repositories.UserRepositories
{
    public interface IUserRepository<UserType> where UserType : class, IUserModel
    {
        //public Task SaveUserAsync(UserType user);
        //public bool CheckLogIn(UserType user);
        //public int GetUserId(string username);
        //public void UpdateUser(int user_id, String username, String email);
        //public UserType GetUser(int user_id);
    }
}
