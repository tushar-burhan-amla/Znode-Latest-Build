using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ILogMessageService
    {
        /// <summary>
        /// Gets the list of log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of log message.</returns>
        LogMessageListModel GetLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of integration log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with integration log message list.</param>
        /// <param name="filters">Filters to be applied on integration log message list.</param>
        /// <param name="sorts">Sorting to be applied on integration log message list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of integration log message.</returns>
        LogMessageListModel GetIntegrationLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of event log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with integration log message list.</param>
        /// <param name="filters">Filters to be applied on integration log message list.</param>
        /// <param name="sorts">Sorting to be applied on integration log message list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of event log message.</returns>
        LogMessageListModel GetEventLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of database log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with integration log message list.</param>
        /// <param name="filters">Filters to be applied on integration log message list.</param>
        /// <param name="sorts">Sorting to be applied on integration log message list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of database log message.</returns>
        LogMessageListModel GetDatabaseLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get LogMessage on the basis of LogMessage id.
        /// </summary>
        /// <param name="logmessageId">logmessageId.</param>
        /// <returns>Returns LogMessage model.</returns>
        LogMessageModel GetLogMessage(string logmessageId, NameValueCollection expands);

        /// <summary>
        /// Get Database LogMessage on the basis of LogMessage id from SQL.
        /// </summary>
        /// <param name="logMessageId">logMessageId.</param>
        /// <returns>Returns LogMessage model.</returns>
        LogMessageModel GetDatabaseLogMessage(string logMessageId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Log Configuration.
        /// </summary>
        /// <returns>Returns LogMessageConfigurationModel model.</returns>
        LogMessageConfigurationModel GetLogConfiguration();


        /// <summary>
        /// Update Log Configuration
        /// </summary>
        /// <param name="model">LogMessageConfigurationModel</param>
        /// <returns>True if successfully updated else false</returns>
        bool UpdateLogConfiguration(LogMessageConfigurationModel model);

        /// <summary>
        /// Purge logs.
        /// </summary>
        /// <param name="logCategoryIds">Log Category Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool PurgeLogs(ParameterModel logCategoryIds);


        /// <summary>
        /// Gets the list of impersonation log activity.
        /// </summary>
        /// <param name="expands">Expands to be retrieved.</param>
        /// <param name="filters">Filters to be applied.</param>
        /// <param name="sorts">Sorting to be applied.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of log activity.</returns>
        ImpersonationActivityListModel GetImpersonationLogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
