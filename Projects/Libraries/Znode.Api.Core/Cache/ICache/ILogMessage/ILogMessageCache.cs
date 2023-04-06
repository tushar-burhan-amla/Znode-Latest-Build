namespace Znode.Engine.Api.Cache
{
    public interface ILogMessageCache
    {
        /// <summary>
        /// Get log message list from mongo
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetLogMessageList(string routeUri, string routeTemplate);


        /// <summary>
        /// Get Publish Catalog from mongo
        /// </summary>
        /// <param name="logmessageId">logmessage Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetLogMessage(string logmessageId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get integration log message list from mongo
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetIntegrationLogMessageList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get event log message list from mongo
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetEventLogMessageList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get database log message list from sql server
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetDatabaseLogMessageList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get log message details 
        /// </summary>
        /// <param name="logmessageId">logmessage Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetDatabaseLogMessage(string logmessageId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get log message configuration
        /// </summary>
        /// <param name="localeId">locale Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetLogConfiguration(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Impersonation Activity log
        /// </summary>  
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetImpersonationLogList(string routeUri, string routeTemplate);
    }
}
