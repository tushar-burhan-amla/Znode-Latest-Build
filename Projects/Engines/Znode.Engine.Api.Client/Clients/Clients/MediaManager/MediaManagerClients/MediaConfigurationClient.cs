using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
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
    public class MediaConfigurationClient : BaseClient, IMediaConfigurationClient
    {
        //Get List Of Servers Available.
        public virtual MediaServerListModel GetMediaServerList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            string endpoint = MediaConfigurationEndpoint.GetMediaServerList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, pageIndex, recordPerPage);

            ApiStatus status = new ApiStatus();
            MediaConfigurationResponse response = GetResourceFromEndpoint<MediaConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            MediaServerListModel list = new MediaServerListModel { MediaServers = response?.MediaServers };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Current Configuration setting Present.
        public virtual MediaConfigurationModel GetMediaConfiguration(FilterCollection filters) 
            => GetMediaConfiguration(filters, null);

        //Get Current Configuration setting Present.
        public virtual MediaConfigurationModel GetMediaConfiguration(FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = MediaConfigurationEndpoint.GetMediaConfiguration();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            MediaConfigurationResponse response = GetResourceFromEndpoint<MediaConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent};
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.MediaConfiguration;
        }

        //Create New Media Configration Setting.
        public virtual MediaConfigurationModel CreateMediaConfiguration(MediaConfigurationModel model)
        {
            string endpoint = MediaConfigurationEndpoint.Create();
            ApiStatus status = new ApiStatus();

            MediaConfigurationResponse response = PostResourceToEndpoint<MediaConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.MediaConfiguration;
        }

        //Update Existing Media Configuration Setting.
        public virtual MediaConfigurationModel UpdateMediaConfiguration(MediaConfigurationModel model)
        {
            string endpoint = MediaConfigurationEndpoint.Update();

            ApiStatus status = new ApiStatus();
            MediaConfigurationResponse response = PutResourceToEndpoint<MediaConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.MediaConfiguration;
        }

        //Get Default Configuration setting for Media.
        public virtual MediaConfigurationModel GetDefaultMediaConfiguration()
        {
            string endpoint = MediaConfigurationEndpoint.GetDefaultMediaConfiguration();

            ApiStatus status = new ApiStatus();
            MediaConfigurationResponse response = GetResourceFromEndpoint<MediaConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.MediaConfiguration;
        }

        //For Sync media 
        public virtual void SyncMedia(string folderName)
        {
            string endpoint = MediaConfigurationEndpoint.SyncMedia(folderName);

            ApiStatus status = new ApiStatus();
            MediaConfigurationResponse response = GetResourceFromEndpoint<MediaConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
        }

        #region Generate Images.
        //Generate all images.
        public virtual bool GenerateImages()
        {
            string endpoint = MediaConfigurationEndpoint.GenerateImages();

            ApiStatus status = new ApiStatus();
            Task<TrueFalseResponse> response = GetResourceFromEndpointAsync<TrueFalseResponse>(endpoint, status);

            return Convert.ToBoolean(response?.Status);
        }
        #endregion
    }
}
