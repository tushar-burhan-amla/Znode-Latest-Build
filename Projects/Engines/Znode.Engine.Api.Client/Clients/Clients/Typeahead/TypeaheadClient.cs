using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class TypeaheadClient : BaseClient, ITypeaheadClient
    {
        //Get Suggestions
        public virtual TypeaheadResponselistModel GetSearchlist(TypeaheadRequestModel model)
        {
            //Get Endpoint.
            string endpoint = TypeaheadEndpoint.GetTypeaheadResponse();

            //Get response
            ApiStatus status = new ApiStatus();
            TypeaheadListResponse response = PostResourceToEndpoint<TypeaheadListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TypeaheadResponselistModel list = new TypeaheadResponselistModel { Typeaheadlist = response?.Typeaheadlist };
            return list;
        }
    }
}
