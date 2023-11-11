namespace NOTEA.Models.ExceptionModels
{
    public struct ExceptionModel
    {
        public string Message { get; }
        public string StackTrace { get; }
        public string Source { get; }
        public DateTime DateTime { get; }   

        public ExceptionModel(Exception exception)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            Source = exception.Source;
            DateTime = DateTime.Now;
        }
    }
}
