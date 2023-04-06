using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebStoreMessageClient : BaseClient, IWebStoreMessageClient
    {
        //Get Message by Message Key, Area and Portal Id.
        public virtual ManageMessageModel GetMessage(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = WebStoreMessageEndpoint.GetMessage();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreMessageResponse response = GetResourceFromEndpoint<WebStoreMessageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Message;
        }

        //Get Messages by Area and Portal Id.
        public virtual ManageMessageListModel GetMessages(ExpandCollection expands, FilterCollection filters, int localeId)
        {
            string endpoint = WebStoreMessageEndpoint.GetMessages(localeId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreMessageListResponse response = GetResourceFromEndpoint<WebStoreMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ManageMessageListModel list = new ManageMessageListModel { ManageMessages = response?.Messages };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get content container based on the container Key passed.
        public virtual ContentContainerDataModel GetContentContainer(string containerKey, int localeId, int portalId = 0, int profileId = 0)
        {
            string endpoint = WebStoreMessageEndpoint.GetContainer();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);
            endpoint = BuildCustomEndpointQueryString(endpoint, "containerKey", containerKey);
            endpoint = BuildCustomEndpointQueryString(endpoint, "localeId", localeId.ToString());
            endpoint = BuildCustomEndpointQueryString(endpoint, "portalId", portalId.ToString());
            endpoint = BuildCustomEndpointQueryString(endpoint, "profileId", profileId.ToString());

            ApiStatus status = new ApiStatus();
            ContentContainerDataResponse response = GetResourceFromEndpoint<ContentContainerDataResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ContentContainerData;
        }
    }
}
