using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebStorePortalClient : BaseClient, IWebStorePortalClient
    {
        //Get Portal information by Portal Id.
        public virtual WebStorePortalModel GetPortal(int portalId, ExpandCollection expands)
        {
            string endpoint = WebStorePortalEndpoint.GetPortal(portalId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            WebStorePortalResponse response = GetResourceFromEndpoint<WebStorePortalResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Portal;
        }

        //Get Portal information by Portal Id, Locale Id and application type.
        public virtual WebStorePortalModel GetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, ExpandCollection expands)
        {
            string endpoint = WebStorePortalEndpoint.GetPortal(portalId, localeId, applicationType);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            WebStorePortalResponse response = GetResourceFromEndpoint<WebStorePortalResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Portal;
        }
        
        //Get Portal information by Portal Id.
        public virtual WebStorePortalModel GetPortal(string domainName, ExpandCollection expands)
        {
            string endpoint = WebStorePortalEndpoint.GetPortal(domainName);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            WebStorePortalResponse response = GetResourceFromEndpoint<WebStorePortalResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Portal;
        }
    }
}
