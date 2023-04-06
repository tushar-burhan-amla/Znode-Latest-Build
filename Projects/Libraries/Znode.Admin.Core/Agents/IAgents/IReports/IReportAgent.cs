using Znode.Engine.Admin.ViewModels;
using System.Web.Mvc;
using System.Collections.Generic;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;

namespace Znode.Engine.Admin.Agents
{
    public interface IReportAgent
    {
        /// <summary>
        /// Set Default Properties for Report Control Setting.
        /// </summary>
        /// <param name="path">Path for the Report on Hosted Server</param>
        /// <returns>Return the ReportViewModel</returns>
        ReportViewModel SetReportControlSetting(string path, bool isDynamicReport = false);

        /// <summary>
        /// Set Report Type Settings.
        /// </summary>
        /// <param name="reportType">Type for the Report</param>
        /// <returns>Return the ReportViewModel</returns>
        ReportViewModel SetReportType(string reportType);

        /// <summary>
        /// Get Attributes and Filters
        /// </summary>
        /// <param name="Product">Type of Attributes and Filters</param>
        /// <returns>Return the DynamicreportViewModel</returns>
        List<List<SelectListItem>> GetExportData(string dynamicReportType);

        /// <summary>
        /// Get the list of operators based on the DataType
        /// </summary>
        /// <param name="dataType">dataType</param>
        /// <returns>String</returns>
        string GetOperators(string reportType, string filterName, out string datatype);

        /// <summary>
        /// Generate the dynamic reports.
        /// </summary>
        /// <param name="model">DynamicReportViewModel</param>
        /// <returns>bool</returns>
        bool GenerateDynamicReport(DynamicReportViewModel model, out string errorMessage);

        /// <summary>
        /// Bind the view for the first time
        /// </summary>
        /// <returns>DynamicReportViewModel</returns>
        DynamicReportViewModel BindViewModel();

        /// <summary>
        /// Get the list of Dynamic Reports
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="recordPerPage">RecordsPerPage</param>
        /// <returns>ReportListViewModel</returns>
        ReportListViewModel GetDynamicReportList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Delete the dynamic reports
        /// </summary>
        /// <param name="reportName">reportName</param>
        /// <returns>bool</returns>
        bool DeleteDynamicReport(string customReportIds, out string errorMessage);

        // <summary>
        /// Get custom report.
        /// </summary>
        /// <param name="customReportId">CustomReportId</param>
        /// <returns>Return custom report details.</returns>
        DynamicReportViewModel GetCustomReport(int customReportId);

        // <summary>
        /// Get custom report row.
        /// </summary>
        /// <param name="reportType">Report Type</param>
        /// <param name="filterName">Filter Name</param>
        /// <param name="OperatorName">Operator Name</param>
        /// <param name="filterValue">Filter Value</param>
        /// <returns>Return custom report row.</returns>
        DynamicReportViewModel GetFilterRow(string reportType, string filterName, string operatorName, string filterValue);

        /// <summary>
        /// Get Column list.
        /// </summary>
        /// <param name="reportId">ReportId</param>
        /// <param name="dynamicReportType">DynamicReport Type</param>
        /// <returns>Return column list.</returns>
        List<List<string>> GetColumnList(int reportId, string dynamicReportType);

        /// <summary>
        /// Get report view name.
        /// </summary>
        /// <param name="reportType">Report Type</param>
        /// <returns>Return name of view name.</returns>
        string GetReportViewName(string reportType);

        /// <summary>
        /// Get View by report type.
        /// </summary>
        /// <param name="reportType">Report Type</param>
        /// <param name="selectedValue">Selected Value</param>
        /// <returns>Return name of view.</returns>
        DynamicReportViewModel GetReportView(string reportType, int selectedValue);
    }
}