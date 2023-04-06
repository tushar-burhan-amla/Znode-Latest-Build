using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IERPConnectorCache
    {
        /// <summary>
        /// Get ERPConfigurator control list.
        /// </summary>
        /// <param name="erpConfiguratorModel">ERPConfigurator Model</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of ERPConnectorControlList.</returns>
        string GetERPConnectorControlList(ERPConfiguratorModel erpConfiguratorModel, string routeUri, string routeTemplate);
    }
}
