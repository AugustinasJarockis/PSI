using NoteaAPI.Models.ExceptionModels;

namespace NoteaAPI.Services.LogServices
{
    public interface ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception);
    }
}
