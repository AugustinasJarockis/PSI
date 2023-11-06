using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models.UserModels
{
    public class UserModel
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string? PasswordCheck { get; set; }
        public UserModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public UserModel()
        {
            Username = "";
            Password = "";
        }
    }
}
