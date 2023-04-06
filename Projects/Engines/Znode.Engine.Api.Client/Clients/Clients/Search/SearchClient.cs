using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class SearchClient : BaseClient, ISearchClient
    {
        public virtual PortalIndexModel GetCatalogIndexData(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = SearchEndpoint.GetCatalogIndexData();

            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();

            PortalIndexResponse response = GetResourceFromEndpoint<PortalIndexResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalIndexModel portalIndexData = response?.PortalIndex;

            return portalIndexData;
        }

        //Inserts create index data for elastic search.
        public virtual PortalIndexModel InsertCreateIndexData(PortalIndexModel portalIndex)
        {
            string endpoint = SearchEndpoint.InsertCreateIndexData();

            ApiStatus status = new ApiStatus();
            PortalIndexResponse response = PostResourceToEndpoint<PortalIndexResponse>(endpoint, JsonConvert.SerializeObject(portalIndex), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.Created, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalIndex;
        }

        public virtual void CreateIndex(string indexName, string revisionType, int catalogId, int searchIndexMonitorId)
        {
            string endpoint = SearchEndpoint.CreateIndex(indexName, revisionType, catalogId, searchIndexMonitorId);

            ApiStatus status = new ApiStatus();
            PortalIndexResponse response = GetResourceFromEndpoint<PortalIndexResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
        }

        //Gets the search result. 
        public virtual KeywordSearchModel Search(SearchRequestModel model, ExpandCollection expands, SortCollection sorts)
        {
            var endpoint = SearchEndpoint.Keyword();
            endpoint += BuildEndpointQueryString(expands, null, sorts, null, null);

            ApiStatus status = new ApiStatus();
            KeywordSearchResponse response = PostResourceToEndpoint<KeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response != null ? response.Search : new KeywordSearchModel { Products = new List<SearchProductModel>() };
        }

        //Get SEO Url details on the basis of SEO Url. 
        public virtual SEOUrlModel GetSEOUrlDetails(string seoUrl, FilterCollection filters)
        {
            string endpoint = SearchEndpoint.GetSEOUrlDetails(seoUrl);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);
            ApiStatus status = new ApiStatus();
            SEOUrlResponse response = GetResourceFromEndpoint<SEOUrlResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.SEOUrl;
        }

        public virtual bool SaveBoostValues(BoostDataModel boostDataModel)
        {
            string endpoint = SearchEndpoint.SaveBoostValue();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(boostDataModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Deletes the boost value of a product, product-category or field if it is removed.
        public virtual bool DeleteBoostValue(BoostDataModel boostDataModel)
        {
            string endpoint = SearchEndpoint.DeleteBoostValue();
            ApiStatus status = new ApiStatus();
            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(boostDataModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get list of search index monitor.
        public virtual SearchIndexMonitorListModel GetSearchIndexMonitorList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetSearchIndexMonitorList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchIndexMonitorListResponse response = GetResourceFromEndpoint<SearchIndexMonitorListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchIndexMonitorListModel list = new SearchIndexMonitorListModel { SearchIndexMonitorList = response?.SearchIndexMonitorList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get list of search index server status.
        public virtual SearchIndexServerStatusListModel GetSearchIndexServerStatusList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetSearchIndexServerStatusList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchIndexServerStatusListResponse response = GetResourceFromEndpoint<SearchIndexServerStatusListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchIndexServerStatusListModel list = new SearchIndexServerStatusListModel { SearchIndexServerStatusList = response?.SearchIndexServerStatusList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets field level boost list
        public virtual SearchDocumentMappingListModel GetFieldLevelBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetFieldBoostList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchDocumentMappingListResponse response = GetResourceFromEndpoint<SearchDocumentMappingListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchDocumentMappingListModel list = new SearchDocumentMappingListModel { SearchDocumentMappingList = response?.SearchDocumentMappingList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets product boost list.
        public virtual SearchGlobalProductBoostListModel GetGlobalProductBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetGlobalProductBoostList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchGlobalProductBoostListResponse response = GetResourceFromEndpoint<SearchGlobalProductBoostListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchGlobalProductBoostListModel list = new SearchGlobalProductBoostListModel { SearchGlobalProductBoostList = response?.SearchGlobalProductBoostList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get product category boost List.
        public virtual SearchGlobalProductCategoryBoostListModel GetGlobalProductCategoryBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetGlobalProductCategoryBoostList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchGlobalProductCategoryBoostListResponse response = GetResourceFromEndpoint<SearchGlobalProductCategoryBoostListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchGlobalProductCategoryBoostListModel list = new SearchGlobalProductCategoryBoostListModel { SearchGlobalProductCategoryList = response?.SearchGlobalProductCategoryBoostList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Delete elastic search index
        public bool DeleteIndex(int catalogIndexId)
        {
            string endpoint = SearchEndpoint.DeleteIndex(catalogIndexId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Keyword Search Suggestion.
        public virtual KeywordSearchModel GetKeywordSearchSuggestion(SearchRequestModel model, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetKeywordSearchSuggestion();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            KeywordSearchResponse response = PostResourceToEndpoint<KeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response != null ? response.Search : new KeywordSearchModel { Products = new List<SearchProductModel>() };
        }

        //Get Search Result.
        public virtual KeywordSearchModel FullTextSearch(SearchRequestModel keywordSearchModel, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection)
        {
            string endpoint = SearchEndpoint.FullTextSearch();
            endpoint += BuildEndpointQueryString(expandCollection, filters, sortCollection, null, null);

            ApiStatus status = new ApiStatus();
            
            KeywordSearchResponse response = PostResourceToEndpoint<KeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(keywordSearchModel), status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return HelperUtility.IsNotNull(response) ? response.Search : new KeywordSearchModel { Products = new List<SearchProductModel>() };
        }

        //Get Facet search result.
        public virtual KeywordSearchModel FacetSearch(SearchRequestModel keywordSearchModel, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection)
        {
            string endpoint = SearchEndpoint.FacetSearch();
            endpoint += BuildEndpointQueryString(expandCollection, filters, sortCollection, null, null);

            ApiStatus status = new ApiStatus();

            KeywordSearchResponse response = PostResourceToEndpoint<KeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(keywordSearchModel), status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return HelperUtility.IsNotNull(response) ? response.Search : new KeywordSearchModel { Products = new List<SearchProductModel>() };
        }

        #region Synonyms
        //Create synonyms for search.
        public virtual SearchSynonymsModel CreateSearchSynonyms(SearchSynonymsModel model)
        {
            string endpoint = SearchEndpoint.CreateSearchSynonyms();

            ApiStatus status = new ApiStatus();
            SearchSynonymsResponse response = PostResourceToEndpoint<SearchSynonymsResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchSynonyms;
        }

        //Get synonyms data for search.
        public virtual SearchSynonymsModel GetSearchSynonyms(int searchSynonymsId, ExpandCollection expands)
        {
            string endpoint = SearchEndpoint.GetSearchSynonyms(searchSynonymsId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            SearchSynonymsResponse response = GetResourceFromEndpoint<SearchSynonymsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchSynonyms;
        }

        //Update synonyms data for search.
        public virtual SearchSynonymsModel UpdateSearchSynonyms(SearchSynonymsModel searchSynonymsModel)
        {
            string endpoint = SearchEndpoint.UpdateSearchSynonyms();

            ApiStatus status = new ApiStatus();
            SearchSynonymsResponse response = PutResourceToEndpoint<SearchSynonymsResponse>(endpoint, JsonConvert.SerializeObject(searchSynonymsModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.SearchSynonyms;
        }

        //Get synonyms list for search.
        public SearchSynonymsListModel GetSearchSynonymsList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetSearchSynonymsList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchSynonymsListResponse response = GetResourceFromEndpoint<SearchSynonymsListResponse>(endpoint, status);

            //Check the status of response for synonyms list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchSynonymsListModel list = new SearchSynonymsListModel { SynonymsList = response?.SynonymsList };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete synonyms by id.
        public virtual bool DeleteSearchSynonyms(ParameterModel searchSynonymsId)
        {
            string endpoint = SearchEndpoint.DeleteSearchSynonyms();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchSynonymsId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Write synonyms.txt for search.
        public virtual bool WriteSearchFile(int publishCatalogId, bool isSynonymsFile)
        {
            string endpoint = SearchEndpoint.WriteSynonymsFile(publishCatalogId, isSynonymsFile);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }
        #endregion

        #region Keywords Redirect
        //Get catalog keywords redirect list.
        public virtual SearchKeywordsRedirectListModel GetCatalogKeywordsRedirectList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage)
        {
            string endpoint = SearchEndpoint.GetCatalogKeywordsRedirectList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchKeywordsRedirectListResponse response = GetResourceFromEndpoint<SearchKeywordsRedirectListResponse>(endpoint, status);

            //Check the status of response of keywords redirect list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchKeywordsRedirectListModel list = new SearchKeywordsRedirectListModel { KeywordsList = response?.KeywordsList };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create keywords and its redirected url for search.
        public virtual SearchKeywordsRedirectModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectModel model)
        {
            string endpoint = SearchEndpoint.CreateSearchKeywordsRedirect();

            ApiStatus status = new ApiStatus();
            SearchKeywordsRedirectResponse response = PostResourceToEndpoint<SearchKeywordsRedirectResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchKeywords;
        }

        //Get keywords details by keywords id for search.
        public virtual SearchKeywordsRedirectModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId, ExpandCollection expands)
        {
            string endpoint = SearchEndpoint.GetSearchKeywordsRedirect(searchKeywordsRedirectId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            SearchKeywordsRedirectResponse response = GetResourceFromEndpoint<SearchKeywordsRedirectResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchKeywords;
        }

        //Update keywords for search.
        public virtual SearchKeywordsRedirectModel UpdateSearchKeywordsRedirect(SearchKeywordsRedirectModel searchKeywordsRedirectModel)
        {
            string endpoint = SearchEndpoint.UpdateSearchKeywordsRedirect();

            ApiStatus status = new ApiStatus();
            SearchKeywordsRedirectResponse response = PutResourceToEndpoint<SearchKeywordsRedirectResponse>(endpoint, JsonConvert.SerializeObject(searchKeywordsRedirectModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.SearchKeywords;
        }

        //Delete keywords by ids.
        public virtual bool DeleteSearchKeywordsRedirect(ParameterModel searchKeywordsRedirectId)
        {
            string endpoint = SearchEndpoint.DeleteSearchKeywordsRedirect();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchKeywordsRedirectId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion
    }
}