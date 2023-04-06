namespace Znode.Engine.Api.Cache
{
    public interface IGeneralSettingCache
    {
        /// <summary>
        /// Get a list of all currencies.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string List(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets cache management data
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCacheManagementData(string routeUri, string routeTemplate);

        /// <summary>
        /// Get global configuration settings for application.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetConfigurationSettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets Power BI setting Details
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetPowerBISettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets stock details setting details
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response</returns>
        string GetStockNoticeSettings(string routeUri, string routeTemplate);

    }
}