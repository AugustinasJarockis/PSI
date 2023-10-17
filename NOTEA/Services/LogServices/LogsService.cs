using NOTEA.Models.ExceptionModels;

namespace NOTEA.Services.LogServices
{
    public class LogsService : ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception)
        {
            using (StreamWriter writer = new StreamWriter("Logs//logs.txt", append: true))
            {
                string info = "Message: " + exception.Message + " Stack trace: " + exception.StackTrace + " Source: " + exception.Source;
                writer.WriteLine(info);
            }
        }
    }
}
