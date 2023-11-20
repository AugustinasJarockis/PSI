using NOTEA.Models.UserModels;

namespace NOTEA.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        public Task SaveUserAsync(UserModel user);
        public bool CheckLogIn(UserModel user);
        public int GetUserId(string username);
    }
}
