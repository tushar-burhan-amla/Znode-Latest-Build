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
    public class PublishBrandClient : BaseClient, IPublishBrandClient
    {
        // Gets the list of Brands.
        public virtual BrandListModel GetPublishBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishBrandEndpoint.GetPublishBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get a brand by brand ID.
        public virtual BrandModel GetPublishBrand(int brandId, int localeId, int portalId)
        {
            string endpoint = PublishBrandEndpoint.GetPublishBrand(brandId, localeId, portalId);

            ApiStatus status = new ApiStatus();
            BrandResponse response = GetResourceFromEndpoint<BrandResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Brand;
        }
    }
}
