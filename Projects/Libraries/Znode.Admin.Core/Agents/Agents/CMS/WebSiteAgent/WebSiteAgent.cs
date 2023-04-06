using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
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
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Agents
{
    public class WebSiteAgent : BaseAgent, IWebSiteAgent
    {
        #region Private Variables
        private readonly IWebSiteClient _webSiteClient;
        private readonly ICatalogClient _catalogClient;
        private readonly ICMSWidgetConfigurationClient _widgetConfigurationClient;
        private readonly ILocaleAgent _localeAgent;
        private readonly IBrandClient _brandClient;
        private readonly IFormBuilderAgent _formBuilderAgent;
        private readonly ICSSClient _cssClient;
        private readonly IThemeClient _themeClient;
        protected readonly IContentContainerClient _contentContainerClient;
        #endregion

        #region Constructor
        public WebSiteAgent(IWebSiteClient webSiteClient, ICMSWidgetConfigurationClient widgetConfigurationClient, IThemeClient themeClient, ICSSClient cssClient, IContentContainerClient contentContainerClient)
        {
            _webSiteClient = GetClient<WebSiteClient>();
            _catalogClient = GetClient<CatalogClient>();
            _widgetConfigurationClient = GetClient<CMSWidgetConfigurationClient>();
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
            _brandClient = GetClient<BrandClient>();
            _formBuilderAgent = new FormBuilderAgent(GetClient<FormBuilderClient>());
            _themeClient = GetClient<IThemeClient>(themeClient);
            _cssClient = GetClient<ICSSClient>(cssClient);
            _contentContainerClient = GetClient<IContentContainerClient>(contentContainerClient);
        }
        #endregion

        #region Public Methods

        #region Web Site Logo
        //Get the List of Portals, having Themes assigned.
        public virtual StoreListViewModel GetPortalList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set filter for LocaleId.
            SetFilter(filters, Convert.ToInt32(DefaultSettingHelper.DefaultLocale), ZnodeLocaleEnum.LocaleId.ToString());

            ZnodeLogging.LogMessage("Input parameters of method GetPortalList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            PortalListModel portalListModel = _webSiteClient.GetPortalList(filters, sorts, pageIndex, pageSize);

            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = portalListModel?.PortalList?.ToViewModel<StoreViewModel>()?.ToList() };
            SetListPagingData(storeListViewModel, portalListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return storeListViewModel?.StoreList?.Count > 0 ? storeListViewModel : new StoreListViewModel() { StoreList = new List<StoreViewModel>() };
        }

        //Get the Web site Logo Details based on Portal Id.
        public virtual WebSiteLogoViewModel GetWebSiteLogoDetails(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Get the WebSite Logo Details.
            WebSiteLogoViewModel logoDetails = _webSiteClient.GetWebSiteLogo(portalId)?.ToViewModel<WebSiteLogoViewModel>();

            ZnodeLogging.LogMessage("ThemeName of WebSiteLogoViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { ThemeName = logoDetails?.ThemeName });
            if (!string.IsNullOrEmpty(logoDetails.ThemeName))
            {
                string currentThemePath = HttpContext.Current.Server.MapPath(Path.Combine(AdminConstants.ThemeFolderPath, $"{ logoDetails.ThemeName}"));

                string parentThemePath = !string.IsNullOrEmpty(logoDetails.ParentThemeName) ? HttpContext.Current.Server.MapPath(Path.Combine(AdminConstants.ThemeFolderPath, $"{ logoDetails.ParentThemeName }")) : null;

                ZnodeLogging.LogMessage("Input parameters of method GetAvailableWidgets: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { currentThemePath = currentThemePath, parentThemePath = parentThemePath });
                logoDetails.Widgets = WidgetHelper.GetAvailableWidgets(currentThemePath, parentThemePath, true);
            }

            logoDetails.Widgets.CMSMappingId = portalId;
            logoDetails.Widgets.TypeOFMapping = ZnodeCMSTypeofMappingEnum.PortalMapping.ToString();
            logoDetails.Widgets.DisplayName = logoDetails.PortalName;
            //Bind theme list.
            logoDetails.ThemeList = StoreViewModelMap.ToThemeList(_themeClient.GetThemes(null, null, null, null).Themes);

            //Bind CSS list.
            logoDetails.CSSList = logoDetails.CMSThemeId > 0 && HelperUtility.IsNotNull(logoDetails?.CMSThemeId) ?
                   StoreViewModelMap.ToCSSList(_cssClient.GetCssListByThemeId(logoDetails.CMSThemeId.GetValueOrDefault(), null, null, null, null)?.CSSs) : new List<SelectListItem>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(logoDetails) ? logoDetails : new WebSiteLogoViewModel();
        }

        //Save the Web Site Logo Details
        public virtual bool SaveWebSiteLogo(WebSiteLogoViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                _webSiteClient.SaveWebSiteLogo(model.ToModel<WebSiteLogoModel>());
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get portal Associated Widgets.
        public virtual CMSWidgetsListViewModel GetPortalAssociatedWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { templatePath = templatePath, cmsMappingId = cmsMappingId, displayName = displayName, fileName = fileName });

            CMSWidgetsListViewModel cmsWidgetsListViewModel = new CMSWidgetsListViewModel();
            if (!string.IsNullOrEmpty(templatePath))
                cmsWidgetsListViewModel = WidgetHelper.GetAvailableWidgets(templatePath, null, false);
            cmsWidgetsListViewModel.CMSMappingId = cmsMappingId;
            cmsWidgetsListViewModel.TypeOFMapping = typeOfMapping;
            cmsWidgetsListViewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            cmsWidgetsListViewModel.FileName = HttpUtility.UrlDecode(fileName);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(cmsWidgetsListViewModel) ? cmsWidgetsListViewModel : new CMSWidgetsListViewModel();
        }

        #endregion

        #region  WebSite Widget Configuration

        #region Slider Banner Configuration
        //Get the Banner Slider Widgets Configuration details.
        public virtual CMSWidgetConfigurationViewModel GetCMSWidgetSliderBanner(int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOFMapping, int? localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();

            //Set Filters for cmsMappingId, cmsWidgetsId, widgetKey and typeOFMapping.
            SetFiltersForIds(filters, cmsMappingId, cmsWidgetsId, widgetKey, typeOFMapping, localeId);
            ZnodeLogging.LogMessage("Input parameters of method GetCMSWidgetSliderBanner: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Get Widget Configuration details.
            CMSWidgetConfigurationViewModel viewModel = _widgetConfigurationClient.GetCMSWidgetSliderBanner(filters).ToViewModel<CMSWidgetConfigurationViewModel>();
            if (viewModel.CMSWidgetSliderBannerId < 1)
            {
                viewModel.CMSMappingId = cmsMappingId;
                viewModel.CMSWidgetsId = cmsWidgetsId;
                viewModel.WidgetsKey = widgetKey;
                viewModel.TypeOFMapping = typeOFMapping;
            }

            //Set Default Slider Data, for Ex: SliderType, NavigationType & TransitionType.
            SetDefaultSliderBannerDetails(viewModel);

            //Set the Slider list.
            viewModel.CMSPortalSliderList = GetCMSSliderList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Save the Web Site Banner Slider Widgets Configuration Details.
        public virtual CMSWidgetConfigurationViewModel SaveCMSWidgetSliderBanner(CMSWidgetConfigurationViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                viewModel.ActionMode = viewModel.CMSWidgetSliderBannerId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CMSWidgetConfigurationModel model = _widgetConfigurationClient.SaveCMSWidgetSliderBanner(viewModel.ToModel<CMSWidgetConfigurationModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                viewModel = Equals(model, null) ? viewModel : model.ToViewModel<CMSWidgetConfigurationViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CMSWidgetConfigurationViewModel)GetViewModelWithErrorMessage(new CMSWidgetConfigurationViewModel(), viewModel.CMSWidgetSliderBannerId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
            }
            return viewModel;
        }
        #endregion

        #region Link Widget Configuration.
        //Get Link Widget Configuration list.
        public virtual LinkWidgetDataViewModel GetLinkWidgetConfigurationList(FilterCollectionDataModel model, int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOfMapping, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetCMSWidgetTitleConfiguration.ToString(), model);
            //set filter for widget and portal id.
            SetFiltersForIds(model.Filters, cmsMappingId, cmsWidgetsId, widgetKey, typeOfMapping, localeId);

            ZnodeLogging.LogMessage("Input parameters of method GetLinkWidgetConfigurationList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters, Expands = model?.Expands, SortCollection = model?.SortCollection });

            LinkWidgetDataViewModel linkWidgets = new LinkWidgetDataViewModel();
            LinkWidgetConfigurationListViewModel list = GetLinkWidgetConfigurationList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Set tool option menus for link widget configuration list grid.
            SetLinkWidgetConfigurationListToolMenu(list);
            linkWidgets.LinkWidgetConfigurationList = list?.LinkWidgetConfigurationList;

            //Get Grid values for widgets.
            linkWidgets.GridModel = FilterHelpers.GetDynamicGridModel(model, linkWidgets?.LinkWidgetConfigurationList, GridListType.View_GetCMSWidgetTitleConfiguration.ToString(), "", null, true, true, linkWidgets?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            linkWidgets.GridModel.TotalRecordCount = linkWidgets.TotalResults;

            linkWidgets.CMSMappingId = cmsMappingId;
            linkWidgets.CMSWidgetsId = cmsWidgetsId;
            linkWidgets.TypeOfMapping = typeOfMapping;
            linkWidgets.WidgetsKey = widgetKey;
            linkWidgets.Locale = _localeAgent.GetLocalesList(localeId);//Set Filter for Locale Id. 
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return linkWidgets;
        }
        //Get the Link Widget Configuration Details based on the widgetConfiguration Id.
        public virtual LinkWidgetConfigurationViewModel GetLinkWidgetConfigurationDetailById(int widgetTitleConfigurationId, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set Filters
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeCMSWidgetTitleConfigurationEnum.CMSWidgetTitleConfigurationId.ToString(), FilterOperators.Equals, widgetTitleConfigurationId.ToString());
            filters.Add(ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            ZnodeLogging.LogMessage("Input parameter of method GetLinkWidgetConfigurationList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Configuration details.
            LinkWidgetConfigurationListViewModel list = GetLinkWidgetConfigurationList(null, filters, null, 1, 10);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return list?.LinkWidgetConfigurationList?.FirstOrDefault();
        }
        //Save Link Widget Configuration.
        public virtual LinkWidgetDataViewModel CreateUpdateLinkWidgetConfiguration(LinkWidgetDataViewModel viewModel, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            message = viewModel.CMSWidgetTitleConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage;
            try
            {
                //Create/Update the Link Widget Configuration Details.
                viewModel.ActionMode = viewModel.CMSWidgetTitleConfigurationId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                viewModel.LocaleId = viewModel?.LocaleId <= 0 ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : viewModel.LocaleId;
                LinkWidgetConfigurationModel model = _widgetConfigurationClient.CreateUpdateLinkWidgetConfiguration(viewModel.ToModel<LinkWidgetConfigurationModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return model?.ToViewModel<LinkWidgetDataViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        message = Admin_Resources.TitleAlreadyExists;
                        return null;
                    default:
                        message = viewModel.CMSWidgetTitleConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage;
                        return null;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = viewModel.CMSWidgetTitleConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage;
                return null;
            }
        }

        //Delete an Link Widget Configuration by id.
        public virtual bool DeleteLinkWidgetConfiguration(string cmsWidgetTitleConfigurationIds, int localeId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(cmsWidgetTitleConfigurationIds))
            {
                try
                {
                    return _widgetConfigurationClient.DeleteLinkWidgetConfiguration(new ParameterModel { Ids = cmsWidgetTitleConfigurationIds }, localeId);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        public virtual LinkWidgetConfigurationViewModel MapParametersOfLinkWidget(int cmsMappingId, int cmsWidgetsId, string widgetsKey, string typeOfMapping, string displayName, string widgetName)
        {
            LinkWidgetConfigurationViewModel viewModel = new LinkWidgetConfigurationViewModel();

            //set viewmodel properties for view.
            viewModel.CMSWidgetsId = cmsWidgetsId;
            viewModel.CMSMappingId = cmsMappingId;
            viewModel.WidgetsKey = widgetsKey;
            viewModel.TypeOfMapping = typeOfMapping;
            viewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            viewModel.WidgetName = HttpUtility.UrlDecode(widgetName);

            return viewModel;
        }
        #endregion

        #region Portal Product Page
        //Get the list of portal page product associated to selected store in website configuration.
        public virtual PortalProductPageViewModel GetPortalProductPage(PortalProductPageViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            PortalProductPageModel portalProductPageModel = _webSiteClient.GetPortalProductPageList(viewModel.PortalId);
            portalProductPageModel.PortalName = viewModel.PortalName;
            viewModel = portalProductPageModel.ToViewModel<PortalProductPageViewModel>();
            viewModel.TemplateNameList = new List<string>();
            viewModel.TemplateNames = new List<SelectListItem>();

            //Get product types.
            viewModel.ProductTypes = GetProductTypes()?.OrderBy(x => x.Key)?.ToDictionary(x => x.Key, x => x.Value);

            //Get PDP Template name list from PDP file names.
            GetPDPTemplateNames(viewModel);

            //Check if Portal Product Page List greater than zero.
            if (viewModel?.PortalProductPageList?.Count > 0)
            {
                for (int index = 0; index < viewModel.ProductTypes.Count; index++)
                    //Bind the template name list to the dropdown List.
                    viewModel.PortalProductPageList[index].TemplateNames = GetSelectListForProductsPage(viewModel, viewModel.PortalProductPageList[index].TemplateName, viewModel.TemplateNameList);
            }
            else
            {
                viewModel.TemplateNames.Add(new SelectListItem { Text = Admin_Resources.TextProductDefaultTemplate, Value = ZnodeConstant.ProductDefaultTemplate });
                foreach (string templateName in viewModel?.TemplateNameList)
                    //Bind the template name list to the dropdown List.
                    viewModel.TemplateNames.Add(new SelectListItem { Text = HelperMethods.GetResourceNameByValue(templateName), Value = templateName });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Assign new PDP template to product type.
        public virtual bool UpdatePortalProductPage(PortalProductPageViewModel portalProductPageViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Get template name list from comma seperated string.
                portalProductPageViewModel.ProductTypeList = portalProductPageViewModel.Templates.Split(',')?.Select(x => x.Split('_')[0])?.ToList();

                //Get product type ids from comma seperated string.
                portalProductPageViewModel.TemplateNameList = portalProductPageViewModel.Templates.Split(',')?.Select(x => x.Substring(x.IndexOf('_')))?.ToList();

                ZnodeLogging.LogMessage("ProductType and TemplateName list count of portalProductPageViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { ProductTypeListCount = portalProductPageViewModel?.ProductTypeList?.Count, TemplateNameListCount = portalProductPageViewModel?.TemplateNameList?.Count });

                return _webSiteClient.UpdatePortalProductPage(portalProductPageViewModel.ToModel<PortalProductPageModel>());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region CMSWidgetProduct 
        //Get associated product list .
        public virtual CMSWidgetProductListViewModel GetAssociatedProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedProductList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            CMSWidgetProductListModel cmsWidgetProductListModel = _widgetConfigurationClient.GetAssociatedProductList(null, filters, sorts, pageIndex, recordPerPage);
            CMSWidgetProductListViewModel cmsWidgetProductListViewModel = new CMSWidgetProductListViewModel { CMSWidgetProducts = cmsWidgetProductListModel?.CMSWidgetProductCategories?.ToViewModel<CMSWidgetProductViewModel>().ToList() };

            //Set paging parameters.
            SetListPagingData(cmsWidgetProductListViewModel, cmsWidgetProductListModel);

            //Set tool options for this grid.
            SetCMSWidgetProductListToolMenu(cmsWidgetProductListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return cmsWidgetProductListModel?.CMSWidgetProductCategories?.Count > 0 ? cmsWidgetProductListViewModel
                : new CMSWidgetProductListViewModel() { CMSWidgetProducts = new List<CMSWidgetProductViewModel>() };
        }

        //Get unassociated product list .
        public virtual ProductDetailsListViewModel GetUnAssociatedProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters of method GetUnAssociatedProductList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            ProductDetailsListModel productList = _widgetConfigurationClient.GetUnAssociatedProductList(null, filters, sorts, pageIndex, recordPerPage);
            ProductDetailsListViewModel productListViewModel = new ProductDetailsListViewModel { ProductDetailList = productList?.ProductDetailList?.ToViewModel<ProductDetailsViewModel>().ToList() };

            //Set paging parameters.
            SetListPagingData(productListViewModel, productList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return productList?.ProductDetailList?.Count > 0 ? productListViewModel
                : new ProductDetailsListViewModel() { ProductDetailList = new List<ProductDetailsViewModel>() };
        }

        //Associate product.
        public virtual bool AssociateProduct(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string SKUs)
        {
            try
            {
                CMSWidgetProductListModel cmsWidgetModel = ContentPageViewModelMap.ToAssociateProductListModel(cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, SKUs);
                cmsWidgetModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                cmsWidgetModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _widgetConfigurationClient.AssociateProduct(cmsWidgetModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Unassociate associated products . 
        public virtual bool UnassociateProduct(string cmsWidgetProductId)
             => _widgetConfigurationClient.UnassociateProduct(new ParameterModel { Ids = cmsWidgetProductId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale), EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview });

        //Edit associated products . 
        public virtual bool EditCMSAssociateProduct(ProductDetailsViewModel productDetailsViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                ProductDetailsModel result = _widgetConfigurationClient.UpdateCMSAssociateProduct(productDetailsViewModel.ToModel<ProductDetailsModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region Media Widget Configuration
        public virtual CMSMediaWidgetConfigurationViewModel SaveAndUpdateMediaWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int MediaId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            CMSMediaWidgetConfigurationViewModel viewModel = new CMSMediaWidgetConfigurationViewModel();
            viewModel.CMSMappingId = mappingId;
            viewModel.WidgetsKey = widgetKey;
            viewModel.TypeOFMapping = mappingType;
            viewModel.CMSWidgetsId = widgetId;
            viewModel.MediaId = MediaId;
            viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;


            _widgetConfigurationClient.SaveAndUpdateMediaWidgetConfiguration(viewModel.ToModel<CMSMediaWidgetConfigurationModel>());

            return viewModel;
        }
        #endregion

        #region Remove Widget Configuration Data
        public virtual bool RemoveWidgetDataFromContentPage(int mappingId, string widgetKey, string widgetCode, out string errorMessage)
        {           
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (mappingId > 0 && !string.IsNullOrEmpty(widgetCode) && !string.IsNullOrEmpty(widgetCode))
            {
                try
                {
                    CmsContainerWidgetConfigurationViewModel removeWidgetConfiguration = new CmsContainerWidgetConfigurationViewModel();
                    removeWidgetConfiguration.CMSMappingId = mappingId;
                    removeWidgetConfiguration.WidgetKey = widgetKey;
                    removeWidgetConfiguration.WidgetCode = widgetCode;
                    removeWidgetConfiguration.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                    return _widgetConfigurationClient.RemoveWidgetDataFromContentPageConfiguration(removeWidgetConfiguration.ToModel<CmsContainerWidgetConfigurationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    errorMessage = ex.Message;
                    return false;
                }
            }
            return false;
        }

        #endregion
        
        #region Text Widget Configuration

        //Get the Banner Slider Widget Configuration details based on the mappingId, widgetId, widgetKey & Widget mappingType.
        public virtual CMSTextWidgetConfigurationViewModel GetTextWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            CMSTextWidgetConfigurationViewModel viewModel = new CMSTextWidgetConfigurationViewModel();

            if (localeId == 0)
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            //Set Filters based on input Parameters
            FilterCollection filters = new FilterCollection();
            SetFilters(filters, widgetId, widgetKey, mappingId, mappingType, localeId);
            ZnodeLogging.LogMessage("Input parameters of method GetTextWidgetConfigurationList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Get Widget Configuration details.
            CMSTextWidgetConfigurationModel model = _widgetConfigurationClient.GetTextWidgetConfigurationList(null, filters, null)?.TextWidgetConfigurationList?.FirstOrDefault();

            if (HelperUtility.IsNotNull(model))
                viewModel = model.ToViewModel<CMSTextWidgetConfigurationViewModel>();

            SetCMSTextWidgetConfigurationViewModel(mappingId, widgetId, widgetKey, mappingType, localeId, viewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return viewModel;
        }

        //Return the Preview feature enable or disabled.
        public virtual bool IsPreviewGloballyEnabled()
            => GetService<IPublishPopupHelper>().GetAvailablePublishStatesforPreview().FirstOrDefault(x => x.ApplicationType == ApplicationTypesEnum.WebstorePreview.ToString()).IsEnabled && ZnodeAdminSettings.EnableCMSPreview;

        //Save the Text Widget Configuration
        public virtual CMSTextWidgetConfigurationViewModel SaveTextWidgetConfiguration(CMSTextWidgetConfigurationViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                viewModel.ActionMode = viewModel.CMSTextWidgetConfigurationId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;

                if (viewModel?.CMSTextWidgetConfigurationId > 0)
                    _widgetConfigurationClient.UpdateTextWidgetConfiguration(viewModel.ToModel<CMSTextWidgetConfigurationModel>());
                else
                {
                    if (HelperUtility.IsNotNull(viewModel))
                        viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    ZnodeLogging.LogMessage("LocaleId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });

                    CMSTextWidgetConfigurationModel model = _widgetConfigurationClient.CreateTextWidgetConfiguration(viewModel.ToModel<CMSTextWidgetConfigurationModel>());
                    if (model?.CMSTextWidgetConfigurationId > 0)
                        viewModel.CMSTextWidgetConfigurationId = model.CMSTextWidgetConfigurationId;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CMSTextWidgetConfigurationViewModel)GetViewModelWithErrorMessage(new CMSTextWidgetConfigurationViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }
        #endregion

        #region Form Widget Configuration.

        //Get the Form Widget Configuration.
        public virtual CMSFormWidgetConfigurationViewModel GetFormWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (localeId < 1)
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            //Set Filters based on input Parameters
            FilterCollection filters = new FilterCollection();
            SetFilters(filters, widgetId, widgetKey, mappingId, mappingType, localeId);
            ZnodeLogging.LogMessage("LocaleId and filters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = localeId, filters = filters });

            //Get Widget Configuration details.
            CMSFormWidgetConfigurationViewModel viewModel = _widgetConfigurationClient.GetFormWidgetConfigurationList(null, filters, null)?.FormWidgetConfigurationList?.FirstOrDefault()?.ToViewModel<CMSFormWidgetConfigurationViewModel>();

            //Set Widget Configuration details.
            viewModel = SetCMSFormWidgetConfigurationViewModel(mappingId, widgetId, widgetKey, mappingType, localeId, viewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Save the Form Widget Configuration
        public virtual CMSFormWidgetConfigurationViewModel SaveFormWidgetConfiguration(CMSFormWidgetConfigurationViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                viewModel.ActionMode = viewModel.CMSFormWidgetConfigurationId < 1 ? AdminConstants.Create : AdminConstants.Edit;
                viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                if (viewModel?.CMSFormWidgetConfigurationId > 0)
                    _widgetConfigurationClient.UpdateFormWidgetConfiguration(viewModel.ToModel<CMSFormWidgetConfigrationModel>());
                else
                {
                    if (HelperUtility.IsNotNull(viewModel))
                        viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    ZnodeLogging.LogMessage("LocaleId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });

                    CMSFormWidgetConfigrationModel model = _widgetConfigurationClient.CreateFormWidgetConfiguration(viewModel.ToModel<CMSFormWidgetConfigrationModel>());
                    if (model?.CMSFormWidgetConfigurationId > 0)
                        viewModel.CMSFormWidgetConfigurationId = model.CMSFormWidgetConfigurationId;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CMSFormWidgetConfigurationViewModel)GetViewModelWithErrorMessage(new CMSFormWidgetConfigurationViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        #endregion

        #region Category Association
        // Get associated category list
        public virtual CategoryListViewModel GetAssociatedCategoryList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsWidgetsId = cmsWidgetsId, cmsMappingId = cmsMappingId, widgetKey = widgetKey, typeOfMapping = typeOfMapping, widgetName = widgetName, displayName = displayName });

            //Set Filters for cmsMappingId, cmsWidgetsId, widgetKey and typeOFMapping.
            SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);

            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedCategorylist: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters, SortCollection = model?.SortCollection });

            //Get list of associated category.
            CategoryListModel list = _widgetConfigurationClient.GetAssociatedCategorylist(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            CategoryListViewModel listViewModel = new CategoryListViewModel { Categories = list?.CMSWidgetProductCategories?.ToViewModel<CategoryViewModel>()?.ToList() };

            if (HelperUtility.IsNotNull(listViewModel?.Categories))
                MapParametersOfCategoryListViewModel(listViewModel, cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName);

            SetListPagingData(listViewModel, list);

            //Set tool options for this grid.
            SetCMSWidgetsCategoryListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return list?.CMSWidgetProductCategories?.Count > 0 ? listViewModel : new CategoryListViewModel() { Categories = new List<CategoryViewModel>(), CMSMappingId = cmsMappingId, CMSWidgetsId = cmsWidgetsId, TypeOFMapping = typeOfMapping, WidgetsKey = widgetKey, WidgetName = HttpUtility.UrlDecode(widgetName), DisplayName = HttpUtility.UrlDecode(displayName) };
        }

        //Get list of unassociate categories.
        public virtual CategoryListViewModel GetUnAssociatedCategoryList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set Filters for cmsMappingId, cmsWidgetsId, widgetKey and typeOFMapping.
            SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);

            ZnodeLogging.LogMessage("Input parameters of method GetUnAssociatedCategoryList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters, SortCollection = model?.SortCollection });
            //Get list of unassociated categories.
            CategoryListModel list = _widgetConfigurationClient.GetUnAssociatedCategoryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            CategoryListViewModel listViewModel = new CategoryListViewModel { Categories = list?.Categories?.ToViewModel<CategoryViewModel>()?.ToList() };
            if (HelperUtility.IsNotNull(listViewModel?.Categories))
                MapParametersOfCategoryListViewModel(listViewModel, cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName);

            SetListPagingData(listViewModel, list);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return list?.Categories?.Count > 0 ? listViewModel : new CategoryListViewModel() { Categories = new List<CategoryViewModel>(), CMSMappingId = cmsMappingId, CMSWidgetsId = cmsWidgetsId, TypeOFMapping = typeOfMapping, WidgetsKey = widgetKey, WidgetName = HttpUtility.UrlDecode(widgetName), DisplayName = HttpUtility.UrlDecode(displayName) };
        }

        //Remove associated categories.
        public virtual bool RemoveAssociatedCategories(string cmsWidgetCategoryId)
               => _widgetConfigurationClient.DeleteCategories(new ParameterModel { Ids = cmsWidgetCategoryId });

        //Associate categories.
        public virtual bool AssociateCategories(int cmsWidgetsId, string categoryCodes, int cmsMappingId, string widgetKey, string typeOFMapping)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _widgetConfigurationClient.AssociateCategories(new ParameterModelForWidgetCategory { CMSWidgetsId = cmsWidgetsId, CategoryCodes = categoryCodes, CMSMappingId = cmsMappingId, WidgetsKey = widgetKey, TypeOFMapping = typeOFMapping });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual bool EditCMSWidgetCategory(CategoryViewModel categoryViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                CategoryModel result = _widgetConfigurationClient.UpdateCMSWidgetCategory(categoryViewModel.ToModel<CategoryModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion     

        #region Brand Association
        // Get associated brand list
        public virtual BrandListViewModel GetAssociatedBrandList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsWidgetsId = cmsWidgetsId, cmsMappingId = cmsMappingId, widgetKey = widgetKey, typeOfMapping = typeOfMapping, widgetName = widgetName, displayName = displayName });

            //Set filters for cmsMappingId, cmsWidgetsId, widgetKey and typeOFMapping.
            SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);

            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedBrandlist: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { Filters = model?.Filters, SortCollection = model?.SortCollection });
            //Get list of associated brands.
            BrandListModel list = _widgetConfigurationClient.GetAssociatedBrandlist(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            BrandListViewModel listViewModel = new BrandListViewModel { Brands = list?.Brands?.ToViewModel<BrandViewModel>()?.ToList() };

            if (HelperUtility.IsNotNull(listViewModel?.Brands))
                MapParametersOfBrandListViewModel(listViewModel, cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName);

            SetListPagingData(listViewModel, list);

            //Set tool options for this grid.
            SetCMSWidgetsBrandListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return list?.Brands?.Count > 0 ? listViewModel : new BrandListViewModel() { Brands = new List<BrandViewModel>(), CMSMappingId = cmsMappingId, CMSWidgetsId = cmsWidgetsId, TypeOFMapping = typeOfMapping, WidgetsKey = widgetKey, WidgetName = HttpUtility.UrlDecode(widgetName), DisplayName = HttpUtility.UrlDecode(displayName) };
        }

        //Get list of unassociate brands.
        public virtual BrandListViewModel GetUnAssociatedBrandList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string widgetName, string displayName, FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsWidgetsId = cmsWidgetsId, cmsMappingId = cmsMappingId, widgetKey = widgetKey, typeOfMapping = typeOfMapping, widgetName = widgetName, displayName = displayName });

            //Set Filters for cmsMappingId, cmsWidgetsId, widgetKey and typeOFMapping.
            SetFilters(model.Filters, cmsWidgetsId, widgetKey, cmsMappingId, typeOfMapping);

            ZnodeLogging.LogMessage("Input parameters of method GetUnAssociatedBrandList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = model?.Filters, SortCollection = model?.SortCollection });
            //Get list of unassociated brands.
            BrandListModel list = _widgetConfigurationClient.GetUnAssociatedBrandList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            BrandListViewModel listViewModel = new BrandListViewModel { Brands = list?.Brands?.ToViewModel<BrandViewModel>()?.ToList() };
            if (HelperUtility.IsNotNull(listViewModel?.Brands))
                MapParametersOfBrandListViewModel(listViewModel, cmsWidgetsId, cmsMappingId, widgetKey, typeOfMapping, widgetName, displayName);

            SetListPagingData(listViewModel, list);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return list?.Brands?.Count > 0 ? listViewModel : new BrandListViewModel() { Brands = new List<BrandViewModel>(), CMSMappingId = cmsMappingId, CMSWidgetsId = cmsWidgetsId, TypeOFMapping = typeOfMapping, WidgetsKey = widgetKey, WidgetName = HttpUtility.UrlDecode(widgetName), DisplayName = HttpUtility.UrlDecode(displayName) };
        }

        //Remove associated brands.
        public virtual bool RemoveAssociatedBrands(string cmsWidgetBrandId)
               => _widgetConfigurationClient.DeleteBrands(new ParameterModel { Ids = cmsWidgetBrandId });

        //Edit associated brands.
        public virtual bool EditCMSWidgetBrand(BrandViewModel brandViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                BrandModel result = _widgetConfigurationClient.UpdateCMSWidgetBrand(brandViewModel.ToModel<BrandModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Associate brands.
        public virtual bool AssociateBrands(int cmsWidgetsId, string brandId, int cmsMappingId, string widgetKey, string typeOFMapping)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                return _widgetConfigurationClient.AssociateBrands(new ParameterModelForWidgetBrand { CMSWidgetsId = cmsWidgetsId, BrandId = brandId, CMSMappingId = cmsMappingId, WidgetsKey = widgetKey, TypeOFMapping = typeOFMapping });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Search Widget Configuration

        //Get the search Configuration details based on the mappingId, widgetId, widgetKey & Widget mappingType.
        public virtual CMSSearchWidgetConfigurationViewModel GetSearchWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { mappingId = mappingId, widgetId = widgetId, widgetKey = widgetKey, mappingType = mappingType, localeId = localeId });

            CMSSearchWidgetConfigurationViewModel viewModel = new CMSSearchWidgetConfigurationViewModel();

            if (localeId == 0)
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            //Set Filters based on input Parameters
            FilterCollection filters = new FilterCollection();
            SetFilters(filters, widgetId, widgetKey, mappingId, mappingType, localeId);

            ZnodeLogging.LogMessage("Input parameter of method GetSearchWidgetConfiguration: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get Widget Configuration details.
            CMSSearchWidgetConfigurationModel model = _widgetConfigurationClient.GetSearchWidgetConfiguration(filters, null);

            if (HelperUtility.IsNotNull(model))
                viewModel = model.ToViewModel<CMSSearchWidgetConfigurationViewModel>();
            viewModel.CMSMappingId = mappingId;
            viewModel.CMSWidgetsId = widgetId;
            viewModel.WidgetsKey = widgetKey;
            viewModel.TypeOFMapping = mappingType;
            viewModel.LocaleId = localeId;
            viewModel.Locales = _localeAgent.GetLocalesList(localeId);
            viewModel.AttributeList = GetAttributeListForSearchWidget(model.SearchableAttributes, model.AttributeCode);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        private List<SelectListItem> GetAttributeListForSearchWidget(List<PublishAttributeModel> searchableAttributes, string attributeCode)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Insert(0, new SelectListItem { Text = Admin_Resources.TextSelectAttribute, Value = "" });
            selectListItems.AddRange(searchableAttributes.Select(x => new SelectListItem { Text = x.AttributeName, Value = x.AttributeCode, Selected = Equals(x.AttributeCode, attributeCode) }).ToList());
            return selectListItems;
        }

        //Save the search Widget Configuration
        public virtual CMSSearchWidgetConfigurationViewModel SaveSearchWidgetConfiguration(CMSSearchWidgetConfigurationViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                viewModel.ActionMode = viewModel.CMSSearchWidgetId == 0 ? AdminConstants.Create : AdminConstants.Edit;
                viewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                if (viewModel?.CMSSearchWidgetId > 0)
                    _widgetConfigurationClient.UpdateSearchWidgetConfiguration(viewModel.ToModel<CMSSearchWidgetConfigurationModel>());
                else
                {
                    if (HelperUtility.IsNotNull(viewModel))
                        viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    ZnodeLogging.LogMessage("LocaleId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });

                    CMSSearchWidgetConfigurationModel model = _widgetConfigurationClient.CreateSearchWidgetConfiguration(viewModel.ToModel<CMSSearchWidgetConfigurationModel>());
                    if (model?.CMSSearchWidgetId > 0)
                        viewModel.CMSSearchWidgetId = model.CMSSearchWidgetId;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CMSSearchWidgetConfigurationViewModel)GetViewModelWithErrorMessage(new CMSSearchWidgetConfigurationViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return viewModel;
        }
        #endregion
        #endregion

        #region Publish & Preview

        //Publish CMS configuration
        public virtual bool Publish(int portalId, out string errorMessage,string targetPublishState = null, string publishContent = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            errorMessage = PIM_Resources.ErrorPublished;
            try
            {

                bool isStorePublished;
                bool isCatalogPublished;

                if (publishContent.Contains(ZnodePublishContentTypeEnum.StoreSettings.ToString()) || publishContent.Contains(ZnodePublishContentTypeEnum.CmsContent.ToString()))
                    isStorePublished = _webSiteClient.Publish(portalId, targetPublishState, publishContent);
                else
                    isStorePublished = true;

                if (publishContent.Contains(ZnodePublishContentTypeEnum.Catalog.ToString()))
                {
                    int catalogId;
                    string catalogIdResult = _webSiteClient.GetAssociatedCatalogId(portalId);
                    int.TryParse(catalogIdResult, out catalogId);
                    //Set isDraft to true if want to publish draft status product only else set false to publish all products
                    bool isDraftProductsOnly = !string.IsNullOrEmpty(publishContent) && publishContent.Contains(ZnodePublishStatesEnum.DRAFT.ToString());

                    isCatalogPublished = (_catalogClient.PublishCatalog(catalogId, targetPublishState, isDraftProductsOnly)?.IsPublished).GetValueOrDefault();
                }
                else
                    isCatalogPublished = true;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                return isStorePublished && isCatalogPublished;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        errorMessage = PIM_Resources.ErrorPublishCatalog;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #endregion
        //Remove filters if already exists and add new filters.
        public virtual void SetFilter(FilterCollection filters, int id, string filterKey)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For Id already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == filterKey))
                {
                    //If Id Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == filterKey);

                    //Add New Id Into filters
                    filters.Add(new FilterTuple(filterKey, FilterOperators.Equals, id.ToString()));
                }
                else
                    filters.Add(new FilterTuple(filterKey, FilterOperators.Equals, id.ToString()));
            }
            ZnodeLogging.LogMessage("Filters generated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

        }

        //Get the widget id by its code.
        public virtual int GetWidgetIdByCode(string widgetCode)
        => _webSiteClient.GetWidgetIdByCode(widgetCode);

        //Get the Form Widget Email Configuration.
        public virtual FormWidgetEmailConfigurationViewModel GetFormWidgetEmailConfiguration(int cMSContentPagesId, int widgetId, string widgetKey, string mappinType, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (localeId < 1)
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            //Set Filters based on input Parameters
            FilterCollection filters = new FilterCollection();
            SetEmailFilters(filters, cMSContentPagesId, localeId);
            //Get Widget Configuration details.
            FormWidgetEmailConfigurationViewModel viewModel = _widgetConfigurationClient.GetFormWidgetEmailConfiguration(cMSContentPagesId).ToViewModel<FormWidgetEmailConfigurationViewModel>();
            FilterCollection formFilters = new FilterCollection();
            SetFilters(formFilters, widgetId, widgetKey, cMSContentPagesId, mappinType, localeId);
            ZnodeLogging.LogMessage("Input parameter for method GetFormWidgetConfigurationList: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { formFilters = formFilters });

            viewModel.FormTitle = _widgetConfigurationClient.GetFormWidgetConfigurationList(null, formFilters, null)?.FormWidgetConfigurationList?.FirstOrDefault()?.FormTitle;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        public virtual FormWidgetEmailConfigurationViewModel SaveFormWidgetEmailConfiguration(FormWidgetEmailConfigurationViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                viewModel.ActionMode = viewModel.FormWidgetEmailConfigurationId < 1 ? AdminConstants.Create : AdminConstants.Edit;
                if (viewModel?.FormWidgetEmailConfigurationId > 0)
                    _widgetConfigurationClient.UpdateFormWidgetEmailConfiguration(viewModel.ToModel<FormWidgetEmailConfigurationModel>());
                else
                {
                    if (HelperUtility.IsNotNull(viewModel))
                        viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    ZnodeLogging.LogMessage("LocaleId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });

                    FormWidgetEmailConfigurationModel model = _widgetConfigurationClient.CreateFormWidgetEmailConfiguration(viewModel.ToModel<FormWidgetEmailConfigurationModel>());
                    if (model?.FormWidgetEmailConfigurationId > 0)
                        viewModel.FormWidgetEmailConfigurationId = model.FormWidgetEmailConfigurationId;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (FormWidgetEmailConfigurationViewModel)GetViewModelWithErrorMessage(new FormWidgetEmailConfigurationViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel;
        }

        public virtual bool IsPreviewPerform(string typeOfMapping)
        {
            if (!string.IsNullOrEmpty(typeOfMapping) && string.Equals(typeOfMapping, ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString(), StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        //Get the List of Content Containers
        public virtual ContentContainerListViewModel ContainerList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ContentContainerListModel listModel = _contentContainerClient.List(null, filters, sortCollection, pageIndex, recordPerPage);
            ContentContainerListViewModel viewModel = new ContentContainerListViewModel { ContentContainers = listModel?.ContainerList?.ToViewModel<ContentContainerViewModel>().ToList() };

            //Set Paging Data 
            SetListPagingData(viewModel, listModel);

            if (listModel?.ContainerList?.Count > 0)
                return viewModel;
            else
                return new ContentContainerListViewModel() { ContentContainers = new List<ContentContainerViewModel>() };
        }

        //Save the Web SiteContainer Widgets Configuration Details.
        public virtual CmsContainerWidgetConfigurationViewModel SaveCmsContainerDetails(CmsContainerWidgetConfigurationViewModel configurationViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                configurationViewModel.EnableCMSPreview = ZnodeAdminSettings.EnableCMSPreview;
                CmsContainerWidgetConfigurationModel model = _widgetConfigurationClient.SaveCmsContainerDetails(configurationViewModel.ToModel<CmsContainerWidgetConfigurationModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                configurationViewModel = Equals(model, null) ? configurationViewModel : model.ToViewModel<CmsContainerWidgetConfigurationViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (CmsContainerWidgetConfigurationViewModel)GetViewModelWithErrorMessage(new CmsContainerWidgetConfigurationViewModel(), configurationViewModel.CMSContainerConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage);
            }
            return configurationViewModel;
        }

        #region Private Methods
        private void MapParametersOfCategoryListViewModel(CategoryListViewModel listViewModel, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName)
        {
            listViewModel.CMSWidgetsId = cmsWidgetsId;
            listViewModel.CMSMappingId = cmsMappingId;
            listViewModel.WidgetsKey = widgetKey;
            listViewModel.TypeOFMapping = typeOFMapping;
            listViewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            listViewModel.WidgetName = HttpUtility.UrlDecode(widgetName);
        }

        //Get the of templates name in select list item format.
        private List<SelectListItem> GetSelectListForProductsPage(PortalProductPageViewModel portalProductPageViewModel, string selected, List<string> templateNames)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            selectList.Add(new SelectListItem { Text = Admin_Resources.TextProductDefaultTemplate, Value = ZnodeConstant.ProductDefaultTemplate });
            //Bind the template name list to the dropdown List.
            foreach (string templateName in templateNames)
                selectList.Add(new SelectListItem { Text = HelperMethods.GetResourceNameByValue(templateName), Value = templateName });

            //Check if updated template name is in bounded template name list to the dropdown list show selected.
            foreach (SelectListItem item in selectList)
            {
                if (Equals(item.Value, selected))
                {
                    item.Selected = true;
                    break;
                }
            }
            ZnodeLogging.LogMessage("Templates name in select list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { selectListCount = selectList?.Count });
            return selectList;
        }

        //Get PDP Template name list from PDP file names.
        private void GetPDPTemplateNames(PortalProductPageViewModel viewModel)
        {
            //Get PDP Template file names.
            FileInfo[] fileNames = new DirectoryInfo(HttpContext.Current.Server.MapPath(AdminConstants.TemplateNamePath)).GetFiles("*.cshtml", SearchOption.AllDirectories);

            foreach (FileInfo fileName in fileNames)
                viewModel.TemplateNameList.Add(Path.GetFileNameWithoutExtension(Convert.ToString(fileName)));
        }

        //This method will get replaced with published product type list code.
        private Dictionary<string, string> GetProductTypes()
        {
            Dictionary<string, string> productTypes = new Dictionary<string, string>();
            productTypes.Add("SimpleProduct", "Simple Product");
            productTypes.Add("GroupedProduct", "Grouped Product");
            productTypes.Add("BundleProduct", "Bundle Product");
            productTypes.Add("ConfigurableProduct", "Configurable Product");
            return productTypes;
        }

        //Set Default Slider Data, for Ex: SliderType, NavigationType & TransitionType Details
        private void SetDefaultSliderBannerDetails(CMSWidgetConfigurationViewModel viewModel)
        {
            //Set the Default Slider Type Data.
            viewModel.SliderTypeList = GetSliderTypes();

            //Set the Default Slider Navigation Type Data.
            viewModel.SliderNavigationTypeList = GetSliderNavigationTypes();

            //Set the Default Slider Transition Type Data.
            viewModel.SliderTransitionTypeList = GetSliderTransitionTypes();
        }

        //Get the Default Slider Type Data.
        private List<SelectListItem> GetSliderTypes()
        {
            List<SelectListItem> lstSliderType = new List<SelectListItem>();
            lstSliderType.Add(new SelectListItem() { Text = "Boxed", Value = "Boxed" });
            lstSliderType.Add(new SelectListItem() { Text = "Full Width", Value = "Full Width" });
            return lstSliderType;
        }

        //Get the Default Slider Navigation Type Data.
        private List<SelectListItem> GetSliderNavigationTypes()
        {
            List<SelectListItem> lstNavigationType = new List<SelectListItem>();
            lstNavigationType.Add(new SelectListItem() { Text = "Dots", Value = "Dots" });
            lstNavigationType.Add(new SelectListItem() { Text = "Arrows", Value = "Arrows" });
            return lstNavigationType;
        }

        //Get the Default Slider Transition Type Data.
        private List<SelectListItem> GetSliderTransitionTypes()
        {
            List<SelectListItem> lstTransitionType = new List<SelectListItem>();
            lstTransitionType.Add(new SelectListItem() { Text = "Fade", Value = "fade" });
            lstTransitionType.Add(new SelectListItem() { Text = "Back Slide", Value = "backSlide" });
            lstTransitionType.Add(new SelectListItem() { Text = "Go Down", Value = "goDown" });
            lstTransitionType.Add(new SelectListItem() { Text = "Scale Up", Value = "fadeUp" });
            return lstTransitionType;
        }

        //Get the CMS Slider List.
        private List<SelectListItem> GetCMSSliderList()
        {
            List<SelectListItem> lstSlider = new List<SelectListItem>();
            SliderClient _sliderClient = new SliderClient();
            var sliderList = _sliderClient.GetSliders(null, null, null, null)?.Sliders;
            if (!Equals(sliderList, null) && sliderList.Count > 0)
            {
                lstSlider = (from slider in sliderList
                             select new SelectListItem()
                             {
                                 Text = slider.Name,
                                 Value = Convert.ToString(slider.CMSSliderId),
                             }).ToList();
            }
            lstSlider.Insert(0, new SelectListItem { Text = Admin_Resources.LabelPleaseSelect, Value = "" });
            return lstSlider;
        }

        //Set Filters for Ids.
        private void SetFiltersForIds(FilterCollection filters, int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOfMapping, int? localeId)
        {
            filters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetTitleConfigurationEnum.CMSWidgetsId.ToString() || x.FilterName == ZnodeCMSWidgetTitleConfigurationEnum.CMSMappingId.ToString()
            || x.FilterName == ZnodeCMSWidgetTitleConfigurationEnum.WidgetsKey.ToString() || x.FilterName == ZnodeCMSWidgetTitleConfigurationEnum.TypeOFMapping.ToString() || x.FilterName == ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString());

            filters.RemoveAll(x => x.Item1 == ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString());
            filters.Add(ZnodeCMSWidgetTitleConfigurationEnum.CMSWidgetsId.ToString(), FilterOperators.Equals, cmsWidgetsId.ToString());
            filters.Add(ZnodeCMSWidgetTitleConfigurationEnum.CMSMappingId.ToString(), FilterOperators.Equals, cmsMappingId.ToString());
            filters.Add(ZnodeCMSWidgetTitleConfigurationEnum.WidgetsKey.ToString(), FilterOperators.Is, widgetKey);
            filters.Add(ZnodeCMSWidgetTitleConfigurationEnum.TypeOFMapping.ToString(), FilterOperators.Like, typeOfMapping);
            if (HelperUtility.IsNotNull(localeId))
                filters.Add(ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId > 0 ? localeId.ToString() : DefaultSettingHelper.DefaultLocale);
        }

        //Get Link Widget Configuration list.
        private LinkWidgetConfigurationListViewModel GetLinkWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int pageIndex, int recordPerPage)

        {
            LinkWidgetConfigurationListModel list = _widgetConfigurationClient.LinkWidgetConfigurationList(expands, filters, sortCollection, pageIndex, recordPerPage);
            LinkWidgetConfigurationListViewModel listViewModel = new LinkWidgetConfigurationListViewModel { LinkWidgetConfigurationList = list?.LinkWidgetConfigurationList?.ToViewModel<LinkWidgetConfigurationViewModel>()?.ToList() };

            SetListPagingData(listViewModel, list);

            return listViewModel?.LinkWidgetConfigurationList?.Count > 0 ? listViewModel
                : new LinkWidgetConfigurationListViewModel { LinkWidgetConfigurationList = new List<LinkWidgetConfigurationViewModel>() };
        }

        //Set tool option menus for link widget configuration list grid.
        private void SetLinkWidgetConfigurationListToolMenu(LinkWidgetConfigurationListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('LinkWidgetConfigurationDeletePopup')", ControllerName = "WebSite", ActionName = "DeleteLinkWidgetConfiguration" });
            }
        }

        //Set tool option menus for widgets category list grid.
        private void SetCMSWidgetsCategoryListToolMenu(CategoryListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('OfferCategoriesDelete')", ControllerName = "WebSite", ActionName = "RemoveAssociatedCategories" });
            }
        }

        //Set tool option menus for widgets brand list grid.
        private void SetCMSWidgetsBrandListToolMenu(BrandListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('OfferBrandsDelete')", ControllerName = "WebSite", ActionName = "RemoveAssociatedBrands" });
            }
        }

        //Map paramters of brand list view model.
        private void MapParametersOfBrandListViewModel(BrandListViewModel listViewModel, int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName)
        {
            listViewModel.CMSWidgetsId = cmsWidgetsId;
            listViewModel.CMSMappingId = cmsMappingId;
            listViewModel.WidgetsKey = widgetKey;
            listViewModel.TypeOFMapping = typeOFMapping;
            listViewModel.DisplayName = HttpUtility.UrlDecode(displayName);
            listViewModel.WidgetName = HttpUtility.UrlDecode(widgetName);
        }

        //Set tool option menus for CMS Widget Product List grid.
        private void SetCMSWidgetProductListToolMenu(CMSWidgetProductListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('CMSWidgetProductDeletePopup')", ControllerName = "WebSite", ActionName = "UnAssociateProduct" });
            }
        }

        public virtual void SetFilters(FilterCollection filters, int cmsWidgetsId, string widgetsKey, int cmsMappingId, string typeOfMapping, int localeId = 0)
        {
            filters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString() || x.FilterName == ZnodeLocaleEnum.LocaleId.ToString()
                || x.FilterName == ZnodeCMSWidgetProductEnum.WidgetsKey.ToString() || x.FilterName == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString() || x.FilterName == ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString());
            if (HelperUtility.IsNotNull(cmsWidgetsId) && cmsWidgetsId > 0)
            {
                filters.Add(ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString(), FilterOperators.Equals, Convert.ToString(cmsWidgetsId));
                filters.Add(ZnodeCMSWidgetProductEnum.WidgetsKey.ToString(), FilterOperators.Is, Convert.ToString(widgetsKey));
                filters.Add(ZnodeCMSWidgetProductEnum.CMSMappingId.ToString(), FilterOperators.Equals, Convert.ToString(cmsMappingId));
                filters.Add(ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString(), FilterOperators.Is, Convert.ToString(typeOfMapping));
            };
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId == 0 ? DefaultSettingHelper.DefaultLocale : localeId.ToString());
        }

        public virtual void SetEmailFilters(FilterCollection filters, int cMSContentPagesId, int localeId = 0)
        {
            filters.RemoveAll(x => x.FilterName == ZnodeFormWidgetEmailConfigurationEnum.CMSContentPagesId.ToString() || x.FilterName == ZnodeLocaleEnum.LocaleId.ToString());
            if (HelperUtility.IsNotNull(cMSContentPagesId) && cMSContentPagesId > 0)
                filters.Add(ZnodeFormWidgetEmailConfigurationEnum.CMSContentPagesId.ToString(), FilterOperators.Equals, Convert.ToString(cMSContentPagesId));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId == 0 ? DefaultSettingHelper.DefaultLocale : localeId.ToString());
        }

        //Set the property of CMSTextWidgetConfigurationViewModel.
        private void SetCMSTextWidgetConfigurationViewModel(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId, CMSTextWidgetConfigurationViewModel viewModel)
        {
            viewModel.CMSMappingId = mappingId;
            viewModel.CMSWidgetsId = widgetId;
            viewModel.WidgetsKey = widgetKey;
            viewModel.TypeOFMapping = mappingType;
            viewModel.LocaleId = localeId;
            viewModel.Locales = _localeAgent.GetLocalesList(localeId);
        }

        //Set the property of CMSFormWidgetConfigurationViewModel.
        private CMSFormWidgetConfigurationViewModel SetCMSFormWidgetConfigurationViewModel(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId, CMSFormWidgetConfigurationViewModel viewModel)
        {
            viewModel = HelperUtility.IsNotNull(viewModel) ? viewModel : new CMSFormWidgetConfigurationViewModel();

            viewModel.CMSMappingId = mappingId;
            viewModel.CMSWidgetsId = widgetId;
            viewModel.WidgetsKey = widgetKey;
            viewModel.TypeOFMapping = mappingType;
            viewModel.LocaleId = localeId;
            viewModel.Locales = _localeAgent.GetLocalesList(localeId);
            viewModel.FormBuilder = _formBuilderAgent.GetFormBuilderList(new ExpandCollection(), new FilterCollection(), new SortCollection(), null, null)?.FormBuilderList;
            return viewModel;
        }

        #endregion
    }
}