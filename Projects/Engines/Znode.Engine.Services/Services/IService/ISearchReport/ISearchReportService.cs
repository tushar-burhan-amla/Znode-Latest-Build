using System.Collections.Specialized;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISearchReportService
    {
        /// <summary>
        /// Get top keyword list report.
        /// </summary>
        /// <param name="expands">expands to get the top search keyword list</param>
        /// <param name="filters">filters to get the top search keyword list</param>
        /// <param name="sorts">sort for order by</param>
        /// <param name="page">paging details</param>
        /// <returns>Top keyword list</returns>
        SearchReportListModel GetTopKeywordsReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get no result found keyword list
        /// </summary>
        /// <param name="expands">expands to get the no result found report</param>
        /// <param name="filters">filter to get the no result found report</param>
        /// <param name="sorts">sorts for order by</param>
        /// <param name="page">paging details</param>
        /// <returns>no result found keyword list</returns>
        SearchReportListModel GetNoResultsFoundReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Save search report data.
        /// </summary>
        /// <param name="model">model with the data for search report</param>
        /// <returns>Update model with search report data</returns>
        SearchReportModel SaveSearchReportData(SearchReportModel model);
    }
}
