using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WebStoreLocatorClient : BaseClient, IWebStoreLocatorClient
    {
        //Get Store Locator list.
        public virtual List<StoreLocatorDataModel> GetPortalList(FilterCollection filters,SortCollection sorts)
        {
            string endpoint = WebStoreLocatorEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreLocatorResponse response = GetResourceFromEndpoint<WebStoreLocatorResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.StoreLocatorList;
        }
    }
}
