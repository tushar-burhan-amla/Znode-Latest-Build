using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IERPConfiguratorService
    {
        /// <summary>
        /// Get ERPConfigurator list from database.
        /// </summary> 
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns ERPConfiguratorListModel</returns>
        ERPConfiguratorListModel GetERPConfiguratorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get all ERP Configurator Classes which are not present in database.
        /// </summary>
        /// <returns>Return List ERP Configurator Classes.</returns>
        ERPConfiguratorListModel GetAllERPConfiguratorClassesNotInDatabase();

        /// <summary>
        /// Create eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorModel">ERPConfigurator Model.</param>
        /// <returns>Returns created eRPConfigurator Model.</returns>
        ERPConfiguratorModel Create(ERPConfiguratorModel eRPConfiguratorModel);

        /// <summary>
        /// Get eRPConfiguratorId on the basis of eRPConfigurator id.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfiguratorId.</param>
        /// <returns>Returns eRPConfigurator model.</returns>
        ERPConfiguratorModel GetERPConfigurator(int eRPConfiguratorId);

        /// <summary>
        /// Update eRPConfigurator data.
        /// </summary>
        /// <param name="eRPConfiguratorModel">ERPConfigurator model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool Update(ERPConfiguratorModel eRPConfiguratorModel);

        /// <summary>
        /// Delete eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorIds">ERPConfigurator Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel eRPConfiguratorIds);

        /// <summary>
        /// Enable disable ERPConfigurator on the basis of eRPConfiguratorId.
        /// </summary>
        /// <param name="eRPConfiguratorId">eRPConfiguratorId to enable disable ERPConfigurator.</param>
        /// <param name="isActive">Enable disable ERPConfigurator on the basis of isActive..</param>
        /// <returns>Returns true/false</returns>
        bool EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive);

        /// <summary>
        /// Get the class name of active ERP.
        /// </summary>
        /// <returns>ActiveERPClassName</returns>
        string GetActiveERPClassName();

        /// <summary>
        /// Get the class Id  of active ERP.
        /// </summary>
        /// <returns>ActiveERPClassID<returns>
        int GetActiveERPClassId();

        /// <summary>
        /// Get the class name of ERP class define by user.
        /// </summary>
        /// <returns>ERPClassName</returns>
        string GetERPClassName();
    }
}
