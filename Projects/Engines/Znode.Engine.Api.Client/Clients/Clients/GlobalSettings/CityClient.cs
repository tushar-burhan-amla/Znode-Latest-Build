using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class CityClient : BaseClient, ICityClient
    {
        // Gets the list of cities.
        public virtual CityListModel GetCityList(FilterCollection filters, SortCollection sorts) => GetCityList(filters, sorts, null, null);

        // Gets the list of cities.
        public virtual CityListModel GetCityList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CityEndpoint.GetCityList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CityListResponse response = GetResourceFromEndpoint<CityListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CityListModel list = new CityListModel { Cities = response?.Cities };
            list.MapPagingDataFromResponse(response);

            return list;
        }

    }
}
