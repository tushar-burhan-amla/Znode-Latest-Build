using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class NavigationClient : BaseClient, INavigationClient
    {
        public virtual NavigationModel GetNavigationDetails(NavigationParamModel model)
        {
            var endpoint = NavigationEndpoint.GetNavigationDetails();
            var status = new ApiStatus();

            var response = PostResourceToEndpoint<NavigationResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.NavigationModel;
        }
    }
}
