using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NOTEA.Models.UserModels
{
    public class UserModel
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        [NotMapped]
        public List<int> Conspects_Id { get; set; }
        public UserModel(string username, string password)
        {
            Username = username;
            Password = password;
            Conspects_Id = new List<int>();
        }
        public UserModel()
        {
            Username = "";
            Password = "";
            Conspects_Id = new List<int>();
        }
    }
}
