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
    public class TouchPointConfigurationClient : BaseClient, ITouchPointConfigurationClient
    {
        // Get the list of TouchPointConfiguration
        public virtual TouchPointConfigurationListModel GetTouchPointConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of Touch Point Configuration.
            string endpoint = ERPTaskSchedulerEndpoint.TouchPointConfigurationList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TouchPointConfigurationListResponse response = GetResourceFromEndpoint<TouchPointConfigurationListResponse>(endpoint, status);

            //check the status of response of type Touch Point Configuration.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TouchPointConfigurationListModel list = new TouchPointConfigurationListModel { TouchPointConfigurationList = response?.TouchPointConfiguration };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Create task schedular for selected touchpoint in connector.
        public virtual bool TriggerTaskScheduler(string connectorTouchPoints)
        {
            string endpoint = ERPTaskSchedulerEndpoint.TriggerTaskScheduler(connectorTouchPoints);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Get the list of Scheduler Log
        public virtual TouchPointConfigurationListModel GetSchedulerLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of Scheduler Log.
            string endpoint = ERPTaskSchedulerEndpoint.GetSchedulerLogList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TouchPointConfigurationListResponse response = GetResourceFromEndpoint<TouchPointConfigurationListResponse>(endpoint, status);

            //check the status of response of type Scheduler Log.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TouchPointConfigurationListModel schedulerLogList = new TouchPointConfigurationListModel { SchedulerLogList = response?.SchedulerLog };
            schedulerLogList.MapPagingDataFromResponse(response);
            return schedulerLogList;
        }
    }
}
