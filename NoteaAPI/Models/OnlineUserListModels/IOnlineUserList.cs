using System.Collections.Concurrent;
using NoteaAPI.Models.UserModels;

namespace NoteaAPI.Models.OnlineUserListModels
{
    public interface IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; }
    }
}
