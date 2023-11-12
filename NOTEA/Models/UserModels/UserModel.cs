using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NOTEA.Models.UserModels
{
    public class UserModel
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
        public UserModel()
        {
            Username = "";
            Password = "";
        }
    }
}
