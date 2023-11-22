using System.ComponentModel.DataAnnotations;

namespace NOTEA.Models.UserModels
{
    public class UserModel : IUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [Key]
        public int Id { get; set; }
        public UserModel(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
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
