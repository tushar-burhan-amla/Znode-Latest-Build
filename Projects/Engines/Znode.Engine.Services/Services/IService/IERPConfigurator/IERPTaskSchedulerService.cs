using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IERPTaskSchedulerService
    {
        /// <summary>
        /// Get ERPTaskScheduler list from database.
        /// </summary> 
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns ERPTaskSchedulerListModel</returns>
        ERPTaskSchedulerListModel GetERPTaskSchedulerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create erpTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerModel">ERPTaskScheduler Model.</param>
        /// <returns>Returns created erpTaskScheduler Model.</returns>
        ERPTaskSchedulerModel Create(ERPTaskSchedulerModel erpTaskSchedulerModel);

        /// <summary>
        /// Get erpTaskScheduler on the basis of erpTaskScheduler id.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskSchedulerId.</param>
        /// <returns>Returns erpTaskScheduler model.</returns>
        ERPTaskSchedulerModel GetERPTaskScheduler(int erpTaskSchedulerId);

        /// <summary>
        /// Delete erpTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerIds">ERPTaskScheduler Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel erpTaskSchedulerIds);

        /// <summary>
        /// Get the ERPTaskSchedulerId From Touch point name
        /// </summary>
        /// <param name="erpConfiguratorId">ERPConfigurator Id</param>
        /// <returns>ERPTaskSchedulerId</returns>
        int GetSchedulerIdByTouchPointName(string erpTouchPointName, int erpConfiguratorId, string schedulerCallFor);

        /// <summary>
        /// Trigger action for the task.
        /// </summary>
        /// <param name="ERPTaskSchedulerId"></param>
        /// <returns></returns>
        string TriggerSchedulerTask(string eRPTaskSchedulerId);

        /// <summary>
        /// Enable disable ERP task Scheduler from task service.
        /// </summary>
        /// <param name="connectorTouchPoints"></param>
        /// <param name="isActive"></param>
        /// <param name="erpConfiguratorId">ERPConfigurator Id</param>
        /// <returns>Returns True/false</returns>
        bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive);

        /// <summary>
        /// Method to create the scheduler
        /// </summary>
        /// <param name="erpTaskSchedulerModel">erpTaskSchedulerModel</param>
        /// <param name="createResult">createResult</param>
        /// <returns>ERPTaskSchedulerModel</returns>
        ERPTaskSchedulerModel CreateScheduler(ERPTaskSchedulerModel erpTaskSchedulerModel, IList<View_ReturnBoolean> createResult);
    }
}
