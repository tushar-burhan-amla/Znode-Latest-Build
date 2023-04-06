using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ILogMessageAgent
    {
        /// <summary>
        /// Gets the list of log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="pageIndex">Start page index of warehouse list.</param>
        /// <param name="pageSize">Page size of warehouse list.</param>
        /// <returns>Returns log message list.</returns>
        LogMessageListViewModel GetLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the list of integration log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with message list.</param>
        /// <param name="filters">Filters to be applied on message list.</param>
        /// <param name="sorts">Sorting to be applied on message list.</param>
        /// <param name="pageIndex">Start page index of message list.</param>
        /// <param name="pageSize">Page size of message list.</param>
        /// <returns>Returns log message list.</returns>
        LogMessageListViewModel GetIntegrationLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the list of event log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with message list.</param>
        /// <param name="filters">Filters to be applied on message list.</param>
        /// <param name="sorts">Sorting to be applied on message list.</param>
        /// <param name="pageIndex">Start page index of message list.</param>
        /// <param name="pageSize">Page size of message list.</param>
        /// <returns>Returns log message list.</returns>
        LogMessageListViewModel GetEventLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the list of database log message.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with message list.</param>
        /// <param name="filters">Filters to be applied on message list.</param>
        /// <param name="sorts">Sorting to be applied on message list.</param>
        /// <param name="pageIndex">Start page index of message list.</param>
        /// <param name="pageSize">Page size of message list.</param>
        /// <returns>Returns log message list.</returns>
        LogMessageListViewModel GetDatabaseLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get log message list by log message id.
        /// </summary>
        /// <param name="logMessageId">log message list Id</param>
        /// <returns>Returns LogMessageViewModel.</returns>
        LogMessageViewModel GetLogMessage(string logMessageId);


        /// <summary>
        /// Get log message list by log message id.
        /// </summary>
        /// <param name="logMessageId">log message list Id</param>
        /// <returns>Returns LogMessageViewModel.</returns>
        LogMessageViewModel GetDatabaseLogMessage(string logMessageId);

        /// <summary>
        /// Get log configuration.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns LogConfigurationViewModel.</returns>
        LogConfigurationViewModel GetLogConfiguration();

        /// <summary>
        /// Purge Logs.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool PurgeLogs(string logCategoryIds);

        /// <summary>
        /// Purge Logs.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Returns true if deleted successfully else false with message.</returns>
        bool PurgeLogs(string logCategoryIds,out string message);

        /// <summary>
        /// Gets the list of impersonation activity.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with list.</param>
        /// <param name="filters">Filters to be applied on list.</param>
        /// <param name="sorts">Sorting to be applied on list.</param>
        /// <param name="pageIndex">Start page index of list.</param>
        /// <param name="pageSize">Page size of list.</param>
        /// <returns>Returns impersonation activity list.</returns>
        ImpersonationLogListViewModel GetImpersonationLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Update Logs Configuration.
        /// </summary>
        /// <param name="logConfigurationViewModel">Log Configuration View Model.</param>
        /// <returns>Returns created model.</returns>
        LogConfigurationViewModel UpdateLogConfiguration(LogConfigurationViewModel logConfigurationViewModel);

        /// <summary>
        /// Set Logging Configuration Settings.
        /// </summary>
        void SetGlobalLoggingSetting();
    }
}
