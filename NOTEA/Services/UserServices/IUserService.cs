using NOTEA.Models.UserModels;

namespace NOTEA.Services.UserServices
{
    public interface IUserService
    {
        public void SaveUsers(UserListModel userList);
        public UserListModel LoadUsers();
    }
}
