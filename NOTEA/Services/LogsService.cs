using Newtonsoft.Json;
using NOTEA.Models;

namespace NOTEA.Services
{
    public class LogsService : ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception)
        {
            using (StreamWriter writer = new StreamWriter("Logs//logs.txt", append: true))
                try
                {
                    string info = "Message: " + exception.Message +" Stack trace: "+ exception.StackTrace + " Source: " + exception.Source;
                    writer.WriteLine(info);
                }
                catch (Exception ex)
                {
                    ExceptionModel info = new ExceptionModel(ex);
                    SaveExceptionInfo(info);
                }
        }
    }
}
