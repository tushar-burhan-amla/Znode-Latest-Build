using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class QuickOrderClient : BaseClient, IQuickOrderClient
    {
        // Gets the list of quick order products.
        public virtual QuickOrderProductListModel GetQuickOrderProductList(FilterCollection filters, QuickOrderParameterModel parameters)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetQuickOrderProductList();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            //Get response
            ApiStatus status = new ApiStatus();
            QuickOrderProductListResponse response = PostResourceToEndpoint<QuickOrderProductListResponse>(endpoint, JsonConvert.SerializeObject(parameters), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            QuickOrderProductListModel list = new QuickOrderProductListModel { Products = response?.Products };
            list.MapPagingDataFromResponse(response);
            return list;
        }


        //This method return random quick order product basic details
        public virtual QuickOrderProductListModel GetDummyQuickOrderProductList(FilterCollection filters, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetDummyQuickOrderProductList();
            endpoint += BuildEndpointQueryString(null, filters, null, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            QuickOrderProductListResponse response = GetResourceFromEndpoint<QuickOrderProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            QuickOrderProductListModel list = new QuickOrderProductListModel { Products = response?.Products };
            list.MapPagingDataFromResponse(response);
            return list;
        }
    }
}
