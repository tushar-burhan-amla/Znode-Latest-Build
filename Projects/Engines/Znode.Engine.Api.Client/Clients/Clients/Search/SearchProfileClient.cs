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
using Znode.Engine.Api.Models.Responses.Search;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class SearchProfileClient : BaseClient, ISearchProfileClient
    {
        #region Search Profile
        //Gets the list of Search Profiles 
        public SearchProfileListModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.GetSearchProfileList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchProfileListResponse response = GetResourceFromEndpoint<SearchProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchProfileListModel list = null;
            if (HelperUtility.IsNotNull(response))
            {
                list = new SearchProfileListModel { SearchProfileList = response.SearchProfileList, PublishCatalogId = response.PublishCatalogId, CatalogName = response.CatalogName };
                list.MapPagingDataFromResponse(response);
            }
            return list;
        }

        //Create Search Profile
        public SearchProfileModel Create(SearchProfileModel searchProfileModel)
        {
            //Get Endpoint
            string endpoint = SearchProfileEndpoint.Create();

            //Get Response
            ApiStatus status = new ApiStatus();
            SearchProfileResponse response = PostResourceToEndpoint<SearchProfileResponse>(endpoint, JsonConvert.SerializeObject(searchProfileModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchProfile;
        }

        //Gets search profile by its id
        public SearchProfileModel GetSearchProfile(int profileId)
        {
            string endpoint = SearchProfileEndpoint.GetSearchProfile(profileId);

            ApiStatus status = new ApiStatus();
            SearchProfileResponse response = GetResourceFromEndpoint<SearchProfileResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchProfile;
        }

        //Updates search profile
        public virtual SearchProfileModel UpdateSearchProfile(SearchProfileModel searchProfileModel)
        {
            string endpoint = SearchProfileEndpoint.UpdateSearchProfile();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            SearchProfileResponse response = PutResourceToEndpoint<SearchProfileResponse>(endpoint, JsonConvert.SerializeObject(searchProfileModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchProfile;
        }

        //gets search profile details
        public SearchProfileModel GetSearchProfileDetails(FilterCollection filters)
        {
            string endpoint = SearchProfileEndpoint.ProfileDetails();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            SearchProfileResponse response = GetResourceFromEndpoint<SearchProfileResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchProfile;
        }

        public KeywordSearchModel GetSearchProfileProducts(SearchProfileModel model, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection)
        {
            string endpoint = SearchProfileEndpoint.GetSearchProfileProducts();
            endpoint += BuildEndpointQueryString(expandCollection, filters, sortCollection, null, null);

            ApiStatus status = new ApiStatus();

            KeywordSearchResponse response = PostResourceToEndpoint<KeywordSearchResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return HelperUtility.IsNotNull(response) ? response.Search : new KeywordSearchModel { Products = new List<SearchProductModel>() };
        }

        //Get Features list by query id.
        public List<SearchFeatureModel> GetFeaturesByQueryId(int queryId)
        {
            string endpoint = SearchProfileEndpoint.GetFeaturesByQueryId(queryId);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchFeaturesListResponse response = GetResourceFromEndpoint<SearchFeaturesListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchFeaturesList;

        }

        //Deletes search Profile
        public bool DeleteSearchProfile(ParameterModel searchProfileId)
        {
            string endpoint = SearchProfileEndpoint.DeleteSearchProfile();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchProfileId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public SearchProfilePortalListModel GetSearchProfilePortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.GetSearchProfilePortalList();

            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchProfilePortalListResponse response = GetResourceFromEndpoint<SearchProfilePortalListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchProfilePortalListModel list = new SearchProfilePortalListModel { SearchProfilePortalList = response?.SearchProfilePortalList };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Set default search profile.
        public virtual bool SetDefaultSearchProfile(PortalSearchProfileModel portalSearchProfileModel)
        {
            string endpoint = SearchProfileEndpoint.SetDefaultSearchProfile();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalSearchProfileModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        #region Search Triggers 
        //Gets list of search profile triggers.
        public SearchTriggersListModel GetSearchProfileTriggerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.GetSearchProfileTriggerList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchTriggersListResponse response = GetResourceFromEndpoint<SearchTriggersListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchTriggersListModel list = new SearchTriggersListModel { SearchTriggersList = response?.SearchTriggersList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create search profile triggers.
        public bool CreateSearchProfileTriggers(SearchTriggersModel searchTriggersModel)
        {
            //Get Endpoint
            string endpoint = SearchProfileEndpoint.CreateSearchProfileTriggers();

            //Get Response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchTriggersModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get search profile trigger on the basis of searchProfileTriggerId id Endpoint.
        public SearchTriggersModel GetSearchProfileTriggers(int searchProfileTriggerId)
        {
            string endpoint = SearchProfileEndpoint.GetSearchProfileTriggers(searchProfileTriggerId);

            ApiStatus status = new ApiStatus();
            SearchTriggersResponse response = GetResourceFromEndpoint<SearchTriggersResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchTrigger;
        }

        //Updates search profile triggers.
        public bool UpdateSearchProfileTriggers(SearchTriggersModel searchTriggersModel)
        {
            string endpoint = SearchProfileEndpoint.UpdateSearchProfileTriggers();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchTriggersModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Deletes search profile triggers.
        public bool DeleteSearchProfileTriggers(ParameterModel searchProfileTriggerId)
        {
            string endpoint = SearchProfileEndpoint.DeleteSearchProfileTriggers();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchProfileTriggerId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public bool AssociatePortalToSearchProfile(SearchProfileParameterModel parameterModelUserProfile)
        {
            string endpoint = SearchProfileEndpoint.AssociatePortalToSearchProfile();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModelUserProfile), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get searchable attributes list based on catalog Id
        public SearchAttributesListModel GetCatalogBasedAttributes(ParameterModel associatedAttributes, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = SearchProfileEndpoint.GetCatalogBasedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            SearchAttributesListResponse response = PostResourceToEndpoint<SearchAttributesListResponse>(endpoint, JsonConvert.SerializeObject(associatedAttributes), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchAttributesListModel list = new SearchAttributesListModel { SearchAttributeList = response?.SearchAttributesList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets unassociated search profile list
        public PortalListModel GetUnAssociatedPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.GetUnAssociatedPortalList();

            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, page, recordPerPage);

            //Get response
            ApiStatus status = new ApiStatus();
            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };

            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion

        #region Search Facets
        //Gets the list of Search Attributes. 
        public SearchAttributesListModel GetAssociatedUnAssociatedCatalogAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.GetSearchAttributeList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchAttributesListResponse response = GetResourceFromEndpoint<SearchAttributesListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchAttributesListModel list = new SearchAttributesListModel { SearchAttributeList = response?.SearchAttributesList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Associate UnAssociated search attributes to search profile.
        public virtual bool AssociateAttributesToProfile(SearchAttributesModel searchAttributesModel)
        {
            string endpoint = SearchProfileEndpoint.AssociateAttributesToProfile();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchAttributesModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //UnAssociate each attributes from search profile.
        public virtual bool UnAssociateAttributesFromProfile(ParameterModel profileIds)
        {
            string endpoint = SearchProfileEndpoint.UnAssociateAttributesFromProfile();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Get field Value List by Catalog id.
        public SearchProfileModel GetfieldValuesList(int publishCatalogId, int searchProfileId)
        {
            string endpoint = SearchProfileEndpoint.GetfieldValuesList(publishCatalogId, searchProfileId);

            ApiStatus status = new ApiStatus();
            SearchProfileResponse response = GetResourceFromEndpoint<SearchProfileResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchProfile;
        }
        #endregion

        public virtual bool PublishSearchProfile(int searchProfileId)
        {
            //Get Endpoint.
            string endpoint = SearchProfileEndpoint.PublishSearchProfile(searchProfileId);
            
            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(searchProfileId), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;

        }

        // To get the catalog list that is not associated with any of the search profiles.
        public virtual TypeaheadResponselistModel GetCatalogListForSearchProfile()
        {
            string endpoint = SearchProfileEndpoint.GetCatalogList();

            //Get response.
            ApiStatus status = new ApiStatus();
            TypeaheadListResponse response = GetResourceFromEndpoint<TypeaheadListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TypeaheadResponselistModel list = new TypeaheadResponselistModel { Typeaheadlist = response?.Typeaheadlist };
            return list;
        }
    }
}
