﻿using Newtonsoft.Json;
using NOTEA.Models.UserModels;

namespace NOTEA.Services.UserServices
{
    public class UserService : IUserService
    {
        private UserListModel LoadUsers()
        {
            string text = File.ReadAllText("Users//Users.txt");
            UserListModel userList = JsonConvert.DeserializeObject<UserListModel>(text);
            return userList;
        }
        public void SaveUser(UserModel user)
        {
            UserListModel userList = LoadUsers();
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
        bool IUserService.CheckLogIn(UserModel user)
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
    }
}
