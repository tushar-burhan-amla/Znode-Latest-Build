namespace Znode.Engine.Api.Cache
{
    public interface IThemeCache
    {
        /// <summary>
        /// Gets theme List.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetThemes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get theme by themeId.
        /// </summary>
        /// <param name="themeId">themeId to get theme</param>
        /// <param name="routeUri">route URL</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>data in string format</returns>
        string GetTheme(int themeId, string routeUri, string routeTemplate);

        #region Associate Store
        /// <summary>
        /// Get associated store list for cms theme.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Portal theme.</returns>
        string GetAssociatedStoreList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated store.</returns>
        string GetUnAssociatedStoreList(string routeUri, string routeTemplate);
        #endregion

        #region CMS Widgets
        /// <summary>
        /// Get area list for theme.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetAreas(string routeUri, string routeTemplate);
        #endregion
    }
}
