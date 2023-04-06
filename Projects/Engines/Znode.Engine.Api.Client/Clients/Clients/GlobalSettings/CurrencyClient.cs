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
    public class CurrencyClient : BaseClient, ICurrencyClient
    {
        // Gets the list of Currencies.
        public virtual CurrencyListModel GetCurrencyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCurrencyList(expands, filters, sorts, null, null);

        // Gets the list of Currencies.
        public virtual CurrencyListModel GetCurrencyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CurrencyEndpoint.GetCurrencyList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CurrencyListResponse response = GetResourceFromEndpoint<CurrencyListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CurrencyListModel list = new CurrencyListModel { Currencies = response?.Currencies };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the list of Currencies.
        public virtual CurrencyListModel GetCurrencyCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CurrencyEndpoint.GetCurrencyCultureList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CurrencyListResponse response = GetResourceFromEndpoint<CurrencyListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CurrencyListModel list = new CurrencyListModel { Currencies = response?.Currencies };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the list of Culture.
        public virtual CultureListModel GetCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCultureList(expands, filters, sorts, null, null);

        // Gets the list of Culture.
        public virtual CultureListModel GetCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CurrencyEndpoint.GetCultureList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CultureListResponse response = GetResourceFromEndpoint<CultureListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CultureListModel list = new CultureListModel { Culture = response?.Culture };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets a Currency as per the filter passed.
        public virtual CurrencyModel GetCurrency(FilterCollection filters)
        {
            string endpoint = CurrencyEndpoint.GetCurrency();
            endpoint += BuildEndpointQueryString(null, filters, null, 0, 0);

            //Get response
            ApiStatus status = new ApiStatus();
            CurrencyResponse response = GetResourceFromEndpoint<CurrencyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Currency;
        }

        // Gets a Culture as per the filter passed.
        public virtual CultureModel GetCultureCode(FilterCollection filters)
        {
            string endpoint = CurrencyEndpoint.GetCultureCode();
            endpoint += BuildEndpointQueryString(null, filters, null, 0, 0);

            //Get response
            ApiStatus status = new ApiStatus();
            CultureResponse response = GetResourceFromEndpoint<CultureResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Culture;
        }


        //Update Currencies
        public virtual bool UpdateCurrency(DefaultGlobalConfigListModel globalConfigurationListModel)
        {
            //Get Endpoint
            string endpoint = CurrencyEndpoint.UpdateCurrency();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalConfigurationListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
