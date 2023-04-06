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
    public class LocaleClient : BaseClient, ILocaleClient
    {
        //Get Locale list.
        public virtual LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
           => GetLocaleList(expands, filters, sorts, null, null);

        //Gets paged Locale List.
        public virtual LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = LocaleEndpoint.GetLocaleList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            LocaleListResponse response = GetResourceFromEndpoint<LocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LocaleListModel list = new LocaleListModel { Locales = response?.Locales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get a Locale.
        public virtual LocaleModel GetLocale(FilterCollection filters)
        {
            string endpoint = LocaleEndpoint.GetLocale();
            endpoint += BuildEndpointQueryString(null, filters, null, 0, 0);

            ApiStatus status = new ApiStatus();
            LocaleResponse response = GetResourceFromEndpoint<LocaleResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Locale;
        }

        //Update Locales.
        public virtual bool UpdateLocale(DefaultGlobalConfigListModel globalConfigurationListModel)
        {
            //Get Endpoint
            string endpoint = LocaleEndpoint.UpdateLocale();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalConfigurationListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
