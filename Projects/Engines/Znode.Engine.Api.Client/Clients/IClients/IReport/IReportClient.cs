using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IReportClient
    {
        /// <summary>
        /// Get Hosted SSRS Report list.
        /// </summary>
        /// <param name="filters">Filters for Report.</param>
        /// <param name="sorts">Sorts for Report.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns ReportListViewModel.</returns>
        ReportListModel List(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Attributes and Filters
        /// </summary>
        /// <param name="filters">Filters for Dynamic Report.</param>
        /// <param name="sorts">Sorts for Dynamic Report.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns DynamicReport.</returns>
        DynamicReportModel GetExportData(string dynamicReportType);

        /// <summary>
        /// Generates the dynamic reports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool GenerateDynamicReport(DynamicReportModel model);

        /// <summary>
        /// Delete the dynamic report.
        /// </summary>
        /// <param name="parameterModel">parameterModel</param>
        /// <returns></returns>
        bool DeleteDynamicReport(ParameterModel parameterModel);
      
        /// <summary>
        /// Get custom report.
        /// </summary>
        /// <param name="customReportId">CustomReportId</param>
        /// <returns>Return custom report details.</returns>
        DynamicReportModel GetCustomReport(int customReportId);
    }
}
