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
    public class StateClient : BaseClient, IStateClient
    {
        // Gets the list of state.
        public virtual StateListModel GetStateList(FilterCollection filters, SortCollection sorts) => GetStateList(filters, sorts, null, null);

        // Gets the list of states.
        public virtual StateListModel GetStateList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = StateEndpoint.GetStateList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            StateListResponse response = GetResourceFromEndpoint<StateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            StateListModel list = new StateListModel { States = response?.States };
            list.MapPagingDataFromResponse(response);

            return list;
        }

    }
}
