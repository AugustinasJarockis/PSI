using NoteaAPI.Models.ExceptionModels;

namespace NoteaAPI.Services.LogServices
{
    public class LogsService : ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception)
        {
            using (StreamWriter writer = new StreamWriter("Logs//logs.txt", append: true))
            {
                string info = "Message: " + exception.Message + " Time: " + exception.DateTime + " Stack trace: " + exception.StackTrace + " Source: " + exception.Source;
                writer.WriteLine(info);
                writer.WriteLine();
                writer.WriteLine("=================================================================================");
                writer.WriteLine();
            }
        }
    }
}
