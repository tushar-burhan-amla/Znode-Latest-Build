using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class SearchCache : BaseCache, ISearchCache
    {
        private readonly ISearchService _service;

        public SearchCache(ISearchService searchService)
        {
            _service = searchService;
        }

        //Gets portal index data.
        public virtual string GetCatalogIndexData(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalIndexModel portalIndex = _service.GetCatalogIndexData(Expands, Filters);
                if (IsNotNull(portalIndex))
                {
                    PortalIndexResponse response = new PortalIndexResponse { PortalIndex = portalIndex };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get SEO Url Details.
        public virtual string GetSEOUrlDetails(string seoUrl, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SEOUrlModel seoUrlModel = _service.GetSEOUrlDetails(seoUrl, Filters);
                if (IsNotNull(seoUrlModel))
                {
                    SEOUrlResponse response = new SEOUrlResponse { SEOUrl = seoUrlModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets search index monitor list.
        public virtual string GetSearchIndexMonitorList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchIndexMonitorListModel searchIndexMonitorList = _service.GetSearchIndexMonitorList(Expands, Filters, Sorts, Page);
                if (IsNotNull(searchIndexMonitorList))
                {
                    SearchIndexMonitorListResponse response = new SearchIndexMonitorListResponse { SearchIndexMonitorList = searchIndexMonitorList.SearchIndexMonitorList };
                    response.MapPagingDataFromModel(searchIndexMonitorList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets search index server status list.
        public virtual string GetSearchIndexServerStatusList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchIndexServerStatusListModel searchIndexServerStatusList = _service.GetSearchIndexServerStatusList(Expands, Filters, Sorts, Page);
                if (IsNotNull(searchIndexServerStatusList))
                {
                    SearchIndexServerStatusListResponse response = new SearchIndexServerStatusListResponse { SearchIndexServerStatusList = searchIndexServerStatusList.SearchIndexServerStatusList };
                    response.MapPagingDataFromModel(searchIndexServerStatusList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //To do: changes required if needed
        #region Elastic search       

        public virtual string FullTextSearch(SearchRequestModel model, string routeUri, string routeTemplate)
         {
                 KeywordSearchModel keywordSearchModel = _service.FullTextSearch(model, Expands, Filters, Sorts, Page);
                //Get response and insert it into cache.
                KeywordSearchResponse response = new KeywordSearchResponse() { Search = keywordSearchModel };
                return InsertIntoCache(routeUri, routeTemplate, response);          
        }

        //Get product details by sku.
        public virtual string GetProductDetailsBySKU(SearchRequestModel model, string routeUri, string routeTemplate)
        {
            PublishProductModel publishProduct= _service.GetProductDetailsBySKU(model, Expands, Filters, Sorts, Page);
            //Get response and insert it into cache.
            PublishProductResponse response = new PublishProductResponse() { PublishProduct = publishProduct };
            return InsertIntoCache(routeUri, routeTemplate, response);
        }

        public virtual string GetSearchProfileProducts(SearchProfileModel model, string routeUri, string routeTemplate)
        {
                KeywordSearchModel keywordSearchModel = _service.GetSearchProfileProducts(model, Expands, Filters, Sorts, Page);
                //Get response and insert it into cache.
                KeywordSearchResponse response = new KeywordSearchResponse() { Search = keywordSearchModel };
                return InsertIntoCache(routeUri, routeTemplate, response);
        }

        public virtual string FacetSearch(SearchRequestModel model, string routeUri, string routeTemplate)
        {  
                KeywordSearchModel keywordSearchModel = _service.FacetSearch(model, Expands, Filters, Sorts, Page);
                //Get response and insert it into cache.
                KeywordSearchResponse response = new KeywordSearchResponse() { Search = keywordSearchModel };

                return InsertIntoCache(routeUri, routeTemplate, response);
            
            
        }

        #endregion

        #region Boost Details
        public virtual string GetGlobalProductBoostList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchGlobalProductBoostListModel productBoostList = _service.GetGlobalProductBoostList(Expands, Filters, Sorts, Page);
                if (IsNotNull(productBoostList))
                {
                    SearchGlobalProductBoostListResponse response = new SearchGlobalProductBoostListResponse { SearchGlobalProductBoostList = productBoostList.SearchGlobalProductBoostList };
                    response.MapPagingDataFromModel(productBoostList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetGlobalProductCategoryBoostList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchGlobalProductCategoryBoostListModel productCategoryBoostList = _service.GetGlobalProductCategoryBoostList(Expands, Filters, Sorts, Page);
                if (IsNotNull(productCategoryBoostList))
                {
                    SearchGlobalProductCategoryBoostListResponse response = new SearchGlobalProductCategoryBoostListResponse { SearchGlobalProductCategoryBoostList = productCategoryBoostList.SearchGlobalProductCategoryList };
                    response.MapPagingDataFromModel(productCategoryBoostList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetFieldBoostList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchDocumentMappingListModel fieldBoostList = _service.GetFieldBoostList(Expands, Filters, Sorts, Page);
                if (IsNotNull(fieldBoostList))
                {
                    SearchDocumentMappingListResponse response = new SearchDocumentMappingListResponse { SearchDocumentMappingList = fieldBoostList.SearchDocumentMappingList };
                    response.MapPagingDataFromModel(fieldBoostList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Keyword Search Suggestion.
        public virtual string GetKeywordSearchSuggestion(SearchRequestModel model, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                KeywordSearchModel keywordSearchModel = _service.GetKeywordSearchSuggestion(model, Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(keywordSearchModel))
                {
                    KeywordSearchResponse response = new KeywordSearchResponse { Search = keywordSearchModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Synonyms
        //Get synonyms data for search.
        public virtual string GetSearchSynonyms(int searchSynonymsId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchSynonymsModel searchSynonymsModel = _service.GetSearchSynonyms(searchSynonymsId, Expands);
                if (IsNotNull(searchSynonymsModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SearchSynonymsResponse { SearchSynonyms = searchSynonymsModel });
            }
            return data;
        }

        //Get synonyms list for search.
        public virtual string GetSearchSynonymsList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchSynonymsListModel searchSynonymsList = _service.GetSearchSynonymsList(Expands, Filters, Sorts, Page);

                if (searchSynonymsList?.SynonymsList?.Count > 0)
                {
                    SearchSynonymsListResponse response = new SearchSynonymsListResponse { SynonymsList = searchSynonymsList.SynonymsList };
                    response.MapPagingDataFromModel(searchSynonymsList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Keywords Redirect
        //Get list of keywords and url.
        public virtual string GetCatalogKeywordsRedirectList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchKeywordsRedirectListModel keywordsRedirectList = _service.GetCatalogKeywordsRedirectList(Expands, Filters, Sorts, Page);
                SearchKeywordsRedirectListResponse response = new SearchKeywordsRedirectListResponse { KeywordsList = keywordsRedirectList.KeywordsList };
                response.MapPagingDataFromModel(keywordsRedirectList);
                data = InsertIntoCache(routeUri, routeTemplate, response);                        
            }
            return data;
        }

        //Get keywords by id.
        public virtual string GetSearchKeywordsRedirect(int searchKeywordsRedirectId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SearchKeywordsRedirectModel searchKeywordsRedirectModel = _service.GetSearchKeywordsRedirect(searchKeywordsRedirectId, Expands);
                if (IsNotNull(searchKeywordsRedirectModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SearchKeywordsRedirectResponse { SearchKeywords = searchKeywordsRedirectModel });
            }
            return data;
        }
        #endregion
    }
}
