using System.Linq;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using System.Collections.ObjectModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using System.Net;
using Znode.Engine.Api.Models.Extensions;
using Newtonsoft.Json;

namespace Znode.Engine.Api.Client
{
    public  class ExportClient : BaseClient, IExportClient
    {
        // Gets the Export Logs to check export status.
        public virtual ExportLogsListModel GetExportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ExportEndpoint.GetExportLogs();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ExportLogsListResponse response = GetResourceFromEndpoint<ExportLogsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ExportLogsListModel list = new ExportLogsListModel { ExportLogs = response?.LogsList?.ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Export File Path.
        public virtual string GetExportFilePath(string tableName)
        {
            string endpoint = ExportEndpoint.GetExportFilePath(tableName);

            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.Response;
        }

       // Delete outdated export files.
        public virtual bool DeleteExportFiles()
        {
            string endpoint = ExportEndpoint.DeleteExportFiles();
            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response;
        }

        // Delete the logs from ZnodeExportLog and ZnodeExportProcessLog table
        public virtual bool DeleteLogs(ParameterModel exportProcessLogIds)
        {
            string endpoint = ExportEndpoint.DeleteLog();
           
            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(exportProcessLogIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
