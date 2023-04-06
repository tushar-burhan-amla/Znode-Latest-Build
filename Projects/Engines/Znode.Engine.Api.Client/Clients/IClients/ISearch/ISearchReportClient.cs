using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISearchReportClient : IBaseClient
    {
        /// <summary>
        /// Get No result found report
        /// </summary>
        /// <param name="expands">expands to get no result found keyword list</param>
        /// <param name="filters">filter to get the no result found keyword list.</param>
        /// <param name="sorts">sorts for order by</param>
        /// <param name="page">page number</param>
        /// <param name="recordPerPage">record per page</param>
        /// <returns>List of keywords with no result found</returns>
        SearchReportListModel GetNoResultsFoundReport(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int page, int recordPerPage);

        /// <summary>
        /// Get top search keyword list
        /// </summary>
        /// <param name="expands">expands to get the top search keyword list</param>
        /// <param name="filters">filters to get the top search keyword list </param>
        /// <param name="sorts">sorts for order by</param>
        /// <param name="page">page number</param>
        /// <param name="recordPerPage">record per page</param>
        /// <returns>List of top search keywords</returns>
        SearchReportListModel GetTopKeywordsReport(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int page, int recordPerPage);

        /// <summary>
        /// Save search report data.
        /// </summary>
        /// <param name="model">model with the data for search report</param>
        /// <returns>Update model with search report data</returns>
        SearchReportModel SaveSearchReport(SearchReportModel model);
    }
}
