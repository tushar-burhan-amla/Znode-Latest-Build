using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchService _service;
        private readonly ISearchCache _cache;        

        public SearchController(ISearchService service)
        {
            _service = service;
            _cache = new SearchCache(_service);            
        }
        /// <summary>
        /// Gets search index data.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PortalIndexResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchIndexData()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCatalogIndexData(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalIndexResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Creates index for search.
        /// </summary>
        /// <param name="portalIndexModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(PortalIndexResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage InsertCreateIndexData([FromBody]PortalIndexModel portalIndexModel)
       {
            HttpResponseMessage response;
            try
            {
                PortalIndexModel portalIndexData = _service.InsertCreateIndexDataByRevisionTypes(portalIndexModel);
                if (!Equals(portalIndexData, null))
                {
                    response = CreateCreatedResponse(new PortalIndexResponse { PortalIndex = portalIndexData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(portalIndexData.CatalogIndexId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Creates Index
        /// </summary>
        /// <param name="indexName">Index Name</param>
        /// <param name="revisionType">Revision Type</param>
        /// <param name="portalId">Catalog ID for which search index is being made.</param>
        /// <param name="searchIndexMonitorId">Search Index monitor for which create index is being called.</param>
        /// <param name="searchIndexServerStatusId">Server status ID.</param>
        /// <param name="isPreviewProductionEnabled">To check whether preview production is enabled or not.</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        /// <param name="newIndexName">New Name for Index Creation</param>
        /// <returns>Creates search indexes.</returns>
        [ResponseType(typeof(PortalIndexResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CreateIndex(string indexName, string revisionType, bool isPreviewProductionEnabled, bool isPublishDraftProductsOnly, int portalId, int searchIndexMonitorId, int searchIndexServerStatusId, string newIndexName)
        {
            HttpResponseMessage response;
            try
            { 
                _service.CreateIndex(indexName, revisionType, portalId, searchIndexMonitorId, searchIndexServerStatusId, newIndexName, isPreviewProductionEnabled, isPublishDraftProductsOnly);
                //Create empty OK response if index is created successfully.
                response = CreateOKResponse(new PortalIndexResponse());
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Creates Index
        /// </summary>
        /// <param name="indexName">Index Name</param>
        /// <param name="revisionType">Revision Type</param>
        /// <param name="portalId">Catalog ID for which search index is being made.</param>
        /// <param name="searchIndexMonitorId">Search Index monitor for which create index is being called.</param>
        /// <param name="searchIndexServerStatusId">Server status ID.</param>
        /// <param name="isPreviewProductionEnabled">To check whether preview production is enabled or not.</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        /// <returns>Creates search indexes.</returns>
        [ResponseType(typeof(PortalIndexResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CreateIndex(string indexName, string revisionType, bool isPreviewProductionEnabled, bool isPublishDraftProductsOnly, int portalId, int searchIndexMonitorId, int searchIndexServerStatusId)
        {
            HttpResponseMessage response;
            try
            {
                _service.CreateIndex(indexName, revisionType, portalId, searchIndexMonitorId, searchIndexServerStatusId, "", isPreviewProductionEnabled, isPublishDraftProductsOnly);
                //Create empty OK response if index is created successfully.
                response = CreateOKResponse(new PortalIndexResponse());
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalIndexResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #region Elastic search
        /// <summary>
        /// Performs full text keyword search.(pass categoryId and other field as per need of filter data.)
        /// </summary>
        /// <param name="model">The model of the keyword search.</param>
        /// <returns></returns>
        [ResponseType(typeof(KeywordSearchResponse))]
        [HttpPost]
        public virtual HttpResponseMessage FullTextSearch([FromBody] SearchRequestModel model)
        {
            HttpResponseMessage response;
            try
            {
                var data = _cache.FullTextSearch(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(PublishProductModel))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductDetailsBySKU([FromBody] SearchRequestModel model)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductDetailsBySKU(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                PublishProductResponse publishProduct = new PublishProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(publishProduct);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                PublishProductResponse publishProduct = new PublishProductResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(publishProduct);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(KeywordSearchResponse))]
        [HttpPost]
        public virtual HttpResponseMessage FacetSearch([FromBody] SearchRequestModel model)
        {
            HttpResponseMessage response;

            try
            {
                var data = _cache.FacetSearch(model, RouteUri, RouteTemplate);

                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        /// <summary>
        ///Get SEO Url details on the basis of SEO Url.
        /// </summary>
        /// <param name="seoUrl">SEO Url.</param>
        /// <returns>Returns SEO Url details.</returns>
        [ResponseType(typeof(SEOUrlResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSEOUrlDetails(string seoUrl)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSEOUrlDetails(seoUrl, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEOUrlResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new SEOUrlResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SEOUrlResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Saves boost values for search.
        /// </summary>
        /// <param name="model">Boost data for product, category and attribute.</param>
        /// <returns>True if boost value is saved; False if boost data fails to save.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveBoostValues([FromBody] BoostDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.SaveBoostVales(model), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Deletes the boost value of a product, product-category or field if it is removed.
        /// </summary>
        /// <param name="model">Boost data model.</param>
        /// <returns>Boost value if the data is removed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteBoostValue([FromBody]BoostDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteBoostValue(model) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of global product boost list.
        /// </summary>
        /// <returns>List of global products with boost value.</returns>
        [ResponseType(typeof(SearchGlobalProductBoostListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGlobalProductBoostList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetGlobalProductBoostList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchGlobalProductBoostListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                SearchGlobalProductBoostListResponse data = new SearchGlobalProductBoostListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets global product category boost list.
        /// </summary>
        /// <returns>List of product category with boost values.</returns>
        [ResponseType(typeof(SearchGlobalProductCategoryBoostListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGlobalProductCategoryBoostList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetGlobalProductCategoryBoostList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchGlobalProductCategoryBoostListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                SearchGlobalProductCategoryBoostListResponse data = new SearchGlobalProductCategoryBoostListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of field level boost list.
        /// </summary>
        /// <returns>List of field level boost.</returns>
        [ResponseType(typeof(SearchDocumentMappingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFieldBoostList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFieldBoostList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchDocumentMappingListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                SearchDocumentMappingListResponse data = new SearchDocumentMappingListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Keyword Search Suggestion.
        /// </summary>
        /// <param name="model">The model of the keyword search.</param>
        /// <returns>Returns suggestions in KeywordSearchModel.</returns>
        [ResponseType(typeof(KeywordSearchResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetKeywordSearchSuggestion([FromBody] SearchRequestModel model)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetKeywordSearchSuggestion(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of search index monitor list.
        /// </summary>
        /// <returns>List of search index monitor.</returns>
        [ResponseType(typeof(SearchIndexMonitorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchIndexMonitorList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSearchIndexMonitorList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchIndexMonitorListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SearchIndexMonitorListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of search index server status list.
        /// </summary>
        /// <returns>List of search index monitor.</returns>
        [ResponseType(typeof(SearchIndexServerStatusListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchIndexServerStatusList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSearchIndexServerStatusList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchIndexServerStatusListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                SearchIndexServerStatusListResponse data = new SearchIndexServerStatusListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        [ResponseType(typeof(KeywordSearchResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetSearchProfileProducts([FromBody] SearchProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                var data = _cache.GetSearchProfileProducts(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                KeywordSearchResponse keywordSearch = new KeywordSearchResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Elastic search index.
        /// </summary>
        /// <param name="catalogIndexId">Catalog index id.</param>
        /// <returns>True false response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage DeleteIndex(int catalogIndexId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete search.
                bool deleted = _service.DeleteIndex(catalogIndexId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #region Synonyms
        /// <summary>
        /// Create synonyms for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns response.</returns>
        [ResponseType(typeof(SearchSynonymsResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSearchSynonyms([FromBody] SearchSynonymsModel model)
        {
            HttpResponseMessage response;
            try
            {
                SearchSynonymsModel SearchSynonyms = _service.CreateSearchSynonyms(model);

                if (!Equals(SearchSynonyms, null))
                {
                    response = CreateCreatedResponse(new SearchSynonymsResponse { SearchSynonyms = SearchSynonyms });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(SearchSynonyms.SearchSynonymsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get synonyms data for search.
        /// </summary>
        /// <param name="searchSynonymsId">Uses synonyms id.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(SearchSynonymsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchSynonyms(int searchSynonymsId)
        {
            HttpResponseMessage response;

            try
            {
                //Get synonyms by id.
                string data = _cache.GetSearchSynonyms(searchSynonymsId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchSynonymsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update synonyms data for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(SearchSynonymsResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateSearchSynonyms([FromBody] SearchSynonymsModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateSearchSynonyms(model);
                response = isUpdated ? CreateOKResponse(new SearchSynonymsResponse { SearchSynonyms = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                SearchSynonymsResponse synonymsResponse = new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(synonymsResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                SearchSynonymsResponse synonymsResponse = new SearchSynonymsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(synonymsResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets synonyms list.
        /// </summary>
        /// <returns>List of synonyms for search.</returns>
        [ResponseType(typeof(SearchSynonymsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchSynonymsList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSearchSynonymsList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchSynonymsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                SearchSynonymsListResponse data = new SearchSynonymsListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete synonyms by id.
        /// </summary>
        /// <param name="SearchSynonymsId">Uses synonyms ids.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteSearchSynonyms([FromBody] ParameterModel SearchSynonymsId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete synonyms.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteSearchSynonyms(SearchSynonymsId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        ///Write Synonyms file for catalog 
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id.</param>
        /// <param name="isSynonymsFile">if true create Synonyms file else keyword file.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage WriteSearchFile(int publishCatalogId,bool isSynonymsFile)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.WriteSearchFile(publishCatalogId, isSynonymsFile) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        #endregion

        #region Keywords Redirect
        /// <summary>
        /// Gets keywords redirect list.
        /// </summary>
        /// <returns>List of keywords and redirected urls.</returns>
        [ResponseType(typeof(SearchKeywordsRedirectListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCatalogKeywordsRedirectList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCatalogKeywordsRedirectList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchKeywordsRedirectListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                SearchKeywordsRedirectListResponse data = new SearchKeywordsRedirectListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create keywords and its redirected url for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns response.</returns>
        [ResponseType(typeof(SearchKeywordsRedirectResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSearchKeywordsRedirect([FromBody] SearchKeywordsRedirectModel model)
        {
            HttpResponseMessage response;
            try
            {
                SearchKeywordsRedirectModel searchKeywords = _service.CreateSearchKeywordsRedirect(model);

                if (!Equals(searchKeywords, null))
                {
                    response = CreateCreatedResponse(new SearchKeywordsRedirectResponse { SearchKeywords = searchKeywords });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(searchKeywords.SearchKeywordsRedirectId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get keywords data for search.
        /// </summary>
        /// <param name="searchKeywordsRedirectId">Uses keywords id.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(SearchKeywordsRedirectResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchKeywordsRedirect(int searchKeywordsRedirectId)
        {
            HttpResponseMessage response;

            try
            {
                //Get keywords by id.
                string data = _cache.GetSearchKeywordsRedirect(searchKeywordsRedirectId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchKeywordsRedirectResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update keywords data for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(SearchKeywordsRedirectResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateSearchKeywordsRedirect([FromBody] SearchKeywordsRedirectModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateSearchKeywordsRedirect(model);
                response = isUpdated ? CreateOKResponse(new SearchKeywordsRedirectResponse { SearchKeywords = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                SearchKeywordsRedirectResponse keywordsResponse = new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(keywordsResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                SearchKeywordsRedirectResponse keywordsResponse = new SearchKeywordsRedirectResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(keywordsResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete keywords by id.
        /// </summary>
        /// <param name="SearchKeywordsRedirectId">Uses keywords ids.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteSearchKeywordsRedirect([FromBody] ParameterModel SearchKeywordsRedirectId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete keywords.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteSearchKeywordsRedirect(SearchKeywordsRedirectId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}