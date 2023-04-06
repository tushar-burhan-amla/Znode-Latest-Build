using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore.Agents
{
    public class SearchAgent : BaseAgent, ISearchAgent
    {
        #region Private Variables

        private ISearchClient _searchClient;
        private readonly ICategoryAgent _categoryAgent;
        //moved it from constructor injection to getter property
        //As this class gets called from Route lookup from where we do not have access to Session object
        //And that causes extra call to API due to GetClient() method
        private ISearchClient searchClient
        {
            get
            {
                if (_searchClient == null)
                {
                    _searchClient = GetClient<ISearchClient>(ZnodeDependencyResolver.GetService<ISearchClient>());
                }
                return _searchClient;
            }
        }

        #endregion

        #region Constructor
        public SearchAgent()
        {
            _categoryAgent = new CategoryAgent(GetClient<WebStoreCategoryClient>(), GetClient<PublishCategoryClient>(), GetClient<PublishBrandClient>());
        }
        #endregion

        #region Public Methods.

        //Get Seo Url details.
        public virtual SEOUrlViewModel GetSeoUrlDetail(string seoUrl)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return GetSeoUrlDetailFromCache(seoUrl);
        }
        public virtual SEOUrlViewModel GetSeoUrlDetailFromCache(string seoUrl)
        {
            string cacheKey = $"SEOUrlCache_{ PortalAgent.CurrentPortal.PortalId}_{PortalAgent.CurrentPortal.LocaleId}_{PortalAgent.CurrentPortal.PublishState}_{GetPublishCatalogId()}";
            if (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || !((Hashtable)HttpRuntime.Cache.Get(cacheKey)).ContainsKey(seoUrl))
            {
                SEOUrlViewModel sEOUrlViewModel = GetSEODetailsFromAPI(seoUrl);
                Hashtable hashtable = new Hashtable();
                if (!HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)))
                {
                    hashtable = (Hashtable)HttpRuntime.Cache.Get(cacheKey);
                }
                if (!hashtable.ContainsKey(seoUrl))
                {
                    hashtable.Add(seoUrl, sEOUrlViewModel);
                }
                HttpRuntime.Cache[cacheKey] = hashtable;
            }
            ZnodeLogging.LogMessage("Get Details from cache.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return (SEOUrlViewModel)((Hashtable)HttpRuntime.Cache.Get(cacheKey))[seoUrl];
        }
        public virtual SEOUrlViewModel GetSEODetailsFromAPI(string seoUrl)
        {
            searchClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            searchClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            searchClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SEOUrlViewModel sEOUrlViewModel = searchClient.GetSEOUrlDetails(seoUrl, FiltersForSeoUrlDetails()).ToViewModel<SEOUrlViewModel>();
            return sEOUrlViewModel;
        }
        //Get facet Search Product list.
        public virtual KeywordSearchModel FacetSearch(SearchRequestViewModel searchRequestModel, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            //Set login user Id in client header to get profile, customer, account based pricing.
            SetLoginUserId();
            searchClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchClient.FacetSearch(GetKeywordSearchModel(searchRequestModel), expands, filters, GetSortForSearch(searchRequestModel.Sort));
        }

        //get search product list.
        public virtual KeywordSearchModel FullTextSearch(SearchRequestViewModel searchRequestModel, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            //Set login user Id in client header to get profile, customer, account based pricing.
            SetLoginUserId();

            searchClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            searchClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            searchClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            searchClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchClient.FullTextSearch(GetKeywordSearchModel(searchRequestModel), expands, filters, sorts);
        }

        //Get Product Suggestions
        public virtual List<AutoComplete> GetSuggestions(string searchTerm, string category, UrlHelper url)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                FilterCollection filter = GetRequiredFilters();
                filter.Add(WebStoreEnum.ProductIndex.ToString(), FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());

                KeywordSearchModel searchModel = searchClient.GetKeywordSearchSuggestion(new SearchRequestModel { Keyword = searchTerm.Trim(), LocaleId = PortalAgent.LocaleId, CatalogId = GetCatalogId().GetValueOrDefault(), PortalId = PortalAgent.CurrentPortal.PortalId, PageSize = 10, IsAutocomplete = true }, new ExpandCollection { "Categories" }, filter, null, 0, 0);
                List<AutoComplete> autoCompleteList = new List<AutoComplete>();
                CategoryHeaderListViewModel categorySeoList = _categoryAgent.GetCategories();
                //Map autocomplete list
                searchModel.Products?.ForEach(x =>
                {

                    if (!string.IsNullOrEmpty(x.CategoryName))
                    {
                        string categorySeoUrl = categorySeoList.Categories.FirstOrDefault(y => y.CategoryId == x.CategoryId)?.SEOPageName;
                        if (HelperUtility.IsNull(categorySeoUrl))
                        {
                            categorySeoList.Categories.ForEach(y => { if (HelperUtility.IsNull(categorySeoUrl)) categorySeoUrl = y.SubCategoryItems.FirstOrDefault(z => z.CategoryId == x.CategoryId)?.SEOPageName; });
                        }
                        categorySeoUrl = HelperUtility.IsNull(categorySeoUrl) ? "category/" + Convert.ToString(x.CategoryId) : categorySeoUrl;
                        AutoComplete autoCompleteModel = new AutoComplete();
                        autoCompleteModel.Name = HttpUtility.HtmlDecode(string.Format($"<div class='d-flex p-2'><div class='search-img'><img class='img-fluid' src='{ x.ImageThumbNailPath}' onerror=this.src='/Content/Images/no-image.png';></div><div class='auto-list'><p><b>{x.Name}</b></p><p>{" in "}<a onclick='Product.prototype.IsCategoryLinkClicked();' href='/{categorySeoUrl}' class='search-category'>{x.CategoryName}</a></p></div></div>"));
                        autoCompleteModel.Id = x.ZnodeProductId;
                        autoCompleteModel.DisplayText = x.Name;
                        autoCompleteModel.Properties.Add(x.CategoryName, x.CategoryId);
                        autoCompleteModel.Properties.Add(WebStoreConstants.ProductSeoUrl, url.GetProductUrl(x?.SEOUrl, x?.ZnodeProductId.ToString()));
                        if (!AlreadyExist(autoCompleteList, autoCompleteModel))
                            autoCompleteList.Add(autoCompleteModel);
                    }
                    else
                    {
                        AutoComplete autoCompleteModel = new AutoComplete();
                        autoCompleteModel.Name = $"{x.Name}";
                        autoCompleteModel.Id = x.ZnodeProductId;
                        autoCompleteModel.DisplayText = x.Name;
                        if (!AlreadyExist(autoCompleteList, autoCompleteModel))
                            autoCompleteList.Add(autoCompleteModel);
                    }
                });
                ZnodeLogging.LogMessage("autoCompleteList:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { autoCompleteList = autoCompleteList });
                return autoCompleteList;
            }
            else
                return new List<AutoComplete>();
        }

        //Set Filter Data From Facets
        public virtual void SetFilterData(SearchRequestViewModel searchRequestModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(searchRequestModel.FacetGroup))
                RemoveInSession(ZnodeConstant.FacetsSearchFields);

            if (searchRequestModel.IsRemoveAll)
                RemoveInSession(ZnodeConstant.FacetsSearchFields);
        }

        //Returns the url if exists for search term.
        public string CheckURLExistForSearchTerm(string searchTerm)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                int? catalogId = GetCatalogId();
                ZnodeLogging.LogMessage("catalogId:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId });
                FilterCollection filters = new FilterCollection();
                if (catalogId > 0)
                    filters.Add(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
                filters.Add(ZnodeSearchKeywordsRedirectEnum.Keywords.ToString(), FilterOperators.Contains, searchTerm);

                ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { filters = filters });
                SearchKeywordsRedirectListModel list = searchClient.GetCatalogKeywordsRedirectList(filters, null, null, 1, 100);
                List<SearchKeywordsRedirectModel> updatedKeywordsList = GetUpdatedKeywordList(list);
                if (updatedKeywordsList.Count > 0)
                    list.KeywordsList.AddRange(updatedKeywordsList);
                return list.KeywordsList?.FirstOrDefault(x => x.Keywords.ToLower().Equals(searchTerm.ToLower().Trim()))?.URL;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return string.Empty;
        }

        //Get Keyword List from comma seperated Keywords
        public List<SearchKeywordsRedirectModel> GetUpdatedKeywordList(SearchKeywordsRedirectListModel list)
        {
            List<SearchKeywordsRedirectModel> updatedKeywordsList = new List<SearchKeywordsRedirectModel>();
            if (list.KeywordsList?.Any(m => m.Keywords.Split(',')?.Length > 1) ?? false)
            {
                list.KeywordsList.ForEach(m =>
                {
                    if (m.Keywords.Split(',').Length > 1)
                    {
                        string[] keywordsArray = m.Keywords.Split(',');
                        foreach (string keyword in keywordsArray)
                        {
                            updatedKeywordsList.Add(new SearchKeywordsRedirectModel()
                            {
                                Keywords = keyword,
                                URL = m.URL
                            });
                        }
                    }
                });
            }

            return updatedKeywordsList;
        }
        #endregion

        #region Private Methods.

        //Get inner search results.
        private List<string> GetInnerSearchList(string innerSearchTerm)
        {
            List<string> innerSearchList = new List<string>();
            if (!string.IsNullOrEmpty(innerSearchTerm))
                innerSearchList = innerSearchTerm.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return innerSearchList;
        }

        //Get keyword search model.
        public SearchRequestModel GetKeywordSearchModel(SearchRequestViewModel searchRequest)
         => new SearchRequestModel()
         {
             CategoryId = searchRequest.CategoryId,
             LocaleId = searchRequest.LocaleId,
             Category = searchRequest.Category,
             Keyword = searchRequest.SearchTerm?.Trim(),
             CatalogId = searchRequest.CatalogId,
             PageNumber = searchRequest.PageNumber,
             PageSize = searchRequest.PageSize,
             RefineBy = searchRequest.FilterList,
             PortalId = searchRequest.PortalId,
             ProfileId = searchRequest.ProfileId,
             IsBrandSearch = searchRequest.IsBrandSearch,
             IsFacetList = searchRequest.IsFacetList,
             UseSuggestion = searchRequest.UseSuggestion,
             IsProductInheritanceEnabled = searchRequest.IsProductInheritanceEnabled
         };

        public SortCollection GetSortForSearch(int sort)
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


        private bool AlreadyExist(List<AutoComplete> autoCompleteList, AutoComplete autoCompleteModel)
       => autoCompleteList.Any(x => x.Name == autoCompleteModel.Name);

        //Filters to get Seo Url details.
        private static FilterCollection FiltersForSeoUrlDetails()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, PortalAgent.LocaleId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.CatalogId, FilterOperators.Equals, GetPublishCatalogId().ToString()));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { filters = filters });

            return filters;
        }

        //Set login user Id in client header to get profile, customer, account based pricing.
        private void SetLoginUserId()
            => searchClient.UserId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

        private static int? GetPublishCatalogId()
        {
            int? catalogId = Helper.GetUserProfileCatalogId();
            return catalogId > 0 ? catalogId : PortalAgent.CurrentPortal.PublishCatalogId;
        }

        #endregion
    }
}