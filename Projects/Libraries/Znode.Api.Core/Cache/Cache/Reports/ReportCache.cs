using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ReportCache : BaseCache, IReportCache
    {
        #region Private Variables
        private readonly IReportService _reportService;
        #endregion

        #region Constructor
        public ReportCache(IReportService service)
        {
            _reportService = service;
        }
        #endregion

        #region Public Methods
        //Get the list of Reports
        public string GetReportList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                ReportListModel reportList = _reportService.GetReportList(Filters, Sorts, Page);
                if (reportList?.ReportList?.Count > 0)
                {
                    ReportListResponse response = new ReportListResponse { ReportList = reportList.ReportList };
                    response.MapPagingDataFromModel(reportList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Attributes and Filters for the type of Export
        public string GetExportData(string routeUri, string routeTemplate, string dynamicReportType= "Product")
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get data from service
                DynamicReportModel importData = _reportService.GetExportData(dynamicReportType);
                DynamicReportResponse response = new DynamicReportResponse { DynamicReport = importData };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }
        //Get Custom report.
        public string GetCustomReport(string routeUri, string routeTemplate, int customReportId)
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get data from service
                DynamicReportModel reportData = _reportService.GetCustomReport(customReportId);
                DynamicReportResponse response = new DynamicReportResponse { DynamicReport = reportData };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }
        #endregion
    }
}