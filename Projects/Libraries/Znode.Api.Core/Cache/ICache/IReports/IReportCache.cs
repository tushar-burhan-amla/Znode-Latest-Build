namespace Znode.Engine.Api.Cache
{
    public interface IReportCache
    {
        /// <summary>
        /// Get the list of all Reports.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of Reports.</returns>
        string GetReportList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Attributes and Filters for the type of Export.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="dynamicReportType">Type of Export</param>
        /// <returns>Attributes and Filters</returns>
        string GetExportData(string routeUri, string routeTemplate, string dynamicReportType = "Product");

        /// <summary>
        /// Get Attributes and Filters for the type of Export.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="customReportId">custom report id</param>
        /// <returns>Attributes and Filters</returns>
        string GetCustomReport(string routeUri, string routeTemplate, int customReportId);
    }
}
