using System.Collections.Concurrent;

namespace NOTEA.Models.UserModels
{
    public interface IOnlineUserList
    {
        public ConcurrentDictionary<int, UserModel> OnlineUsers { get; set; }
    }
}
