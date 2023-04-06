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
    public class SearchReportClient : BaseClient, ISearchReportClient
    {
        //Get no result found search keyword report.
        public SearchReportListModel GetNoResultsFoundReport(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int page, int recordPerPage)
        {

            string endpoint = SearchReportEndpoint.GetNoResultsFoundReport();
            endpoint += BuildEndpointQueryString(null, filters, sorts, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchReportListResponse response = GetResourceFromEndpoint<SearchReportListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchReportListModel searchReportList = new SearchReportListModel { SearchReportList = response?.SearchReportList };

            searchReportList.MapPagingDataFromResponse(response);

            return searchReportList;
        }

        //Get top keyword search keyword list.
        public SearchReportListModel GetTopKeywordsReport(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int page, int recordPerPage)
        {
            string endpoint = SearchReportEndpoint.GetTopKeywordsReport();
            endpoint += BuildEndpointQueryString(null, filters, sorts, page, recordPerPage);

            ApiStatus status = new ApiStatus();

            SearchReportListResponse response = GetResourceFromEndpoint<SearchReportListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchReportListModel searchReportList = new SearchReportListModel { SearchReportList = response?.SearchReportList };

            searchReportList.MapPagingDataFromResponse(response);

            return searchReportList;
        }

        //Save search report data
        public SearchReportModel SaveSearchReport(SearchReportModel model)
        {
            string endpoint = SearchReportEndpoint.SaveSearchReport();

            ApiStatus status = new ApiStatus();
            SearchReportResponse response = PostResourceToEndpoint<SearchReportResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchReport;
        }
    }
}
