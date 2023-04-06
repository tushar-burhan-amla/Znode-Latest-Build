using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface ICMSPageSearchCache
    {
        #region CMS page search index

        /// <summary>
        /// Get list of Cms Pages Search Index Monitor.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>        
        /// <returns>List of Cms Page search index server status list.</returns>
        string GetCmsPageSearchIndexMonitorList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get CMS Pages index data.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Get CMS Pages index data.</returns>
        string GetCmsPageIndexData(string routeUri, string routeTemplate);
        #endregion

        #region CMS page search request

        /// <summary>
        /// Get CMS page search for search keyword
        /// </summary>
        /// <param name="model">Search request model</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Return search CMS pages for search keyword</returns>
        string FullTextContentPageSearch(CMSPageSearchRequestModel model, string routeUri, string routeTemplate);

        #endregion

    }
}
