namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface ERPTaskSchedulerCache.
    /// </summary>
    public interface IERPTaskSchedulerCache
    {
        /// <summary>
        /// Get ERPTaskScheduler list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of ERPTaskScheduler.</returns>
        string GetERPTaskSchedulerList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get erpTaskScheduler on the basis of erpTaskScheduler id.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns erpTaskScheduler.</returns>
        string GetERPTaskScheduler(int erpTaskSchedulerId, string routeUri, string routeTemplate);
    }
}