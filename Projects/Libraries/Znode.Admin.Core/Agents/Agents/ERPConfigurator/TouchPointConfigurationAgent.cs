using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class TouchPointConfigurationAgent : BaseAgent, ITouchPointConfigurationAgent
    {
        #region Private Variables
        private readonly ITouchPointConfigurationClient _touchPointConfigurationClient;
        #endregion

        #region Constructor
        public TouchPointConfigurationAgent(ITouchPointConfigurationClient touchPointConfigurationClient)
        {
            _touchPointConfigurationClient = GetClient<ITouchPointConfigurationClient>(touchPointConfigurationClient);
        }
        #endregion

        #region public Methods
        //Get the list of Touch Point Configuration.
        public virtual TouchPointConfigurationListViewModel GetTouchPointConfigurationList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, bool isAssigned = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Remove old filters of IsAssigned
            filters.RemoveAll(x => x.FilterName == FilterKeys.IsAssigned);
            filters.Add(new FilterTuple(FilterKeys.IsAssigned, FilterOperators.Equals, isAssigned.ToString()));
            TouchPointConfigurationListModel touchPointConfigurationList = _touchPointConfigurationClient.GetTouchPointConfigurationList(expands, filters, sorts, pageIndex, pageSize);
            ZnodeLogging.LogMessage("Input parameters expands,filters and sorts:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            TouchPointConfigurationListViewModel listViewModel = new TouchPointConfigurationListViewModel { TouchPointConfigurationList = touchPointConfigurationList?.TouchPointConfigurationList?.ToViewModel<TouchPointConfigurationViewModel>().ToList() };
            SetListPagingData(listViewModel, touchPointConfigurationList);
            //Set tool menu for warehouse list grid view.
            if (!isAssigned)
                SetTouchPointConfigurationListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return touchPointConfigurationList?.TouchPointConfigurationList?.Count > 0 ? listViewModel : new TouchPointConfigurationListViewModel() { TouchPointConfigurationList = new List<TouchPointConfigurationViewModel>() };
        }

        // Create task schedular for selected touchpoint in connector.
        public virtual bool TriggerTaskScheduler(string connectorTouchPoints, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            message = ERP_Resources.FailedTriggerScheduler;
            try
            {
                return _touchPointConfigurationClient.TriggerTaskScheduler(connectorTouchPoints);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotFound:
                        message = ERP_Resources.FailedTriggerSchedulerDueToParameterMismatch;
                        return false;

                    case ErrorCodes.FileNotFound:
                        message = ERP_Resources.ErrorFileNotAvailable;
                        return false;

                    default:
                        message = ERP_Resources.FailedTriggerScheduler;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = ERP_Resources.FailedTriggerScheduler;
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return false;
            }
        }

        // Get the list of Scheduler Log history
        public virtual TouchPointConfigurationListViewModel GetSchedulerLogList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, string schedulerName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Remove old filters of SchedulerName
            if (filters.Exists(x => x.Item1 == ZnodeConstant.SchedulerName.ToString()))
                filters.RemoveAll(x => x.FilterName == ZnodeConstant.SchedulerName);
            ZnodeLogging.LogMessage("Input parameters expands,filters and sorts:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            TouchPointConfigurationListModel schedulerLogList = _touchPointConfigurationClient.GetSchedulerLogList(expands, filters, sorts, pageIndex, pageSize);
            TouchPointConfigurationListViewModel listViewModel = new TouchPointConfigurationListViewModel { SchedulerLogList = schedulerLogList?.SchedulerLogList?.ToViewModel<TouchPointConfigurationViewModel>().ToList() };
            SetListPagingData(listViewModel, schedulerLogList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return schedulerLogList?.SchedulerLogList?.Count > 0 ? listViewModel : new TouchPointConfigurationListViewModel() { SchedulerLogList = new List<TouchPointConfigurationViewModel>() };
        }

        // Get the list of Scheduler Log details
        public virtual TouchPointConfigurationViewModel SchedulerLogDetails(string schedulerName, string recordId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            //Remove old filters of SchedulerName
            if (filters.Exists(x => x.Item1 == ZnodeConstant.SchedulerName.ToString()))
                filters.RemoveAll(x => x.FilterName == ZnodeConstant.SchedulerName);
            else
                filters.Add(ZnodeConstant.SchedulerName, null, schedulerName);

            //Remove old filters of SchedulerName
            if (filters.Exists(x => string.Equals(x.Item1, ZnodeConstant.RecordId, StringComparison.CurrentCultureIgnoreCase)))
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.RecordId, StringComparison.CurrentCultureIgnoreCase));
            else
                filters.Add(ZnodeConstant.RecordId.ToLower(), null, recordId);

            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new {filters = filters});

            TouchPointConfigurationListModel schedulerLogList = _touchPointConfigurationClient.GetSchedulerLogList(null, filters, null, 1, 1);
            TouchPointConfigurationViewModel model = new TouchPointConfigurationViewModel
            {
                LogName = schedulerLogList.SchedulerLogList.FirstOrDefault().LogName,
                SchedulerName = schedulerLogList.SchedulerLogList.FirstOrDefault().SchedulerName,
                EventID = schedulerLogList.SchedulerLogList.FirstOrDefault().EventID,
                DateTime = schedulerLogList.SchedulerLogList.FirstOrDefault().DateTime,
                Level = schedulerLogList.SchedulerLogList.FirstOrDefault().Level,
                RecordId = schedulerLogList.SchedulerLogList.FirstOrDefault().RecordId,
                TaskCategory = schedulerLogList.SchedulerLogList.FirstOrDefault().TaskCategory,
                MachineName = schedulerLogList.SchedulerLogList.FirstOrDefault().MachineName,
                CorrelationId = schedulerLogList.SchedulerLogList.FirstOrDefault().CorrelationId,
                OperationalCode = schedulerLogList.SchedulerLogList.FirstOrDefault().OperationalCode,
                LogDetails = schedulerLogList.SchedulerLogList.FirstOrDefault().LogDetails,
            };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return model;
        }
        #endregion

        #region Private Methods.
        //Set tool menu for warehouse list grid view.
        private void SetTouchPointConfigurationListToolMenu(TouchPointConfigurationListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TouchPointConfigurationDeletePopup')", ControllerName = "TouchPointConfiguration", ActionName = "Delete" });
            }
        }
        #endregion
    }
}