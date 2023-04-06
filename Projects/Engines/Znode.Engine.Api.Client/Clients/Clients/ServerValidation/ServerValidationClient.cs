using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ServerValidationClient : BaseClient, IServerValidationClient
    {
        public virtual Dictionary<string, string> validateControl(ValidateServerModel model)
        {
            //Get Endpoint
            var endpoint = ServerValidationEndpoint.ValidateDataAtServerSide();

            //Get Serialize object as a response.
            var status = new ApiStatus();
            var response = PostResourceToEndpoint<ServerValidationResponses>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ErrorDictionary;
        }
    }
}
