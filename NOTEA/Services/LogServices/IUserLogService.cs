

namespace NOTEA.Services.LogServices
{
    public interface IUserLogService
    {
        public void SaveLogInfo(string info, int user_id);
        public void SeparateLogInfo(int user_id);
    }
}
