using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NOTEA.Models;
using System.IO;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        UserLog user = new UserLog();
        UserListModel userList = new UserListModel();


        public IActionResult SignIn()
        {
            return View(user);
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password)
        {
            user.username = username;
            user.password = password;
            userList = LoadUsers();
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

            return View(user);
        }

        public UserListModel LoadUsers()
        {
            string text = System.IO.File.ReadAllText("Users//Users.txt");
            UserListModel userList = JsonConvert.DeserializeObject<UserListModel>(text);
            return userList;
        }
    }
}
