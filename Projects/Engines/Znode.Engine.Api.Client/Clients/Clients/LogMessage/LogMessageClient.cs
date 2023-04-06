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
    public class LogMessageClient : BaseClient, ILogMessageClient
    {
        public LogMessageModel GetLogMessage(string logMessageId)
        {
            string endpoint = LogMessageEndpoint.GetLogMessage(logMessageId);

            ApiStatus status = new ApiStatus();
            LogMessageResponse response = GetResourceFromEndpoint<LogMessageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.LogMessage;
        }

        //Get database log message details
        public LogMessageModel GetDatabaseLogMessage(string logMessageId)
        {
            string endpoint = LogMessageEndpoint.GetDatabaseLogMessage(logMessageId);

            ApiStatus status = new ApiStatus();
            LogMessageResponse response = GetResourceFromEndpoint<LogMessageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.LogMessage;
        }

        public LogMessageConfigurationModel GetLogConfiguration()
        {
            string endpoint = LogMessageEndpoint.GetLogConfiguration();

            ApiStatus status = new ApiStatus();
            LogMessageConfigurationResponse response = GetResourceFromEndpoint<LogMessageConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.LogMessageConfiguration;
        }

        //Update Log Configuration.
        public virtual LogMessageConfigurationModel UpdateLogConfiguration(LogMessageConfigurationModel logMessageConfigurationModel)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.UpdateLogConfiguration();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            LogMessageConfigurationResponse response = PostResourceToEndpoint<LogMessageConfigurationResponse>(endpoint, JsonConvert.SerializeObject(logMessageConfigurationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.LogMessageConfiguration;
        }

        public LogMessageListModel GetLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.GetLogMessageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            LogMessageListResponse response = GetResourceFromEndpoint<LogMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LogMessageListModel list = new LogMessageListModel { LogMessageList = response?.LogMessageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public LogMessageListModel GetIntegrationLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.GetIntegrationLogMessageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            LogMessageListResponse response = GetResourceFromEndpoint<LogMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LogMessageListModel list = new LogMessageListModel { LogMessageList = response?.LogMessageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get event log message list
        public LogMessageListModel GetEventLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.GetEventLogMessageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            LogMessageListResponse response = GetResourceFromEndpoint<LogMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LogMessageListModel list = new LogMessageListModel { LogMessageList = response?.LogMessageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Database log message list
        public LogMessageListModel GetDatabaseLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.GetDatabaseLogMessageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            LogMessageListResponse response = GetResourceFromEndpoint<LogMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LogMessageListModel list = new LogMessageListModel { LogMessageList = response?.LogMessageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Purge Logs.
        public virtual bool PurgeLogs(ParameterModel logCategoryIds)
        {
            string endpoint = LogMessageEndpoint.PurgeLogs();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(logCategoryIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #region Impersonation
        public ImpersonationActivityListModel GetImpersonationLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = LogMessageEndpoint.GetImpersonationLogList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ImpersonationActivityListResponse response = GetResourceFromEndpoint<ImpersonationActivityListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImpersonationActivityListModel list = new ImpersonationActivityListModel { LogActivityList = response?.LogActivityList };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion
    }
}
