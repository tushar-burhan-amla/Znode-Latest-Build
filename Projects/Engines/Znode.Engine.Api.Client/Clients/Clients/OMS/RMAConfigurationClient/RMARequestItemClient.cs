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
    public class RMARequestItemClient : BaseClient, IRMARequestItemClient
    {
        //Get RMA Request Item List.
        public virtual RMARequestItemListModel GetRMARequestItemList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            return GetRMARequestItemList(expands, filters, sorts, null, null);
        }

        //Get RMA Request Item List.
        public virtual RMARequestItemListModel GetRMARequestItemList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RMARequestItemEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            RMARequestItemListResponse response = GetResourceFromEndpoint<RMARequestItemListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RMARequestItemListModel list = new RMARequestItemListModel { RMARequestItemList = response?.RMARequestItems };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get RMA Request Items For Gift Card
        public virtual RMARequestItemListModel GetRMARequestItemsForGiftCard(string orderLineItems)
        {
            var endpoint = RMARequestItemEndpoint.GetRMARequestItemsForGiftCard(orderLineItems);

            var status = new ApiStatus();
            var response = GetResourceFromEndpoint<RMARequestItemListResponse>(endpoint, status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            var list = new RMARequestItemListModel { RMARequestItemList = (Equals(response, null)) ? null : response.RMARequestItems };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create RMA Request Item
        public virtual RMARequestItemModel CreateRMARequestItem(RMARequestItemModel model)
        {
            var endpoint = RMARequestItemEndpoint.Create();

            var status = new ApiStatus();
            var response = PostResourceToEndpoint<RMARequestItemResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return (Equals(response, null)) ? null : response.RMARequestItem;
        }
    }
}
