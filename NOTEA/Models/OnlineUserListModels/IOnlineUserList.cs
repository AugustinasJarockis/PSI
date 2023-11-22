using System.Collections.Concurrent;
using NOTEA.Models.UserModels;

namespace NOTEA.Models.OnlineUserListModels
{
    public interface IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; }
    }
}
