using Newtonsoft.Json;
using NOTEA.Models.UserModels;
using NOTEA.Exceptions;

namespace NOTEA.Services.UserServices
{
    public class UserService : IUserRepository
    {
        private UserListModel LoadUsers()
        {
            string text = File.ReadAllText("Users//Users.txt");
            UserListModel userList = JsonConvert.DeserializeObject<UserListModel>(text);
            return userList;
        }
        public async Task SaveUserAsync(UserModel user)
        {
            UserListModel userList = LoadUsers();
            foreach (var userModel in userList.userList)
            {
                if (user.Username == userModel.Username)
                {
                    throw new UsernameTakenException();
                }
            }
            userList.userList.Add(user);
            using (StreamWriter writer = new StreamWriter("Users//Users.txt"))
            try
            {
                string serializedJSON = JsonConvert.SerializeObject(userList);
                writer.Write(serializedJSON);
            }
            catch (Exception exp)
            {
                 Console.WriteLine("Error: could not save user ");
            }
        }
        bool IUserRepository.CheckLogIn(UserModel user)
        {
            UserListModel userList = LoadUsers();
            foreach (var userModel in userList.userList)
            {
                if (userModel.Username.Equals(user.Username) && userModel.Password.Equals(user.Password))
                {
                    return true;
                }
            }
            return false;
        }
        public int GetUserId(string username)
        {
            return -1;
        }
    }
}
