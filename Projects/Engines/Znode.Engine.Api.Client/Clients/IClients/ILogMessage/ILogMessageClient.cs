using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ILogMessageClient : IBaseClient
    {
        /// <summary>
        /// Get log message from mongo
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>log message List Model</returns>
        LogMessageListModel GetLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get integration log messages from mongo
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>log message List Model</returns>
        LogMessageListModel GetIntegrationLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get event log messages from mongo
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>log message List Model</returns>
        LogMessageListModel GetEventLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get database log messages from sql server
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>log message List Model</returns>
        LogMessageListModel GetDatabaseLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///  Get log message from mongo
        /// </summary>
        /// <param name="logMessageId">Log message Id</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Log message Model</returns>
        LogMessageModel GetLogMessage(string logMessageId);

        /// <summary>
        ///  Get database log message from sql
        /// </summary>
        /// <param name="logMessageId">Log message Id</param>
        /// <returns>Log message Model</returns>
        LogMessageModel GetDatabaseLogMessage(string logMessageId);


        /// <summary>
        ///  Get log configuration from Global Setting
        /// </summary>
        /// <param name=""></param>
        /// <returns>Log Message Configuration Model</returns>
        LogMessageConfigurationModel GetLogConfiguration();

        /// <summary>
        ///  Update log configuration in Global Setting
        /// </summary>
        /// <param name=""></param>
        /// <returns>Log Message Configuration Model</returns>
        LogMessageConfigurationModel UpdateLogConfiguration(LogMessageConfigurationModel logMessageConfigurationModel);

        /// <summary>
        /// Purge Logs.
        /// </summary>
        /// <param name="logCategoryIds">logCategoryIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool PurgeLogs(ParameterModel logCategoryIds);


        /// <summary>
        /// Get impersonation activity from sql
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>List Model</returns>
        ImpersonationActivityListModel GetImpersonationLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
