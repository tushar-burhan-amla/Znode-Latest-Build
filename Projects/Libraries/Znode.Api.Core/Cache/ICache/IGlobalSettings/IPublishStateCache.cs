namespace Znode.Engine.Api.Cache
{
    public interface IPublishStateCache
    {
        /// <summary>
        /// Get a list of all available publish state to application type mappings.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetPublishStateMappingList(string routeUri, string routeTemplate);
    }
}