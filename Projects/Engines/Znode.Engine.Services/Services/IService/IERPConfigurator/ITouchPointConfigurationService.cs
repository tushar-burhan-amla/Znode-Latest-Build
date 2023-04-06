using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ITouchPointConfigurationService
    {
        /// <summary>
        /// Get TouchPointConfiguration list from database.
        /// </summary> 
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns TouchPointConfigurationListModel</returns>
        TouchPointConfigurationListModel GetTouchPointConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create task scheduler which runs on same time.
        /// </summary>
        /// <param name="connectorTouchPoints"></param>
        /// <returns></returns>
        bool TriggerTaskScheduler(string connectorTouchPoints);

        /// <summary>
        /// Get Scheduler Log List list task scheduler
        /// </summary> 
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns Scheduler Log List</returns>
        TouchPointConfigurationListModel SchedulerLogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Send Email to Site Admin
        /// </summary>
        /// <param name="errorMessage">error Message.</param>
        /// <returns>Returns true if send email successfully else return false.</returns>
        bool SendSchedulerActivityLog(ERPSchedulerLogActivityModel erpSchedulerLogActivityModel);

        /// <summary>
        /// Generate email for scheduler activity log
        /// </summary>      
        /// <param name="storeLogo">Store logo</param>
        /// <param name="eRPTaskScheduler">Task scheduler repository</param>
        /// <param name="schedulerStatus">Scheduler status</param>
        /// <param name="templateContent">content</param>
        /// <param name="errorMessage">error Message.</param>
        /// <param name="messageText">Generated email text.</param>
        void GenerateSchedulerActivityLogEmailText(string storeLogo, Libraries.Data.DataModel.ZnodeERPTaskScheduler eRPTaskScheduler, string schedulerStatus, string templateContent, string errorMessage, out string messageText);
    }
}
