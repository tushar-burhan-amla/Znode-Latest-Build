using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ContentContainerController : BaseController
    {

        #region Private Variables
        private readonly IContentContainerAgent _contentContainerAgent;
        private readonly IGlobalAttributeEntityAgent _globalAttributeEntityAgent;
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;
        private const string createSchedulerView = AdminConstants.CreateSchedulerView;
        #endregion

        #region Public Constructor
        public ContentContainerController(IContentContainerAgent contentContainerAgent, IGlobalAttributeEntityAgent globalAttributeEntityAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent)
        {
            _contentContainerAgent = contentContainerAgent;
            _globalAttributeEntityAgent = globalAttributeEntityAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
        }
        #endregion

        //Get the list of Content Containers.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int entityId = 0, string entityType = null)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCMSContentContainer.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSContentContainer.ToString(), model);

            //Get the list of Content Containers.
            ContentContainerListViewModel contentContainerList = _contentContainerAgent.List(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            contentContainerList.GridModel = FilterHelpers.GetDynamicGridModel(model, contentContainerList.ContentContainers, GridListType.ZnodeCMSContentContainer.ToString(), string.Empty, null, true, true, contentContainerList?.GridModel?.FilterColumn?.ToolMenuList);
            contentContainerList.GridModel.TotalRecordCount = contentContainerList.TotalResults;

            //Returns the content containers list.
            return ActionView(AdminConstants.ListView, contentContainerList);

        }

        
        //Get the List of content container variants
        public virtual ActionResult GetAssociatedVariantList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string containerKey)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCMSAssociatedVariant.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSAssociatedVariant.ToString(), model);

            //Get the List of content container variants.
            ContainerVariantListViewModel variantList = _contentContainerAgent.GetAssociatedVariantList(containerKey, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            variantList.ContainerKey = containerKey;
            //Get the grid model.
            variantList.GridModel = FilterHelpers.GetDynamicGridModel(model, variantList.ContainerVariants, GridListType.ZnodeCMSAssociatedVariant.ToString(), string.Empty, null, true, true, variantList?.GridModel?.FilterColumn?.ToolMenuList);
            variantList.GridModel.TotalRecordCount = variantList.TotalResults;

            //Returns the variants list.
            return ActionView("VariantList", variantList); 

        }

        //Create Content Container
        [HttpGet]
        public virtual ActionResult Create()
        {
            ContentContainerViewModel model = new ContentContainerViewModel();
            _contentContainerAgent.BindContentContainerModel(model);
            return ActionView(AdminConstants.Create, model);
        }

        //Create Content Container
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            if (ModelState.IsValid)
            {
                bool isRedirectToEditVariantScreen = Convert.ToBoolean(model.GetValue("IsRedirectToEditVariantScreen"));
                int defaultLocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ContentContainerViewModel contentWidgetViewModel = _contentContainerAgent.Create(model);
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.ContentContainerCreate, NotificationType.success);
                if (isRedirectToEditVariantScreen)
                    return RedirectToAction<ContentContainerController>(x => x.EditAssociatedVariant(contentWidgetViewModel.ContainerProfileVariantId, defaultLocaleId));
                return RedirectToAction<ContentContainerController>(x => x.Edit(contentWidgetViewModel.ContainerKey));
            }
           
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.ContentContainerCreateError, NotificationType.error);
            return ActionView(AdminConstants.Create, model);
        }

        //Edit Content Container
        public virtual ActionResult Edit(string ContainerKey)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.Edit, _contentContainerAgent.Edit(ContainerKey));
        }

        //Edit Content Container
        [HttpPost]
        public virtual ActionResult Edit(ContentContainerViewModel model)
        {
            ContentContainerViewModel viewModel = _contentContainerAgent.Update(model);
            TempData[AdminConstants.Notifications] = viewModel.HasError
                ? GenerateNotificationMessages(Admin_Resources.ContentContainerUpdateError, NotificationType.error)
                : GenerateNotificationMessages(Admin_Resources.ContentContainerUpdate, NotificationType.success);
            return RedirectToAction<ContentContainerController>(x => x.Edit(viewModel.ContainerKey));
        }

        //Variants Associated to the Content Container
        public virtual ActionResult GetVariants(int contentContainerId)
        {
            ContainerVariantViewModel variant = new ContainerVariantViewModel();
            variant.Portals = _contentContainerAgent.GetAvailablePortals();
            variant.ContentContainerId = contentContainerId;
            return ActionView("AssociateVariant", variant);
        }

        //Get Profiles which are added as a variant for container
        public virtual JsonResult GetUnassociatedProfiles(string containerKey)
        {
            List<SelectListItem> profile = _contentContainerAgent.GetUnassociatedProfiles(containerKey);
            return Json(new { ProfileList = profile }, JsonRequestBehavior.AllowGet);
        }

        //Associate Variant to a container
        [HttpPost]
        public virtual JsonResult AssociateVariants(ContainerVariantViewModel variant)
        {
            string message = "";
            List<ContainerVariantViewModel> variants = _contentContainerAgent.AssociateVariant(variant, out message);
            return Json(new { VariantList = variants, message= message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPut]
        public virtual JsonResult AssociateContainerTemplate(int variantId, int containerTemplateId)
        {
            return Json(new { status = _contentContainerAgent.AssociateContainerTemplate(variantId, containerTemplateId) }, JsonRequestBehavior.AllowGet);

        }

        //Get Associated Attribute Details
        public virtual ActionResult GetEntityAttributeDetails(int entityId, string entityType)
            => ActionView("ContainerAttributes", _globalAttributeEntityAgent.GetEntityAttributeDetails(entityId, entityType));

        //Save Attribute Details
        [HttpPost]
        public virtual ActionResult SaveEntityDetails([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {

            string errorMessage = string.Empty;

            EntityAttributeViewModel entityAttributeViewModel = _globalAttributeEntityAgent.SaveEntityAttributeDetails(model, out errorMessage);
            SetNotificationMessage(entityAttributeViewModel.IsSuccess ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(errorMessage));

            return RedirectToAction<ContentContainerController>(x => x.GetEntityAttributeDetails(entityAttributeViewModel.EntityValueId, entityAttributeViewModel.EntityType));
        }

        //Delete associated container variant
        public virtual JsonResult DeleteAssociatedVariant(string containerProfileVariantId, string containerKey)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(containerProfileVariantId))
            {
                bool status = _contentContainerAgent.DeleteAssociatedVariant(containerProfileVariantId, containerKey, out message);
                return Json(new { status = status, message = status ? Admin_Resources.VariantDeleteSuccess : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message }, JsonRequestBehavior.AllowGet);
        }

        //Delete Content Container
        public virtual JsonResult Delete(string contentContainerId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(contentContainerId))
            {
                bool status = _contentContainerAgent.DeleteContentContainer(contentContainerId, out message);
                return Json(new { status, message = status ? Admin_Resources.SuccessContentContainerDelete : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Verify if the container Exist
        [HttpGet]
        public virtual ActionResult IsContainerExist(string containerKey)
           => Json(new { data = _contentContainerAgent.IsContainerExist(containerKey) }, JsonRequestBehavior.AllowGet);

        //Get Associated Attribute Details
        [HttpGet]
        public virtual ActionResult GetGlobalAttributesForDefaultData(string familyCode, string entityType)
            => ActionView("ContainerAttributes", _globalAttributeEntityAgent.GetGlobalAttributesForDefaultVariantData(familyCode, entityType));

        // Get Global Attribute details on the basis of Variant id and localeid
        [HttpGet]
        public virtual ActionResult EditAssociatedVariant(int containerProfileVariantId, int localeId = 0)
       => ActionView("EditAssociatedVariant", _globalAttributeEntityAgent.GetGlobalAttributesForAssociatedVariant(containerProfileVariantId, localeId));

        // Get Global Attribute details on locale change
        [HttpGet]
        public virtual ActionResult GetAttributesDataOnLocaleChange(int variantId, int localeId = 0)
        {
            ViewBag.profileVariantId = variantId;
            GlobalAttributeEntityDetailsViewModel globalAttributeEntityDetailsViewModel = _globalAttributeEntityAgent.GetGlobalAttributesForAssociatedVariant(variantId, localeId)?.entityAttributeModel;
            string partialView = RenderRazorViewToString("ContainerAttributes", globalAttributeEntityDetailsViewModel);
            return Json(new
            {
                html = partialView,
                familyCode = globalAttributeEntityDetailsViewModel?.FamilyCode,
            }, JsonRequestBehavior.AllowGet);
        }

        //Save Variant Attribute Data
        [HttpPost]
        public virtual ActionResult SaveAssociatedVariantData([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            bool isRedirectToEditScreen = Convert.ToBoolean(model.GetValue("IsRedirectToEditScreen"));
            string errorMessage = string.Empty;
            int localeId = Convert.ToInt32(model.GetValue("LocaleId"));
            int variantId = Convert.ToInt32(model.GetValue("ProfileVariantId"));
            string ContainerKey = Convert.ToString(model.GetValue("ContainerKey"));
            if (_contentContainerAgent.SaveAssociatedVariantData(model))
            {
                EntityAttributeViewModel entityAttributeViewModel = _globalAttributeEntityAgent.SaveEntityAttributeDetails(model, out errorMessage);
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.ContentContainerUpdateStatus, NotificationType.success);
                if (isRedirectToEditScreen)
                {
                    return RedirectToAction<ContentContainerController>(x => x.Edit(ContainerKey));
                }
                return RedirectToAction<ContentContainerController>(x => x.EditAssociatedVariant(variantId, localeId));

            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.ErrorFailedToUpdate, NotificationType.error);
            return RedirectToAction<ContentContainerController>(x => x.EditAssociatedVariant(variantId, localeId));
        }

        //This method will enable or disable the container variantIds.
        public virtual ActionResult ActivateDeactivateVariant(string containerProfileVariantIds, bool isActivate)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(containerProfileVariantIds))
            {
                bool status = _contentContainerAgent.ActivateDeactivateVariant(containerProfileVariantIds, isActivate, out message);
                string successMessage = isActivate ? Admin_Resources.VariantActivatedSuccessMessage : Admin_Resources.VariantDeactivatedSuccessMessage;
                return JsonReturnType(status, status ? successMessage : message);
            }
            return JsonReturnType(false, message);
        }

        #region Publish Container
        //Publish the entire content container along with the associated variant data.
        public virtual JsonResult PublishContentContainer(string containerKey, string targetPublishState = null)
        {
            if (!string.IsNullOrEmpty(containerKey))
            {
                string errorMessage;
                bool status = _contentContainerAgent.PublishContentContainer(containerKey, out errorMessage, targetPublishState);

               return  JsonReturnType(status, status ? Admin_Resources.TextPublishedSuccessfully : errorMessage);
            }
            else
                return JsonReturnType(false, Admin_Resources.Error);
        }

        //update the container general information and then publish it along with all the associated variants data
        public virtual ActionResult UpdateAndPublishContentContainer(ContentContainerViewModel contentContainerViewModel)
        {
            string errorMessage = string.Empty;
            ContentContainerViewModel viewModel = _contentContainerAgent.Update(contentContainerViewModel);

            if (!viewModel.HasError)
                SetNotificationMessage(_contentContainerAgent.PublishContentContainer(contentContainerViewModel.ContainerKey, out errorMessage, contentContainerViewModel.TargetPublishState) ?
                    GetSuccessNotificationMessage(Admin_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));

            //redirect to edit container screen. 
            return RedirectToAction<ContentContainerController>(x => x.Edit(viewModel.ContainerKey));
        }


        //Publish the entire content container along with the associated variant data.
        public virtual JsonResult PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState = null)
        {
            if (!string.IsNullOrEmpty(containerKey) && containerProfileVariantId > 0)
            {
                string errorMessage;
                bool status = _contentContainerAgent.PublishContainerVariant(containerKey, containerProfileVariantId, out errorMessage, targetPublishState);
                return JsonReturnType(status, status ? Admin_Resources.TextPublishedSuccessfully : errorMessage);
            }
            else
                return JsonReturnType(false, Admin_Resources.Error);
        }

        //update the variant information and then publish the variants data
        public virtual ActionResult UpdateAndPublishContainerVariant([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            string errorMessage = string.Empty;
            int localeId = Convert.ToInt32(model.GetValue("LocaleId"));
            int variantId = Convert.ToInt32(model.GetValue("ProfileVariantId"));
            string containerKey = Convert.ToString(model.GetValue("ContainerKey"));
            string targetPublishState = Convert.ToString(model.GetValue("TargetPublishState"));
            if (_contentContainerAgent.SaveAssociatedVariantData(model))
            {
                EntityAttributeViewModel entityAttributeViewModel = _globalAttributeEntityAgent.SaveEntityAttributeDetails(model, out errorMessage);
                if (string.IsNullOrEmpty(errorMessage) && !entityAttributeViewModel.HasError)
                {
                    SetNotificationMessage(_contentContainerAgent.PublishContainerVariant(containerKey, variantId, out errorMessage, targetPublishState) ?
                    GetSuccessNotificationMessage(Admin_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
                    return RedirectToAction<ContentContainerController>(x => x.EditAssociatedVariant(variantId, localeId));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));
            }
            //redirect to edit variant screen. 
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.ErrorPublish, NotificationType.error);
            return RedirectToAction<ContentContainerController>(x => x.EditAssociatedVariant(variantId, localeId));
        }

        #endregion


        #region Scheduler
        //Get type method to Create new ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult CreateScheduler(string ConnectorTouchPoints, string indexName = "", string schedulerCallFor = "", int portalId = 0, int contentContainerId = 0, int contentContainerIndexId = 0)
        {
            if (!string.IsNullOrEmpty(ConnectorTouchPoints))
            {
                ConnectorTouchPoints = HttpUtility.UrlDecode(ConnectorTouchPoints);
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(ConnectorTouchPoints, schedulerCallFor);
                if (erpTaskSchedulerId == 0)
                {
                    ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.SetTaskSchedulerData(ConnectorTouchPoints, indexName, schedulerCallFor, portalId, contentContainerId, contentContainerIndexId);
                    return ActionView(createSchedulerView, erpTaskSchedulerViewModel);
                }
                else
                    return RedirectToAction<ContentContainerController>(x => x.EditScheduler(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, contentContainerId, contentContainerIndexId));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }
        }

        //Get:Edit ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult EditScheduler(int erpTaskSchedulerId = 0, string indexName = "", string schedulerCallFor = "", int portalId = 0, int contentContainerId = 0, int contentContainerIndexId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return Equals(schedulerCallFor, Libraries.ECommerce.Utilities.ZnodeConstant.PublishContentContainer) ? RedirectToAction<ContentContainerController>(x => x.List(null, 0, null)) : action;

            if (erpTaskSchedulerId == 0)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.GetTaskSchedulerDataForUpdate(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, contentContainerId, contentContainerIndexId);

            return ActionView(createSchedulerView, erpTaskSchedulerViewModel);
        }

        #endregion
    }
}
