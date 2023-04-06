using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
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
    public class WebStoreProductClient : BaseClient, IWebStoreProductClient
    {
        //Get product list for webstore.
        public virtual WebStoreProductListModel ProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = WebStoreProductEndpoints.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            WebStoreProductListResponse response = GetResourceFromEndpoint<WebStoreProductListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreProductListModel list = new WebStoreProductListModel { ProductList = response?.ProductsList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get product by product id.
        public virtual WebStoreProductModel GetProduct(int productId, ExpandCollection expands)
        {
            string endpoint = WebStoreProductEndpoints.GetProduct(productId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            WebStoreProductResponse response = GetResourceFromEndpoint<WebStoreProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Product;
        }

        //Get associated product list.
        public virtual WebStoreProductListModel GetAssociatedProducts(ParameterModel productIds)
        {
            string endpoint = WebStoreProductEndpoints.GetAssociatedProducts();

            ApiStatus status = new ApiStatus();
            WebStoreProductListResponse response = PostResourceToEndpoint<WebStoreProductListResponse>(endpoint, JsonConvert.SerializeObject(productIds), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreProductListModel list = new WebStoreProductListModel { ProductList = response?.ProductsList };

            return list;
        }

        //Get product highlights by product id.
        public virtual HighlightListModel GetProductHighlights(ParameterProductModel parameterModel, int productId,int localeId)
        {
            string endpoint = WebStoreProductEndpoints.GetProductHighlights(productId, localeId);

            ApiStatus status = new ApiStatus();
            HighlightListResponse response = PostResourceToEndpoint<HighlightListResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            HighlightListModel list = new HighlightListModel { HighlightList = response?.Highlights };

            return list;
        }
    }
}
