namespace Znode.Engine.Api.Models
{
    public class LogMessageConfigurationModel : BaseModel
    {
        public bool? IsDataBaseLoggingEnabled { get; set; } = true;
        public bool? IsErrorLoggingEnabled { get; set; } = true;
        public bool? IsEventLoggingEnabled { get; set; } = true;
        public bool? IsIntegrationLoggingEnabled { get; set; } = true;
        public bool PurgeDatabaseLogs { get; set; }
        public bool PurgeEventLogs { get; set; }
        public bool PurgeErrorLogs { get; set; }
        public bool PurgeIntegrationLogs { get; set; }
        public bool IsLoggingLevelsEnabledError { get; set; } = true;
        public bool IsLoggingLevelsEnabledWarning { get; set; } = true;
        public bool IsLoggingLevelsEnabledInfo { get; set; } = true;
        public bool IsLoggingLevelsEnabledDebug { get; set; } = true;
        public bool IsLoggingLevelsEnabledAll { get; set; } = true;
    }
}
