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
    public class RMARequestClient : BaseClient, IRMARequestClient
    {
        //To do.
        //Get Order RMA Flag.
        public virtual bool GetOrderRMAFlag(int omsOrderDetailsId)
        {
            //Get Endpoint.
            string endpoint = RMARequestEndpoint.GetOrderRMAFlag(omsOrderDetailsId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get RMA Request.
        public virtual RMARequestListModel GetRMARequest(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RMARequestEndpoint.GetRMARequestList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            RMARequestListResponse response = GetResourceFromEndpoint<RMARequestListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RMARequestListModel list = new RMARequestListModel { RMARequests = response?.RMARequestList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get RMARequest Gift Card Details
        public virtual IssuedGiftCardListModel GetRMARequestGiftCardDetails(int rmaRequestId)
        {
            string endpoint = RMARequestEndpoint.GetRMAGiftCardDetails(rmaRequestId);

            ApiStatus status = new ApiStatus();
            IssuedGiftCardListResponse response = GetResourceFromEndpoint<IssuedGiftCardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new IssuedGiftCardListModel { IssuedGiftCardModels = (HelperUtility.IsNull(response)) ? null : response.IssuedGiftCards };
        }

        //Get RMA Request by rmaRequestId
        public virtual RMARequestModel GetRMARequest(int rmaRequestId)
        {
            var endpoint = RMARequestEndpoint.Get(rmaRequestId);

            var status = new ApiStatus();
            var response = GetResourceFromEndpoint<RMARequestResponse>(endpoint, status);

            var expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return (HelperUtility.IsNull(response)) ? null : response.RMARequest;
        }

        //Update RMARequest
        public virtual RMARequestModel UpdateRMARequest(int rmaRequestId, RMARequestModel rmaRequestModel)
        {
            var endpoint = RMARequestEndpoint.UpdateRMARequest(rmaRequestId);
            ApiStatus status = new ApiStatus();

            var response = PutResourceToEndpoint<RMARequestResponse>(endpoint, JsonConvert.SerializeObject(rmaRequestModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return (HelperUtility.IsNull(response)) ? null : response.RMARequest;
        }

        //Create RMA request.
        public virtual RMARequestModel CreateRMARequest(RMARequestModel model)
        {
            var endpoint = RMARequestEndpoint.Create();

            var status = new ApiStatus();
            var response = PostResourceToEndpoint<RMARequestResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return (HelperUtility.IsNull(response)) ? null : response.RMARequest;
        }

        //Check status for mail sent
        public virtual bool IsStatusEmailSent(int rmaRequestId)
        {
            var endpoint = RMARequestEndpoint.SendRMAStatusMail(rmaRequestId);

            var status = new ApiStatus();
            var response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Check gift card mail send status
        public virtual bool IsGiftCardMailSent(GiftCardModel model)
        {
            string endpoint = RMARequestEndpoint.SendGiftCardMail();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
