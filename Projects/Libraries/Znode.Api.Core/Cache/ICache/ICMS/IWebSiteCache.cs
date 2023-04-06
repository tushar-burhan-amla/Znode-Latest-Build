namespace Znode.Engine.Api.Cache
{
    public interface IWebSiteCache
    {
        /// <summary>
        /// Gets Portal List having the Themes Assigned.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetPortalList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of portal page product associated to selected store in website configuration.
        /// </summary>
        /// <param name="portalId">Id of store to get portal page product.</param>
        /// <param name="routeUri">route URL</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Data in string format</returns>
        string GetPortalProductPageList(int portalId, string routeUri, string routeTemplate);
    }
}