using NOTEA.Models.UserModels;

namespace NOTEA.Services.UserServices
{
    public interface IUserService
    {
        public void SaveUser(UserModel user);
        public UserListModel LoadUsers();
    }
}
