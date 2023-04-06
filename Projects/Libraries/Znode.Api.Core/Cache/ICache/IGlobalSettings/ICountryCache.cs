namespace Znode.Engine.Api.Cache
{
    public interface ICountryCache
    {
        /// <summary>
        /// Get a list of all countries.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCountries(string routeUri, string routeTemplate);

        /// <summary>
        /// Get country as per filter passed.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCountry(string routeUri, string routeTemplate);
    }
}