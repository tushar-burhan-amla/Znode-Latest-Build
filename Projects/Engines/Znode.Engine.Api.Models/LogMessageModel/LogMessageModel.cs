namespace Znode.Engine.Api.Models
{
    public class LogMessageModel : BaseModel
    {
        public string LogMessageId { get; set; }
        public string Component { get; set; }
        public string LogMessage { get; set; }
        public string TraceLevel { get; set; }
        public string StackTraceMessage { get; set; }
        public string DomainName { get; set; }
        public string ApplicationType { get; set; }
    }
}
