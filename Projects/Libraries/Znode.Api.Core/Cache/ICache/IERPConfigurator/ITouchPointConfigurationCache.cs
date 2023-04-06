namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface TouchPointConfigurationCache.
    /// </summary>
    public interface ITouchPointConfigurationCache
    {
        /// <summary>
        /// Get TouchPointConfiguration list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of TouchPointConfiguration.</returns>
        string GetTouchPointConfigurationList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Scheduler Log List.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of Scheduler Log.</returns>
        string SchedulerLogList(string routeUri, string routeTemplate);
    }
}