namespace Znode.Engine.Api.Cache
{
    public interface ILocaleCache
    {
        #region Public method

        /// <summary>
        /// Get Locale list
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>return Locale List</returns>
        string GetLocaleList(string routeUri, string routeTemplate);

        /// <summary>
        /// This method is used to get a Locale.
        /// </summary>
        /// <param name="routeUri">Url of api route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>return a locale</returns>
        string GetLocale(string routeUri, string routeTemplate);

        #endregion
    }
}