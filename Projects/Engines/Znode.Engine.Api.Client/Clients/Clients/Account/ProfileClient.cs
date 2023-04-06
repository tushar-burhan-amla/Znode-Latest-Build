using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ProfileClient : BaseClient, IProfileClient
    {
        #region Profile
        //Get profile by profile Id
        public virtual ProfileModel GetProfile(int profileId)
        {
            //Get Endpoint.
            string endpoint = ProfileEndPoint.GetProfileByProfileId(profileId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ProfileResponse response = GetResourceFromEndpoint<ProfileResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Profile;
        }

        //Get profile list 
        public virtual ProfileListModel GetProfileList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ProfileEndPoint.GetProfileList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Profile list.
            ProfileListModel profileList = new ProfileListModel { Profiles = response?.Profiles };
            profileList.MapPagingDataFromResponse(response);

            return profileList;
        }

        //Save profile 
        public virtual ProfileModel SaveProfile(ProfileModel model)
        {
            //Get Endpoint
            string endpoint = ProfileEndPoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProfileResponse response = PostResourceToEndpoint<ProfileResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Profile;
        }

        //Update profile 
        public virtual ProfileModel UpdateProfile(ProfileModel model)
        {
            //Get Endpoint
            string endpoint = ProfileEndPoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProfileResponse response = PostResourceToEndpoint<ProfileResponse>(endpoint, JsonConvert.SerializeObject(model), status);
           
            //Check and set status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Profile;
        }

        //Delete profile by profile id
        public virtual bool DeleteProfile(ParameterModel profileId)
        {
            //Get Endpoint.
            string endpoint = ProfileEndPoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion

        #region Profile Base Catalog
        //Get profile catalog list.
        public virtual ProfileCatalogListModel GetProfileCatalogList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ProfileEndPoint.GetProfileCatalogList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            ProfileCatalogListResponse response = GetResourceFromEndpoint<ProfileCatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Profile catalog list.
            ProfileCatalogListModel profileCatalogList = new ProfileCatalogListModel { ProfileCatalogs = response?.ProfileCatalogs };
            profileCatalogList.MapPagingDataFromResponse(response);

            return profileCatalogList;
        }

        //Delete associated catalog to profile by profileCatalogId.
        public virtual bool DeleteAssociatedProfileCatalog(int profileId)
        {
            //Get Endpoint.
            string endpoint = ProfileEndPoint.DeleteAssociatedProfileCatalog(profileId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Associate catalog to profile.
        public virtual bool AssociateCatalogToProfile(ProfileCatalogModel profileCatalogModel)
        {
            //creating endpoint here.
            string endpoint = ProfileEndPoint.AssociateCatalogToProfile();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileCatalogModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }
        #endregion
    }
}

