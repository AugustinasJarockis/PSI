using System.Collections.Concurrent;

namespace NOTEA.Models.UserModels
{
    public class OnlineUserList : IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; } = new ConcurrentDictionary<int, UserModel>();
    }
}
