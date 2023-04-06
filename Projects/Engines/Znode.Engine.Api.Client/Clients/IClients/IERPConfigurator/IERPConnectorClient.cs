using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IERPConnectorClient : IBaseClient
    {
        /// <summary>
        /// Get ERPConnectorControls.
        /// </summary>
        /// <param name="erpConfiguratorModel">ERPConfiguratorModel</param>
        /// <returns>returns ERPConnectorControlListModel</returns>
        ERPConnectorControlListModel GetERPConnectorControls(ERPConfiguratorModel erpConfiguratorModel);

        /// <summary>
        /// Method to Save ERP Control Data in json file.
        /// </summary>
        /// <param name="model">BindData Model to create.</param>
        /// <returns>Returns created ERP Connector List View Model.</returns>
        ERPConnectorControlListModel CreateERPControlData(ERPConnectorControlListModel erpConnectorControlListModel);
    }
}
