using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ReportClient : BaseClient, IReportClient
    {
        //Get the Available List.
        public virtual ReportListModel List(FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage)
        {
            //Get Endpoint.
            string endpoint = ReportEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sortCollection, pageIndex, recordPerPage);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportListResponse response = GetResourceFromEndpoint<ReportListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Reports list.
            ReportListModel list = new ReportListModel { ReportList = response?.ReportList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the Attributes and Filters for the type of Export
        public virtual DynamicReportModel GetExportData(string dynamicReportType)
        {
            //Get Endpoint.
            string endpoint = ReportEndpoint.GetExportData(dynamicReportType);

            //Get response.
            ApiStatus status = new ApiStatus();
            DynamicReportResponse response = GetResourceFromEndpoint<DynamicReportResponse>(endpoint, status);

            //Attributes and Filters
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.DynamicReport;
        }

        //Get custom report.
        public virtual DynamicReportModel GetCustomReport(int customReportId)
        {
            //Get Endpoint.
            string endpoint = ReportEndpoint.GetCustomReport(customReportId);

            //Get response.
            ApiStatus status = new ApiStatus();
            DynamicReportResponse response = GetResourceFromEndpoint<DynamicReportResponse>(endpoint, status);

            //Attributes and Filters
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.DynamicReport;
        }

        public virtual bool GenerateDynamicReport(DynamicReportModel model)
        {
            string endpoint = ReportEndpoint.GenerateDynamicReport();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Delete dynamic reports
        public virtual bool DeleteDynamicReport(ParameterModel parameterModel)
        {
            //Create Endpoint to delete custom report.
            string endpoint = ReportEndpoint.DeleteDynamicReport();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            //check the status of response of custom report.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
