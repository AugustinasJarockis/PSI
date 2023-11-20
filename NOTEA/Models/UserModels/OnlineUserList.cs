using System.Collections.Concurrent;

namespace NOTEA.Models.UserModels
{
    public static class OnlineUserList
    {
        public static ConcurrentDictionary<int, UserModel> onlineUsers = new ConcurrentDictionary<int, UserModel>();
    }
}
