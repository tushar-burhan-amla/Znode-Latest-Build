
namespace Znode.Engine.Api.Cache
{
    public interface IApplicationSettingsCache
    {
        /// <summary>
        /// Get dynamic grid column and filters configuration XML.
        /// </summary>
        /// <param name="itemName">string ItemName for XMl</param>
        /// <param name="routeUri">routeUri to maintain cache</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Returns XML string</returns>
        string GetFilterConfigurationXML(string itemName, string routeUri, string routeTemplate, int? userId = null);

        #region XML Editor
        /// <summary>
        /// Get Xml configuration list from database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns Xml string</returns>
        string GetApplicationSettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Get column list of selected object it can be Table / View / Procedure .
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="entityType"></param>
        /// <param name="entityName"></param>
        /// <returns>Returns Collections of Columns</returns>
        string GetColumnList(string routeUri, string routeTemplate, string entityType, string entityName);

        /// <summary>
        /// Get View by ItemViewId.
        /// </summary>
        /// <param name="itemViewId">ItemViewId</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetView(int itemViewId, string routeUri, string routeTemplate);
        #endregion
    }
}
