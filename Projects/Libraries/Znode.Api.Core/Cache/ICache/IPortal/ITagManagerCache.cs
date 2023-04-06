namespace Znode.Engine.Api.Cache
{
    public interface ITagManagerCache
    {
        /// <summary>
        /// Get tag manager data for store.
        /// </summary>
        /// <param name="portalId">Id to get tag manager data for store.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns string of data.</returns>
        string GetTagManager(int portalId, string routeUri, string routeTemplate);
    }
}
