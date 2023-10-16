using Newtonsoft.Json;
using NOTEA.Models;

namespace NOTEA.Services
{
    public class UserService : IUserService
    {
        public UserListModel LoadUsers()
        {
            string text = File.ReadAllText("Users//Users.txt");
            UserListModel userList = JsonConvert.DeserializeObject<UserListModel>(text);
            return userList;
        }
        public void SaveUsers(UserListModel userList)
        {
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
    }
}
