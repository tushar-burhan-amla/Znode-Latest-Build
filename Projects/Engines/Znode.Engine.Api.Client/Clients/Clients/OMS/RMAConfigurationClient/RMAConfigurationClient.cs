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
    public class RMAConfigurationClient : BaseClient, IRMAConfigurationClient
    {
        #region RMA Configuration
        //Create or Update RMA Configuration.
        public virtual RMAConfigurationModel CreateRMAConfiguration(RMAConfigurationModel model)
        {
            //Get Endpoint.
            string endpoint = RMAConfigurationEndpoint.CreateRMAConfiguration();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RMAConfigurationResponse response = PostResourceToEndpoint<RMAConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.RMAConfiguration;
        }

        //Get RMA Configuration .
        public virtual RMAConfigurationModel GetRMAConfiguration()
        {
            string endpoint = RMAConfigurationEndpoint.GetRMAConfiguration();

            ApiStatus status = new ApiStatus();
            RMAConfigurationResponse response = GetResourceFromEndpoint<RMAConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RMAConfiguration;
        }
        #endregion

        #region Reason For Return
        //Gets the list of Reason For Return.
        public virtual RequestStatusListModel GetReasonForReturnList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RMAConfigurationEndpoint.GetReasonForReturnList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            RequestStatusListResponse response = GetResourceFromEndpoint<RequestStatusListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RequestStatusListModel list = new RequestStatusListModel { RequestStatusList = response?.RequestStatusList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create Reason For Return.
        public virtual RequestStatusModel CreateReasonForReturn(RequestStatusModel model)
        {
            //Get Endpoint.
            string endpoint = RMAConfigurationEndpoint.CreateReasonForReturn();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RequestStatusResponse response = PostResourceToEndpoint<RequestStatusResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.RequestStatus;
        }

        //Get Reason For Return on the basis of rmaReasonForReturnId.
        public virtual RequestStatusModel GetReasonForReturn(int rmaReasonForReturnId)
        {
            string endpoint = RMAConfigurationEndpoint.GetReasonForReturn(rmaReasonForReturnId);

            ApiStatus status = new ApiStatus();
            RequestStatusResponse response = GetResourceFromEndpoint<RequestStatusResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RequestStatus;
        }

        //Update Reason For Return data.
        public virtual RequestStatusModel UpdateReasonForReturn(RequestStatusModel model)
        {
            string endpoint = RMAConfigurationEndpoint.UpdateReasonForReturn();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RequestStatusResponse response = PutResourceToEndpoint<RequestStatusResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.RequestStatus;
        }

        //Delete Reason For Return.
        public virtual bool DeleteReasonForReturn(ParameterModel rmaReasonForReturnId)
        {
            string endpoint = RMAConfigurationEndpoint.DeleteReasonForReturn();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(rmaReasonForReturnId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Request Status
        //Get the list of Request Status.
        public virtual RequestStatusListModel GetRequestStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = RMAConfigurationEndpoint.GetRequestStatusList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            RequestStatusListResponse response = GetResourceFromEndpoint<RequestStatusListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            RequestStatusListModel list = new RequestStatusListModel { RequestStatusList = response?.RequestStatusList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Request Status on the basis of Request Status id.
        public virtual RequestStatusModel GetRequestStatus(int rmaRequestStatusId)
        {
            string endpoint = RMAConfigurationEndpoint.GetRequestStatus(rmaRequestStatusId);

            ApiStatus status = new ApiStatus();
            RequestStatusResponse response = GetResourceFromEndpoint<RequestStatusResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RequestStatus;
        }

        //Update Request Status data.
        public virtual RequestStatusModel UpdateRequestStatus(RequestStatusModel model)
        {
            string endpoint = RMAConfigurationEndpoint.UpdateRequestStatus();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            RequestStatusResponse response = PutResourceToEndpoint<RequestStatusResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.RequestStatus;
        }

        //Delete RequestStatus.
        public virtual bool DeleteRequestStatus(ParameterModel rmaRequestStatusIds)
        {
            string endpoint = RMAConfigurationEndpoint.DeleteRequestStatus();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(rmaRequestStatusIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion
    }
}
