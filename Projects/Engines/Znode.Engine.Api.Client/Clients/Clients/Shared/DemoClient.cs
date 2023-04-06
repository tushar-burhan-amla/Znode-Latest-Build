using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using static Znode.Engine.Api.Client.Endpoints.UsersEndpoint;

namespace Znode.Engine.Api.Client.Clients
{
    public class DemoClient : BaseClient, IDemoClient
    {
        public virtual UserModel GetAccount(int accountId, ExpandCollection expands)
        {
            var endpoint = Get(accountId);
            endpoint += BuildEndpointQueryString(expands);

            var status = new ApiStatus();
            var response = GetResourceFromEndpoint<UserResponse>(endpoint, status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.User;
        }
    }
}
