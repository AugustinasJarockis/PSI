using System.Collections.Concurrent;
using NoteaAPI.Models.UserModels;

namespace NoteaAPI.Models.OnlineUserListModels
{
    public class OnlineUserList : IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; } = new ConcurrentDictionary<int, UserModel>();
    }
}
