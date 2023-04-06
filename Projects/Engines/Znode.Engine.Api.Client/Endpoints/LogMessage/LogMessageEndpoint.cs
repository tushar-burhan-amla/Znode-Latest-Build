namespace Znode.Engine.Api.Client.Endpoints
{
    public class LogMessageEndpoint : BaseEndpoint
    {
        public static string GetLogMessageList()
            => $"{ApiRoot}/logmessage/list";

        public static string GetIntegrationLogMessageList()
            => $"{ApiRoot}/logmessage/integrationloglist";

        public static string GetEventLogMessageList()
            => $"{ApiRoot}/logmessage/eventloglist";

        public static string GetDatabaseLogMessageList()
            => $"{ApiRoot}/logmessage/databaseloglist";

        public static string GetLogMessage(string logMessageId)
            => $"{ApiRoot}/logmessage/getlogmessage/{logMessageId}";

        public static string GetDatabaseLogMessage(string logMessageId)
            => $"{ApiRoot}/logmessage/getdatabaselogmessage/{logMessageId}";

        public static string GetLogConfiguration()
            => $"{ApiRoot}/logmessage/getlogconfiguration";

        public static string UpdateLogConfiguration() 
            => $"{ApiRoot}/logmessage/updatelogconfiguration";

        public static string PurgeLogs() 
            => $"{ApiRoot}/logmessage/purgelogs";

        public static string GetImpersonationLogList()
            => $"{ApiRoot}/logmessage/impersonationloglist";
    }
}
