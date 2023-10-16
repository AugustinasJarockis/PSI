using NOTEA.Models;

namespace NOTEA.Services
{
    public interface IUserService
    {
        public void SaveUsers(UserListModel userList);
        public UserListModel LoadUsers();
    }
}
