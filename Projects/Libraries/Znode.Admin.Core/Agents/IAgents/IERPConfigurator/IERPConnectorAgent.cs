using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IERPConnectorAgent
    {
        /// <summary>
        /// Get list of ERPConnector control.
        /// </summary>
        /// <returns>Returns ERPConnectorListViewModel</returns>
        ERPConnectorListViewModel GetERPConnectorControls();

        /// <summary>
        ///Method to Save ERP Control Data in json file.
        /// </summary>
        /// <param name="model">BindData Model to create.</param>
        /// <returns>Returns created ERP Connector List View Model.</returns>
        ERPConnectorListViewModel CreateERPControlData(BindDataModel bindDataModel);
    }
}
