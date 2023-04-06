using Newtonsoft.Json;
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
    public class UrlRedirectClient : BaseClient, IUrlRedirectClient
    {
        //Create Url Redirect.
        public virtual UrlRedirectModel CreateUrlRedirect(UrlRedirectModel model)
        {
            string endpoint = UrlRedirectEndpoint.Create();

            ApiStatus status = new ApiStatus();
            UrlRedirectResponse response = PostResourceToEndpoint<UrlRedirectResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.UrlRedirect;
        }

        //Get Url Redirect list.
        public virtual UrlRedirectListModel GetUrlRedirectList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = UrlRedirectEndpoint.GetUrlRedirectList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            UrlRedirectResponse response = GetResourceFromEndpoint<UrlRedirectResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            UrlRedirectListModel list = new UrlRedirectListModel { UrlRedirects = response?.UrlRedirectList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Url Redirect on the basis of filter.
        public virtual UrlRedirectModel GetUrlRedirect(FilterCollection filters)
        {
            string endpoint = UrlRedirectEndpoint.GetUrlRedirect();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            UrlRedirectResponse response = GetResourceFromEndpoint<UrlRedirectResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.UrlRedirect;
        }

        //Update Url Redirect.
        public virtual UrlRedirectModel UpdateUrlRedirect(UrlRedirectModel urlRedirectModel)
        {
            string endpoint = UrlRedirectEndpoint.Update();

            ApiStatus status = new ApiStatus();
            UrlRedirectResponse response = PutResourceToEndpoint<UrlRedirectResponse>(endpoint, JsonConvert.SerializeObject(urlRedirectModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.UrlRedirect;
        }

        //Delete Url Redirect.
        public virtual bool DeleteUrlRedirect(ParameterModel urlRedirectId)
        {
            string endpoint = UrlRedirectEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(urlRedirectId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
