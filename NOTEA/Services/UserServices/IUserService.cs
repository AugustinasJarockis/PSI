using NOTEA.Models.UserModels;

namespace NOTEA.Services.UserServices
{
    public interface IUserService
    {
        public Task SaveUserAsync(UserModel user);
        public bool CheckLogIn(UserModel user);
    }
}
