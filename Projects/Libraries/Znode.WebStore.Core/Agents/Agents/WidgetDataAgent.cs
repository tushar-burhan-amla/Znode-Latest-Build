using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Agents
{
    public class WidgetDataAgent : BaseAgent, IWidgetDataAgent
    {
        #region Private Variables
        //Field(s) not marked as readonly is intentional. Since on Locale change, the client instance has to be recycled.
        private IWebStoreWidgetClient _widgetClient;
        private readonly IPublishProductClient _productClient;
        private readonly ICategoryAgent _categoryAgent;
        private readonly ISearchAgent _searchAgent;
        private readonly ICartAgent _cartAgent;
        private readonly IProductAgent _productAgent;
        private readonly IBlogNewsClient _blogNewsClient;
        private readonly IContentPageClient _contentPageClient;
        private readonly IPublishCategoryClient _publishCategoryClient;
        private readonly ICMSPageSearchClient _cmsPageSearchClient;
        private readonly IWebstoreHelper _webstoreHelper;
        #endregion

        #region Constructor
        public WidgetDataAgent(IWebStoreWidgetClient widgetClient, IPublishProductClient productClient, IPublishCategoryClient publishCategoryClient, IBlogNewsClient blogNewsClient, IContentPageClient contentPageClient, ISearchClient searchClient, ICMSPageSearchClient cmsPageSearchClient)
        {
            _widgetClient = GetClient<IWebStoreWidgetClient>(widgetClient);
            _productClient = GetClient<IPublishProductClient>(productClient);
            _categoryAgent = new CategoryAgent(GetClient<WebStoreCategoryClient>(), GetClient<PublishCategoryClient>(), GetClient<PublishBrandClient>());
            _searchAgent = DependencyResolver.Current.GetService<ISearchAgent>();
            _cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<AccountQuoteClient>(), GetClient<UserClient>());
            _productAgent = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>());
            _blogNewsClient = GetClient<IBlogNewsClient>(blogNewsClient);
            _contentPageClient = GetClient(contentPageClient);
            _publishCategoryClient = GetClient(publishCategoryClient);
            _cmsPageSearchClient = GetClient<ICMSPageSearchClient>(cmsPageSearchClient);
            _webstoreHelper = GetService<IWebstoreHelper>();
        }
        #endregion

        #region Public Methods
        //Async call to get slider data.
        public virtual async Task<WidgetSliderBannerViewModel> GetSliderAsync(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            WebStoreWidgetParameterModel webStoreWidgetParameterModel = new WebStoreWidgetParameterModel();
            webStoreWidgetParameterModel.CMSMappingId = parameter.CMSMappingId;
            webStoreWidgetParameterModel.WidgetCode = parameter.WidgetCode;
            webStoreWidgetParameterModel.WidgetKey = parameter.WidgetKey;
            webStoreWidgetParameterModel.TypeOfMapping = parameter.TypeOfMapping;

            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            var model = await _widgetClient.GetSliderAsync(webStoreWidgetParameterModel);
            WidgetSliderBannerViewModel widgetSliderBannerViewModel = model?.CMSSliderId > 0 ? /*model.ToViewModel<WidgetSliderBannerViewModel>()*/ToViewModel(model) : new WidgetSliderBannerViewModel();

            ZnodeLogging.LogMessage("widgetSliderBannerViewModel with WidgetSliderBannerId:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { WidgetSliderBannerId = widgetSliderBannerViewModel.WidgetSliderBannerId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return widgetSliderBannerViewModel;
        }

        //Get slider data.
        public virtual WidgetSliderBannerViewModel GetSlider(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            string cacheKey = string.Concat(CachedKeys.SliderBannerKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                WidgetSliderBannerViewModel model = GetSliderData(parameter);

                //Get Cache Duration Settings By MappingKey.
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey, "SliderBannerCacheDuration");
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return Helper.GetFromCache<WidgetSliderBannerViewModel>(cacheKey);
        }

        //Get cached product list data.
        public virtual WidgetProductListViewModel GetProducts(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            string cacheKey = string.Concat(CachedKeys.ProductListKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GetCatalogId().GetValueOrDefault(), GenerateKey(parameter));
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                WidgetProductListViewModel model = GetProductsData(parameter);

                if (model?.Products?.Count > 0)
                {
                    foreach (WidgetProductViewModel product in model.Products)
                    {
                        string minQuantity = product.ProductViewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity);
                        decimal? minimumQuantity = !string.IsNullOrEmpty(minQuantity) ? Convert.ToDecimal(product.ProductViewModel.Attributes?.Value(ZnodeConstant.MinimumQuantity)) : 0;                        //
                    }
                }
                Helper.AddIntoCache(model, cacheKey, "ProductListCacheDuration");
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return Helper.GetFromCache<WidgetProductListViewModel>(cacheKey);
        }


        //Get the Details Of Media Widget
        public virtual WidgetMediaViewModel GetMediaWidgetDetails(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            string cacheKey = string.Concat(CachedKeys.MediaKey_, Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));

            WidgetMediaViewModel mediaViewModel = GetMediaWidget(parameter);

            return mediaViewModel;
        }

        //Get link widget data.
        public virtual WidgetTitleListViewModel GetLinkData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            string cacheKey = string.Concat(CachedKeys.LinkKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            WidgetTitleListViewModel model = null;
            if (HelperUtility.IsNotNull(HttpContext.Current.Cache[cacheKey]))
                model = Helper.GetFromCache<WidgetTitleListViewModel>(cacheKey);

            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]) || HelperUtility.IsNull(model?.TitleList) || model.TitleList.Count < 1)
            {
                model = GetLinkWidget(parameter);

                object _requiredImage = false;
                parameter.properties?.TryGetValue("IsImageRequired", out _requiredImage);

                model.IsImageRequired = Convert.ToBoolean(_requiredImage);

                //Get Cache Duration Settings By MappingKey.
                if (HelperUtility.IsNotNull(model?.TitleList))
                    Helper.AddIntoCache(model, cacheKey, "LinkWidgetCacheDuration");

            }

            ZnodeLogging.LogMessage("TitleList count:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { TitleListCount = model?.TitleList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model;
        }

        //Get cached category list data.
        public virtual WidgetCategoryListViewModel GetCategories(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            string cacheKey = string.Concat(CachedKeys.CategoryListKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                WidgetCategoryListViewModel model = GetCategoriesData(parameter);

                //Get Cache Duration Settings By MappingKey.
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey);

            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return Helper.GetFromCache<WidgetCategoryListViewModel>(cacheKey);
        }

        //Get cached facets list data.
        public virtual SearchResultViewModel GetFacetList(WidgetParameter parameter, int pageNum = 1, int pageSize = 16, int sortValue = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            SearchResultViewModel viewModel = new SearchResultViewModel();

            SearchRequestViewModel requestModel = GetSearchRequestModel(parameter, pageNum, pageSize);

            requestModel.IsFacetList = true;

            object brandName = string.Empty;
            parameter.properties?.TryGetValue(ZnodeConstant.Brand, out brandName);
            FilterCollection filters = GetRequiredFilters();
            filters.Add(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.Equals, parameter.CMSMappingId.ToString());
            filters.Add("IsFacet", FilterOperators.Equals, "True");

            KeywordSearchModel searchResult = GetSearchResult(requestModel, filters, true);
            SetPagingParameters(parameter, pageNum, pageSize, sortValue, requestModel);

            SetFilterParameter(parameter, viewModel, brandName?.ToString());

            if (HelperUtility.IsNotNull(searchResult))
            {
                viewModel.Location = searchResult.Location;
                viewModel.Facets = SearchMap.ToFacetViewModel(searchResult.Facets, parameter.CMSMappingId, parameter.TypeOfMapping, requestModel.SearchTerm, requestModel.BrandName);
                viewModel.FacetFilters = SetFilterParameter(parameter.CMSMappingId, parameter.TypeOfMapping, parameter.properties, viewModel.Facets);
                if (Equals(parameter.TypeOfMapping, ZnodeConstant.Brand))
                {
                    viewModel.BrandId = parameter.CMSMappingId;
                    viewModel.BrandName = requestModel.BrandName;
                }
                else
                {
                    viewModel.CategoryId = parameter.CMSMappingId;
                    viewModel.SearchTerm = requestModel.SearchTerm;
                }
                if (searchResult.Categories?.Count > 0)
                {
                    viewModel.Categories = searchResult.Categories.ToViewModel<SearchCategoryViewModel>().ToList();
                    viewModel.Categories.ForEach(x => x.Categoryurl = "?SearchTerm=" + HttpUtility.UrlEncode(requestModel.SearchTerm) + "&categoryId=" + x.CategoryId);
                }
            }

            ZnodeLogging.LogMessage("FacetList count:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { FacetListCount = viewModel?.Facets?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //Get Content pages list
        public virtual ContentPageListViewModel GetContentPages(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            WebStoreContentPageListModel list = GetContentPageList();

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return list?.ContentPageList?.Count > 0 ? new ContentPageListViewModel { ContentPageList = list?.ContentPageList?.ToViewModel<ContentPageViewModel>().ToList() }
                : new ContentPageListViewModel { ContentPageList = new List<ContentPageViewModel>() };
        }

        //get content of content page
        public virtual WidgetTextViewModel GetContent(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            // get data from cache ,if data is not cached make a call else get cached data for portal.
            WidgetTextViewModel model = null;
            string cacheKey = string.Concat(CachedKeys.TextWidgetKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            if (HelperUtility.IsNotNull(HttpContext.Current.Cache[cacheKey]))
                model = Helper.GetFromCache<WidgetTextViewModel>(cacheKey);

            if (HelperUtility.IsNull(model))
            {
                model = GetContentPageData(parameter);
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return model;
            }
            return model;
        }


        //Get Tag manager data (Meta tags , google tag manager)
        public virtual WidgetTextViewModel GetTagManager(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            string _value = HelperUtility.IsNotNull(parameter.properties) && parameter.properties.ContainsKey("ScriptKey") ? Convert.ToString(parameter.properties["ScriptKey"]) : string.Empty;
            //If data is not cached make a call else get cached data for Tag manager.
            string cacheKey = string.Concat(CachedKeys.TagManager_, Convert.ToString(PortalAgent.CurrentPortal?.PortalId), parameter.CMSMappingId, _value, PortalAgent.CurrentPortal.PublishState);
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                WidgetTextViewModel model = GetTagManagerData(parameter);

                //Get Cache Duration Settings By MappingKey.
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey, "CurrentPortalCacheDuration");
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return Helper.GetFromCache<WidgetTextViewModel>(cacheKey);
        }

        //Get sub categories of selected category
        public virtual List<CategoryViewModel> GetSubCategories(WidgetParameter widgetparameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Sorting For Category List.
            SortCollection sorts = new SortCollection();
            sorts.Add(SortKeys.DisplayOrder, SortDirections.Ascending);

            PublishCategoryListModel model = _publishCategoryClient.GetPublishCategoryList(new ExpandCollection { ZnodeConstant.SEO }, GetFilterForSubCategory(widgetparameter), sorts);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model?.PublishCategories?.Count > 0 ? model.PublishCategories.ToViewModel<CategoryViewModel>().ToList() : new List<CategoryViewModel>();
        }

        //Get category products of selected category.
        public virtual ProductListViewModel GetCategoryProducts(WidgetParameter widgetparameter, int pageNum = 1, int pageSize = 16, int sortValue = 0, bool isAssociatedCategoriesRequired = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ProductListViewModel productlist = new ProductListViewModel();
            SearchRequestViewModel searchRequestModel = GetSearchRequestModel(widgetparameter);
            searchRequestModel.IsFacetList = true;
            //Set category parameter.
            RemoveInSession(string.Format(WebStoreConstants.LastSelectedCategoryForPortal, PortalAgent.CurrentPortal.PortalId));
            SaveInSession(string.Format(WebStoreConstants.LastSelectedCategoryForPortal, PortalAgent.CurrentPortal.PortalId), widgetparameter.CMSMappingId);

            //Set paging parameters.
            SetPagingParameters(widgetparameter, pageNum, pageSize, sortValue, searchRequestModel);

            FilterCollection filters = GetFilterForProducts(widgetparameter);

            searchRequestModel.IsProductInheritanceEnabled = PortalAgent.CurrentPortal.IsProductInheritanceEnabled;
            KeywordSearchModel searchResult = GetSearchResult(searchRequestModel, filters);
                       
            GetSearchProducts(searchResult, productlist);

            //Bind facet details.
            productlist.SearchResultViewModel = BindCategoryFacet(searchResult, widgetparameter, searchRequestModel, isAssociatedCategoriesRequired);

            bool isEnableCompare = PortalAgent.CurrentPortal.EnableCompare;
            
            productlist?.Products?.ForEach(x => { x.CategoryId = widgetparameter.CMSMappingId; x.CategoryName = widgetparameter.TypeOfMapping; x.EnableProductCompare = isEnableCompare; x.HighlightLists = _webstoreHelper.GetHighlightListFromAttributes(x.Attributes, x.SKU, x.PublishProductId); });
            
            productlist.TotalProductCount = searchResult.TotalProductCount;
            productlist.TotalCMSPageCount = searchResult.TotalCMSPageCount;
            productlist.SuggestTerm = searchResult.SuggestTerm;
           
            //Map search profile id for search report.
            productlist.SearchProfileId = searchResult.SearchProfileId;
            //Binding Page Number,use to redirect on PDP page only if page number eqs to 1. 
            if (Convert.ToBoolean(widgetparameter.properties?.ContainsKey(ZnodeConstant.PageNumber)))
                productlist.PageNumber = Convert.ToInt32(widgetparameter?.properties[ZnodeConstant.PageNumber]);

            productlist.IsSearchFromSuggestions = searchResult.IsSearchFromSuggestion;
            productlist.SearchKeyword = searchRequestModel.SearchTerm;

            productlist.TotalPages = (searchRequestModel.PageSize.Equals(-1)) ? 1 : ((int)(searchResult.TotalProductCount + (searchRequestModel.PageSize - 1)) / searchRequestModel.PageSize).Value;
            int productsCount = productlist.Products?.Count ?? 0;

            productlist.SearchResultCountText = GetProductMessage(widgetparameter, searchRequestModel, searchResult, productsCount);

            //Set Found Products count.
            if (Equals(widgetparameter.TypeOfMapping, ZnodeConstant.Brand))
                productlist.SearchTextName = widgetparameter.properties[ZnodeConstant.Brand].ToString();
            else
                productlist.SearchTextName = widgetparameter.TypeOfMapping;

            ZnodeLogging.LogMessage("TotalProductCount:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { TotalProductCount = productlist?.TotalProductCount });
            return productlist;
        }


        protected virtual SearchResultViewModel BindFacet(KeywordSearchModel searchResult, WidgetParameter parameter, SearchRequestViewModel searchRequestModel)
        {
            SearchResultViewModel viewModel = new SearchResultViewModel();

            if (HelperUtility.IsNotNull(searchResult))
            {
                viewModel.Location = searchResult.Location;
                viewModel.Facets = SearchMap.ToFacetViewModel(searchResult.Facets, parameter.CMSMappingId, parameter.TypeOfMapping, searchRequestModel.SearchTerm, searchRequestModel.BrandName);
                viewModel.FacetFilters = SetFilterParameter(parameter.CMSMappingId, parameter.TypeOfMapping, parameter.properties, viewModel.Facets);
                if (Equals(parameter.TypeOfMapping, ZnodeConstant.Brand))
                {
                    viewModel.BrandId = parameter.CMSMappingId;
                    viewModel.BrandName = searchRequestModel.BrandName;

                }
                else
                {
                    viewModel.CategoryId = parameter.CMSMappingId;
                    viewModel.SearchTerm = searchRequestModel.SearchTerm;
                }
                if (searchResult.Categories?.Count > 0)
                {
                    viewModel.Categories = searchResult.Categories.ToViewModel<SearchCategoryViewModel>().ToList();
                    viewModel.Categories.ForEach(x => x.Categoryurl = "?SearchTerm=" + HttpUtility.UrlEncode(searchRequestModel.SearchTerm) + "&categoryId=" + x.CategoryId);
                }
            }
            return viewModel;
        }

        // Bind Associated Categories.
        protected virtual SearchResultViewModel BindCategoryFacet(KeywordSearchModel searchResult, WidgetParameter parameter, SearchRequestViewModel searchRequestModel, bool isAssociatedCategoryRequired)
        {
            SearchResultViewModel searchResultViewModel = BindFacet(searchResult, parameter, searchRequestModel);
            if (isAssociatedCategoryRequired)
            {
                if (searchResult?.AssociatedCategoryIds?.Count > 0)
                {
                    GetAssociatedCategoryList(searchResultViewModel, searchResult);
                }
            }
            return searchResultViewModel;
        }

        // Get associated category list.
        protected virtual void GetAssociatedCategoryList(SearchResultViewModel searchResultViewModel, KeywordSearchModel searchResult)
        {
            List<CategoryHeaderViewModel> parentCategories = _categoryAgent.GetCategories().Categories?.ToList();
            List<CategoryHeaderViewModel> categoryHeaderViewModelList = new List<CategoryHeaderViewModel>();

            foreach (CategoryHeaderViewModel parentcategory in parentCategories)
            {
                if (Convert.ToBoolean(searchResult?.AssociatedCategoryIds?.Contains(parentcategory.CategoryId)))
                    categoryHeaderViewModelList.Add(parentcategory);
                parentcategory?.SubCategoryItems?.ForEach(x =>
                {
                    if (Convert.ToBoolean(searchResult?.AssociatedCategoryIds?.Contains(x.CategoryId)))
                    {
                        categoryHeaderViewModelList?.Add(x.ToModel<CategoryHeaderViewModel, CategorySubHeaderViewModel>());
                    }
                    GetAssociatedChildCategoryList(searchResult, x, categoryHeaderViewModelList);
                });
            }
            searchResultViewModel.AssociatedCategoryList = categoryHeaderViewModelList?.GroupBy(x => x.CategoryCode)?.Select(g => g.First())?.ToList();
        }

        public virtual FilterCollection GetFilterForProducts(WidgetParameter parameter, bool isFacet = false)
        {
            FilterCollection filters = GetRequiredFilters();
            filters.Add(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.Equals, parameter.CMSMappingId.ToString());

            filters.Add(HelperUtility.StoreFeature.Enable_CMS_Page_Results.ToString(), FilterOperators.Equals,
                 Convert.ToString((PortalAgent.CurrentPortal.PortalFeatureValues?.FirstOrDefault(x => x.Key == HelperUtility.StoreFeature.Enable_CMS_Page_Results.ToString()).Value).GetValueOrDefault()));

            if (isFacet)
                filters.Add("IsFacet", FilterOperators.Equals, "True");

            //bind the store level inventory flag to get the product inventory of default and all locations set for the current store.
            bool isGetAllLocationsInventory = Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, ZnodeConstant.DisplayAllWarehousesStock, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);
            if (isGetAllLocationsInventory)
                filters.Add(ZnodeConstant.IsGetAllLocationsInventory, FilterOperators.Equals, isGetAllLocationsInventory.ToString());
            ZnodeLogging.LogMessage("Filters for products:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return filters;
        }

        //Get quick view details for a product.
        public virtual ProductViewModel GetProductQuickView(WidgetParameter widgetparameter)
        {
            //Set login user Id in client header to get profile, customer, account based pricing.
            SetLoginUserId();
            return _productAgent.GetProduct(widgetparameter.CMSMappingId);
        }

        //Get link products.
        public virtual List<LinkProductViewModel> GetLinkProductList(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            WebStoreLinkProductListModel linkProductList = _widgetClient.GetLinkProductList(ToWidgetParameterModel(parameter), GenerateKey(parameter), new ExpandCollection { ZnodeConstant.Promotions, ZnodeConstant.SEO });
 
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            if(linkProductList?.LinkProductList?.Count > 0)
            {
                List<LinkProductViewModel> linkProductViewModels = linkProductList?.LinkProductList.ToViewModel<LinkProductViewModel>().ToList();                
                linkProductViewModels.ForEach(y =>
                {
                    if (y.PublishProduct?.Count > 0)
                    {
                        for (int i = 0; i < y.PublishProduct.Count; i++)
                        {
                            if (PortalAgent.CurrentPortal.IsAddToCartOptionForProductSlidersEnabled)
                            {
                                y.PublishProduct[i] = _productAgent.GetProduct(y.PublishProduct[i].PublishProductId);
                                y.PublishProduct[i].IsAddToCartOptionForProductSlidersEnabled = true;
                            }
                            y.PublishProduct[i].HighlightLists = _webstoreHelper.GetHighlightListFromAttributes(y.PublishProduct[i].Attributes, y.PublishProduct[i].SKU, y.PublishProduct[i].PublishProductId);

                        }
                    }                                      
                });
                return linkProductViewModels;
            }
            return new List<LinkProductViewModel>();
        }

        //Get count of cart items.
        public virtual decimal GetCartCount()
        {
            decimal cartcount = _cartAgent.GetCartCount();

            return Convert.ToDecimal(Helper.GetRoundOffQuantity(cartcount));
        }

        //Get cached brand list data.
        public virtual WidgetBrandListViewModel GetBrands(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            string cacheKey = string.Concat(CachedKeys.BrandListKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                WidgetBrandListViewModel model = GetBrandsData(parameter);

                //Get Cache Duration Settings By MappingKey.
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey, "BrandListCacheDuration");

            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return Helper.GetFromCache<WidgetBrandListViewModel>(cacheKey);
        }

        //Get Form Builder Attribute Group Model information.
        public virtual WidgetFormConfigurationViewModel GetFormConfiguration(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //If data is not cached make a call else get cached data for portal.
            WidgetFormConfigurationViewModel model = null;
            string cacheKey = string.Concat(CachedKeys.FormAttributeGroupKey_, Convert.ToString(Helper.GetUserProfileCatalogId()), Convert.ToString(Helper.GetUserAccountId()), Convert.ToString(PortalAgent.CurrentPortal?.PortalId), GenerateKey(parameter));
            if (HelperUtility.IsNotNull(HttpContext.Current.Cache[cacheKey]))
                model = Helper.GetFromCache<WidgetFormConfigurationViewModel>(cacheKey);

            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            if (HelperUtility.IsNull(model))
            {
                model = _widgetClient.GetFormConfiguration(ToWidgetParameterModel(parameter)).ToViewModel<WidgetFormConfigurationViewModel>();
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return model;
            }
            return model;
        }

        public virtual WidgetSearchDataViewModel GetSearchWidgetData(WidgetParameter widgetparameter, int pageNum = 1, int pageSize = 16, int sortValue = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            SearchRequestViewModel searchRequestModel = GetSearchRequestModel(widgetparameter);

            //Set category parameter.
            RemoveInSession(string.Format(WebStoreConstants.LastSelectedCategoryForPortal, PortalAgent.CurrentPortal.PortalId));
            SaveInSession(string.Format(WebStoreConstants.LastSelectedCategoryForPortal, PortalAgent.CurrentPortal.PortalId), widgetparameter.CMSMappingId);

            //Set paging parameters.
            SetPagingParameters(widgetparameter, pageNum, pageSize, sortValue, searchRequestModel);

            WebStoreSearchWidgetParameterModel widgetParameterModel = ToSearchWidgetParameterModel(widgetparameter);
            widgetParameterModel.PageSize = searchRequestModel.PageSize;
            widgetParameterModel.PageNumber = searchRequestModel.PageNumber;
            widgetParameterModel.RefineBy = searchRequestModel.FilterList;
            widgetParameterModel.Sort = searchRequestModel.Sort;

            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            _widgetClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            WebStoreWidgetSearchModel model = _widgetClient.GetSearchWidgetData(widgetParameterModel, GetProductExpands(), GetRequiredFilters(), GetSortForSearch(searchRequestModel.Sort));
            WidgetSearchDataViewModel viewModel = new WidgetSearchDataViewModel();
            viewModel.Products = model.Products?.ToViewModel<ProductViewModel>()?.ToList();
            viewModel.Facets = SearchMap.ToFacetViewModel(model.Facets, widgetparameter.CMSMappingId, ZnodeConstant.SearchWidget, "", "");
            viewModel.FacetFilters = SetFilterParameter(widgetparameter.CMSMappingId, ZnodeConstant.SearchWidget, widgetparameter.properties, viewModel.Facets);
            viewModel.TotalPages = (searchRequestModel.PageSize.Equals(-1)) ? 1 : ((int)(model.TotalProductCount + (searchRequestModel.PageSize - 1)) / searchRequestModel.PageSize).Value;
            int productsCount = model.Products?.Count ?? 0;

            StringBuilder message = new StringBuilder();
            bool viewAll = GetFromSession<bool>(WebStoreConstants.ViewAllMode);
            if ((model.TotalProductCount > searchRequestModel.PageSize) && !viewAll)
                message.Append(string.Format(WebStore_Resources.TextFoundProductsOutOf, productsCount, model.TotalProductCount));
            else
                message.Append(string.Format(WebStore_Resources.TextFoundProducts, model.TotalProductCount));

            message.Append(string.IsNullOrEmpty(searchRequestModel.SearchTerm) ? string.Empty : string.Format(WebStore_Resources.TextForKeyword, searchRequestModel.SearchTerm));
            viewModel.SearchResultCountText = message.ToString();

            ZnodeLogging.LogMessage("Facets, FacetFilters and Products list count:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { FacetsCount = viewModel?.Facets?.Count, FacetFiltersCount = viewModel?.FacetFilters?.Count, ProductsCount = viewModel?.Products?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return viewModel;
        }

        //
        public virtual ECertTotalBalanceViewModel GetECertTotalBalance(WidgetParameter parameter, decimal availableBalance = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ECertTotalBalanceViewModel eCertTotalBalanceViewModel;
            try
            {
                eCertTotalBalanceViewModel = (availableBalance == 0 && HelperUtility.IsNotNull(parameter)) ? GetECertTotalBalanceData(parameter)?.ToViewModel<ECertTotalBalanceViewModel>() ?? new ECertTotalBalanceViewModel()
                                                                                               : new ECertTotalBalanceViewModel()
                                                                                               {
                                                                                                   AvailableTotal = availableBalance,
                                                                                                   TraceMessage = "Received available balance directly used."
                                                                                               };
            }
            catch (Exception)
            {
                eCertTotalBalanceViewModel = new ECertTotalBalanceViewModel()
                {
                    AvailableTotal = 0,
                    HasError = true,
                    ErrorMessage = WebStore_Resources.ErrorSelectRecord
                };
            }
            eCertTotalBalanceViewModel.CultureCode = PortalAgent.CurrentPortal.CultureCode;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return eCertTotalBalanceViewModel;
        }

        //Get page default size
        public virtual string GetPageDefaultSize()
        {
            string pageValue = Convert.ToString(PortalAgent.CurrentPortal?.PageList?.FirstOrDefault(m => m.IsDefault == true)?.PageValue);
            return string.IsNullOrEmpty(pageValue) ? "16" : pageValue;
        }

        //Get Default Page Size set at the store.
        public virtual int GetDefaultPageSize()
        {
            int? pageValue = PortalAgent.CurrentPortal?.PageList?.FirstOrDefault(m => m.IsDefault == true)?.PageValue;
            return pageValue > 0 ? pageValue.GetValueOrDefault() : 16;
        }

        //Get Default sort value set at the store.
        public virtual string GetPortalDefaultSortValue()
        {
            // Arrange the sort list assign to store as per display order and get the first record.
            // If the sort list not assign to store then it will return null value.
            string sortValue =Convert.ToString(PortalAgent.CurrentPortal?.SortList.OrderBy(x => x.DisplayOrder)?.FirstOrDefault()?.SortValue);
            return string.IsNullOrEmpty(sortValue)? null : sortValue;
        }

        //Get Container data.
        public virtual string GetContainer(WidgetParameter parameter)
            => _widgetClient.GetContainer(ToWidgetParameterModel(parameter));
        #endregion

        #region //TODO Mapper - Will move to map class
        //Map CMSWidgetConfigurationModel to WidgetSliderBannerViewModel.
        public WidgetSliderBannerViewModel ToViewModel(CMSWidgetConfigurationModel model)
        {
            return new WidgetSliderBannerViewModel()
            {
                WidgetSliderBannerId = model.CMSWidgetSliderBannerId,
                SliderId = model.CMSSliderId,
                MappingId = model.CMSMappingId,
                Navigation = model.Navigation,
                Type = model.Type,
                TypeOFMapping = model.TypeOFMapping,
                AutoPlay = model.AutoPlay,
                AutoplayHoverPause = model.AutoplayHoverPause,
                AutoplayTimeOut = model.AutoplayTimeOut,
                TransactionStyle = model.TransactionStyle,
                WidgetsKey = model.WidgetsKey,
                SliderBanners = new List<SliderBannerViewModel>(model.SliderBanners.Select(ToModel).ToList()),
            };
        }

        //Map BannerModel to SliderBannerViewModel.
        public SliderBannerViewModel ToModel(BannerModel viewModel)
        {
            return new SliderBannerViewModel()
            {
                SliderBannerId = viewModel.CMSSliderBannerId,
                BannerSequence = viewModel.BannerSequence,
                MediaPath = viewModel.MediaPath,
                ImageAlternateText = viewModel.ImageAlternateText,
                ButtonLabelName = viewModel.ButtonLabelName,
                ButtonLink = viewModel.ButtonLink,
                ActivationDate = viewModel.ActivationDate,
                ExpirationDate = viewModel.ExpirationDate,
                SliderBannerTitle = viewModel.Title,
                TextAlignment = viewModel.TextAlignment,
                Description = viewModel.Description
            };
        }

        // Get associated child category list.
        protected virtual void GetAssociatedChildCategoryList(KeywordSearchModel searchResult, CategorySubHeaderViewModel category, List<CategoryHeaderViewModel> categoryHeaderViewModelList)
        {
            if (category?.ChildCategoryItems?.Count() > 0)
            {
                category?.ChildCategoryItems?.ForEach(x => {
                    if (Convert.ToBoolean(searchResult?.AssociatedCategoryIds?.Contains(x.CategoryId)))
                    {
                        categoryHeaderViewModelList?.Add(x.ToModel<CategoryHeaderViewModel, CategorySubHeaderViewModel>());
                    }
                    GetAssociatedChildCategoryList(searchResult, x, categoryHeaderViewModelList);
                });
            }
        }

        #endregion

        #region Private Methods
        //Get search request model.
        public virtual SearchRequestViewModel GetSearchRequestModel(WidgetParameter widgetparameter, int pageNum = 0, int pageSize = 16)
        {
            object searchtearm = string.Empty;
            object brandName = string.Empty;
            widgetparameter.properties?.TryGetValue("SearchTerm", out searchtearm);

            object useSuggestion = string.Empty;
            widgetparameter.properties?.TryGetValue(WebStoreConstants.UseSuggestion, out useSuggestion);

            //Set search request model properties.
            SearchRequestViewModel requestModel = SetSearchRequestModel(widgetparameter, pageNum, pageSize, searchtearm);

            widgetparameter.properties?.TryGetValue(ZnodeConstant.Brand, out brandName);

            Dictionary<string, List<string>> searchValueList = GetFilterData();
            if (!string.IsNullOrEmpty((string)brandName))
            {
                searchValueList.Remove(ZnodeConstant.Brand);
                searchValueList.Add(ZnodeConstant.Brand, new List<string> { Convert.ToString(brandName) });
                requestModel.FilterList = searchValueList;
                requestModel.CategoryId = 0;
                requestModel.IsBrandSearch = true;

            }
            else
                requestModel.FilterList = HelperUtility.IsNotNull(searchValueList) ? searchValueList : null;

            requestModel.UseSuggestion = !Equals((string)useSuggestion, "false");
            requestModel.BrandName = Convert.ToString(brandName);

            return requestModel;
        }

        protected Dictionary<string, List<string>> GetFilterData()//Dictionary<string, List<string>> searchValueList)
        {
            Dictionary<string, List<string>> searchValueList = new Dictionary<string, List<string>>();
            string facetGroup = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Request.QueryString["FacetGroup"])) ? HttpContext.Current.Request.QueryString["FacetGroup"] : !string.IsNullOrEmpty(Convert.ToString(SessionHelper.GetDataFromSession<string>("FacetGroup"))) ? Convert.ToString(SessionHelper.GetDataFromSession<string>("FacetGroup")) : null;
            bool isFromSearch = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Request.QueryString["fromSearch"])) ? Convert.ToBoolean(HttpContext.Current.Request.QueryString["fromSearch"]) : false;
            char groupAndValueSeparator = '|', groupsSeparator = ',', valuesSeperator = '~';
            if (isFromSearch && !string.IsNullOrEmpty(facetGroup))
            {
                string[] facetGroups = facetGroup.Split(groupsSeparator);

                foreach (var facetItem in facetGroups)
                {
                    var decodeFacetItem = HttpUtility.UrlDecode(facetItem);
                    string[] facetArray = decodeFacetItem.Split(groupAndValueSeparator);
                    string facetName = facetArray[0];
                    List<string> facetValueList = new List<string>();
                    foreach (var item in facetArray[1].Split(valuesSeperator))
                    {
                        facetValueList.Add(item);
                    }
                    if (!searchValueList.ContainsKey(facetName))
                        searchValueList.Add(facetName, facetValueList);
                }
            }
            return searchValueList;
        }

        private void GetValuesFromParameter(WidgetParameter widgetparameter, out object searchtearm, out object brandName, out object useSuggestion)
        {
            searchtearm = string.Empty;
            brandName = string.Empty;
            useSuggestion = string.Empty;
            if (widgetparameter.properties?.Count > 0)
            {
                widgetparameter.properties.TryGetValue("SearchTerm", out searchtearm);
                widgetparameter.properties.TryGetValue(WebStoreConstants.UseSuggestion, out useSuggestion);
                widgetparameter.properties.TryGetValue(ZnodeConstant.Brand, out brandName);
            }
        }
        //Set search request model properties.
        public virtual SearchRequestViewModel SetSearchRequestModel(WidgetParameter widgetparameter, int pageNum, int pageSize, object searchTerm)
        {
            SearchRequestViewModel requestModel = new SearchRequestViewModel();
            requestModel.Category = widgetparameter.TypeOfMapping;

            requestModel.CatalogId = GetCatalogId().GetValueOrDefault();
            requestModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            requestModel.CategoryId = widgetparameter.CMSMappingId;
            requestModel.LocaleId = PortalAgent.LocaleId;
            requestModel.ProfileId = Helper.GetProfileId();
            requestModel.PageNumber = pageNum;
            requestModel.PageSize = pageSize;
            requestModel.SearchTerm = (string)searchTerm;
            requestModel.IsProductInheritanceEnabled = PortalAgent.CurrentPortal.IsProductInheritanceEnabled;
            return requestModel;
        }

        //Get search products.
        protected virtual void GetSearchProducts(KeywordSearchModel searchResult, ProductListViewModel productlist)
        {
            //Get Product list data.
            if (searchResult?.Products?.Count > 0)
            {
                productlist.Products = searchResult.Products?.Count > 0 ? searchResult.Products?.ToViewModel<ProductViewModel>()?.ToList() : new List<ProductViewModel>();
            }
        }

        //Get slider information with banner data.
        private WidgetSliderBannerViewModel GetSliderData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Below client object is being recycled intentionally. Since on Locale change, it has to be recycled to have the right locale sent to the API.
            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            CMSWidgetConfigurationModel model = _widgetClient.GetSlider(ToWidgetParameterModel(parameter), GenerateKey(parameter));

            ZnodeLogging.LogMessage("CMSSliderId, CMSWidgetSliderBannerId and CMSWidgetsId:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CMSSliderId = model?.CMSSliderId, CMSWidgetSliderBannerId = model?.CMSWidgetSliderBannerId, CMSWidgetsId = model?.CMSWidgetsId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model?.CMSSliderId > 0 ? model.ToViewModel<WidgetSliderBannerViewModel>() : new WidgetSliderBannerViewModel();
        }

        //Get product list information.
        private WidgetProductListViewModel GetProductsData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Set login user Id in client header to get profile, customer, account based pricing.
            SetLoginUserId();

            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            WebStoreWidgetProductListModel model = _widgetClient.GetProducts(ToWidgetParameterModel(parameter), GenerateKey(parameter), GetProductExpand());

            List<WidgetProductViewModel> productList = model?.Products?.ToViewModel<WidgetProductViewModel>().ToList();
            productList.ForEach(x =>
            {
                x.ProductViewModel.HighlightLists = _webstoreHelper.GetHighlightListFromAttributes(x.ProductViewModel.Attributes, x.ProductViewModel.SKU, x.ProductViewModel.PublishProductId);
            });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return new WidgetProductListViewModel { Products = productList, DisplayName = model.DisplayName };
        }

        //Set widget parameters.
        private WebStoreWidgetParameterModel ToWidgetParameterModel(WidgetParameter parameter)
            => new WebStoreWidgetParameterModel()
            {
                LocaleId = PortalAgent.LocaleId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                CMSMappingId = parameter.CMSMappingId,
                WidgetCode = parameter.WidgetCode,
                WidgetKey = parameter.WidgetKey,
                TypeOfMapping = parameter.TypeOfMapping,
                DisplayName = parameter.DisplayName,
                ProfileId = Helper.GetProfileId(),
                ProductProfileId = GetProfileId(),
                PublishCatalogId = GetCatalogId().GetValueOrDefault()
            };

        private WebStoreSearchWidgetParameterModel ToSearchWidgetParameterModel(WidgetParameter parameter)
           => new WebStoreSearchWidgetParameterModel()
           {
               LocaleId = PortalAgent.LocaleId,
               PortalId = PortalAgent.CurrentPortal.PortalId,
               CMSMappingId = parameter.CMSMappingId,
               WidgetCode = parameter.WidgetCode,
               WidgetKey = parameter.WidgetKey,
               TypeOfMapping = parameter.TypeOfMapping,
               DisplayName = parameter.DisplayName,
               ProfileId = Helper.GetProfileId(),
               ProductProfileId = GetProfileId(),
               CatalogId = GetCatalogId().GetValueOrDefault()
           };


        //Get link widget data.
        private WidgetTitleListViewModel GetLinkWidget(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return new WidgetTitleListViewModel { TitleList = _widgetClient.GetLinkWidget(ToWidgetParameterModel(parameter), GenerateKey(parameter))?.LinkWidgetConfigurationList?.ToViewModel<WidgetTitleViewModel>().ToList() };
        }


        private WidgetMediaViewModel GetMediaWidget(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            CMSMediaWidgetConfigurationModel model = _widgetClient.GetMediaWidgetDetails(ToWidgetParameterModel(parameter), GenerateKey(parameter), GetProductExpand());
            return model.ToViewModel<WidgetMediaViewModel>();
        }

        //Get category list information.
        private WidgetCategoryListViewModel GetCategoriesData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return new WidgetCategoryListViewModel { Categories = _widgetClient.GetCategories(ToWidgetParameterModel(parameter), GenerateKey(parameter))?.Categories?.ToViewModel<WidgetCategoryViewModel>().ToList() };
        }

        //Generate Key.
        private string GenerateKey(WidgetParameter parameter)
            => string.Concat(parameter.WidgetCode, parameter.WidgetKey, parameter.TypeOfMapping, parameter.CMSMappingId, PortalAgent.LocaleId, Helper.GetProfileId(), PortalAgent.CurrentPortal.PublishState);

        //Get filter for content page list.
        private FilterCollection GetFilterForContentPage()
        {
            PortalViewModel currentPortal = PortalAgent.CurrentPortal;

            UserViewModel currentUser = new UserAgent(GetClient<CountryClient>(), GetClient<WebStoreUserClient>(), GetClient<WishListClient>(), GetClient<UserClient>(), GetClient<PublishProductClient>(), GetClient<CustomerReviewClient>(), GetClient<OrderClient>(), GetClient<GiftCardClient>(), GetClient<AccountClient>(), GetClient<AccountQuoteClient>(), GetClient<OrderStateClient>(), GetClient<PortalCountryClient>(), GetClient<ShippingClient>(), GetClient<PaymentClient>(), GetClient<CustomerClient>(), GetClient<StateClient>(), GetClient<PortalProfileClient>()).GetUserViewModelFromSession();
            int? ProfileId = currentUser?.Profiles?.Count > 0 ? currentUser?.ProfileId > 0 ? currentUser.ProfileId : currentUser.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId : PortalAgent.CurrentPortal.ProfileId;

            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(currentPortal))
            {
                filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, currentPortal.PortalId.ToString());
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
                filters.Add(WebStoreEnum.ProfileIds.ToString(), FilterOperators.In, Convert.ToString(ProfileId));
            }

            ZnodeLogging.LogMessage("Filters for content page:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return filters;
        }

        //Get Filter For SubCategory.
        private FilterCollection GetFilterForSubCategory(WidgetParameter widgetparameter)
        {
            FilterCollection filters = GetRequiredFilters();
            filters.Add(WebStoreEnum.ZnodeParentCategoryIds.ToString(), FilterOperators.Contains, widgetparameter.CMSMappingId.ToString());
            filters.Add(WebStoreEnum.IsCallFromWebstore.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            ZnodeLogging.LogMessage("Filters for sub category:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { Filters = filters });
            return filters;
        }

        //Get content page data
        private WidgetTextViewModel GetContentPageData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            WebStoreContentPageListModel contentPages = GetContentPageList();

            if (HelperUtility.IsNotNull(contentPages)) {

                WebStoreContentPageModel contentPage = contentPages.ContentPageList?.FirstOrDefault(x => x.ContentPageId == parameter.CMSMappingId && x.TypeOFMapping == parameter.TypeOfMapping && x.WidgetsKey == parameter.WidgetKey && x.LocaleId == PortalAgent.LocaleId);

                if (HelperUtility.IsNull(contentPage))
                    contentPage = contentPages.ContentPageList?.FirstOrDefault(x => x.ContentPageId == parameter.CMSMappingId && x.TypeOFMapping == parameter.TypeOfMapping && x.WidgetsKey == parameter.WidgetKey && x.LocaleId == Convert.ToInt32(DefaultSettingHelper.DefaultLocale));

                ZnodeLogging.LogMessage("WebStoreContentPageModel with ContentPageId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ContentPageId = contentPage?.ContentPageId });
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

                return contentPage?.ToViewModel<WidgetTextViewModel>();
            }
            return new WidgetTextViewModel();
        }

        private WebStoreContentPageListModel GetContentPageList()
        {
            //Explicitly setting the current portal's publish state and locale value in client class since the calls coming from widget helper occur prior to the publish state setup in the GetClient method.
            _contentPageClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _contentPageClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _contentPageClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());

            string cachekey = GenerateCacheKeyForContentPage();

            WebStoreContentPageListModel contentPages;
            //If data is not cached make a call else get cached data for portal.
            if (HelperUtility.IsNull(Helper.GetFromCache<WebStoreContentPageListModel>(cachekey)))
            {
                contentPages = _contentPageClient.GetContentPagesList(GetFilterForContentPage());

                if (contentPages?.ContentPageList?.Count > 0)
                {
                    Helper.AddIntoCache(contentPages, cachekey, "CurrentPortalCacheDuration");
                }
            }

            contentPages = Helper.GetFromCache<WebStoreContentPageListModel>(cachekey);
            
            return contentPages;
        }

        private string GenerateCacheKeyForContentPage()
        {
            PortalViewModel currentPortal = PortalAgent.CurrentPortal;
            return $"TextWidgetContent_{currentPortal.PortalId}{currentPortal.LocaleId}{Convert.ToInt32(Helper.GetProfileId())}{Helper.GetUserProfileCatalogId()}{Helper.GetUserAccountId()}{currentPortal.PublishState}";
        }

        //Get tag manager data.
        private WidgetTextViewModel GetTagManagerData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            string _value = HelperUtility.IsNotNull(parameter.properties) && parameter.properties.ContainsKey("ScriptKey") ? Convert.ToString(parameter.properties["ScriptKey"]) : string.Empty;
            string tagManagerScript;
            if (!string.IsNullOrEmpty(_value))
            {
                //Get content/message on the basis of key.
                string googleTagManagerScript = Helper.GetMessage(_value, (string)HttpContext.Current.Request.RequestContext.RouteData.Values["controller"]);

                string _accountId = _value == WebStoreConstants.GoogleAnalyticsScript ? PortalAgent.CurrentPortal.AnalyticsUId : PortalAgent.CurrentPortal.ContainerId;

                if (_value == WebStoreConstants.GoogleAnalyticsScript)
                    tagManagerScript = (PortalAgent.CurrentPortal.AnalyticsIsActive && HelperUtility.IsNotNull(_accountId)) ? googleTagManagerScript.Replace("{0}", _accountId) : string.Empty;
                else
                    tagManagerScript = (PortalAgent.CurrentPortal.IsEnabledTagManager && HelperUtility.IsNotNull(_accountId)) ? googleTagManagerScript.Replace("{0}", _accountId) : string.Empty;

                return new WidgetTextViewModel { Text = tagManagerScript };
            }
            return new WidgetTextViewModel { Text = string.Empty };
        }

        //Get product expands to get quick view data.
        public virtual ExpandCollection GetProductExpands()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            if (!Helper.IsAsyncPrice)
                expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.SEO);
            expands.Add(ExpandKeys.AssociatedProducts);
            ZnodeLogging.LogMessage("Product expands:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ProductExpands = expands });
            return expands;
        }

        //Get product expands to get quick view data.
        private ExpandCollection GetExpandsForProductQuickView()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.SEO);
            expands.Add(ExpandKeys.AddOns);
            return expands;
        }
        //Set search paging parameters.
        protected virtual void SetPagingParameters(WidgetParameter widgetparameter, int pageNum, int pageSize, int sortValue, SearchRequestViewModel searchRequestModel)
        {
            if (widgetparameter?.properties?.Count > 1)
            {
                SetPageSize(widgetparameter, pageSize, searchRequestModel);
                SetSorting(widgetparameter, sortValue, searchRequestModel);
                searchRequestModel.PageNumber = !Equals(Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageNumber]), 0) ? (Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageNumber])) : pageNum;
            }
            else
            {
                searchRequestModel.PageSize = pageSize;
                searchRequestModel.PageNumber = pageNum;
            }
        }

        //Set filter values for already added filter.
        public virtual List<Tuple<string, List<KeyValuePair<string, string>>>> SetFilterParameter(int cmsMappingId, string typeOfMapping, Dictionary<string, object> properties, List<FacetViewModel> Facets)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Get facet values from querystring.
            Dictionary<string, List<string>> searchValueList = GetFilterData();

            object searchTerm = string.Empty;
            properties?.TryGetValue("SearchTerm", out searchTerm);

            if (Equals(typeOfMapping, ZnodeConstant.Brand))
                searchValueList?.Remove(ZnodeConstant.Brand);
            List<Tuple<string, List<KeyValuePair<string, string>>>> facetFilters = new List<Tuple<string, List<KeyValuePair<string, string>>>>();
            if (HelperUtility.IsNotNull(searchValueList))
            {
                foreach (var item in searchValueList)
                {
                    List<string> data = item.Value;
                    foreach (string value in data)
                    {
                        string facetValue = Facets.FirstOrDefault(x => x.AttributeName == item.Key)?.AttributeValues?.FirstOrDefault(y => y.AttributeValue == value)?.Label;

                        if (string.IsNullOrEmpty(facetValue))
                            facetValue = value;
                        string url = string.Empty;
                        switch (typeOfMapping)
                        {
                            case ZnodeConstant.Brand:
                                url = $"Search?FacetGroup={item.Key}&FacetValue={HttpUtility.UrlEncode(value)}&brandId={cmsMappingId}&IsRemove=true";
                                break;
                            case ZnodeConstant.SearchWidget:
                                url = $"Search?FacetGroup={item.Key}&FacetValue={HttpUtility.UrlEncode(value)}&pageId={cmsMappingId}&IsRemove=true";
                                break;
                            default:
                                url = $"Search?FacetGroup={item.Key}&FacetValue={HttpUtility.UrlEncode(value)}&categoryId={cmsMappingId}&IsRemove=true&SearchTerm={searchTerm}";
                                break;
                        }

                        if (facetFilters.Any(x => x.Item1 == item.Key))
                            facetFilters.Find(x => x.Item1 == item.Key).Item2.Add(new KeyValuePair<string, string>(facetValue, url));
                        else
                            facetFilters.Add(new Tuple<string, List<KeyValuePair<string, string>>>(item.Key, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(facetValue, url) }));
                    }
                }

            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return facetFilters;
        }

        //Get Search Result.
        public virtual KeywordSearchModel GetSearchResult(SearchRequestViewModel searchRequestModel, FilterCollection filters, bool isfacetCall = false)
        {
            bool isWithCategoryFacet = Equals(searchRequestModel.Category, ZnodeConstant.Search);
            return _searchAgent.FullTextSearch(searchRequestModel, GetProductExpandForSearch(isWithCategoryFacet, isfacetCall), filters, GetSortForSearch(searchRequestModel.Sort));
        }

        //Set filter values for already added filter.
        public virtual void SetFilterParameter(WidgetParameter parameter, SearchResultViewModel viewModel, string brandName = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Get facet values from querystring.
            Dictionary<string, List<string>> searchValueList = GetFilterData();

            object searchTerm = string.Empty;
            parameter.properties?.TryGetValue("SearchTerm", out searchTerm);

            if (Equals(parameter.TypeOfMapping, ZnodeConstant.Brand))
                searchValueList?.Remove(ZnodeConstant.Brand);

            if (HelperUtility.IsNotNull(searchValueList))
            {
                viewModel.FacetFilters = new List<Tuple<string, List<KeyValuePair<string, string>>>>();
                foreach (var item in searchValueList)
                {
                    var data = item.Value;
                    foreach (string value in data)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            string url = string.Empty;
                            if (Equals(parameter.TypeOfMapping, ZnodeConstant.Brand))
                                url = "Search?FacetGroup=" + item.Key + "&FacetValue=" + HttpUtility.UrlEncode(value) + "&brandId=" + parameter.CMSMappingId + "&IsRemove=true";
                            else
                                url = "Search?FacetGroup=" + item.Key + "&FacetValue=" + HttpUtility.UrlEncode(value) + "&categoryId=" + parameter.CMSMappingId + "&IsRemove=true" + "&SearchTerm=" + searchTerm;
                            if (viewModel.FacetFilters.Any(x => x.Item1 == item.Key))
                                viewModel.FacetFilters.Find(x => x.Item1 == item.Key).Item2.Add(new KeyValuePair<string, string>(value, url));
                            else
                                viewModel.FacetFilters.Add(new Tuple<string, List<KeyValuePair<string, string>>>(item.Key, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(value, url) }));
                        }
                    }
                }
                if (Equals(parameter.TypeOfMapping, ZnodeConstant.Brand))
                {
                    viewModel.BrandId = parameter.CMSMappingId;
                    viewModel.BrandName = brandName;
                }
                else
                {
                    viewModel.CategoryId = parameter.CMSMappingId;
                    viewModel.SearchTerm = searchTerm?.ToString() ?? string.Empty;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
        }

        //Get Expand For Product Search.
        public virtual ExpandCollection GetProductExpandForSearch(bool isWithCategoryFacet, bool isfacetCall = false)
        {
            //Sets expand required by products.
            ExpandCollection expands = GetProductExpands();

            //Sets search expands.
            if (isfacetCall)
                expands.Add(ExpandKeys.Facets);

            if (isWithCategoryFacet)
                expands.Add(ExpandKeys.Categories);

            ZnodeLogging.LogMessage("Product expand for search:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ProductExpandForSearch = expands });
            return expands;
        }

        //Get product expands.
        private ExpandCollection GetProductExpand()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.ProductReviews);
            if (!Helper.IsAsyncPrice)
                expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.ProductTemplate);
            expands.Add(ExpandKeys.AddOns);
            expands.Add(ExpandKeys.SEO);
            return expands;
        }

        //Set PageSize for product grid for both category and search page.
        private void SetPageSize(WidgetParameter widgetparameter, int pageSize, SearchRequestViewModel searchRequestModel)
        {
            //If widget parameter doesnot contain properties for paging, then get page size value from session else the default page size.
            if ((HelperUtility.IsNull((widgetparameter.properties[WebStoreConstants.PageSize]))) || (Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageSize])) == 0)
            {
                if (HelperUtility.IsNotNull(GetFromSession<int?>(WebStoreConstants.PageSizeValue)))
                {
                    searchRequestModel.PageSize = Convert.ToInt32(GetFromSession<int?>(WebStoreConstants.PageSizeValue));
                    widgetparameter.properties[WebStoreConstants.PageSize] = Convert.ToInt32(GetFromSession<int?>(WebStoreConstants.PageSizeValue));
                }
                else
                {
                    if (widgetparameter.WidgetCode != "Facet")
                        SaveInSession<int>(WebStoreConstants.PageSizeValue, pageSize);
                    searchRequestModel.PageSize = pageSize;
                }
            }
            else
            {
                SaveInSession<int>(WebStoreConstants.PageSizeValue, Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageSize]));
                searchRequestModel.PageSize = Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageSize]);
            }
        }

        //Set PageSize for product grid for both category and search page.
        private void SetSorting(WidgetParameter widgetparameter, int sortValue, SearchRequestViewModel searchRequestModel)
        {
            //If widget parameter doesnot contain properties for paging, then get page size value from session else the default page size.
            if (HelperUtility.IsNull((widgetparameter.properties[WebStoreConstants.Sort])))
            {
                Dictionary<string, List<string>> searchValueList = GetFilterData();
                if (HelperUtility.IsNotNull(GetFromSession<int?>(WebStoreConstants.SortValue)) && (searchValueList?.Count > 0))
                {
                    searchRequestModel.Sort = Convert.ToInt32(GetFromSession<int>(WebStoreConstants.SortValue));
                    widgetparameter.properties[WebStoreConstants.Sort] = Convert.ToInt32(GetFromSession<int>(WebStoreConstants.SortValue));
                }
                else
                {
                    if (widgetparameter.WidgetCode != "Facet")
                        SaveInSession<int>(WebStoreConstants.SortValue, sortValue);
                }
            }
            else
            {
                SaveInSession<int>(WebStoreConstants.SortValue, Convert.ToInt32(widgetparameter.properties[WebStoreConstants.Sort]));
                searchRequestModel.Sort = Convert.ToInt32(widgetparameter.properties[WebStoreConstants.Sort]);
            }
        }

        //Set login user Id in client header to get profile, customer, account based pricing.
        private void SetLoginUserId()
            => _widgetClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

        //Get brand list information.
        private WidgetBrandListViewModel GetBrandsData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            _widgetClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _widgetClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _widgetClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            return new WidgetBrandListViewModel { Brands = _widgetClient.GetBrands(ToWidgetParameterModel(parameter), GenerateKey(parameter))?.Brands?.ToViewModel<WidgetBrandViewModel>().ToList() };
        }

        //Get Sort Collection.
        public virtual SortCollection GetSortForSearch(int sort)
        {
            switch (sort)
            {
                case (int)SortEnum.NameAToZ:
                    return new SortCollection() { { "ProductName", DynamicGridConstants.ASCKey } };
                case (int)SortEnum.NameZToA:
                    return new SortCollection() { { "ProductName", DynamicGridConstants.DESCKey } };
                case (int)SortEnum.PriceHighToLow:
                    return new SortCollection() { { "Price", DynamicGridConstants.DESCKey } };
                case (int)SortEnum.PriceLowToHigh:
                    return new SortCollection() { { "Price", DynamicGridConstants.ASCKey } };
                case (int)SortEnum.HighestRating:
                    return new SortCollection() { { "HighestRated", DynamicGridConstants.DESCKey } };
                case (int)SortEnum.MostReviewed:
                    return new SortCollection() { { "MostReviewed", DynamicGridConstants.DESCKey } };
                case (int)SortEnum.OutOfStock:
                    return new SortCollection() { { "OutOfStock", DynamicGridConstants.DESCKey } };
                case (int)SortEnum.InStock:
                    return new SortCollection() { { "InStock", DynamicGridConstants.DESCKey } };
                default:
                    return new SortCollection();
            }
        }

        //Get Product message.
        protected virtual string GetProductMessage(WidgetParameter widgetparameter, SearchRequestViewModel searchRequestModel, KeywordSearchModel searchResult, int productsCount)
        {
            StringBuilder message = new StringBuilder();
            bool viewAll = GetFromSession<bool>(WebStoreConstants.ViewAllMode);
            if ((searchResult.TotalProductCount > searchRequestModel.PageSize) && !viewAll)
                message.Append(string.Format(WebStore_Resources.TextFoundProductsOutOf, productsCount, searchResult.TotalProductCount));
            else
                message.Append(string.Format(WebStore_Resources.TextFoundProducts, searchResult.TotalProductCount));

            message.Append(string.IsNullOrEmpty(searchRequestModel.SearchTerm) ? string.Empty : string.Format(WebStore_Resources.TextForKeyword, searchRequestModel.SearchTerm));
            message.Append(
                (widgetparameter.CMSMappingId > 0 & searchResult.Categories?.Count > 0) ?
                string.Format(WebStore_Resources.TextInCategory, searchResult.Categories?.Where(category => category.CategoryId == widgetparameter.CMSMappingId)?.Select(category => category.CategoryName)?.FirstOrDefault())
                : string.Empty);

            return message.ToString();
        }

        //Get total available for ECertificate.
        private ECertTotalBalanceModel GetECertTotalBalanceData(WidgetParameter parameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            //Set login user Id in client header to get profile, customer, account based pricing.
            SetLoginUserId();

            var userDetail = base.GetUserDetails();
            return _widgetClient.GetECertTotalBalance(ToWidgetParameterModel(parameter), userDetail?.UserName, userDetail?.Email);
        }

        #region CMS page search request

        //Get cmspage search result base on search keyword.
        public virtual CMSPageListViewModel GetSearchCMSPages(WidgetParameter widgetparameter)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            CMSPageListViewModel cmsPageListViewModel = new CMSPageListViewModel();

            //Create request for cmspage search page. #Step 1
            CMSPageSearchRequestViewModel searchRequestModel = GetCMSSearchRequestModel(widgetparameter);

            //Get filter required for cmspage search.  #Step 2
            FilterCollection filters = GetFilterForCMSPages();

            //Get cmspage search result base on keyword
            CMSKeywordSearchModel cmsSearchResult = FullTextContentPageSearch(searchRequestModel, filters);

            //Get cmspage search result model to show on view
            GetSearchedCMSPages(cmsSearchResult, cmsPageListViewModel);

            cmsPageListViewModel.TotalCMSPageCount = cmsSearchResult.TotalCMSPageCount;
            cmsPageListViewModel.SearchKeyword = searchRequestModel.SearchTerm;
            cmsPageListViewModel.PageNumber = Convert.ToInt32(searchRequestModel.PageNumber);
            cmsPageListViewModel.PageSize = Convert.ToInt32(searchRequestModel.PageSize);
            cmsPageListViewModel.TotalPages = searchRequestModel.PageSize.Equals(-1) ? 1 : (Convert.ToInt32(cmsSearchResult.TotalCMSPageCount + (searchRequestModel.PageSize - 1)) / searchRequestModel.PageSize).Value;

            //Get cms page message
            cmsPageListViewModel.SearchResultCountText = GetCMSPageMessage(searchRequestModel, cmsSearchResult.TotalCMSPageCount, cmsPageListViewModel.CMSPages.Count);
            ZnodeLogging.LogMessage("TotalCMSPageCount:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { TotalCMSPageCount = cmsPageListViewModel.TotalCMSPageCount });

            return cmsPageListViewModel;
        }

        //Get Cms pages base on keyword search by user
        public virtual CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestViewModel searchRequestModel, FilterCollection filters)
        {
            return _cmsPageSearchClient.FullTextContentPageSearch(GetKeywordSearchModel(searchRequestModel), filters);
        }

        //Get filter list for Cms page search.
        public FilterCollection GetFilterForCMSPages()
        {
            FilterCollection filters = new FilterCollection();
            int[] profileIds = { Convert.ToInt32(Helper.GetProfileId()), 0 };

            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.ProfileId.ToString(), FilterOperators.In, string.Join(",", profileIds));

            return filters;
        }

        //Get search cms page.
        public void GetSearchedCMSPages(CMSKeywordSearchModel searchResult, CMSPageListViewModel cmsPageList)
        {
            cmsPageList.CMSPages = searchResult.CMSPages?.Count > 0 ? searchResult.CMSPages.ToViewModel<CMSPageViewModel>().ToList() : new List<CMSPageViewModel>();
        }

        //Get search request model.
        public CMSPageSearchRequestViewModel GetCMSSearchRequestModel(WidgetParameter widgetparameter)
        {
            CMSPageSearchRequestViewModel searchRequestModel = new CMSPageSearchRequestViewModel();

            //Set search request model properties.
            searchRequestModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            searchRequestModel.LocaleId = PortalAgent.LocaleId;
            searchRequestModel.ProfileId = Convert.ToInt32(Helper.GetProfileId());
            searchRequestModel.SearchTerm = Convert.ToString(widgetparameter.properties[WebStoreConstants.SearchTerm]);

            //Set content page paging parameters.
            SetContentPagination(searchRequestModel, widgetparameter);

            return searchRequestModel;
        }

        //Get cms page message
        public string GetCMSPageMessage(CMSPageSearchRequestViewModel searchRequestModel, int totalCMSPageCount, int cmsPageCount)
        {
            StringBuilder message = new StringBuilder();

            if (totalCMSPageCount > searchRequestModel.PageSize)
                message.Append(string.Format(WebStore_Resources.TextFoundProductsOutOf, cmsPageCount, totalCMSPageCount));
            else
                message.Append(string.Format(WebStore_Resources.TextFoundProducts, totalCMSPageCount));

            message.Append(string.Format(WebStore_Resources.TextForKeyword, searchRequestModel.SearchTerm));

            return message.ToString();
        }

        //Get keyword search model.
        public CMSPageSearchRequestModel GetKeywordSearchModel(CMSPageSearchRequestViewModel searchRequest)
         => new CMSPageSearchRequestModel()
         {
             LocaleId = searchRequest.LocaleId,
             Keyword = searchRequest.SearchTerm.Trim(),
             PageNumber = searchRequest.PageNumber,
             PageSize = Convert.ToInt32(searchRequest.PageSize),
             PortalId = searchRequest.PortalId,
             ProfileId = searchRequest.ProfileId
         };

        public void SetContentPagination(CMSPageSearchRequestViewModel searchRequestModel, WidgetParameter widgetparameter)
        {
            searchRequestModel.PageNumber = Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageNumber]);
            searchRequestModel.PageSize = Convert.ToInt32(widgetparameter.properties[WebStoreConstants.PageSize]);

            SaveInSession<int>(WebStoreConstants.CMSPageSizeValue, Convert.ToInt32(searchRequestModel.PageSize));
            SaveInSession<int>(WebStoreConstants.CMSPageNumber, Convert.ToInt32(searchRequestModel.PageNumber));
        }
        #endregion

        #endregion
    }
}