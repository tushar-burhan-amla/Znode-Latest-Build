using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ITouchPointConfigurationAgent
    {
        /// <summary>
        /// Get the list of Touch Point Configuration.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns TouchPointConfiguration list ViewModel.</returns>
        TouchPointConfigurationListViewModel GetTouchPointConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize , bool isAssigned);

        /// <summary>
        /// Create task schedular for selected touchpoint in connector.
        /// </summary>
        /// <param name="connectorTouchPoints"></param>
        /// <returns>Return bool result for task schedular created or not.</returns>
        bool TriggerTaskScheduler(string connectorTouchPoints, out string message);

        /// <summary>
        /// Get the list of Scheduler Log
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns Scheduler Log list.</returns>
        TouchPointConfigurationListViewModel GetSchedulerLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string schedulerName);

        /// <summary>
        /// Get the Details of Scheduler Log
        /// </summary>
        /// <returns>Returns Scheduler Log Details.</returns>
        TouchPointConfigurationViewModel SchedulerLogDetails(string schedulerName, string RecordId);
    }
}
