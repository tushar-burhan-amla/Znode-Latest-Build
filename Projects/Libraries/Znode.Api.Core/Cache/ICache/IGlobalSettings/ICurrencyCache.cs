namespace Znode.Engine.Api.Cache
{
    public interface ICurrencyCache
    {
        /// <summary>
        /// Get a list of all currencies.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCurrencies(string routeUri, string routeTemplate);

        /// <summary>
        /// Get currency as per filter passed.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCurrency(string routeUri, string routeTemplate);

        /// <summary>
        /// Get culture as per filter passed.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCulture(string routeUri, string routeTemplate);

        /// <summary>
        /// Get culture as per filter passed.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCultureCode(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a list of all currencies.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCurrencyCultureList(string routeUri, string routeTemplate);
    }
}