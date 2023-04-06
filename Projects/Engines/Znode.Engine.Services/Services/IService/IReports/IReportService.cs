using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Get the list of all Reports.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>ReportList Model.</returns>
        ReportListModel GetReportList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Attributes and Filters for the type of Export
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>DynamicReport Model.</returns>
        DynamicReportModel GetExportData(string dynamicReportType);

        /// <summary>
        /// Generate the dynamic report.
        /// </summary>
        /// <param name="model">DynamicReportModel</param>
        /// <returns>bool</returns>
        bool GenerateDynamicReport(DynamicReportModel model, out int errorCode);

        /// <summary>
        /// Get custom report.
        /// </summary>
        /// <param name="customReportId">CustomReportId</param>
        /// <returns>Return custom report details.</returns>
        DynamicReportModel GetCustomReport(int customReportId);

        /// <summary>
        /// Delete custom report.
        /// </summary>
        /// <param name="customReportIds">customReport Ids</param>
        /// <returns>Return status as true or false.</returns>
        bool DeleteCustomReport(ParameterModel customReportIds);
    }
}
