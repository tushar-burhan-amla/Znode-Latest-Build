using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ImportLogsClient : BaseClient, IImportLogsClient
    {
        // Gets the Import Logs to check import status
        public ImportLogsListModel GetImportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportLogsEndpoint.GetImportLogs();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportLogsListResponse response = GetResourceFromEndpoint<ImportLogsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogsListModel list = new ImportLogsListModel { ImportLogs = response?.LogsList?.ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the Import Logs details on the basis of importLogId
        public ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportLogsEndpoint.GetImportLogDetails(importProcessLogId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportLogDetailsListResponse response = GetResourceFromEndpoint<ImportLogDetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogDetailsListModel list = new ImportLogDetailsListModel { ImportLogDetails = response?.LogDetailsList?.ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the import log current status.
        public ImportLogsListModel GetImportLogStatus(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportLogsEndpoint.GetImportLogStatus(importProcessLogId);

            ApiStatus status = new ApiStatus();
            ImportLogsListResponse response = GetResourceFromEndpoint<ImportLogsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogsListModel list = new ImportLogsListModel { ImportLogs = response?.LogsList?.ToList() };

            return list;
        }

        // Delete the logs from ZnodeImportLog and ZnodeImportProcessLog table
        public bool DeleteLogs(int importProcessLogId)
        {
            string endpoint = ImportLogsEndpoint.DeleteLog(importProcessLogId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response;
        }
    }
}
