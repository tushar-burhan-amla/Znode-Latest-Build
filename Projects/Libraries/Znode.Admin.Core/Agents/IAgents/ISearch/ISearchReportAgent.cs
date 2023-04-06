using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ISearchReportAgent
    {
        /// <summary>
        /// get top keyword search reports
        /// </summary>
        /// <param name="filters">filters to get the top keyword search report</param>
        /// <param name="sorts">sorts for top keyword search report</param>
        /// <param name="page">page number</param>
        /// <param name="recordPerPage">record per page</param>
        /// <param name="portalId">portal id to get the result portal wise</param>
        /// <param name="portalName">portal name</param>
        /// <returns>search top keyword list.</returns>
        SearchReportListViewModel GetTopKeywordsReport(FilterCollection filters, SortCollection sorts, int page, int recordPerPage, int portalId, string portalName);

        /// <summary>
        /// get no result found search report
        /// </summary>
        /// <param name="filters">filters to get the no result found keyword list</param>
        /// <param name="sorts">sorts for no result found keyword list</param>
        /// <param name="page">page number</param>
        /// <param name="recordPerPage">record per page</param>
        /// <param name="portalId">portal id to get report portal wise</param>
        /// <param name="portalName">portal name</param>
        /// <returns>no result found keyword list</returns>
        SearchReportListViewModel GetNoResultsFoundReport(FilterCollection filters, SortCollection sorts, int page, int recordPerPage, int portalId, string portalName);

        /// <summary>
        /// Get tab structure for search activity report.
        /// </summary>
        /// <returns>tab structure for search activity report</returns>
        SearchReportViewModel GetTabStructureSearchReport();
    }
}
