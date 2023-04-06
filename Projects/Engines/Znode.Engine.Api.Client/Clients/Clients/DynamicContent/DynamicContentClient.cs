using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class DynamicContentClient : BaseClient, IDynamicContentClient
    {
        public virtual EditorFormatListModel GetEditorFormats(int portalId)
        {
            string endpoint = DynamicContentEndpoint.GetEditorFormats(portalId);

            ApiStatus status = new ApiStatus();
            EditorFormatsResponse response = GetResourceFromEndpoint<EditorFormatsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.EditorFormatList;
        }
    }
}
