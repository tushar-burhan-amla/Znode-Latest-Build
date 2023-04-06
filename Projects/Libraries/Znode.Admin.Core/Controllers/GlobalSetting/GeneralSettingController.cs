using System.Collections.Generic;
using System.Web.Mvc;

using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class GeneralSettingController : BaseController
    {
        #region Private Variables
        private readonly IGeneralSettingAgent _generalSettingAgent;
        private readonly IDomainAgent _domainAgent;
        private readonly string analyticsDetailsView = "AnalyticsDetails";
        private readonly string stockNoticeSettingView = "StockNoticeSettings";
        private readonly string powerBISettingView= "PowerBISettings";
        #endregion

        #region Constructor
        public GeneralSettingController(IGeneralSettingAgent generalSettingAgent, IDomainAgent domainAgent)
        {
            _generalSettingAgent = generalSettingAgent;
            _domainAgent = domainAgent;
        }
        #endregion

        #region Public Methods
        //Method to get general setting list.
        public virtual ActionResult List()
        {
            GlobalSettingViewModel model = _generalSettingAgent.List();
            List<SelectListItem> roundOffList = _generalSettingAgent.GetRoundOffList();
            List<SelectListItem> environmetList = _generalSettingAgent.GetEnvironmentsList();
            model.PriceRoundOffList = roundOffList;
            model.InventoryRoundOffList = roundOffList;
            model.EnvironmentsList = environmetList;
            return View(model);
        }

        //Action for Update General Setting.
        [HttpPost]
        public virtual ActionResult Update(GlobalSettingViewModel model)
        {
            HelperMethods.ClearCache(AdminConstants.DefaultGlobalSettingCacheKey);

            TempData[AdminConstants.Notifications] = _generalSettingAgent.Update(model)
                ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
                : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);
            return RedirectToAction<GeneralSettingController>(x => x.List());
        }

        #region Cache Management
        //Get Cache Management data
        [HttpGet]
        public virtual ActionResult GetCacheManagementData()
            => View("CacheManagement", _generalSettingAgent.CacheData());

        //Refresh Cache Data
        [HttpPost]
        public virtual ActionResult RefreshCache(int id, string domainIds = null)
        {
            CacheViewModel cacheViewModel = _generalSettingAgent.RefreshCache(new CacheViewModel() { ApplicationCacheId = id, DomainIds = domainIds });
            return Json(new
            {
                Data = cacheViewModel,
                Message = cacheViewModel.HasError ? cacheViewModel.ErrorMessage : cacheViewModel.SuccessMessage,
                HasError = cacheViewModel.HasError
            }, JsonRequestBehavior.AllowGet);
        }

        //Refresh Cache Data
        [HttpPost]
        public virtual ActionResult UpdateCacheStatus(CacheListViewModel cacheListViewModel)
        {
            string errorMessage = Admin_Resources.UpdateErrorMessage;
            SetNotificationMessage(_generalSettingAgent.UpdateCacheStatus(cacheListViewModel, ref errorMessage) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(errorMessage));
            return RedirectToAction<GeneralSettingController>(x => x.GetCacheManagementData());
        }

        //Get webstore domain list.
        public virtual ActionResult GetWebstoreDomains([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, bool isCloudflareDomains = false)
        {
            _domainAgent.SetFilters(model.Filters, ZnodeDomainEnum.ApplicationType.ToString(), FilterOperators.NotContains, ApplicationTypesEnum.Admin.ToString() + "," + ApplicationTypesEnum.API.ToString());
            if (isCloudflareDomains)
            {
                model.Filters.Add(new FilterTuple(FilterKeys.applicationType, FilterOperators.Equals, ApplicationCacheTypeEnum.CloudflareCache.ToString()));
            }
            //Get webstore domain list.
            DomainListViewModel domainList = _domainAgent.GetDomains(0, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            domainList.GridModel = FilterHelpers.GetDynamicGridModel(model, domainList.Domains, isCloudflareDomains ? GridListType.ZnodeCloudflareDomainList.ToString() : GridListType.ZnodeDomainList.ToString(), string.Empty, null, true, true, domainList.GridModel?.FilterColumn?.ToolMenuList);
            domainList.GridModel.TotalRecordCount = domainList.TotalResults;

            //Returns the domain list.
            return ActionView("WebstoreDomains", domainList);
        }

        //Get webstore domain list.
        public virtual ActionResult GetWebstoreDomainsForCloudflare([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
            => GetWebstoreDomains(model, true);

        #endregion

        #region Publish Settings

        public virtual ActionResult PublishSettings([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePublishStateApplicationTypeMapping.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePublishStateApplicationTypeMapping.ToString(), model);
            PublishStateMappingListViewModel mappingList = _generalSettingAgent.GetPublishStateMappingList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            mappingList.GridModel = FilterHelpers.GetDynamicGridModel(model, mappingList.PublishStateMappingList, GridListType.ZnodePublishStateApplicationTypeMapping.ToString(), string.Empty, null, true, true, mappingList?.GridModel?.FilterColumn?.ToolMenuList);

            //set the total record count.
            mappingList.GridModel.TotalRecordCount = mappingList.TotalResults;

            return ActionView(mappingList);
        }

        [HttpPut]
        public virtual JsonResult EnableDisablePublishStateMapping(int publishStateMappingId, bool isEnabled)
        {
            if (publishStateMappingId > 0)
            {
                string message = string.Empty;
                bool status = _generalSettingAgent.EnableDisablePublishStateMapping(publishStateMappingId, isEnabled, out message);
                if (status)
                {
                    if (isEnabled)
                        return Json(new { status = status, message = Admin_Resources.EnableMessage });
                    else
                        return Json(new { status = status, message = Admin_Resources.DisableMessage });
                }
                else
                    return Json(new { status = status, message = message });
            }
            else
                return Json(new { status = false, message = Admin_Resources.HttpCode_500_RequestValidationErrorMsg });
        }

        #endregion

        #endregion
      

        #region Configuration Settings

        //Get global configuration settings for application.
        [HttpGet]
        public virtual ActionResult GetConfigurationSettings()
        {
            return View("ConfigurationSetting", _generalSettingAgent.GetConfigurationSettings());
        }

        //Update global configuration settings for application.
        [HttpPost]
        public virtual ActionResult UpdateConfigurationSettings(ConfigurationSettingViewModel model)
        {
            SetNotificationMessage(_generalSettingAgent.UpdateConfigurationSettings(model) ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success) : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error));
            return RedirectToAction<GeneralSettingController>(x => x.GetConfigurationSettings());
        }

        #endregion

        #region Power BI

        [HttpGet]
        public virtual ActionResult GetPowerBISettings()
            => View(powerBISettingView, _generalSettingAgent.GetPowerBISettings());

        [HttpPost]
        public virtual ActionResult UpdatePowerBISettings(PowerBISettingsViewModel model)
        {
            bool isUpdated = _generalSettingAgent.UpdatePowerBISettings(model);
            TempData[AdminConstants.Notifications] = isUpdated
                ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
                : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);

            if (isUpdated)
                return View(powerBISettingView, model);
            else
                return RedirectToAction<GeneralSettingController>(x => x.GetPowerBISettings());
        }
        #endregion

        //Get Date format from global setting 
        public virtual ActionResult GetDateFormatGlobalSetting()
        {
            return Json(new { dateFormat = HelperMethods.GetStringDateFormat() }, JsonRequestBehavior.AllowGet);
        }

        //Get time format from global setting
        public virtual ActionResult GetTimeFormatGlobalSetting()
        {
            return Json(new { timeFormat = HelperMethods.GetStringTimeFormat() }, JsonRequestBehavior.AllowGet);
        }

        #region Stock Notice Settings

        // Get stock notice settings
        [HttpGet]
        public virtual ActionResult GetStockNoticeSettings()
            => View(stockNoticeSettingView, _generalSettingAgent.GetStockNoticeSettings());

        // Update stock notice settings
        [HttpPost]
        public virtual ActionResult UpdateStockNoticeSettings(StockNoticeSettingsViewModel model)
        {
            bool isUpdated = _generalSettingAgent.UpdateStockNoticeSettings(model);
            TempData[AdminConstants.Notifications] = isUpdated
                ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
                : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);

            if (isUpdated)
                return View(stockNoticeSettingView, model);
            else
                return RedirectToAction<GeneralSettingController>(x => x.GetStockNoticeSettings());
        }

        #endregion

    }
}