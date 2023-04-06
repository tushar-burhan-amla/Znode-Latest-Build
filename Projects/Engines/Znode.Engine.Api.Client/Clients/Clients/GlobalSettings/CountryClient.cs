using Newtonsoft.Json;
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
    public class CountryClient : BaseClient, ICountryClient
    {
        // Gets the list of countries.
        public virtual CountryListModel GetCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCountryList(expands, filters, sorts, null, null);

        // Gets the list of countries.
        public virtual CountryListModel GetCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CountryEndpoint.GetCountryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CountryListResponse response = GetResourceFromEndpoint<CountryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CountryListModel list = new CountryListModel { Countries = response?.Countries };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets a country as per the filter passed.
        public virtual CountryModel GetCountry(FilterCollection filters)
        {
            string endpoint = CountryEndpoint.GetCountry();
            endpoint += BuildEndpointQueryString(null, filters, null, 0, 0);

            //Get response
            ApiStatus status = new ApiStatus();
            CountryResponse response = GetResourceFromEndpoint<CountryResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Country;
        }

        //Update Countries
        public virtual bool UpdateCountry(DefaultGlobalConfigListModel globalConfigurationListModel)
        {
            //Get Endpoint
            string endpoint = CountryEndpoint.UpdateCountry();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalConfigurationListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
