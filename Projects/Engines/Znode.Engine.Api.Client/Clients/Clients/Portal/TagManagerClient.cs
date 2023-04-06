using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class TagManagerClient : BaseClient, ITagManagerClient
    {
        //Get tag manager data by portal id.
        public virtual TagManagerModel GetTagManager(int portalId, ExpandCollection expands)
        {
            string endpoint = TagManagerEndpoint.GetTagManager(portalId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();

            TagManagerResponse response = GetResourceFromEndpoint<TagManagerResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TagManagerModel;
        }

        //Save tag manager data for store.
        [HttpPut]
        public virtual bool SaveTagManager(TagManagerModel tagManagerModel)
        {
            string endpoint = TagManagerEndpoint.SaveTagManager();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(tagManagerModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return (response?.IsSuccess).GetValueOrDefault();
        }
    }
}
