using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.Search;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Api.Controllers
{
    public class SearchProfileController : BaseController
    {
        #region Private Variables
        private readonly ISearchProfileCache _cache;
        private readonly ISearchProfileService _searchProfileService;
        #endregion

        #region Constructor
        public SearchProfileController(ISearchProfileService searchProfileService)
        {
            _searchProfileService = searchProfileService;
            _cache = new SearchProfileCache(_searchProfileService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get list of Search Profiles.
        /// </summary>
        /// <returns>Returns list of SearchProfiles.</returns>
        [ResponseType(typeof(SearchProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchProfilesList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Creates a search profile
        /// </summary>
        /// <param name="model">model with search profile details</param>
        /// <returns>returns created search profile in model</returns>
        [ResponseType(typeof(SearchProfileResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] SearchProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                SearchProfileModel searchProfile = _searchProfileService.Create(model);

                if (HelperUtility.IsNotNull(searchProfile))
                {
                    response = CreateCreatedResponse(new SearchProfileResponse { SearchProfile = searchProfile });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(searchProfile.SearchProfileId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.CreationFailed });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get SearchProfile by SearchProfile id.
        /// </summary>
        /// <param name="searchProfileId">SearchProfile id to get SearchProfile details.</param>
        /// <returns>Returns SearchProfile model.</returns>
        [ResponseType(typeof(SearchProfileResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int searchProfileId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchProfile(searchProfileId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update SearchProfile details.
        /// </summary>
        /// <param name="searchProfileModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(SearchProfileResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] SearchProfileModel searchProfileModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update SearchProfile.
                response = _searchProfileService.UpdateSearchProfile(searchProfileModel) ? CreateCreatedResponse(new SearchProfileResponse { SearchProfile = searchProfileModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(searchProfileModel.SearchProfileId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.InternalItemNotUpdated });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Set default search profile.
        /// </summary>
        /// <param name="portalSearchProfileModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SetDefaultSearchProfile([FromBody] PortalSearchProfileModel portalSearchProfileModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.SetDefaultSearchProfile(portalSearchProfileModel) });
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
        /// Delete SearchProfile.
        /// </summary>
        /// <param name="searchProfileId">SearchProfile Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel searchProfileId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.DeleteSearchProfile(searchProfileId) });
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
        /// Get search features list.
        /// </summary>        
        /// <returns>Returns account list.</returns>
        [ResponseType(typeof(SearchProfileResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDetails()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetSearchProfileDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        [ResponseType(typeof(SearchFeaturesListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFeaturesByQueryId(int queryId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetFeaturesByQueryId(queryId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchFeaturesListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchFeaturesListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        #region Search Triggers
        /// <summary>
        /// Get list of Search Profiles Triggers.
        /// </summary>
        /// <returns>Returns list of SearchProfilesTriggers.</returns>
        [ResponseType(typeof(SearchTriggersListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchTriggerList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchTriggerList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchTriggersListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchTriggersListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Creates a search profile triggers.
        /// </summary>
        /// <param name="model">model with search profile triggers</param>
        /// <returns>returns created search profile triggers in model</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSearchTriggers([FromBody] SearchTriggersModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.CreateSearchTriggers(model) });
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
        /// Get Search Profile triggers by searchProfileTriggerId.
        /// </summary>
        /// <param name="searchProfileTriggerId">searchProfileTriggerId to get Search Profile Triggers.</param>
        /// <returns>Returns Search Profile Triggers model.</returns>
        [ResponseType(typeof(SearchTriggersResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchTrigger(int searchProfileTriggerId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchTrigger(searchProfileTriggerId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchTriggersResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchTriggersResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchTriggersResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Search Profile Triggers.
        /// </summary>
        /// <param name="searchTriggerModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateSearchTriggers([FromBody] SearchTriggersModel searchTriggerModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.UpdateSearchTriggers(searchTriggerModel) });
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
        /// Delete Search Profile Triggers.
        /// </summary>
        /// <param name="searchProfileTriggerId">searchProfileTriggerId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteSearchTriggers(ParameterModel searchProfileTriggerId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.DeleteSearchTriggers(searchProfileTriggerId) });
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
        #endregion

        #region Search Facets
        /// <summary>
        /// Associate UnAssociated search attributes to search profile.
        /// </summary>
        /// <returns>Returns list of search attributes.</returns>
        [ResponseType(typeof(SearchAttributesListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedUnAssociatedCatalogAttributes()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedUnAssociatedCatalogAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchAttributesListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchAttributesListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchAttributesListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// UnAssociate search attributes from search profile.
        /// </summary>
        /// <param name="searchProfilesAttributeMappingIds">searchProfilesAttributeMappingIds to unassociate attributes.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateAttributesFromProfile([FromBody] ParameterModel searchProfilesAttributeMappingIds)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.UnAssociateAttributesFromProfile(searchProfilesAttributeMappingIds) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate UnAssociated search attributes to search profile.
        /// </summary>
        /// <param name="searchAttributesModel">searchAttributesModel</param>
        /// <returns>Returns associate profiles if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateAttributesToProfile([FromBody] SearchAttributesModel searchAttributesModel)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.AssociateAttributesToProfile(searchAttributesModel) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        [ResponseType(typeof(SearchProfilePortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage SearchProfilePortalList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.SearchProfilePortalList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchProfilePortalListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfilePortalListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get list of Unassociated portals
        /// </summary>
        /// <returns>Returns list of Unassociated portals</returns>
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UnAssociatedPortalList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssociatedPortalList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateUnassociatePortalToSearchProfile(SearchProfileParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _searchProfileService.AssociatePortalToSearchProfile(model) });
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

        [ResponseType(typeof(SearchAttributesListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CatalogBasedAttributes(ParameterModel associatedAttributes)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCatalogBasedAttributes(associatedAttributes, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchAttributesListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchAttributesListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchAttributesListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get field value list by catalog id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id.</param>
        /// <param name="searchProfileId">search profile id.</param>
        /// <returns>field list for field value.</returns>
        [ResponseType(typeof(SearchProfileResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFieldValuesList(int publishCatalogId, int searchProfileId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetFieldValuesList(publishCatalogId,searchProfileId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SearchProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        [HttpPost]
        public virtual HttpResponseMessage PublishSearchProfile(int searchProfileId)
        {
            HttpResponseMessage response;
            try
            {
                ISearchService _searchService = GetService<ISearchService>();

                bool publishStatus = _searchProfileService.PublishSearchProfile(searchProfileId);
                if (publishStatus)
                {
                    _searchService.IndexCreationAfterSearchProfilePublish(searchProfileId);
                }
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = publishStatus });

            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;

        }

        // To get the catalog list that is not associated with any of the search profiles.
        [HttpGet]
        public virtual HttpResponseMessage GetCatalogList()
        {
            HttpResponseMessage response;

            try
            {
                TypeaheadResponselistModel data = _searchProfileService.GetCatalogListForSearchProfile();
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }
    }
}
