using NCrontab;

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

using static NCrontab.CrontabSchedule;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ERPTaskSchedulerAgent : BaseAgent, IERPTaskSchedulerAgent
    {
        #region Private Variables
        private readonly IERPTaskSchedulerClient _erpTaskSchedulerClient;
        private readonly IERPConfiguratorAgent _erpConfiguratorAgent;
        #endregion

        #region Constructor
        public ERPTaskSchedulerAgent(IERPTaskSchedulerClient erpTaskSchedulerClient)
        {
            _erpTaskSchedulerClient = GetClient<IERPTaskSchedulerClient>(erpTaskSchedulerClient);
            _erpConfiguratorAgent = new ERPConfiguratorAgent(GetClient<ERPConfiguratorClient>());
        }
        #endregion

        #region public virtual Methods
        //Get the list of ERP Task Scheduler.
        public virtual ERPTaskSchedulerListViewModel GetERPTaskSchedulerList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands,filters and sorts:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { expands = expands, sorts = sorts, filters = filters });
            ERPTaskSchedulerListModel erpTaskSchedulerList = _erpTaskSchedulerClient.GetERPTaskSchedulerList(expands, filters, sorts, pageIndex, pageSize);
            ERPTaskSchedulerListViewModel listViewModel = new ERPTaskSchedulerListViewModel { ERPTaskSchedulerList = erpTaskSchedulerList?.ERPTaskSchedulerList?.ToViewModel<ERPTaskSchedulerViewModel>().ToList() };
            SetListPagingData(listViewModel, erpTaskSchedulerList);

            //Set tool menu for ERP Task Scheduler list grid view.
            SetERPTaskSchedulerListToolMenus(listViewModel);
            return erpTaskSchedulerList?.ERPTaskSchedulerList?.Count > 0 ? listViewModel : new ERPTaskSchedulerListViewModel() { ERPTaskSchedulerList = new List<ERPTaskSchedulerViewModel>() };
        }

        //Get erpTaskScheduler by erpTaskScheduler id.
        public virtual ERPTaskSchedulerViewModel GetERPTaskScheduler(int erpTaskSchedulerId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerClient.GetERPTaskScheduler(erpTaskSchedulerId).ToViewModel<ERPTaskSchedulerViewModel>();

            erpTaskSchedulerViewModel.StartTime = GetSeparatedTime(Convert.ToDateTime(erpTaskSchedulerViewModel.StartDate));

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpTaskSchedulerViewModel;
        }

        //Create ERP Task Scheduler.
        public virtual ERPTaskSchedulerViewModel Create(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            try
            {
                if ((!erpTaskSchedulerViewModel.IsAssignTouchPoint && erpTaskSchedulerViewModel.SchedulerType == ZnodeConstant.Scheduler) 
                    || erpTaskSchedulerViewModel.SchedulerCallFor == ZnodeConstant.SearchIndex 
                    || erpTaskSchedulerViewModel.SchedulerCallFor == ZnodeConstant.ProductFeed || erpTaskSchedulerViewModel.SchedulerCallFor == ZnodeConstant.PublishContentContainer)
                {
                    erpTaskSchedulerViewModel.SchedulerType = !string.IsNullOrEmpty(erpTaskSchedulerViewModel.SchedulerType) ? erpTaskSchedulerViewModel.SchedulerType : ZnodeConstant.Scheduler;

                    erpTaskSchedulerViewModel.StartDate = erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.Recurring ? null : (DateTime?)GetDateTime(Convert.ToDateTime(erpTaskSchedulerViewModel.StartDate), Convert.ToDateTime(erpTaskSchedulerViewModel.StartTime));
                }

                if (!erpTaskSchedulerViewModel.IsAssignTouchPoint)
                {
                    if (erpTaskSchedulerViewModel.SchedulerType != ZnodeConstant.Scheduler)
                        erpTaskSchedulerViewModel.SchedulerFrequency = ZnodeConstant.OneTime;
                }

                ERPTaskSchedulerModel erpTaskSchedulerModel = _erpTaskSchedulerClient.Create(erpTaskSchedulerViewModel.ToModel<ERPTaskSchedulerModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return IsNotNull(erpTaskSchedulerModel) ? erpTaskSchedulerModel.ToViewModel<ERPTaskSchedulerViewModel>() : new ERPTaskSchedulerViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(erpTaskSchedulerViewModel, ERP_Resources.AlreadyExistSchedulerName);
                    default:
                        if (erpTaskSchedulerViewModel.ERPTaskSchedulerId == 0)
                            return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(erpTaskSchedulerViewModel, Admin_Resources.ErrorFailedToCreate);
                        else
                            return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(erpTaskSchedulerViewModel, Admin_Resources.UpdateError);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(erpTaskSchedulerViewModel, Admin_Resources.UpdateError);
            }
        }

        //Delete erpTaskScheduler.
        public virtual bool Delete(string erpTaskSchedulerId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _erpTaskSchedulerClient.Delete(new ParameterModel { Ids = erpTaskSchedulerId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = ERP_Resources.ErrorDeleteERPTaskScheduler;
                        return false;
                    case ErrorCodes.NotDeleteActiveRecord:
                        errorMessage = ERP_Resources.ErrorDeleteActiveTouchPointClass;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Method for  ERPTaskSchedulerId From Touch point name
        public virtual int GetSchedulerIdByTouchPointName(string erpTouchPointName, string schedulerCallFor)
           => _erpTaskSchedulerClient.GetSchedulerIdByTouchPointName(erpTouchPointName, schedulerCallFor);

        //Enable/disable ERP task scheduler from windows service.
        public virtual bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorMessageEnableDisable;
            try
            {
                return _erpTaskSchedulerClient.EnableDisableTaskScheduler(ERPTaskSchedulerId, isActive);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotFound:
                        errorMessage = ERP_Resources.ErrorTaskSchedulerNotFound;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                return false;
            }
        }

        //Validate erpTaskScheduler model.
        public virtual ERPTaskSchedulerViewModel CheckValidation(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel, out bool status)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            if (erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.OneTime)
            {
                return ValidateOneTimeSchedulerModel(erpTaskSchedulerViewModel, out status);
            }
            else if (erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.Recurring)
            {
                return ValidateRecurringSchedulerModel(erpTaskSchedulerViewModel, out status);
            }
            else
            {
                status = true;
                return erpTaskSchedulerViewModel;
            }
        }

        protected virtual ERPTaskSchedulerViewModel ValidateRecurringSchedulerModel(ERPTaskSchedulerViewModel model, out bool status)
        {
            status = false;

            // Validation for model.CronExpression is handled by data annotation.
            if(string.IsNullOrEmpty(model.CronExpression))
            {
                status = false;
                return model;
            }
            else if (ValidateCronExpression(model.CronExpression) == false)
                return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(model, ERP_Resources.ErrorInvalidCronExpression);
            else
            {
                status = true;
                return model;
            }
        }

        protected virtual ERPTaskSchedulerViewModel ValidateOneTimeSchedulerModel(ERPTaskSchedulerViewModel model, out bool status)
        {
            status = false;

            if (model.StartDate?.Date < DateTime.Today.Date)
                return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(model, ERP_Resources.ErrorStartDateGreaterThan);
            else if (model.StartDate?.Date == DateTime.Today.Date && Convert.ToDateTime(model.StartTime).TimeOfDay < DateTime.Now.TimeOfDay)
                return (ERPTaskSchedulerViewModel)GetViewModelWithErrorMessage(model, ERP_Resources.ErrorStartTimeEarlierThanNow);
            else
            {
                status = true;
                return model;
            }
        }

        public virtual bool ValidateCronExpression(string cronExpression)
        {
            CrontabSchedule cronSchedule = TryParse(cronExpression, new ParseOptions
            {
                IncludingSeconds = false
            });

            return IsNotNull(cronSchedule);
        }

        //Set task scheduler data.
        public virtual ERPTaskSchedulerViewModel SetTaskSchedulerData(string ConnectorTouchPoints, string indexName, string schedulerCallFor, int portalId, int catalogId, int portalIndexId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            return new ERPTaskSchedulerViewModel
            {
                ERPClassName = _erpConfiguratorAgent.GetERPClassName(),
                SchedulerFrequency = ZnodeConstant.OneTime,
                SchedulerType = ZnodeConstant.Scheduler,
                TouchPointName = ConnectorTouchPoints,
                IsEnabled = true,
                SchedulerCallFor = schedulerCallFor,
                PortalId = portalId,
                CatalogId = catalogId,
                IndexName = indexName,
                CatalogIndexId = portalIndexId
            };
        }

        //Get task scheduler data for update.
        public virtual ERPTaskSchedulerViewModel GetTaskSchedulerDataForUpdate(int erpTaskSchedulerId, string indexName, string schedulerCallFor, int portalId, int catalogId, int portalIndexId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = GetERPTaskScheduler(erpTaskSchedulerId);
            erpTaskSchedulerViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();
            erpTaskSchedulerViewModel.SchedulerCallFor = string.IsNullOrEmpty(schedulerCallFor) ? erpTaskSchedulerViewModel.SchedulerCallFor : schedulerCallFor;
            erpTaskSchedulerViewModel.PortalId = portalId;
            erpTaskSchedulerViewModel.CatalogId = catalogId;
            erpTaskSchedulerViewModel.IndexName = indexName;
            erpTaskSchedulerViewModel.CatalogIndexId = portalIndexId;
            return erpTaskSchedulerViewModel;
        }
        #endregion

        #region Private Methods
        //Method for Set ERP TaskScheduler List Tool Menus
        private void SetERPTaskSchedulerListToolMenus(ERPTaskSchedulerListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ERPTaskSchedulerDeletePopup')", ControllerName = "ERPTaskScheduler", ActionName = "Delete" });
            }
        }

        //Return Concatenated Date and time
        private DateTime GetDateTime(DateTime date, DateTime time) => date.Date + time.TimeOfDay;

        //Return separate Time
        private string GetSeparatedTime(DateTime dateTime) => (Convert.ToString(dateTime.TimeOfDay));
        #endregion
    }
}