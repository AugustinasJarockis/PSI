using System.Collections.Concurrent;
using NOTEA.Models.UserModels;

namespace NOTEA.Models.OnlineUserListModels
{
    public class OnlineUserList : IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; } = new ConcurrentDictionary<int, UserModel>();
    }
}
