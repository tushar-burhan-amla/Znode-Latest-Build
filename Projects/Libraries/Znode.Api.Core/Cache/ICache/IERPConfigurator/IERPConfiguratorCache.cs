namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface ERPConfiguratorCache.
    /// </summary>
    public interface IERPConfiguratorCache
    {
        /// <summary>
        /// Get ERPConfigurator list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of ERPConfigurator.</returns>
        string GetERPConfiguratorList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get all ERP Configurator Classes which are not present in database.
        /// </summary>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Return List ERP Configurator Classes.</returns>
        string GetAllERPConfiguratorClassesNotInDatabase(string routeUri, string routeTemplate);

        /// <summary>
        /// Get eRPConfigurator on the basis of eRPConfigurator id.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns eRPConfigurator.</returns>
        string GetERPConfigurator(int eRPConfiguratorId, string routeUri, string routeTemplate);
    }
}