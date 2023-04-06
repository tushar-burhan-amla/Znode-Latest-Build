namespace Znode.Engine.Api.Cache
{
    public interface ISearchReportCache
    {
        /// <summary>
        /// Get no result found search keyword
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>List of no result found search keywords</returns>
        string GetNoResultsFoundReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get top search keyword list
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>list of top search keyword</returns>
        string GetTopKeywordsReport(string routeUri, string routeTemplate);
    }
}
