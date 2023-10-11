using NOTEA.Models;

namespace NOTEA.Services
{
    public interface ILogsService
    {
        public void SaveExceptionInfo(ExceptionModel exception);
    }
}
