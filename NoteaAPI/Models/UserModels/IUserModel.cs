using System.ComponentModel.DataAnnotations;
namespace NoteaAPI.Models.UserModels
{
    public interface IUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [Key]
        public int Id { get; set; }
    }
}
