using NOTEA.Models.ExceptionModels;

namespace NOTEA.Services.LogServices
{
    public interface ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception);
    }
}
