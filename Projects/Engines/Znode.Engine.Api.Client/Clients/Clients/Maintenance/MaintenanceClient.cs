using System.Collections.ObjectModel;
using System.Net;

using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class MaintenanceClient : BaseClient, IMaintenanceClient
    {
        //To delete published data of all catalog, store,cms & elastic search.
        public virtual bool PurgeAllPublishedData()
        {
            string endpoint = MaintenanceEndpoint.PurgeAllPublishedData();

            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response;
        }
    }
}
