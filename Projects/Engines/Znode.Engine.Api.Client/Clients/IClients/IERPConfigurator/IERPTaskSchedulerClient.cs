using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IERPTaskSchedulerClient : IBaseClient
    {
        /// <summary>
        /// Get the list of ERPTaskScheduler
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ERPTaskScheduler list model.</returns>
        ERPTaskSchedulerListModel GetERPTaskSchedulerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create ERPTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerModel">ERPTaskScheduler Model.</param>
        /// <returns>Returns created ERPTaskScheduler Model.</returns>
        ERPTaskSchedulerModel Create(ERPTaskSchedulerModel erpTaskSchedulerModel);

        /// <summary>
        /// Get erpTaskScheduler on the basis of erpTaskScheduler id.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskSchedulerId to get erpTaskScheduler details.</param>
        /// <returns>Returns ERPTaskSchedulerModel.</returns>
        ERPTaskSchedulerModel GetERPTaskScheduler(int erpTaskSchedulerId);

        /// <summary>
        /// Delete ERPTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel erpTaskSchedulerId);

        /// <summary>
        /// Get the ERPTaskSchedulerId From Touch point name
        /// </summary>
        /// <returns>ERPTaskSchedulerId</returns>
        int GetSchedulerIdByTouchPointName(string erpTouchPointName, string schedulerCallFor);

        /// <summary>
        /// Enable/disable ERP task scheduler from windows service.
        /// </summary>
        /// <param name="connectorTouchPoints">ConnectorTouchPoints</param>
        /// <param name="isActive">True/false</param>
        /// <returns></returns>
        bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive);
    }
}
