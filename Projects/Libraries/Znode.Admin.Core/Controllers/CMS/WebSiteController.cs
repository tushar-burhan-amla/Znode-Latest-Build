using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using Znode.Admin.Core.Controllers;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class WebSiteController: BaseController
    {
        #region Private Readonly members
        private readonly IWebSiteAgent _websiteAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly ISliderAgent _sliderAgent;
        private readonly ILocaleAgent _localeAgent;
        private readonly IContentContainerAgent _contentContainerAgent;
        private readonly IEmailTemplateAgent _emailTemplateAgent;
        private const string CreateEditSlider = "_CreateSlider";
        private const string ManagePartialView = "_ManagePartial";
        private const string CreateEditBanner = "_CreateEditBanner";
        private const string ManageSliderBannerView = "_ManageSliderBanner";
        private const string PartialLinkWidgetCreateEditView = "_PartialLinkWidgetCreateEdit";
        private const string createEditLinkWidgetConfiguration = "_CreateEditLinkWidgetConfiguration";
        private readonly string FormWidgetEmail = "FormWidgetEmailConfiguration";
        private readonly string EmailTemplateListPanel = "_emailTemplateListPanel";
        private readonly string ManageFormWidget = "ManageFormWidget";
        private const string ManageContentContainerView = "_ManageContentContainer";
        #endregion

        #region Public Constructor
        public WebSiteController(IWebSiteAgent websiteAgent, ISliderAgent sliderAgent, IEmailTemplateAgent emailTemplateAgent, ILocaleAgent localeAgent, IStoreAgent storeAgent, IContentContainerAgent contentContainerAgent)
        {
            _websiteAgent = websiteAgent;
            _storeAgent = storeAgent;
            _sliderAgent = sliderAgent;
            _localeAgent = localeAgent;
            _emailTemplateAgent = emailTemplateAgent;
            _contentContainerAgent = contentContainerAgent;
        }
        #endregion

        #region Public Methods

        #region Web Site Logo

        //Get Portal associated widgets.
        public virtual ActionResult GetPortalAssociatedWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName)
         => PartialView(AdminConstants.WidgetList, _websiteAgent.GetPortalAssociatedWidgets(cmsMappingId, typeOfMapping, templatePath, displayName, fileName));

        #endregion

        #region Slider
        //Get the list of slider.
        public virtual ActionResult GetSliderList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCMSSlider.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSSlider.ToString(), model);
            SliderListViewModel sliderList = _sliderAgent.GetSliders(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            sliderList.GridModel = FilterHelpers.GetDynamicGridModel(model, sliderList.Sliders, GridListType.ZnodeCMSSlider.ToString(), string.Empty, null, true, true, sliderList?.GridModel?.FilterColumn?.ToolMenuList);
            //set the total record count
            sliderList.GridModel.TotalRecordCount = sliderList.TotalResults;

            //returns the view
            return ActionView("_GetSliderList", sliderList);
        }

        //Method return a View to add new slider.
        [HttpGet]
        public virtual ActionResult CreateSlider()
            => PartialView(CreateEditSlider);

        //Method to add a new slider, having parameter SliderViewModel contains slider name.
        [HttpPost]
        public virtual ActionResult CreateSlider(SliderViewModel sliderViewModel)
        {
            sliderViewModel = _sliderAgent.CreateSlider(sliderViewModel);
            if(sliderViewModel.HasError)
                SetNotificationMessage(GetErrorNotificationMessage(sliderViewModel.ErrorMessage));
            else
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));

            return Json(new {
                isSuccess = sliderViewModel.CMSSliderId < 1 ? false : true,
                cmsSliderId = sliderViewModel?.CMSSliderId,
                name = sliderViewModel?.Name,
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult EditSlider(int cmsSliderId, string data)
        {
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                SliderViewModel sliderViewModel = _sliderAgent.UpdateSlider(cmsSliderId, data);
                if(!sliderViewModel.HasError)
                    return Json(new {
                        status = true, message = Admin_Resources.UpdateMessage
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new {
                        status = false, message = sliderViewModel.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = PIM_Resources.UpdateErrorMessage
            }, JsonRequestBehavior.AllowGet);

        }

        //Delete slider.
        public virtual JsonResult DeleteSlider(string cmsSliderId)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderId))
            {
                status = _sliderAgent.DeleteSlider(cmsSliderId, out message);

                return Json(new {
                    status = status, message = status ? Admin_Resources.DeleteMessage : message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = Admin_Resources.DeleteErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        //Publish particular Slider
        public virtual ActionResult PublishSlider(string cmsSliderId)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderId))
            {
                status = _sliderAgent.PublishSlider(cmsSliderId, out message);

                return Json(new {
                    status = status, message = status ? Admin_Resources.TextPublishedSuccessfully : message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = PIM_Resources.ErrorPublished
            }, JsonRequestBehavior.AllowGet);
        }

        //Publish particular slider with consideration to new preview functionality.
        public virtual ActionResult PublishSliderWithPreview(string cmsSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderId))
            {
                status = _sliderAgent.PublishSlider(cmsSliderId, portalId, localeId, out message, targetPublishState, takeFromDraftFirst);

                return Json(new {
                    status = status, message = status ? Admin_Resources.TextPublishedSuccessfully : message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = PIM_Resources.ErrorPublished
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UpdateAndPublishSliderWithPreview(string cmsSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderId))
            {
                status = _sliderAgent.PublishSlider(cmsSliderId, portalId, localeId, out message, targetPublishState, takeFromDraftFirst);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(message));
                return RedirectToAction<WebSiteController>(x => x.GetSliderList(null));
            } else
                SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorPublished));
            return RedirectToAction<WebSiteController>(x => x.GetSliderList(null));
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        public virtual ActionResult UpdateAndPublishSlider(string cmsSliderId)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderId))
            {
                status = _sliderAgent.PublishSlider(cmsSliderId, out message);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(message));
                return RedirectToAction<WebSiteController>(x => x.GetSliderList(null));
            } else
                SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorPublished));
            return RedirectToAction<WebSiteController>(x => x.GetSliderList(null));
        }

        //Check Slider name already exists or not.
        public virtual JsonResult IsSliderNameExist(string Name, int CMSSliderId = 0)
          => Json(!_sliderAgent.CheckSliderNameExist(Name, CMSSliderId), JsonRequestBehavior.AllowGet);
        #endregion

        #region Banners
        //Get banner list for selected slider..
        public virtual ActionResult GetBannerList(int cmsSliderId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetCMSSliderBannerPath.ToString(), model);
            _websiteAgent.SetFilter(model.Filters, cmsSliderId, ZnodeCMSSliderBannerEnum.CMSSliderId.ToString());
            _websiteAgent.SetFilter(model.Filters, Convert.ToInt32(DefaultSettingHelper.DefaultLocale), ZnodeLocaleEnum.LocaleId.ToString());
            BannerListViewModel bannerList = _sliderAgent.GetBannerList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            bannerList.CMSSliderId = cmsSliderId;

            bannerList.GridModel = FilterHelpers.GetDynamicGridModel(model, bannerList?.Banners, GridListType.View_GetCMSSliderBannerPath.ToString(), string.Empty, null, true, true, bannerList?.GridModel?.FilterColumn?.ToolMenuList);
            bannerList.GridModel.TotalRecordCount = bannerList.TotalResults;

            return ActionView("_GetBannerList", bannerList);
        }

        //Get:Create banner.
        public virtual ActionResult CreateBanner(int cmsSliderId)
        {
            BannerViewModel bannerViewModel = new BannerViewModel() { CMSSliderId = cmsSliderId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) };
            bannerViewModel.Locales = _localeAgent.GetLocalesList(bannerViewModel.LocaleId);
            return View(CreateEditBanner, bannerViewModel);
        }

        //Post:Create banner.
        [HttpPost]
        public virtual ActionResult CreateBanner(BannerViewModel bannerViewModel)
        {
            if(ModelState.IsValid)
            {
                BannerViewModel viewModel = _sliderAgent.CreateBanner(bannerViewModel);

                if(!viewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<WebSiteController>(x => x.EditBanner(viewModel.CMSSliderBannerId, viewModel.LocaleId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(bannerViewModel.ErrorMessage));
            bannerViewModel.Locales = _localeAgent.GetLocalesList(bannerViewModel.LocaleId);
            return View(CreateEditBanner, bannerViewModel);
        }

        //Get:Edit banner.
        [HttpGet]
        public virtual ActionResult EditBanner(int cmsSliderBannerId, int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if(action != null)
                return action;
            return ActionView(CreateEditBanner, _sliderAgent.GetBanner(cmsSliderBannerId, localeId));
        }
        //Post:Edit banner.
        [HttpPost]
        public virtual ActionResult EditBanner(BannerViewModel bannerViewModel)
        {
            if(ModelState.IsValid)
            {
                SetNotificationMessage(_sliderAgent.UpdateBanner(bannerViewModel).HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<WebSiteController>(x => x.EditBanner(bannerViewModel.CMSSliderBannerId, bannerViewModel.LocaleId));
            }
            return View(CreateEditBanner, bannerViewModel);
        }

        //Inline edit and update the banner sequence for respective banner
        public virtual ActionResult EditBannerSequence(int cmsSliderBannerId, string data)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                BannerViewModel bannerViewModel = _sliderAgent.UpdateBannerSequence(cmsSliderBannerId, data);
                if (!bannerViewModel.HasError)
                    return Json(new
                    {
                        status = true,
                        message = Admin_Resources.UpdateMessage
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new
                    {
                        status = false,
                        message = bannerViewModel.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = false,
                message = PIM_Resources.UpdateErrorMessage
            }, JsonRequestBehavior.AllowGet);

        }

        //Delete banner.
        public virtual JsonResult DeleteBanner(string cmsSliderBannerId)
        {
            string message = string.Empty;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsSliderBannerId))
            {
                status = _sliderAgent.DeleteBanner(cmsSliderBannerId);
                message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            }
            return Json(new {
                status = status, message = message
            }, JsonRequestBehavior.AllowGet);
        }

        //Check Banner Name Already exists or not.
        [HttpPost]
        public virtual JsonResult IsBannerNameExist(string Title, int CMSSliderBannerId = 0, int CMSSliderId = 0)
        => Json(!_sliderAgent.CheckBannerNameExist(Title, CMSSliderBannerId, CMSSliderId), JsonRequestBehavior.AllowGet);

        #endregion

        #region Link Widget Configuration.
        //Get Link Widget Configuration list.
        public virtual ActionResult GetLinkWidgetConfigurationList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsMappingId, int cmsWidgetsId, string widgetsKey, string typeOFMapping, string displayName = "", string widgetName = "", string fileName = "", int localeId = 0)
        {
            LinkWidgetDataViewModel linkWidgetDataModel = _websiteAgent.GetLinkWidgetConfigurationList(model, cmsMappingId, cmsWidgetsId, widgetsKey, typeOFMapping, localeId);
            linkWidgetDataModel.DisplayName = HttpUtility.UrlDecode(displayName);
            linkWidgetDataModel.WidgetName = HttpUtility.UrlDecode(widgetName);
            linkWidgetDataModel.FileName = HttpUtility.UrlDecode(fileName);
            return ActionView("_ManageLinkWidgetConfiguration", linkWidgetDataModel);
        }

        //Save Link Widget Configuration data.
        [HttpPost]
        public virtual JsonResult CreateUpdateLinkWidgetConfiguration(LinkWidgetDataViewModel viewModel)
        {
            string message = string.Empty;
            bool status = false;
            string partialView = string.Empty;

            //Save Link Widget Configuration details.
            LinkWidgetDataViewModel model = _websiteAgent.CreateUpdateLinkWidgetConfiguration(viewModel, out message);
            if(HelperUtility.IsNotNull(model) && model.CMSWidgetTitleConfigurationId > 0)
            {
                status = true;
                viewModel.CMSWidgetTitleConfigurationId = model.CMSWidgetTitleConfigurationId;
                LinkWidgetConfigurationViewModel linkModel = _websiteAgent.GetLinkWidgetConfigurationDetailById((int)model.CMSWidgetTitleConfigurationId, viewModel.LocaleId);
                if(HelperUtility.IsNotNull(linkModel))
                    partialView = RenderRazorViewToString(PartialLinkWidgetCreateEditView, linkModel);
            }

            return Json(new {
                status = status,
                html = partialView,
                message= Equals(viewModel.TypeOfMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString())?
                (status ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                : string.IsNullOrEmpty(message) ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage) : message):
                (status ? (_websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage) : message),
                cmsWidgetTitleConfigurationId = viewModel.CMSWidgetTitleConfigurationId
            }, JsonRequestBehavior.AllowGet);
        }

        //Delete Get Link Widget Configuration.
        public virtual JsonResult DeleteLinkWidgetConfiguration(string cmsWidgetTitleConfigurationId, int localeId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if(!string.IsNullOrEmpty(cmsWidgetTitleConfigurationId))
            {
                status = _websiteAgent.DeleteLinkWidgetConfiguration(cmsWidgetTitleConfigurationId, localeId, out message);

                if(status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new {
                status = status, message = message
            }, JsonRequestBehavior.AllowGet);
        }

        //Add New Link widget configuration.
        [HttpGet]
        public virtual ActionResult AddNewLinkWidgetConfiguration(int cmsMappingId, int cmsWidgetsId, string widgetsKey, string typeOFMapping, int? localeId, string displayName = "", string widgetName = "", string fileName = "")
        {
            LinkWidgetConfigurationViewModel linkWidgetDataModel = new LinkWidgetConfigurationViewModel();
            linkWidgetDataModel.CMSMappingId = cmsMappingId;
            linkWidgetDataModel.CMSWidgetsId = cmsWidgetsId;
            linkWidgetDataModel.WidgetsKey = widgetsKey;
            linkWidgetDataModel.TypeOfMapping = typeOFMapping;
            linkWidgetDataModel.DisplayName = displayName;
            linkWidgetDataModel.WidgetName = widgetName;
            linkWidgetDataModel.FileName = fileName;
            linkWidgetDataModel.LocaleId = localeId;
            //Convert View Result into the String.
            string partialView = RenderRazorViewToString(PartialLinkWidgetCreateEditView, linkWidgetDataModel);
            return Json(new {
                html = partialView
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CMS Widget Slider Banner
        //Get the Widget Configuration details.
        public virtual ActionResult ManageCMSWidgetSliderBanner(int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOFMapping, string displayName = "", string widgetName = "", string fileName = "", string widgetCode = "")
        {
            ActionResult action = GotoBackURL();
            if(HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }

            CMSWidgetConfigurationViewModel model = _websiteAgent.GetCMSWidgetSliderBanner(cmsMappingId, cmsWidgetsId, widgetKey, typeOFMapping, null);
            model.DisplayName = HttpUtility.UrlDecode(displayName);
            model.WidgetName = HttpUtility.UrlDecode(widgetName);
            model.FileName = HttpUtility.UrlDecode(fileName);
            model.WidgetCode = widgetCode;
            return ActionView(ManageSliderBannerView, model);
        }

        [HttpPost]
        //Save the Web Site Widget Configuration details.
        public virtual ActionResult SaveCMSWidgetSliderBanner(CMSWidgetConfigurationViewModel model)
        {
            //Update hidden value.
            ModelState.Remove(AdminConstants.JobIndicator);
            if(ModelState.IsValid)
            {
                if(Equals(model?.WidgetCode, AdminConstants.OfferBanner))
                {
                    model.Type = null;
                    model.Navigation = null;
                    model.TransactionStyle = null;
                }
                CMSWidgetConfigurationViewModel viewModel = _websiteAgent.SaveCMSWidgetSliderBanner(model);
                if(viewModel.HasError)
                    SetNotificationMessage(GetErrorNotificationMessage(model.CMSWidgetSliderBannerId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage));
                else
                {
                    if(Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage));
                        return RedirectToAction<WebSiteController>(x => x.ManageCMSWidgetSliderBanner(model.CMSMappingId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetName, model.FileName, model.WidgetCode));
                    }
                    else
                    {
                        string message = _websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage;
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSMappingId, "Widgets", 0, false, string.Empty));
                    }                    
                }
            }
            return ActionView(ManageSliderBannerView, model);
        }
        #endregion

        #region Product Page
        //Get the list of portal page product associated to selected store in website configuration.
        public virtual ActionResult GetPortalProductPage(int portalId = 0, string portalName = null)
        => ActionView("_PortalProductPage", _websiteAgent.GetPortalProductPage(new PortalProductPageViewModel() { PortalId = portalId, PortalName = portalName }));

        //Assign new pdp template to product type.
        public virtual ActionResult UpdatePortalProductPage(PortalProductPageViewModel portalProductPageViewModel)
        {
            if(ModelState.IsValid)
            {
                //Save the updated datails of portal product page.
                bool isUpdated = _websiteAgent.UpdatePortalProductPage(portalProductPageViewModel);
                string message = isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;
                return Json(new {
                    status = isUpdated, message = message
                }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction<WebSiteController>(x => x.GetPortalProductPage(portalProductPageViewModel.PortalId, portalProductPageViewModel.PortalName));
        }
        #endregion

        #region Form Widget Configuration

        //Get Form Widget Configuration details.
        public virtual ActionResult ManageFormWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, string displayName = "", string widgetName = "", string fileName = "", int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if(HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }
            //Get Form Widget Configuration details.
            CMSFormWidgetConfigurationViewModel model = _websiteAgent.GetFormWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, localeId);
            model.DisplayName = HttpUtility.UrlDecode(displayName);
            model.WidgetName = HttpUtility.UrlDecode(widgetName);
            model.FileName = HttpUtility.UrlDecode(fileName);
            return ActionView("ManageFormWidget", model);

        }

        //Save Form Widget Configuration details.
        [HttpPost]
        public virtual ActionResult SaveFormWidgetConfiguration(CMSFormWidgetConfigurationViewModel model)
        {
            bool status = true;
            string message = string.Empty;
            CMSFormWidgetConfigurationViewModel viewModel = null;
            if(ModelState.IsValid)
            {
                //Save the Form Widget Configuration details.
                viewModel = _websiteAgent.SaveFormWidgetConfiguration(model);
                if(viewModel.HasError)
                    status = false;

                message = status ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                                       : (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
                if(status)
                {
                    if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<WebSiteController>(x => x.ManageFormWidgetConfiguration(model.CMSMappingId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetName, model.FileName, model.LocaleId));
                    }
                    else
                    {
                        message = _websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage;
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSMappingId, "Widgets", 0, false, string.Empty));
                    }                      
                } else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
           
                return Json(new {
                status = status,
                message = message
               }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Text Widget Configuration
        //Get Text Widget Configuration details.
        public virtual ActionResult ManageTextWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, string displayName = "", string widgetName = "", string fileName = "", int localeId = 0)
        {
            ActionResult action = GotoBackURL();

            if(HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }
            //Get Text Widget Configuration details.
            CMSTextWidgetConfigurationViewModel model = _websiteAgent.GetTextWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, localeId);
            model.DisplayName = HttpUtility.UrlDecode(displayName);
            model.WidgetName = HttpUtility.UrlDecode(widgetName);
            model.FileName = HttpUtility.UrlDecode(fileName);
            return ActionView("ManageTextWidget", model);
        }

        public virtual ActionResult ErrorOnWidget()
        {
            return ActionView("ErrorOnWidget", new CMSTextWidgetConfigurationViewModel());
        }

        //Save Text Widget Configuration details.
        [HttpPost]
        public virtual ActionResult SaveTextWidgetConfiguration(CMSTextWidgetConfigurationViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = true;

                //Save the Text Widget Configuration details.
                CMSTextWidgetConfigurationViewModel viewModel = _websiteAgent.SaveTextWidgetConfiguration(model);
                if (viewModel.HasError)
                    status = false;

                string message = status ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                                        : (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
                if (status)
                {
                    if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<WebSiteController>(x => x.ManageTextWidgetConfiguration(model.CMSMappingId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetName, model.FileName, model.LocaleId));
                    }
                    else
                    {
                        message = _websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage;
                        SetNotificationMessage(GetSuccessNotificationMessage(message));

                        return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSMappingId, model.FileName, model.LocaleId, false, string.Empty));
                    }
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return ActionView("ManageTextWidget", model);
        }
        #endregion

        #region Media Widget Configuration
        //Update Media Widget Details
        public virtual ActionResult ManageMediaWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, string displayName = "", string widgetName = "", string fileName = "", int MediaId = 0)
        {
            //Get Text Widget Configuration details.
            CMSMediaWidgetConfigurationViewModel model = _websiteAgent.SaveAndUpdateMediaWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, MediaId);
            if (HelperUtility.IsNull(model) || model.HasError)
                return Json(new { status = false, message = Admin_Resources.ErrorFailedToUpdate}, JsonRequestBehavior.AllowGet);
            else
            {
                string message = _websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.SuccessUpdate;               
                return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Remove Widget Configuration Data

        public virtual ActionResult RemoveWidgetDataFromContentPage(int mappingId, string widgetKey, string widgetCode, string typeOfMapping)
        {
            string message = string.Empty;
            bool status = false;

            status= _websiteAgent.RemoveWidgetDataFromContentPage(mappingId, widgetKey, widgetCode, out message);
            if (status)
                message = status ? ((_websiteAgent.IsPreviewGloballyEnabled() && _websiteAgent.IsPreviewPerform(typeOfMapping)) ? Admin_Resources.SuccessPublish : Admin_Resources.SuccessUpdate) : String.IsNullOrEmpty(message)? message : Admin_Resources.DeleteErrorMessage;

            return Json(new { status = status, message =  message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Category association
        //Get associated categories based on cms widgets.
        public virtual ActionResult GetAssociatedCategoryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName = "")
        {

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSWidgetCategory.ToString(), model);
            CategoryListViewModel list = _websiteAgent.GetAssociatedCategoryList(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName, model);
            list.FileName = HttpUtility.UrlDecode(fileName);

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Categories, GridListType.ZnodeCMSWidgetCategory.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_AssociatedCategoryList", list);
        }

        //Get unassociated categories.
        public virtual ActionResult GetUnAssociatedCategoryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociateCategoryToSpecialOffer.ToString(), model);
            CategoryListViewModel list = _websiteAgent.GetUnAssociatedCategoryList(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName, model);
            list.FileName = HttpUtility.UrlDecode(fileName);

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Categories, GridListType.AssociateCategoryToSpecialOffer.ToString(), string.Empty, null, true);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_UnAssociatedCategoryList", list);
        }

        //Unassociate categories from associated category list.
        public virtual JsonResult RemoveAssociatedCategories(string cmsWidgetCategoryId)
        {
            if(!string.IsNullOrEmpty(cmsWidgetCategoryId))
            {
                bool status = _websiteAgent.RemoveAssociatedCategories(cmsWidgetCategoryId);
                return Json(new {
                    status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.UnassignError
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = Admin_Resources.UnassignError
            }, JsonRequestBehavior.AllowGet);
        }

        //Associate categories.
        public virtual JsonResult AssociateCategories(string categoryCodes, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping)
        {
            SetNotificationMessage(_websiteAgent.AssociateCategories(cmsWidgetsId, categoryCodes, cmsMappingId, widgetKey, typeOFMapping)
               ? GetSuccessNotificationMessage(Admin_Resources.AssociatedSuccessMessage)
               : GetErrorNotificationMessage(Admin_Resources.AssociatedErrorMessage));
            return Json(JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult EditCMSWidgetCategory(int cmsWidgetCategoryId, string data)
        {
            CategoryViewModel categoryViewModel = JsonConvert.DeserializeObject<CategoryViewModel[]>(data)[0];

            bool status = false;
            string message = string.Empty;
            if(ModelState.IsValid)
                status = _websiteAgent.EditCMSWidgetCategory(
                    new CategoryViewModel {
                        CMSWidgetCategoryId = cmsWidgetCategoryId,

                        DisplayOrder = categoryViewModel.DisplayOrder
                    });

            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new {
                status = status, message = message, cmsWidgetCategoryId = categoryViewModel.CMSWidgetCategoryId
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region CMSWidgetProduct 
        //Get associated product list .
        public virtual ActionResult GetAssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName = "")
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedCMSOfferPageProduct.ToString(), model);
            _websiteAgent.SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);
            CMSWidgetProductListViewModel cmsWidgetProductListViewModel = _websiteAgent.GetAssociatedProductList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            cmsWidgetProductListViewModel.CMSWidgetsId = cmsWidgetsId;
            cmsWidgetProductListViewModel.WidgetsKey = widgetKey;
            cmsWidgetProductListViewModel.CMSMappingId = cmsMappingId;
            cmsWidgetProductListViewModel.TypeOfMapping = typeOfMapping;
            cmsWidgetProductListViewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            cmsWidgetProductListViewModel.WidgetName = HttpUtility.UrlDecode(widgetName);
            cmsWidgetProductListViewModel.FileName = HttpUtility.UrlDecode(fileName);

            cmsWidgetProductListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, cmsWidgetProductListViewModel?.CMSWidgetProducts, GridListType.AssociatedCMSOfferPageProduct.ToString(), string.Empty, null, true, true, cmsWidgetProductListViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            cmsWidgetProductListViewModel.GridModel.TotalRecordCount = cmsWidgetProductListViewModel.TotalResults;

            return ActionView("_AssociatedProductList", cmsWidgetProductListViewModel);
        }

        //Get unassociated product list.
        public virtual ActionResult GetUnAssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName = "")
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCMSOfferPageProduct.ToString(), model);
            _websiteAgent.SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);
            ProductDetailsListViewModel productList = _websiteAgent.GetUnAssociatedProductList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            productList.CMSWidgetsId = cmsWidgetsId;
            productList.WidgetsKey = widgetKey;
            productList.CMSMappingId = cmsMappingId;
            productList.TypeOfMapping = typeOfMapping;
            productList.DisplayName = HttpUtility.UrlDecode(displayName);
            productList.WidgetName = HttpUtility.UrlDecode(widgetName);
            productList.FileName = HttpUtility.UrlDecode(fileName);
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList?.ProductDetailList, GridListType.UnAssociatedCMSOfferPageProduct.ToString(), string.Empty, null, true);
            productList.GridModel.TotalRecordCount = productList.TotalResults;
            return ActionView("_UnAssociatedProductList", productList);
        }

        //Associate product .
        public virtual JsonResult AssociateProduct(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string SKUs)
        {
            string message = string.Empty;
            bool status = _websiteAgent.AssociateProduct(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, SKUs);
            message = status ? (_websiteAgent.IsPreviewGloballyEnabled() && _websiteAgent.IsPreviewPerform(typeOfMapping) ? Admin_Resources.SuccessPublish : Admin_Resources.AssociateProductSuccessMessage) : Admin_Resources.ErrorAssociateProductMessage;
            if(status)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(message));
            }
            return Json(new
            {
                status = status,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        //Unassociate associated products.
        public virtual JsonResult UnAssociateProduct(string cmsWidgetProductId)
        {
            if (!string.IsNullOrEmpty(cmsWidgetProductId))
            {
                
                bool status = _websiteAgent.UnassociateProduct(cmsWidgetProductId);
                string message = status ? (_websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.DeleteMessage : Admin_Resources.UnassignSuccessful) : Admin_Resources.ErrorFailedToDelete;

                if (status)
                {                    
                    SetNotificationMessage(GetSuccessNotificationMessage(message));
                }                
                return Json(new
                {
                    status = status,
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = false,
                message = Admin_Resources.ErrorFailedToDelete
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult EditCMSAssociateProduct(int cmsWidgetProductId, string data)
        {
            ProductDetailsViewModel productDetailsViewModel = JsonConvert.DeserializeObject<ProductDetailsViewModel[]>(data)[0];

            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
                status = _websiteAgent.EditCMSAssociateProduct(
                    new ProductDetailsViewModel
                    {
                        CMSWidgetProductId = cmsWidgetProductId,

                        DisplayOrder = productDetailsViewModel.DisplayOrder
                    });

            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new
            {
                status = status,
                message = message,
                cmsWidgetProductId = productDetailsViewModel.CMSWidgetProductId
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Brand association
        //Get associated categories based on cms widgets.
        public virtual ActionResult GetAssociatedBrandList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName = "")
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSWidgetBrand.ToString(), model);
            BrandListViewModel list = _websiteAgent.GetAssociatedBrandList(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName, model);
            list.FileName = HttpUtility.UrlDecode(fileName);

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Brands, GridListType.ZnodeCMSWidgetBrand.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_AssociatedBrandList", list);
        }

        //Get unassociated categories.
        public virtual ActionResult GetUnAssociatedBrandList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, string fileName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociateBrandToSpecialOffer.ToString(), model);
            BrandListViewModel list = _websiteAgent.GetUnAssociatedBrandList(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName, model);
            list.FileName = HttpUtility.UrlDecode(fileName);

            //Get the grid model.
            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.Brands, GridListType.AssociateBrandToSpecialOffer.ToString(), string.Empty, null, true);

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("_UnAssociatedBrandList", list);
        }

        //Unassociate brands from associated brand list.
        public virtual JsonResult RemoveAssociatedBrands(string cmsWidgetBrandId)
        {
            if(!string.IsNullOrEmpty(cmsWidgetBrandId))
            {
                bool status = _websiteAgent.RemoveAssociatedBrands(cmsWidgetBrandId);
                return Json(new {
                    status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.UnassignError
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                status = false, message = Admin_Resources.UnassignError
            }, JsonRequestBehavior.AllowGet);
        }


        public virtual ActionResult EditCMSWidgetBrand(int cmsWidgetBrandId, string data)
        {
            BrandViewModel brandViewModel = JsonConvert.DeserializeObject<BrandViewModel[]>(data)[0];

            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
                status = _websiteAgent.EditCMSWidgetBrand(
                    new BrandViewModel
                    {
                        CMSWidgetBrandId = cmsWidgetBrandId,

                        DisplayOrder = brandViewModel.DisplayOrder
                    });

            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new
            {
                status = status,
                message = message,
                cmsWidgetBrandId = brandViewModel.CMSWidgetBrandId
            }, JsonRequestBehavior.AllowGet);

        }



        //Associate brands.
        public virtual JsonResult AssociateBrands(string brandId, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping)
        {
            SetNotificationMessage(_websiteAgent.AssociateBrands(cmsWidgetsId, brandId, cmsMappingId, widgetKey, typeOFMapping)
               ? GetSuccessNotificationMessage(Admin_Resources.AssociatedSuccessMessage)
               : GetErrorNotificationMessage(Admin_Resources.AssociatedErrorMessage));
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Form Widget Email Notification

        //Get email template list.
        public virtual ActionResult EmailTemplateList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string checkEmailType = "")
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeFormWidgetEmailTemplate.ToString(), model);
            //Get list of email template.
            EmailTemplateListViewModel emailTemplateList = _emailTemplateAgent.EmailTemplates(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            emailTemplateList.CheckEmailType = checkEmailType;
            //Get the grid model.
            emailTemplateList.GridModel = FilterHelpers.GetDynamicGridModel(model, emailTemplateList.EmailTemplateList, GridListType.ZnodeFormWidgetEmailTemplate.ToString(), string.Empty, null, true);
            emailTemplateList.GridModel.TotalRecordCount = emailTemplateList.TotalResults;
            return ActionView(EmailTemplateListPanel, emailTemplateList);
        }

        //Get Form Widget Email Configuration
        public virtual ActionResult FormWidgetEmailConfiguration(int cmsMappingId, int widgetId, string widgetKey, string mappingType, string displayName = "", string widgetName = "", string fileName = "", int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if(HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }
            //Get Form Widget Configuration details.
            FormWidgetEmailConfigurationViewModel formWidgetEmailConfigurationViewModel = _websiteAgent.GetFormWidgetEmailConfiguration(cmsMappingId, widgetId, widgetKey, mappingType, localeId);
            if(cmsMappingId > 0)
            {
                formWidgetEmailConfigurationViewModel.CMSContentPagesId = cmsMappingId;
                formWidgetEmailConfigurationViewModel.CMSWidgetsId = widgetId;
                formWidgetEmailConfigurationViewModel.WidgetsKey = widgetKey;
                formWidgetEmailConfigurationViewModel.TypeOFMapping = mappingType;
                formWidgetEmailConfigurationViewModel.DisplayName = displayName;
            }
            return ActionView(FormWidgetEmail, formWidgetEmailConfigurationViewModel);
        }

        //Save Form Widget Email Configuration details.
        [HttpPost]
        public virtual ActionResult FormWidgetEmailConfiguration(FormWidgetEmailConfigurationViewModel model)
        {
            FormWidgetEmailConfigurationViewModel viewModel = null;
            bool status = true;
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(model.NotificationEmailId) || !string.IsNullOrEmpty(model.AcknowledgementEmailTemplate))
                {
                    //Save the Form Widget Configuration details.
                    viewModel = _websiteAgent.SaveFormWidgetEmailConfiguration(model);
                    if(viewModel.HasError)
                        status = false;
                    message = status ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                                           : (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
                    if(status)
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                            return RedirectToAction<WebSiteController>(x => x.FormWidgetEmailConfiguration(model.CMSContentPagesId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetsKey, string.Empty, model.LocaleId));
                        else
                        {
                            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.CMSNotificationMessageForContentPage));
                            return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSContentPagesId, "Widgets", 0, false, string.Empty));
                        }
                    }
                    SetNotificationMessage(GetErrorNotificationMessage(message));
                    if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                        return RedirectToAction<WebSiteController>(x => x.FormWidgetEmailConfiguration(model.CMSContentPagesId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetsKey, string.Empty, model.LocaleId));
                }
                else
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordSavedSuccessMessage));
                    if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                        return RedirectToAction<WebSiteController>(x => x.FormWidgetEmailConfiguration(model.CMSContentPagesId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetsKey, string.Empty, model.LocaleId));
                    else
                    { 
                      SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.CMSNotificationMessageForContentPage));
                      return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSContentPagesId, "Widgets", 0, false, string.Empty));   
                     }
                }
            }
            if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                return RedirectToAction<WebSiteController>(x => x.FormWidgetEmailConfiguration(model.CMSContentPagesId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetsKey, string.Empty, model.LocaleId));
            else
                return Json(new
                {
                    status = status,
                    message = message,
                    CMSContentPagesId = model.CMSContentPagesId
                }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CMS Widget Content Container
        //Get the Widget Configuration details.
        public virtual ActionResult ManageCMSContentContainerWidget([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOFMapping, string displayName = "", string widgetName = "", string fileName = "", string widgetCode = "")
        {
            ActionResult action = GotoBackURL();
            if (HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }
            CmsContainerWidgetConfigurationViewModel viewModel = new CmsContainerWidgetConfigurationViewModel();

            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCMSContainerWidget.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSContainerWidget.ToString(), model);

            //Get the list of Content Containers.
            ContentContainerListViewModel contentContainerList = _websiteAgent.ContainerList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            viewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, contentContainerList.ContentContainers, GridListType.ZnodeCMSContainerWidget.ToString(), string.Empty, null, true, true, contentContainerList?.GridModel?.FilterColumn?.ToolMenuList);
            viewModel.GridModel.TotalRecordCount = contentContainerList.TotalResults;

            viewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            viewModel.WidgetName = HttpUtility.UrlDecode(widgetName);
            viewModel.FileName = HttpUtility.UrlDecode(fileName);
            viewModel.WidgetCode = widgetCode;
            viewModel.TypeOFMapping = typeOFMapping;
            viewModel.WidgetKey = widgetKey;
            viewModel.CMSMappingId = cmsMappingId;
            return ActionView(ManageContentContainerView, viewModel);
        }

        [HttpPost]
        //Save the Widget Container details.
        public virtual ActionResult SaveContainerDetails(CmsContainerWidgetConfigurationViewModel configurationViewModel)
        {
            CmsContainerWidgetConfigurationViewModel viewModel = _websiteAgent.SaveCmsContainerDetails(configurationViewModel);
            if (Equals(configurationViewModel.TypeOFMapping, ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString()))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(_websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage));
                return RedirectToAction<ContentController>(x => x.EditContentPage(configurationViewModel.CMSMappingId, configurationViewModel.FileName, 0, false, string.Empty));
            }
            else
            {
                SetNotificationMessage(GetSuccessNotificationMessage(viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage));
                return RedirectToAction<StoreExperienceController>(x => x.StoreSetting(configurationViewModel.CMSMappingId, configurationViewModel.FileName));
            }


        }
        #endregion

        public virtual ActionResult ManageSearchWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, string displayName = "", string widgetName = "", string fileName = "", int localeId = 0)
        {
            ActionResult action = GotoBackURL();

            if(HelperUtility.IsNotNull(action))
            {
                //Set flag for identify the back url of action.
                action.SetPropertyValue("Url", action.GetProperty("Url") + "&isFromWebSiteController=true");
                return action;
            }
            //Get Text Widget Configuration details.
            CMSSearchWidgetConfigurationViewModel model = _websiteAgent.GetSearchWidgetConfiguration(mappingId, widgetId, widgetKey, mappingType, localeId);
            model.DisplayName = HttpUtility.UrlDecode(displayName);
            model.WidgetName = HttpUtility.UrlDecode(widgetName);
            model.FileName = HttpUtility.UrlDecode(fileName);
            return ActionView("ManageSearchWidget", model);
        }

        [HttpPost]
        public virtual ActionResult SaveSearchWidgetConfiguration(CMSSearchWidgetConfigurationViewModel model)
        {
            if(ModelState.IsValid)
            {
                bool status = true;

                //Save the Text Widget Configuration details.
                CMSSearchWidgetConfigurationViewModel viewModel = _websiteAgent.SaveSearchWidgetConfiguration(model);
                if(viewModel.HasError)
                    status = false;

                string message = status ? (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                                        : (viewModel.ActionMode == AdminConstants.Create ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
                if(status)
                {
                    if (Equals(model.TypeOFMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<WebSiteController>(x => x.ManageSearchWidgetConfiguration(model.CMSMappingId, model.CMSWidgetsId, model.WidgetsKey, model.TypeOFMapping, model.DisplayName, model.WidgetName, model.FileName, model.LocaleId));
                    }
                    else
                    {
                        message = _websiteAgent.IsPreviewGloballyEnabled() ? Admin_Resources.SuccessPublish : Admin_Resources.RecordSavedSuccessMessage;
                        SetNotificationMessage(GetSuccessNotificationMessage(message));
                        return RedirectToAction<ContentController>(x => x.EditContentPage(model.CMSMappingId, model.FileName, 0, false, string.Empty));
                    }              
                } else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return ActionView("ManageTextWidget", model);
        }
        #endregion

    }
}