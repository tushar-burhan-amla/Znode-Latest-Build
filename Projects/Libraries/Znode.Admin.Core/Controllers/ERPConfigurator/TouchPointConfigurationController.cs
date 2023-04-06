using System.Web.Mvc;

using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class TouchPointConfigurationController : BaseController
    {
        #region Private Variables
        private readonly ITouchPointConfigurationAgent _touchPointConfigurationAgent;
        private readonly IERPConfiguratorAgent _erpConfiguratorAgent;
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;

        private readonly string SCHEDULER_LOG_DETAILS_VIEW_PATH = "/Views/TouchPointConfiguration/_SchedulerLogDetails.cshtml";
        private readonly string ONE_TIME_VIEW_PATH = "/Views/TouchPointConfiguration/_OneTime.cshtml";
        private readonly string RECURRING_VIEW_PATH = "/Views/TouchPointConfiguration/_Recurring.cshtml";
        #endregion

        #region Constructor
        public TouchPointConfigurationController(ITouchPointConfigurationAgent touchPointConfigurationAgent, IERPConfiguratorAgent erpConfiguratorAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent)
        {
            _touchPointConfigurationAgent = touchPointConfigurationAgent;
            _erpConfiguratorAgent = erpConfiguratorAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
        }
        #endregion

        #region Public Action Methods
        //Get the list of all Touch Point Configuration
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ZnodeTouchPointConfiguration.ToString(), model);
            //Get Active ERP ClassName
            string erpClassName = _erpConfiguratorAgent.GetERPClassName();
            if (!string.IsNullOrEmpty(erpClassName))
            {
                //Get the list of TouchPointConfiguration.
                TouchPointConfigurationListViewModel touchPointConfigurationList = _touchPointConfigurationAgent.GetTouchPointConfigurationList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);

                //Get the grid model.
                touchPointConfigurationList.GridModel = FilterHelpers.GetDynamicGridModel(model, touchPointConfigurationList.TouchPointConfigurationList, GridListType.View_ZnodeTouchPointConfiguration.ToString(), string.Empty, null, true, true, touchPointConfigurationList?.GridModel?.FilterColumn?.ToolMenuList);
                touchPointConfigurationList.GridModel.TotalRecordCount = touchPointConfigurationList.TotalResults;
                touchPointConfigurationList.ERPClassName = erpClassName;
                return ActionView(touchPointConfigurationList);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ERPClassFailedMessage));
                return RedirectToAction<ProviderEngineController>(x => x.ERPConfiguratorList(null));
            }
        }
        //Get type method to Create new ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult Create(string ConnectorTouchPoints, string indexName = "", string schedulerCallFor = "", int portalId = 0,int catalogId=0, int catalogIndexId = 0)
        {
            if (!string.IsNullOrEmpty(ConnectorTouchPoints))
            {
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(ConnectorTouchPoints, schedulerCallFor);
                if (erpTaskSchedulerId == 0)
                {
                    ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.SetTaskSchedulerData(ConnectorTouchPoints, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);

                    return Request.IsAjaxRequest() ? ActionView("_CreateScheduler", erpTaskSchedulerViewModel) : View(AdminConstants.Create, erpTaskSchedulerViewModel);
                }
                else
                    return RedirectToAction<TouchPointConfigurationController>(x => x.Edit(erpTaskSchedulerId, indexName, schedulerCallFor, portalId,catalogId, catalogIndexId));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }
        }

        //Post type method to Create new ERP Task Scheduler.
        [HttpPost]
        public virtual ActionResult Create(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel)
        {
            bool status = true;
            erpTaskSchedulerViewModel.SchedulerType = Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex) ? ZnodeConstant.Scheduler : erpTaskSchedulerViewModel.SchedulerType;            
            if (erpTaskSchedulerViewModel.SchedulerType == ZnodeConstant.Scheduler)
                _erpTaskSchedulerAgent.CheckValidation(erpTaskSchedulerViewModel, out status);
          
            if (erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.OneTime)
            {
                ModelState.Remove("erpTaskSchedulerViewModel.CronExpression");
                ModelState.Remove("CronExpression");

            }
            if (ModelState.IsValid && status)
            {
                erpTaskSchedulerViewModel.DomainName = ZnodeAdminSettings.ZnodeApiRootUri;
                erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.Create(erpTaskSchedulerViewModel);
                if (!erpTaskSchedulerViewModel.HasError && erpTaskSchedulerViewModel.ERPTaskSchedulerId > 0)
                {
                    if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishCatalog)
                        || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.RecommendationDataGeneration) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishContentContainer))
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);

                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SchedulerCreatedSuccessfully));
                    return RedirectToAction<TouchPointConfigurationController>(x => x.Edit(erpTaskSchedulerViewModel.ERPTaskSchedulerId, erpTaskSchedulerViewModel.IndexName, erpTaskSchedulerViewModel.SchedulerCallFor, erpTaskSchedulerViewModel.PortalId, erpTaskSchedulerViewModel.CatalogId, erpTaskSchedulerViewModel.CatalogIndexId));
                }
            }
            
            erpTaskSchedulerViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();

            if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishCatalog)
                || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.RecommendationDataGeneration)|| Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishContentContainer))
                return Json(new { status = false, message = erpTaskSchedulerViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            else
                SetNotificationMessage(GetErrorNotificationMessage(erpTaskSchedulerViewModel.ErrorMessage));
            return View(erpTaskSchedulerViewModel);
        }

        //Get:Edit ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult Edit(int erpTaskSchedulerId = 0, string indexName = "", string schedulerCallFor = "", int portalId = 0,int catalogId=0, int portalIndexId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return Equals(schedulerCallFor, ZnodeConstant.ProductFeed) ? RedirectToAction<ProductFeedController>(x => x.List(null)) : action;

            if (erpTaskSchedulerId == 0)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.GetTaskSchedulerDataForUpdate(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, catalogId, portalIndexId);

            return Request.IsAjaxRequest() ? ActionView("_CreateScheduler", erpTaskSchedulerViewModel) : View(AdminConstants.Create, erpTaskSchedulerViewModel);
        }

        //Post:Edit ERPTaskScheduler.
        [HttpPost]
        public virtual ActionResult Edit(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel)
        {
            bool status = false;
            _erpTaskSchedulerAgent.CheckValidation(erpTaskSchedulerViewModel, out status);

            if(erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.OneTime)
            {
                ModelState.Remove("erpTaskSchedulerViewModel.CronExpression");
                ModelState.Remove("CronExpression");

            }
            if (ModelState.IsValid && status)
            {
                erpTaskSchedulerViewModel.DomainName = ZnodeAdminSettings.ZnodeApiRootUri;
                erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.Create(erpTaskSchedulerViewModel);
                if (!erpTaskSchedulerViewModel.HasError && erpTaskSchedulerViewModel.ERPTaskSchedulerId > 0)
                {
                    if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishCatalog)
                        || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.RecommendationDataGeneration) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishContentContainer))
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);

                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SchedulerUpdatedSuccessfully));
                    return RedirectToAction<TouchPointConfigurationController>(x => x.Edit(erpTaskSchedulerViewModel.ERPTaskSchedulerId, erpTaskSchedulerViewModel.IndexName, erpTaskSchedulerViewModel.SchedulerCallFor, erpTaskSchedulerViewModel.PortalId, erpTaskSchedulerViewModel.CatalogId, erpTaskSchedulerViewModel.CatalogIndexId));
                }
            }
            erpTaskSchedulerViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();
            
            if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishCatalog)
                || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.RecommendationDataGeneration) || Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.PublishContentContainer))
                return Json(new { status = !erpTaskSchedulerViewModel.HasError, message = erpTaskSchedulerViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            else
                SetNotificationMessage(GetErrorNotificationMessage(erpTaskSchedulerViewModel.ErrorMessage));
            return View(AdminConstants.Create, erpTaskSchedulerViewModel);
        }

        //Delete ERPTaskScheduler.
        public virtual JsonResult Delete(string erpTaskSchedulerId)
        {
            if (!string.IsNullOrEmpty(erpTaskSchedulerId))
            {
                string message = string.Empty;
                bool status = _erpTaskSchedulerAgent.Delete(erpTaskSchedulerId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Enable/disable ERP task scheduler from windows service.
        public virtual ActionResult EnableDisableScheduler(int ERPTaskSchedulerId, bool IsEnabled)
        {
            if (ERPTaskSchedulerId == 0)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorTaskSchedulerNotFound));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            string message = string.Empty;
            bool status = _erpTaskSchedulerAgent.EnableDisableTaskScheduler(ERPTaskSchedulerId, IsEnabled, out message);
            if (status && IsEnabled)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
            else if (status && !IsEnabled)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(message));

            return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
        }

        //Get:Trigger task scheduler for ERPConfigurator.
        [HttpGet]
        public virtual ActionResult Trigger(string connectorTouchPoints)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(connectorTouchPoints))
            {
                status = _touchPointConfigurationAgent.TriggerTaskScheduler(connectorTouchPoints, out message);

                SetNotificationMessage(status ? GetSuccessNotificationMessage(ERP_Resources.SuccessTriggerScheduler) : GetErrorNotificationMessage(message ?? ERP_Resources.FailedTriggerScheduler));

                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }
            SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorTriggerScheduler));
            return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
        }

        //Get the list of all Scheduler Log
        public virtual ActionResult SchedulerLogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string schedulerName, bool IsAllScheduler = true)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.TouchPointSchedulerHistory.ToString(), model);
            //Get Active ERP ClassName
            string erpClassName = _erpConfiguratorAgent.GetERPClassName();
            if (string.IsNullOrEmpty(erpClassName))
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ERPClassFailedMessage));
                return RedirectToAction<ProviderEngineController>(x => x.ERPConfiguratorList(null));
            }

            if (string.IsNullOrEmpty(schedulerName) && !IsAllScheduler)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorSchedulerNotCreated));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            TouchPointConfigurationListViewModel schedulerLogList = new TouchPointConfigurationListViewModel();
            //Get the list of Scheduler Log
            schedulerLogList = _touchPointConfigurationAgent.GetSchedulerLogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, schedulerName);

            schedulerLogList.SchedulerName = schedulerName;
            //Get the grid model.
            schedulerLogList.GridModel = FilterHelpers.GetDynamicGridModel(model, schedulerLogList.SchedulerLogList, GridListType.TouchPointSchedulerHistory.ToString(), string.Empty, null, true, true, schedulerLogList?.GridModel?.FilterColumn?.ToolMenuList);
            schedulerLogList.GridModel.TotalRecordCount = schedulerLogList.TotalResults;
            schedulerLogList.ERPClassName = erpClassName;
            return ActionView(schedulerLogList);
        }

        //Get: Scheduler Log Details partial view
        public virtual ActionResult SchedulerLogDetails(string schedulerName, string recordId)
        {
            TouchPointConfigurationViewModel model = _touchPointConfigurationAgent.SchedulerLogDetails(schedulerName, recordId);
            return ActionView(SCHEDULER_LOG_DETAILS_VIEW_PATH, model);
        }

        //Get list of unassign touchpoints.
        public virtual ActionResult GetUnAssignedTouchPointsList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get Active ERP ClassName
            string erpClassName = _erpConfiguratorAgent.GetERPClassName();
            if (!string.IsNullOrEmpty(erpClassName))
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.View_UnassignZnodeTouchPointConfiguration.ToString(), model);
                //Get the list of TouchPointConfiguration.
                TouchPointConfigurationListViewModel touchPointConfigurationList = _touchPointConfigurationAgent.GetTouchPointConfigurationList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);

                //Get the grid model.
                touchPointConfigurationList.GridModel = FilterHelpers.GetDynamicGridModel(model, touchPointConfigurationList.TouchPointConfigurationList, GridListType.View_UnassignZnodeTouchPointConfiguration.ToString(), string.Empty, null, true, true, touchPointConfigurationList?.GridModel?.FilterColumn?.ToolMenuList);
                touchPointConfigurationList.GridModel.TotalRecordCount = touchPointConfigurationList.TotalResults;
                touchPointConfigurationList.ERPClassName = erpClassName;
                return PartialView("_UnAssignedTouchPointsList", touchPointConfigurationList);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ERPClassFailedMessage));
                return RedirectToAction<ProviderEngineController>(x => x.ERPConfiguratorList(null));
            }
        }

        //Assign touch points to activated erp. 
        public virtual JsonResult AssignTouchPointToActiveERP(string touchPointNames)
        {
            if (!string.IsNullOrEmpty(touchPointNames))
            {
                ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = new ERPTaskSchedulerViewModel
                {
                    TouchPointName = touchPointNames,
                    SchedulerCallFor = ZnodeConstant.ERP,
                    IsAssignTouchPoint = true
                };

                erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.Create(erpTaskSchedulerViewModel);
                return Json(new { status = !erpTaskSchedulerViewModel.HasError, message = !erpTaskSchedulerViewModel.HasError ? Admin_Resources.AssignSuccessful : Admin_Resources.UnassignSuccessful }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult OneTime()
        {
            return PartialView(ONE_TIME_VIEW_PATH, new ERPTaskSchedulerViewModel
            {
                IsEnabled = true
            });
        }

        public virtual ActionResult Recurring()
        {
            return PartialView(RECURRING_VIEW_PATH, new ERPTaskSchedulerViewModel
            {
                IsEnabled = true
            });
        }
        #endregion
    }
}