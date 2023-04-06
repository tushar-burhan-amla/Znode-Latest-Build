using Newtonsoft.Json;
using System.Collections.Generic;
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
    public class RMAReturnClient : BaseClient, IRMAReturnClient
    {
        //Get order details by order number for return.
        public virtual OrderModel GetOrderDetailsForReturn(int userId, string orderNumber, bool  isFromAdmin = false)
        {
            string endpoint = RMAReturnEndpoint.GetOrderDetailsForReturn(userId, orderNumber, isFromAdmin);
           
            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        // Gets the list of order returns.
        public virtual RMAReturnListModel GetReturnList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RMAReturnEndpoint.GetReturnList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            RMAReturnListResponse response = GetResourceFromEndpoint<RMAReturnListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RMAReturnListModel list = new RMAReturnListModel { Returns = response?.ReturnList.Returns };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Check if order is eligible for return
        public virtual bool IsOrderEligibleForReturn(int userId, int portalId, string orderNumber)
        {
            string endpoint = RMAReturnEndpoint.IsOrderEligibleForReturn(userId, portalId, orderNumber);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get order return details by return number
        public virtual RMAReturnModel GetReturnDetails(string returnNumber, ExpandCollection expands = null)
        {
            string endpoint = RMAReturnEndpoint.GetReturnDetails(returnNumber);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            ApiStatus status = new ApiStatus();
            RMAReturnResponse response = GetResourceFromEndpoint<RMAReturnResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Return;
        }

        //Insert or update order return details.
        public virtual RMAReturnModel SaveOrderReturn(RMAReturnModel returnModel)
        {
            string endpoint = RMAReturnEndpoint.SaveOrderReturn();
            ApiStatus status = new ApiStatus();
            RMAReturnResponse response = PostResourceToEndpoint<RMAReturnResponse>(endpoint, JsonConvert.SerializeObject(returnModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Return;
        }

        //Delete order return on the basis of return number.
        public virtual bool DeleteOrderReturn(string returnNumber, int userId)
        {
            string endpoint = RMAReturnEndpoint.DeleteOrderReturn(returnNumber, userId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(returnNumber), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Submit order return.
        public virtual RMAReturnModel SubmitOrderReturn(RMAReturnModel returnModel)
        {
            string endpoint = RMAReturnEndpoint.SubmitOrderReturn();
            ApiStatus status = new ApiStatus();
            RMAReturnResponse response = PostResourceToEndpoint<RMAReturnResponse>(endpoint, JsonConvert.SerializeObject(returnModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Return;
        }

        //Perform calculations for an order return line item.
        public virtual RMAReturnCalculateModel CalculateOrderReturn(RMAReturnCalculateModel returnCalculateModel)
        {
            //Get Endpoint
            string endpoint = RMAReturnEndpoint.CalculateOrderReturn();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RMAReturnCalculateResponse response = PostResourceToEndpoint<RMAReturnCalculateResponse>(endpoint, JsonConvert.SerializeObject(returnCalculateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ReturnCalculateModel;
        }

        //Get List of Return Status
        public virtual RMAReturnStateListModel GetReturnStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = RMAReturnEndpoint.GetReturnStatusList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            RMAReturnStateListResponse response = GetResourceFromEndpoint<RMAReturnStateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RMAReturnStateListModel list = new RMAReturnStateListModel { ReturnStates = response?.ReturnStates };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get order return details by return number
        public virtual RMAReturnModel GetReturnDetailsForAdmin(string returnNumber)
        {
            string endpoint = RMAReturnEndpoint.GetReturnDetailsForAdmin(returnNumber);

            ApiStatus status = new ApiStatus();
            RMAReturnResponse response = GetResourceFromEndpoint<RMAReturnResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Return;
        }

        //Create return history.
        public virtual bool CreateReturnHistory(List<RMAReturnHistoryModel> returnHistoryModel)
        {
            string endpoint = RMAReturnEndpoint.CreateReturnHistory();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(returnHistoryModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Save return notes.
        public virtual bool SaveReturnNotes(RMAReturnNotesModel rmaReturnNotesModel)
        {
            string endpoint = RMAReturnEndpoint.SaveReturnNotes();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(rmaReturnNotesModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        //Validate order lineitems for create return
        public virtual RMAReturnModel IsValidReturnItems(RMAReturnModel returnModel)
        {
            string endpoint = RMAReturnEndpoint.IsValidReturnItem();
            ApiStatus status = new ApiStatus();
            RMAReturnResponse response = PostResourceToEndpoint<RMAReturnResponse>(endpoint, JsonConvert.SerializeObject(returnModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Return;
        }
    }
}
