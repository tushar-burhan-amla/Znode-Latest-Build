using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ProductReviewStateClient : BaseClient, IProductReviewStateClient
    {
        public virtual ProductReviewStateListModel GetProductReviewStates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductReviewStateEndpoint.ProductReviewStateList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductReviewStateListResponse response = GetResourceFromEndpoint<ProductReviewStateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductReviewStateListModel list = new ProductReviewStateListModel { ProductReviewStates = response?.ProductReviewStates };
            list.MapPagingDataFromResponse(response);

            return list;
        }
    }
}
