namespace Znode.Engine.Api.Cache
{
    public interface ICityCache
    {
        /// <summary>
        /// Get a list of all cities.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCityList(string routeUri, string routeTemplate);
    }
}