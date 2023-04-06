using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
using System.Net;

namespace Znode.Engine.Admin.Agents
{
    public class SEOSettingAgent : BaseAgent, ISEOSettingAgent
    {
        #region Private Variables
        private readonly ISEOSettingClient _client;
        private readonly IPublishProductClient _publishedProductClient;
        private readonly IPublishCategoryClient _publishedCategoryClient;
        private readonly IPortalClient _portalClient;
        private readonly IContentAgent _contentAgent;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Public Constructor
        public SEOSettingAgent(ISEOSettingClient client, IPortalClient portalClient, IPublishProductClient publishedProductClient, IPublishCategoryClient publishedCategoryClient)
        {
            _client = GetClient<ISEOSettingClient>(client);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _publishedProductClient = GetClient<IPublishProductClient>(publishedProductClient);
            _publishedCategoryClient = GetClient<IPublishCategoryClient>(publishedCategoryClient);
            _contentAgent = new ContentAgent(_portalClient, GetClient<ContentPageClient>(), GetClient<CMSWidgetConfigurationClient>(), GetClient<PortalProfileClient>(), GetClient<TemplateClient>(), _client, GetClient<LocaleClient>());
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
        }
        #endregion

        #region public virtual Methods

        #region Default SEO Settings
        //Get the Default Portal SEO Settings
        public virtual PortalSEOSettingViewModel GetPortalSEOSetting(int portalId)
        {
            PortalSEOSettingViewModel portalSEOSettingViewModel;

            //Get Default SEO Setting for the Portal.
            if (portalId > 0)
                return GetPortalSEOSettingDetails(portalId);

            portalSEOSettingViewModel = new PortalSEOSettingViewModel();
            //Bind the Available Portal List.
            SetPortalSelectItemList(portalSEOSettingViewModel);
            //Select the First Portal to get the SEO Details
            string firstPortalId = portalSEOSettingViewModel?.PortalList?.Select(x => x.Value)?.FirstOrDefault();

            //Get Default SEO Setting for the Portal.
            if (!string.IsNullOrEmpty(firstPortalId))
                return GetPortalSEOSettingDetails(Convert.ToInt32(firstPortalId));

            return portalSEOSettingViewModel;
        }

        //Create the Default Portal SEO Settings
        public virtual PortalSEOSettingViewModel CreatePortalSEOSetting(PortalSEOSettingViewModel model)
        {
            PortalSEOSettingViewModel viewModel = new PortalSEOSettingViewModel();
            if (IsNotNull(model))
            {
                viewModel = _client.CreatePortalSEOSetting(model.ToModel<PortalSEOSettingModel>()).ToViewModel<PortalSEOSettingViewModel>();
                SetPortalSelectItemList(viewModel);
            }
            return viewModel;
        }

        //Update the Default Portal SEO Settings
        public virtual PortalSEOSettingViewModel UpdatePortalSEOSetting(PortalSEOSettingViewModel model)
        {
            PortalSEOSettingViewModel viewModel = new PortalSEOSettingViewModel();
            if (IsNotNull(model))
            {
                viewModel = _client.UpdatePortalSEOSetting(model.ToModel<PortalSEOSettingModel>()).ToViewModel<PortalSEOSettingViewModel>();
                SetPortalSelectItemList(viewModel);
            }
            return viewModel;
        }
        #endregion

        //Get Portal List.
        public virtual List<SelectListItem> GetPortalList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            PortalSEOSettingViewModel viewModel = new PortalSEOSettingViewModel();
            SetPortalSelectItemList(viewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return viewModel.PortalList;
        }

        //Get Publish Product List for the SEO.
        public virtual ProductDetailsListViewModel GetPublishedProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeConstant.AdminSEO.ToString());

            //SetLocaleFilterIfNotPresent(ref filters);
            if (!filters.Any(x => string.Equals(x.FilterName, FilterKeys.ZnodeCategoryIds)))
                filters.Add(FilterKeys.ZnodeCategoryIds, FilterOperators.NotEquals, "0");
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            SetFilterForLocale(filters, localeId);

            //Get portal Id and portal List.
            PortalSEOSettingViewModel viewModel = GetportalID(portalId);

            //Get publish catalog Id by Portal Id.
            int publishCatalogId = GetPublishCatalogId(viewModel.PortalId);

