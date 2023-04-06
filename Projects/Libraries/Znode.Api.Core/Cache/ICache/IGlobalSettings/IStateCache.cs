namespace Znode.Engine.Api.Cache
{
    public interface IStateCache
    {
        /// <summary>
        /// Get a list of all states.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetStateList(string routeUri, string routeTemplate);
    }
}