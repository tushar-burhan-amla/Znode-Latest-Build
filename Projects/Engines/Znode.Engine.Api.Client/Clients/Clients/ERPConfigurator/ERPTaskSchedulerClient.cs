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
    public class ERPTaskSchedulerClient : BaseClient, IERPTaskSchedulerClient
    {
        // Get the list of ERPTaskScheduler
        public virtual ERPTaskSchedulerListModel GetERPTaskSchedulerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of ERP Task Scheduler.
            string endpoint = ERPTaskSchedulerEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);
            ApiStatus status = new ApiStatus();
            ERPTaskSchedulerListResponse response = GetResourceFromEndpoint<ERPTaskSchedulerListResponse>(endpoint, status);
            //check the status of response of type ERP Task Scheduler.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            ERPTaskSchedulerListModel list = new ERPTaskSchedulerListModel { ERPTaskSchedulerList = response?.ERPTaskScheduler };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Create ERPTaskScheduler.
        public virtual ERPTaskSchedulerModel Create(ERPTaskSchedulerModel erpTaskSchedulerModel)
        {
            string endpoint = ERPTaskSchedulerEndpoint.Create();
            ApiStatus status = new ApiStatus();
            ERPTaskSchedulerResponse response = PostResourceToEndpoint<ERPTaskSchedulerResponse>(endpoint, JsonConvert.SerializeObject(erpTaskSchedulerModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ERPTaskScheduler;
        }

        // Get erpTaskScheduler on the basis of erpTaskScheduler id.
        public virtual ERPTaskSchedulerModel GetERPTaskScheduler(int erpTaskSchedulerId)
        {
            string endpoint = ERPTaskSchedulerEndpoint.GetERPTaskScheduler(erpTaskSchedulerId);
            ApiStatus status = new ApiStatus();
            ERPTaskSchedulerResponse response = GetResourceFromEndpoint<ERPTaskSchedulerResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.ERPTaskScheduler;
        }

        // Delete ERPTaskScheduler.
        public virtual bool Delete(ParameterModel erpTaskSchedulerId)
        {
            string endpoint = ERPTaskSchedulerEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(erpTaskSchedulerId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Get the ERPTaskSchedulerId From Touch point name
        public virtual int GetSchedulerIdByTouchPointName(string erpTouchPointName , string schedulerCallFor)
        {
            string endpoint = ERPConfiguratorEndpoint.GetSchedulerIdByTouchPointName();
            ApiStatus status = new ApiStatus();
            ParameterKeyModel model = new ParameterKeyModel { ParameterKey = erpTouchPointName , SchedulerCallFor= schedulerCallFor };
            ERPConfiguratorResponse response = PostResourceToEndpoint<ERPConfiguratorResponse>(endpoint,JsonConvert.SerializeObject(model), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK,HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ERPTaskSchedulerId;
        }

        // Enable/disable ERP task scheduler from windows service.
        public virtual bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive)
        {
            string endpoint = ERPTaskSchedulerEndpoint.EnableDisableTaskScheduler(ERPTaskSchedulerId, isActive);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(ERPTaskSchedulerId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
