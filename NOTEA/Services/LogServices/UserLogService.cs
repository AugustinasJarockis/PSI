
namespace NOTEA.Services.LogServices
{
    public class UserLogService : IUserLogService
    {
        public void SaveLogInfo(string info, int user_id)
        {
            if (user_id != 0) 
            { 
                using (StreamWriter writer = new StreamWriter("Logs//" + user_id + "UserLogs.txt", append: true))
                {
                    writer.WriteLine(DateTime.Now + " " + info);
                }
            }
        }

        public void SeparateLogInfo(int user_id)
        {
            if (user_id != 0)
            {
                using (StreamWriter writer = new StreamWriter("Logs//" + user_id + "UserLogs.txt", append: true))
                {
                    writer.WriteLine();
                    writer.WriteLine("-------------------------------------------------------------------");
                    writer.WriteLine();
                }
            }
        }
    }
}
