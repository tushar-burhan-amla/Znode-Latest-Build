using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class PortalUnitClient : BaseClient, IPortalUnitClient
    {
        public virtual PortalUnitModel GetPortalUnit(int portalId)
        {
            string endpoint = PortalUnitEndpoint.Get(portalId);
            ApiStatus status = new ApiStatus();
            PortalUnitResponse response = GetResourceFromEndpoint<PortalUnitResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PortalUnit;
        }

        public virtual PortalUnitModel CreateUpdatePortalUnit(PortalUnitModel portalUnitModel)
        {
            string endpoint = PortalUnitEndpoint.Update();
            ApiStatus status = new ApiStatus();
            PortalUnitResponse response = PutResourceToEndpoint<PortalUnitResponse>(endpoint, Newtonsoft.Json.JsonConvert.SerializeObject(portalUnitModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.PortalUnit;
        }
    }
}
