using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IERPTaskSchedulerAgent
    {

        #region ERP Task Scheduler
        /// <summary>
        /// Get the list of ERP Task Scheduler.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ERPTaskScheduler list ViewModel.</returns>
        ERPTaskSchedulerListViewModel GetERPTaskSchedulerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get erpTaskScheduler list by ERPTaskSchedulerid.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler list Id</param>
        /// <returns>Returns ERPTaskSchedulerViewModel.</returns>
        ERPTaskSchedulerViewModel GetERPTaskScheduler(int erpTaskSchedulerId);

        /// <summary>
        /// Create new ERP Task Scheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerModel">ERPTaskScheduler ViewModel.</param>
        /// <returns>Returns true if ERPTaskScheduler created else returns false.</returns>
        ERPTaskSchedulerViewModel Create(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel);

        /// <summary>
        /// Delete erpTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(string erpTaskSchedulerId, out string errorMessage);

        /// <summary>
        /// Get the ERPTaskSchedulerId From Touch point name
        /// </summary>
        /// <returns>ERPTaskSchedulerId</returns>
        int GetSchedulerIdByTouchPointName(string erpTouchPointName, string schedulerCallFor);

        /// <summary>
        /// Enable/disable ERP task scheduler from windows service.
        /// </summary>
        /// <param name="connectorTouchPoints">connectorTouchPoints</param>
        /// <param name="isActive">true/false</param>
        /// <returns></returns>
        bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive, out string errorMessage);

        /// <summary>
        /// Check validation.
        /// </summary>
        ERPTaskSchedulerViewModel CheckValidation(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel, out bool status);

        /// <summary>
        /// Set task scheduler data.
        /// </summary>
        /// <param name="ConnectorTouchPoints">ConnectorTouchPoints</param>
        /// <param name="indexName">index name</param>
        /// <param name="schedulerCallFor">scheduler caller name </param>
        /// <param name="portalId">portal id</param>
        /// <param name="catalogIndexId">portal Index Id</param>
        /// <returns>ERPTaskSchedulerViewModel</returns>
        ERPTaskSchedulerViewModel SetTaskSchedulerData(string ConnectorTouchPoints, string indexName, string schedulerCallFor, int portalId, int catalogId, int catalogIndexId);

        /// <summary>
        /// Get task scheduler data for update.
        /// </summary>
        /// <param name="erpTaskSchedulerId">erpTaskSchedulerId</param>
        /// <param name="indexName">index name</param>
        /// <param name="schedulerCallFor">scheduler caller name </param>
        /// <param name="portalId">portal id</param>
        /// <param name="catalogIndexId">portal Index Id</param>
        /// <returns>ERPTaskSchedulerViewModel</returns>
        ERPTaskSchedulerViewModel GetTaskSchedulerDataForUpdate(int erpTaskSchedulerId, string indexName, string schedulerCallFor, int portalId, int catalogId, int catalogIndexId);

        /// <summary>
        /// Validate the Cron expression
        /// </summary>
        /// <param name="cronExpression">Cron expression to be validated</param>
        /// <returns>True if the Cron expression is valid, false otherwise.</returns>
        bool ValidateCronExpression(string cronExpression);
        #endregion
    }
}
