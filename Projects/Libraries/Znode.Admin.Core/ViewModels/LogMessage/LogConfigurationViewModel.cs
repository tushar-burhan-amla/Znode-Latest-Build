
namespace Znode.Engine.Admin.ViewModels
{
    public class LogConfigurationViewModel : BaseViewModel
    {
        public bool? IsDataBaseLoggingEnabled { get; set; } = true;
        public bool? IsErrorLoggingEnabled { get; set; } = true;
        public bool? IsEventLoggingEnabled { get; set; } = true;
        public bool? IsIntegrationLoggingEnabled { get; set; } = true;
        public bool PurgeDatabaseLogs { get; set; }
        public bool PurgeEventLogs { get; set; }
        public bool PurgeErrorLogs { get; set; }
        public bool PurgeIntegrationLogs { get; set; }
        public int LogCategoryIdToBeDeleted { get; set; }
        public bool IsLoggingLevelsEnabledOff { get; set; } 
        public bool IsLoggingLevelsEnabledFatal { get; set; } 
        public bool IsLoggingLevelsEnabledError { get; set; }
        public bool IsLoggingLevelsEnabledWarning { get; set; } 
        public bool IsLoggingLevelsEnabledInfo { get; set; } 
        public bool IsLoggingLevelsEnabledDebug { get; set; } 
        public bool IsLoggingLevelsEnabledAll { get; set; } 

      

    }
}
