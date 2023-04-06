using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ITouchPointConfigurationClient : IBaseClient
    {
        /// <summary>
        /// Get the list of TouchPointConfiguration
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns TouchPointConfiguration list model.</returns>
        TouchPointConfigurationListModel GetTouchPointConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create task schedular for selected touchpoint in connector.
        /// </summary>
        /// <param name="connectorTouchPoints"></param>
        /// <returns></returns>
        bool TriggerTaskScheduler(string connectorTouchPoints);

        /// <summary>
        /// Get the list of Scheduler Log
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns Scheduler Log list.</returns>
        TouchPointConfigurationListModel GetSchedulerLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
