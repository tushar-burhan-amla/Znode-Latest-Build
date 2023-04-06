using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class SearchBoostAndBuryRuleController : BaseController
    {
        #region Private Variables
        private readonly ISearchBoostAndBuryRuleCache _cache;
        private readonly ISearchBoostAndBuryRuleService _searchBoostAndBuryRuleService;
        #endregion


        public SearchBoostAndBuryRuleController(ISearchBoostAndBuryRuleService searchBoostAndBuryRuleService)
        {
            _searchBoostAndBuryRuleService = searchBoostAndBuryRuleService;
            _cache = new SearchBoostAndBuryRuleCache(_searchBoostAndBuryRuleService);
        }

        /// <summary>
        /// Get list of Search Profiles.
        /// </summary>
        /// <returns>Returns list of SearchProfiles.</returns>
        [ResponseType(typeof(SearchBoostAndBuryRuleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetBoostAndBuryRules(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchBoostAndBuryRuleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Create boost and bury rule.
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">Model for boost and bury rule.</param>
        /// <returns>Returns created boost and bury rule model.</returns>
        [ResponseType(typeof(SearchBoostAndBuryRuleResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateBoostAndBuryRule([FromBody] SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            HttpResponseMessage response;
            try
            {
                SearchBoostAndBuryRuleModel searchBoostAndBuryRule = _searchBoostAndBuryRuleService.CreateBoostAndBuryRule(searchBoostAndBuryRuleModel);

                if (HelperUtility.IsNotNull(searchBoostAndBuryRule))
                {
                    response = CreateCreatedResponse(new SearchBoostAndBuryRuleResponse { SearchBoostAndBuryRule = searchBoostAndBuryRule });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(searchBoostAndBuryRule.SearchCatalogRuleId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreCaseRequestResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get boost and bury rule data on the basis of search Catalog Rule Id.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Catalog Rule Id to get boost and bury rule details.</param>
        /// <returns>Returns boost and bury details.</returns>
        [ResponseType(typeof(SearchBoostAndBuryRuleResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBoostAndBuryRule(int searchCatalogRuleId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetBoostAndBuryRule(searchCatalogRuleId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchBoostAndBuryRuleResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update case request data.
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">Model to update case request data.</param>
        /// <returns>Returns updated case request model.</returns>
        [ResponseType(typeof(SearchBoostAndBuryRuleResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateBoostAndBuryRule([FromBody] SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update case request.
                response = _searchBoostAndBuryRuleService.UpdateBoostAndBuryRule(searchBoostAndBuryRuleModel) ? CreateCreatedResponse(new SearchBoostAndBuryRuleResponse { SearchBoostAndBuryRule = searchBoostAndBuryRuleModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(searchBoostAndBuryRuleModel.SearchCatalogRuleId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete Search catalog rule.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Rule Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel searchCatalogRuleId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchBoostAndBuryRuleService.Delete(searchCatalogRuleId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Paused an existing catalog search rule for a while.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Rule Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PausedSearchRule(ParameterModel searchCatalogRuleId, bool isPause)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchBoostAndBuryRuleService.PausedSearchRule(searchCatalogRuleId, isPause) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of Search field list.
        /// </summary>
        /// <returns>Returns list of search field list.</returns>
        [ResponseType(typeof(SearchBoostAndBuryRuleResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchableFieldList(int publishCatalogId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchableFieldList(publishCatalogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchBoostAndBuryRuleResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchBoostAndBuryRuleResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }


        [ResponseType(typeof(BoostAndBuryAutocompleteResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetAutoSuggestion(BoostAndBuryParameterModel parameterModel)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAutoSuggestion(parameterModel, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BoostAndBuryAutocompleteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BoostAndBuryAutocompleteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BoostAndBuryAutocompleteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }
    }
}
