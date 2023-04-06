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
    public class WebStoreCaseRequestClient : BaseClient, IWebStoreCaseRequestClient
    {
        #region Case Requests
        public virtual WebStoreCaseRequestModel CreateContactUs(WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            //Get Endpoint.
            string endpoint = WebStoreCaseRequestEndpoint.CreateContactUs();
            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestResponse response = PostResourceToEndpoint<WebStoreCaseRequestResponse>(endpoint, JsonConvert.SerializeObject(webStoreCaseRequestModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.CaseRequest;
        }

        //Get the list of case request.
        public virtual WebStoreCaseRequestListModel GetCaseRequests(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = WebStoreCaseRequestEndpoint.GetCaseRequests();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestListResponse response = GetResourceFromEndpoint<WebStoreCaseRequestListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreCaseRequestListModel list = new WebStoreCaseRequestListModel { CaseRequestList = response?.CaseRequests };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create Case Request.
        public virtual WebStoreCaseRequestModel CreateCaseRequest(WebStoreCaseRequestModel caseRequestModel)
        {
            string endpoint = WebStoreCaseRequestEndpoint.CreateCaseRequest();

            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestResponse response = PostResourceToEndpoint<WebStoreCaseRequestResponse>(endpoint, JsonConvert.SerializeObject(caseRequestModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.CaseRequest;
        }

        //Reply Customer.
        public virtual WebStoreCaseRequestModel ReplyCustomer(WebStoreCaseRequestModel model)
        {
            string endpoint = WebStoreCaseRequestEndpoint.ReplyCustomer();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestResponse response = PutResourceToEndpoint<WebStoreCaseRequestResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CaseRequest;
        }

        //Get case request on the basis of caseRequestId.
        public virtual WebStoreCaseRequestModel GetCaseRequest(int caseRequestId)
        {
            string endpoint = WebStoreCaseRequestEndpoint.GetCaseRequest(caseRequestId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestResponse response = GetResourceFromEndpoint<WebStoreCaseRequestResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CaseRequest;
        }

        //Update case request data.
        public virtual WebStoreCaseRequestModel UpdateCaseRequest(WebStoreCaseRequestModel caseRequestModel)
        {
            string endpoint = WebStoreCaseRequestEndpoint.UpdateCaseRequest();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            WebStoreCaseRequestResponse response = PutResourceToEndpoint<WebStoreCaseRequestResponse>(endpoint, JsonConvert.SerializeObject(caseRequestModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CaseRequest;
        }

        //Get the list of case Priority.
        public virtual CasePriorityListModel GetCasePriorityList()
        {
            string endpoint = WebStoreCaseRequestEndpoint.GetCasePriorityList();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            CasePriorityListResponse response = GetResourceFromEndpoint<CasePriorityListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CasePriorityListModel list = new CasePriorityListModel { CasePriorities = response?.CasePriorities };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the list of case status.
        public virtual CaseStatusListModel GetCaseStatusList()
        {
            string endpoint = WebStoreCaseRequestEndpoint.GetCaseStatusList();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            CaseStatusListResponse response = GetResourceFromEndpoint<CaseStatusListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CaseStatusListModel list = new CaseStatusListModel { CaseStatuses = response?.CaseStatus };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the list of case types.
        public virtual CaseTypeListModel GetCaseTypeList()
        {
            string endpoint = WebStoreCaseRequestEndpoint.GetCaseTypeList();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            CaseTypeListResponse response = GetResourceFromEndpoint<CaseTypeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CaseTypeListModel list = new CaseTypeListModel {CaseTypes  = response?.CaseTypes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        #endregion
    }
}
