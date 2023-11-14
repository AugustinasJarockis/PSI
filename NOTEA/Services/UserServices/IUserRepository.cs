using NOTEA.Models.UserModels;

namespace NOTEA.Services.UserServices
{
    public interface IUserRepository
    {
        public Task SaveUserAsync(UserModel user);
        public bool CheckLogIn(UserModel user);
        public int GetUserId(string username);
    }
}