            ProductDetailsListViewModel productDetails = new ProductDetailsListViewModel() { PortalId = viewModel.PortalId, StoreName = viewModel.StoreName };
            productDetails.Locales = _localeAgent.GetLocalesList(localeId);
            if (publishCatalogId > 0)
            {
                //Get product details according to publishCatalogId. 
                GetProductDetails(expands, filters, sorts, pageIndex, pageSize, viewModel, publishCatalogId, productDetails);
                return productDetails;
            }
            else
                return new ProductDetailsListViewModel() { PortalId = viewModel.PortalId };
        }

        //Get Products list for SEO
        public virtual ProductDetailsListViewModel GetProductsForSEO(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            ExpandCollection expands = new ExpandCollection();
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            SetFilterForLocale(filters, localeId);

            //Get portal Id and portal List.
            PortalSEOSettingViewModel viewModel = GetportalID(portalId);

            ProductDetailsListViewModel productDetails = new ProductDetailsListViewModel() { PortalId = viewModel.PortalId, StoreName = viewModel.StoreName };
            productDetails.Locales = _localeAgent.GetLocalesList(localeId);

            //Get product details. 
            GetProductsDetailsForSEO(expands, filters, sorts, pageIndex, pageSize, viewModel, productDetails);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return productDetails;
        }

        //Get Publish Category List for the SEO.
        public virtual CategoryListViewModel GetPublishedCategories(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeConstant.SEO.ToString());
            //SetLocaleFilterIfNotPresent(ref filters);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            SetFilterForLocale(filters, localeId);

            //Get portal Id and portal List.
            PortalSEOSettingViewModel viewModel = GetportalID(portalId);

            //Get publish catalog Id by portal id.
            int publishCatalogId = GetPublishCatalogId(viewModel.PortalId);
            CategoryListViewModel categoryList = new CategoryListViewModel() { PortalId = viewModel.PortalId, StoreName = viewModel.StoreName };
            categoryList.Locales = _localeAgent.GetLocalesList(localeId);
            if (publishCatalogId > 0)
                //Get category details according to publishCatalogId. 
                GetCategoryList(expands, filters, sorts, pageIndex, pageSize, publishCatalogId, categoryList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return categoryList;
        }

        //Create the SEO Details.
        public virtual SEODetailsViewModel CreateSEODetails(SEODetailsViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(model))
                {
                    string seoPageName = model.ItemName;
                    string seoTypeName = model.SEOTypeName;
                    int? productId = model.PimProductId;
                    model.SEOCode = WebUtility.HtmlDecode(model.SEOCode);
                    model.RobotTagValue = IsNotNull(model.RobotTag) ? model.RobotTag.ToString() : RobotTag.None.ToString();
                    //Create the New SEO Details.
                    model = _client.CreateSEODetails(model.ToModel<SEODetailsModel>()).ToViewModel<SEODetailsViewModel>();
                    if (IsNotNull(model))
                    {
                        model.ItemName = seoPageName;
                        model.SEOTypeName = seoTypeName;
                    }
                    model.PimProductId = productId;
                }
                if (model?.CMSSEODetailId <= 0)
                {
                    return (SEODetailsViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.SeoEnterAnyInputMessage);
                }                
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                SetErrorMessage(ex.ErrorCode, model);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (SEODetailsViewModel)GetViewModelWithErrorMessage(new SEODetailsViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Update the SEO Details
        public virtual SEODetailsViewModel UpdateSEODetails(SEODetailsViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(model))
                {
                    model.LocaleId = model.LocaleId == 0 ? GetLocaleValue() : model.LocaleId;
                    string seoTypeName = model.SEOTypeName;
                    int? productId = model.PimProductId;
                    model.SEOCode = WebUtility.HtmlDecode(model.SEOCode);
                    model.RobotTagValue = IsNotNull(model.RobotTag) ? model.RobotTag.ToString() : RobotTag.None.ToString();
                    model = _client.UpdateSEODetails(model.ToModel<SEODetailsModel>()).ToViewModel<SEODetailsViewModel>();

                    if (IsNotNull(model))
                    {
                        model.SEOTypeName = seoTypeName;
                        if (!string.IsNullOrEmpty(model?.RobotTagValue))
                            model.RobotTag = (RobotTag)Enum.Parse(typeof(RobotTag), model.RobotTagValue, true);
                    }
                    model.PimProductId = productId;
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                SetErrorMessage(ex.ErrorCode, model);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (SEODetailsViewModel)GetViewModelWithErrorMessage(new SEODetailsViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get the SEO Details for the Publish Product, Publish Category and Content Pages.
        public virtual SEODetailsViewModel GetSEODetails(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SEODetailsViewModel seoDetailViewModel;
            if (!string.IsNullOrEmpty(seoCode))
            {
                localeId = localeId == 0 ? GetLocaleValue() : localeId;
                seoDetailViewModel = _client.GetSEODetailsBySEOCode(seoCode, seoTypeId, localeId, portalId).ToViewModel<SEODetailsViewModel>();
                if (IsNotNull(seoDetailViewModel))
                {
                    seoDetailViewModel.Locales = new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList(seoDetailViewModel.LocaleId);
                    seoDetailViewModel.PortalId = portalId;
                    if (!string.IsNullOrEmpty(seoDetailViewModel?.RobotTagValue))
                        seoDetailViewModel.RobotTag = (RobotTag)Enum.Parse(typeof(RobotTag), seoDetailViewModel.RobotTagValue, true);
                    return seoDetailViewModel;
                }
            }
            seoDetailViewModel = new SEODetailsViewModel() { PortalId = portalId };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return seoDetailViewModel;
        }

        //Set Filter for portalId.
        public virtual FilterCollection SetFilter(FilterCollectionDataModel model, int portalId, List<SelectListItem> portalList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Checking For PortalId already Exists in Filters Or Not                      
            if (model.Filters.Exists(x => x.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower()))
                //If PortalId Already present in filters Remove It    
                model.Filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower());

            if (portalId > 0)
                model.Filters.Add(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());
            else
                model.Filters.Add(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());


            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model.Filters;
        }

        public virtual FilterCollection SetFilterForLocale(FilterCollectionDataModel model, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (model.Filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                model.Filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                model.Filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                model.Filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model.Filters;
        }

        // Get Content Page List Details.
        public virtual ContentPageListViewModel GetContentPageDetails(FilterCollectionDataModel model, int portalId, List<SelectListItem> PortalList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            SetFilterForLocale(model, localeId);

            ContentPageListViewModel contentPageList = _contentAgent.GetContentPageListViewModel(model, -1, true);

            contentPageList.PortalList = PortalList;
            portalId = portalId <= 0 ? Convert.ToInt32(GetPortalList()?.ToList()?.FirstOrDefault().Value) : portalId;
            contentPageList.PortalId = portalId;
            contentPageList.SEOTypeId = (int)SEOType.ContentPage;
            contentPageList.Locales = _localeAgent.GetLocalesList(GetLocaleValue());
            foreach (var portalData in contentPageList.PortalList)
            {
                if (Equals(portalData.Value, portalId.ToString()))
                {
                    contentPageList.PortalName = portalData.Text;
                    return contentPageList;
                }
            }
            return contentPageList;
        }
        public bool Publish(string seoCode, int portalId, int localeId, int seoTypeId, out string errorMessage, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                //decode the seoCode and replace character '&' with its ASCII value '038' in order to pass it to API controller.
                seoCode = WebUtility.HtmlDecode(HttpUtility.UrlDecode(seoCode))?.Replace("&", "038");
                var result = _client.Publish(seoCode, portalId, localeId, seoTypeId, targetPublishState, takeFromDraftFirst);

                if (IsNull(result?.IsPublished) || result?.IsPublished == false)
                    errorMessage = string.IsNullOrEmpty(result?.ErrorMessage) ? Admin_Resources.ErrorPublished : result?.ErrorMessage;
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.ErrorMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = Admin_Resources.ErrorDataRequired;
                        return false;
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.ErrorSEOURLSameName;
                        return false;
                    case ErrorCodes.StoreNotPublishedForAssociatedEntity:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.EntityNotFoundDuringPublish:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.ProductSeoPublishError:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.CategorySeoPublishError:
                        errorMessage = ex.Message;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorPublished;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        [Obsolete("To be discontinued in one of the upcoming versions.")]
        public bool Publish(string seoCode, int portalId, int localeId, int seoTypeId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = PIM_Resources.ErrorPublished;
            if (localeId == 0)
                localeId = GetLocaleValue();
            try
            {
                var result = _client.Publish(seoCode, portalId, localeId, seoTypeId);
                errorMessage = result?.ErrorMessage;
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.ErrorMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = Admin_Resources.ErrorDataRequired;
                        return false;

                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.ErrorSEOURLSameName;
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_productCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_productCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_productCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return localeId;
        }

        //Get Category List for the SEO.
        public virtual CategoryListViewModel GetCategoriesForSEO(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            ExpandCollection expands = new ExpandCollection();

            SetFilterForLocale(filters, localeId);

            //Get portal Id and portal List.
            PortalSEOSettingViewModel viewModel = GetportalID(portalId);

            CategoryListViewModel categoryList = new CategoryListViewModel() { PortalId = viewModel.PortalId, StoreName = viewModel.StoreName };
            categoryList.Locales = _localeAgent.GetLocalesList(localeId);
            if (viewModel.PortalId > 0)
                //Get category details according to PortalId. 
                GetCategoryListForSEO(expands, filters, sorts, pageIndex, pageSize, categoryList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return categoryList;
        }

        #endregion

        #region Private Methods
        //Set the Available Portal, and Bind the Drop Down Values.
        private void SetPortalSelectItemList(PortalSEOSettingViewModel viewModel)
        {
            List<PortalModel> portalList = _portalClient.GetPortalList(null, null, null, null, null)?.PortalList;
            if (portalList?.Count > 0)
            {
                ZnodeLogging.LogMessage("portalList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalList?.Count());
                viewModel.PortalList = portalList.OrderBy(x => x.StoreName).ToList().Select(x => new SelectListItem { Text = x.StoreName, Value = x.PortalId.ToString(), Selected = viewModel.PortalId == x.PortalId }).ToList();
            }
            foreach (var portalData in viewModel.PortalList)
            {
                if (Equals(portalData.Value, viewModel.PortalId.ToString()))
                    viewModel.StoreName = portalData.Text;
            }
        }

        //Set the Error Message for the SEO Detail Model.
        private void SetErrorMessage(int? errorCode, SEODetailsViewModel seoDetailsViewModel)
        {
            switch (errorCode)
            {
                case ErrorCodes.InvalidData:
                    seoDetailsViewModel.ErrorMessage = Admin_Resources.ErrorEditSEODetail;
                    break;
                case ErrorCodes.AlreadyExist:
                    seoDetailsViewModel.ErrorMessage = Admin_Resources.ErrorSEOURLSameName;
                    break;
            }
            seoDetailsViewModel.HasError = true;
        }

        //Set the Values for the Product List
        private static void SetProductListViewModel(PublishProductListModel productListModel, ProductDetailsListViewModel productDetails)
        {
            productListModel?.PublishProducts?.ForEach(product => productDetails.ProductDetailList.Add(new ProductDetailsViewModel
            {
                Image = product.ProductImagePath,
                ProductName = product.ProductName,
                SKU = product.SKU,
                ItemId = product.PimProductId,
                IsActive = product.IsActive,
                SEOUrl = product.SEOUrl,
                SEODescription = product.SEODescription,
                SEOKeywords = product.SEOKeywords,
                SEOTitle = product.SEOTitle,
                PublishStatus = product.IsPublish,
                SEOCode = product.SEOCode,


            }));
        }

        //Set the Values for the Category List
        private static void SetCategoryListViewModel(PublishCategoryListModel categoryListModel, CategoryListViewModel categoryList)
        {
            categoryListModel.PublishCategories.ForEach(category => categoryList.Categories.Add(new CategoryViewModel
            {
                SEOCode = category.SEOCode,
                CategoryName = category.CategoryName,
                Status = category.IsActive.ToString(),
                SEOUrl = category.SEOUrl,
                SEOTitle = category.SEOTitle,
                SEOKeywords = category.SEOKeywords,
                SEODescription = category.SEODescription,
                PublishStatus = Convert.ToString(category.IsPublish)
            }));
        }

        //Get the Default SEO Detail for the Portal.
        private PortalSEOSettingViewModel GetPortalSEOSettingDetails(int portalId)
        {
            PortalSEOSettingViewModel portalSEOSettingViewModel = _client.GetPortalSEOSetting(portalId).ToViewModel<PortalSEOSettingViewModel>();
            if (IsNotNull(portalSEOSettingViewModel))
            {
                SetPortalSelectItemList(portalSEOSettingViewModel);

                return portalSEOSettingViewModel;
            }
            portalSEOSettingViewModel = new PortalSEOSettingViewModel();
            portalSEOSettingViewModel.PortalId = portalId;

            SetPortalSelectItemList(portalSEOSettingViewModel);
            return portalSEOSettingViewModel;
        }

        //Get PublishCatalogId by portalId.
        private int GetPublishCatalogId(int portalId)
        {
            if (portalId > 0)
            {
                PortalModel model = _portalClient.GetPortal(portalId, null);
                return IsNotNull(model) ? model.PublishCatalogId.Value : 0;
            }
            else return 0;
        }

        // Get Category list according to publishCatalogId.       
        private void GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int publishCatalogId, CategoryListViewModel categoryList)
        {
            filters.RemoveAll(x => x.FilterName == FilterKeys.ZnodeCatalogId);
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, categoryList.PortalId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            PublishCategoryListModel categoryListModel = _publishedCategoryClient.GetPublishCategoryList(expands, filters, sorts, pageIndex, pageSize);

            if (categoryListModel?.PublishCategories?.Count > 0)
                SetCategoryListViewModel(categoryListModel, categoryList);

            // Set List Paging Details.
            SetListPagingData(categoryList, categoryListModel);
        }

        // Get Category list according to PortalId.       
        private void GetCategoryListForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, CategoryListViewModel categoryList)
        {
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, categoryList.PortalId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            PublishCategoryListModel categoryListModel = _publishedCategoryClient.GetCategoryListForSEO(expands, filters, sorts, pageIndex, pageSize);

            if (categoryListModel?.PublishCategories?.Count > 0)
                SetCategoryListViewModel(categoryListModel, categoryList);

            // Set List Paging Details.
            SetListPagingData(categoryList, categoryListModel);
        }

        //Get product details according to publishCatalogId. 
        private void GetProductDetails(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, PortalSEOSettingViewModel viewModel, int publishCatalogId, ProductDetailsListViewModel productDetails)
        {
            filters.RemoveAll(x => x.FilterName == FilterKeys.ZnodeCatalogId);
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, viewModel.PortalId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            PublishProductListModel productListModel = _publishedProductClient.GetPublishProductList(expands, filters, sorts, pageIndex, pageSize);
            productDetails.PortalList = viewModel.PortalList;
            productDetails.Locale = viewModel.Locale;
            //Get Image path.
            SetImagePath(productListModel);

            //Set the Values for the Product List
            SetProductListViewModel(productListModel, productDetails);

            // Set List Paging Details.
            SetListPagingData(productDetails, productListModel);
        }

        //Get product details according to publishCatalogId. 
        private void GetProductsDetailsForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, PortalSEOSettingViewModel viewModel, ProductDetailsListViewModel productDetails)
        {
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, viewModel.PortalId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });
            PublishProductListModel productListModel = _client.GetProductsForSEO(expands, filters, sorts, pageIndex, pageSize);
            productDetails.PortalList = viewModel.PortalList;
            productDetails.Locale = viewModel.Locale;

            //Set the Values for the Product List
            SetProductListViewModel(productListModel, productDetails);

            // Set List Paging Details.
            SetListPagingData(productDetails, productListModel);
        }
        //Get portal list.
        private PortalSEOSettingViewModel GetportalID(int portalId)
        {
            PortalSEOSettingViewModel viewModel = new PortalSEOSettingViewModel();

            portalId = portalId <= 0 ? Convert.ToInt32(GetPortalList()?.ToList()?.FirstOrDefault().Value) : portalId;
            //Set the Available Portal, and Bind the Drop Down Values.
            viewModel.PortalId = portalId;
            SetPortalSelectItemList(viewModel);
            return viewModel;
        }
        private void SetFilterForLocale(FilterCollection filters, int localeId)
        {
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
        }


        #endregion

        //Get the SEO details for product page.
        [Obsolete]
        public virtual SEODetailsViewModel GetProductSEODetails(int itemId, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SEODetailsViewModel seoDetailViewModel;
            if (itemId > 0)
            {
                localeId = localeId == 0 ? GetLocaleValue() : localeId;
                var portalList = GetPortalList();
                ZnodeLogging.LogMessage("portalList:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { portalList = portalList });

                if (portalId == 0)
                    portalId = Convert.ToInt32(portalList?.OrderBy(a => Convert.ToInt32(a.Value)).FirstOrDefault()?.Value);
                seoDetailViewModel = _client.GetSEODetailId(itemId, seoTypeId, localeId, portalId).ToViewModel<SEODetailsViewModel>();
                if (IsNotNull(seoDetailViewModel))
                {
                    seoDetailViewModel.Locales = new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList(seoDetailViewModel.LocaleId);
                    seoDetailViewModel.PortalId = portalId;
                    seoDetailViewModel.Portals = portalList;
                    seoDetailViewModel.SEOId = itemId; // This is publish productId.                   
                    return seoDetailViewModel;
                }
            }
            seoDetailViewModel = new SEODetailsViewModel() { PortalId = portalId };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return seoDetailViewModel;
        }

        //Get the SEO details for product page.
        public virtual SEODetailsViewModel GetSEODetailsBySEOCode(int seoTypeId, int localeId, int portalId, string seoCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SEODetailsViewModel seoDetailViewModel;
            if (!string.IsNullOrEmpty(seoCode))
            {
                localeId = localeId == 0 ? GetLocaleValue() : localeId;
                var portalList = GetPortalList();
                ZnodeLogging.LogMessage("portalList:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { portalList = portalList });
                if (portalId == 0)
                    portalId = Convert.ToInt32(portalList?.OrderBy(a => Convert.ToInt32(a.Value)).FirstOrDefault()?.Value);
                seoDetailViewModel = _client.GetSEODetailsBySEOCode(seoCode, seoTypeId, localeId, portalId).ToViewModel<SEODetailsViewModel>();

                if (IsNotNull(seoDetailViewModel))
                {
                    seoDetailViewModel.Locales = new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList(seoDetailViewModel.LocaleId);
                    seoDetailViewModel.PortalId = portalId;
                    seoDetailViewModel.Portals = portalList;
                    seoDetailViewModel.SEOCode = seoCode;//SKU if product,CategoryCode if Category,BrandCode if Brand,pagename if ContentPage and blog&NewsCode if Blog&News.
                    if (!string.IsNullOrEmpty(seoDetailViewModel?.RobotTagValue))
                        seoDetailViewModel.RobotTag = (RobotTag)Enum.Parse(typeof(RobotTag), seoDetailViewModel.RobotTagValue, true);
                    return seoDetailViewModel;
                }
            }
            seoDetailViewModel = new SEODetailsViewModel() { PortalId = portalId };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return seoDetailViewModel;
        }

        //Get the SEO details for product page.
        public virtual SEODetailsViewModel GetDefaultSEODetails(int seoTypeId, int localeId, int portalId, string seoCode, int itemId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            SEODetailsViewModel seoDetailViewModel;
            if (!string.IsNullOrEmpty(seoCode))
            {
                localeId = localeId == 0 ? GetLocaleValue() : localeId;
                var portalList = GetPortalList();
                ZnodeLogging.LogMessage("portalList:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { portalList = portalList });
                if (portalId == 0)
                    portalId = Convert.ToInt32(portalList?.OrderBy(a => Convert.ToInt32(a.Value)).FirstOrDefault()?.Value);
                seoDetailViewModel = _client.GetDefaultSEODetails(seoCode, seoTypeId, localeId, portalId, itemId).ToViewModel<SEODetailsViewModel>();

                if (IsNotNull(seoDetailViewModel))
                {
                    seoDetailViewModel.Locales = new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList(seoDetailViewModel.LocaleId);
                    seoDetailViewModel.PortalId = portalId;
                    seoDetailViewModel.Portals = portalList;
                    seoDetailViewModel.SEOCode = seoCode;//SKU if product,CategoryCode if Category,BrandCode if Brand,pagename if ContentPage and blog&NewsCode if Blog&News.
                    if (!string.IsNullOrEmpty(seoDetailViewModel?.RobotTagValue))
                        seoDetailViewModel.RobotTag = (RobotTag)Enum.Parse(typeof(RobotTag), seoDetailViewModel.RobotTagValue, true);
                    return seoDetailViewModel;
                }
            }
            seoDetailViewModel = new SEODetailsViewModel() { PortalId = portalId };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return seoDetailViewModel;
        }
        public virtual ContentPageViewModel GetDefaultCMSSEODetails(int seoTypeId, int localeId, int portalId, string seoCode, int itemId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            seoTypeId = (int)SEODetailsEnum.Content_Page;
            seoCode = "ContentPage";
            SEODetailsViewModel seoDetailViewModel;
            seoDetailViewModel = _client.GetDefaultSEODetails(seoCode, seoTypeId, localeId, portalId, itemId).ToViewModel<SEODetailsViewModel>();
            //Get Content Page Details by Id.
            ContentPageViewModel contentPageModel = new ContentPageViewModel();
            if(IsNotNull(seoDetailViewModel))
            {
                contentPageModel.SEOTitle = seoDetailViewModel.SEOTitle;
                contentPageModel.SEOKeywords = seoDetailViewModel.SEOKeywords;
                contentPageModel.SEODescription = seoDetailViewModel.SEODescription;
            }
            return HelperUtility.IsNotNull(contentPageModel) ? contentPageModel : new ContentPageViewModel();
        }

        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        public virtual bool DeleteSeo(int seoTypeId, int portalId, string seoCode = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return _client.DeleteSeo(seoTypeId, portalId, seoCode);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
    }
}