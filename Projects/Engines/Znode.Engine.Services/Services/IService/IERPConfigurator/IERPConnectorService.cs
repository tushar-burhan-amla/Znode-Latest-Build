using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IERPConnectorService
    {
        /// <summary>
        /// Get ERPConnector list.
        /// </summary>
        /// <param name="erpConfiguratorModel">ERPConfiguratorModel</param>
        /// <returns>returns ERPConnectorControlListModel</returns>
        ERPConnectorControlListModel GetERPConnectorControls(ERPConfiguratorModel erpConfiguratorModel);

        /// <summary>
        /// Save ERPControl data to json file.
        /// </summary>
        /// <param name="erpConnectorControlListModel">ERPConnectorControlListModel</param>
        /// <returns>return ERPConnectorControlListModel </returns>
        ERPConnectorControlListModel SaveERPControlData(ERPConnectorControlListModel erpConnectorControlListModel);
    }
}
