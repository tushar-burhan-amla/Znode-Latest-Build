using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class PublishHistoryClient : BaseClient, IPublishHistoryClient
    {
        public virtual PublishHistoryListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PublishHistoryEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PublishHistoryListResponse response = GetResourceFromEndpoint<PublishHistoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //check the status of response of type CMS Widgets type.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishHistoryListModel list = new PublishHistoryListModel { PublishHistoryList = response?.PublishHistory };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Delete product logs by versionId.
        public virtual bool DeleteProductLogs(int versionId)
        {
            //Get Endpoint.
            string endpoint = PublishHistoryEndpoint.Delete(versionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(versionId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
