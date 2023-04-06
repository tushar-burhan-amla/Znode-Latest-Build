using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class LogMessageController : BaseController
    {

        #region Private Variables

        private readonly ILogMessageAgent _logMessageAgent;

        #endregion      

        #region Public Constructor
        public LogMessageController(ILogMessageAgent logMessageAgent)
        {
            _logMessageAgent = logMessageAgent;
        }
        #endregion

        #region Public Method
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeLogMessage.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeLogMessage.ToString(), model);
            //Get log message list.
            LogMessageListViewModel logMessageList = _logMessageAgent.GetLogMessageList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            logMessageList.GridModel = FilterHelpers.GetDynamicGridModel(model, logMessageList?.LogMessageList, GridListType.ZnodeLogMessage.ToString(), string.Empty, null, true, true, logMessageList?.GridModel?.FilterColumn?.ToolMenuList);
            logMessageList.GridModel.TotalRecordCount = logMessageList.TotalResults;

            //Returns the log message list.
            return ActionView(logMessageList);
        }

        public virtual ActionResult IntegrationLogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeIntegrationLogMessage.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeIntegrationLogMessage.ToString(), model);
            //Get integration log message list.
            LogMessageListViewModel logMessageList = _logMessageAgent.GetIntegrationLogMessageList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            logMessageList.GridModel = FilterHelpers.GetDynamicGridModel(model, logMessageList?.LogMessageList, GridListType.ZnodeIntegrationLogMessage.ToString(), string.Empty, null, true, true, logMessageList?.GridModel?.FilterColumn?.ToolMenuList);
            logMessageList.GridModel.TotalRecordCount = logMessageList.TotalResults;

            //Returns the integration log message list.
            return ActionView(logMessageList);
        }

        //Get event log messages list
        public virtual ActionResult EventLogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeEventLogMessage.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeEventLogMessage.ToString(), model);
            //Get integration log message list.
            LogMessageListViewModel logMessageList = _logMessageAgent.GetEventLogMessageList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            logMessageList.GridModel = FilterHelpers.GetDynamicGridModel(model, logMessageList?.LogMessageList, GridListType.ZnodeEventLogMessage.ToString(), string.Empty, null, true, true, logMessageList?.GridModel?.FilterColumn?.ToolMenuList);
            logMessageList.GridModel.TotalRecordCount = logMessageList.TotalResults;

            //Returns the integration log message list.
            return ActionView(logMessageList);
        }

        //Get database log messages list
        public virtual ActionResult DatabaseLogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeDatabaseLogMessage.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeDatabaseLogMessage.ToString(), model);
            //Get integration log message list.
            LogMessageListViewModel logMessageList = _logMessageAgent.GetDatabaseLogMessageList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            logMessageList.GridModel = FilterHelpers.GetDynamicGridModel(model, logMessageList?.LogMessageList, GridListType.ZnodeDatabaseLogMessage.ToString(), string.Empty, null, true, true, logMessageList?.GridModel?.FilterColumn?.ToolMenuList);
            logMessageList.GridModel.TotalRecordCount = logMessageList.TotalResults;

            //Returns the integration log message list.
            return ActionView(logMessageList);
        }      

        //Get:Edit log message
        [HttpGet]
        public virtual ActionResult GetLogMessage(string logMessageId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            LogMessageViewModel logmessageViewModel = _logMessageAgent.GetLogMessage(logMessageId);
            return ActionView("View", logmessageViewModel);
        }

        //Get:Edit integration log message
        [HttpGet]
        public virtual ActionResult GetIntegrationLogMessage(string logMessageId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            LogMessageViewModel logmessageViewModel = _logMessageAgent.GetLogMessage(logMessageId);
            return ActionView("IntegrationLogView", logmessageViewModel);
        }

        //Get:Edit event log message
        [HttpGet]
        public virtual ActionResult GetEventLogMessage(string logMessageId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            LogMessageViewModel logmessageViewModel = _logMessageAgent.GetLogMessage(logMessageId);
            return ActionView("EventLogView", logmessageViewModel);
        }

        //Get:Edit database log message
        [HttpGet]
        public virtual ActionResult GetDatabaseLogMessage(string logMessageId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            LogMessageViewModel logmessageViewModel = _logMessageAgent.GetDatabaseLogMessage(logMessageId);
            return ActionView("DatabaseLogView", logmessageViewModel);
        }

        [HttpGet]
        public virtual ActionResult ConfigureLogs()
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            LogConfigurationViewModel logConfigurationViewModel = _logMessageAgent.GetLogConfiguration();
            return ActionView(logConfigurationViewModel);
        }

        [HttpPost]
        public virtual ActionResult ConfigureLogs(LogConfigurationViewModel logConfigurationViewModel)
        {
            if (ModelState.IsValid)
            {
                logConfigurationViewModel = _logMessageAgent.UpdateLogConfiguration(logConfigurationViewModel);
                if (logConfigurationViewModel.HasError)
                    SetNotificationMessage(GetErrorNotificationMessage(logConfigurationViewModel.ErrorMessage));
                else
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.LoggingConfigurationUpdatedSuccessMessage));
            }
            
            return ActionView(logConfigurationViewModel);
        }

        [HttpPost]
        public virtual JsonResult PurgeLogs(string logCategoryIds)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(logCategoryIds))
            {
                status = _logMessageAgent.PurgeLogs(logCategoryIds, out message);
                return Json(new { status = status, message = string.IsNullOrEmpty(message) ? Admin_Resources.FailedDeleteMultipleLogsMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.FailedDeleteMultipleLogsMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Impersonation

       
        public virtual ActionResult ImpersonationLogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {           
            // Get and Set Filters from Cookies if exists.
             FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeImersonationActivityLog.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeImersonationActivityLog.ToString(), model);

            //Get Impersonation activity list.
            ImpersonationLogListViewModel impersonationLogListViewModel =  _logMessageAgent.GetImpersonationLogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            impersonationLogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, impersonationLogListViewModel?.ImpersonationActivityList, GridListType.ZnodeImersonationActivityLog.ToString(), string.Empty, null, true, true, impersonationLogListViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            impersonationLogListViewModel.GridModel.TotalRecordCount = impersonationLogListViewModel.TotalResults;

            //Returns impersonation activity list.
            return ActionView(impersonationLogListViewModel);
        }
        #endregion

    }
}
