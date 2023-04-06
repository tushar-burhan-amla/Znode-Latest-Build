using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IERPConfiguratorClient :IBaseClient
    {
        /// <summary>
        /// Get the list of ERPConfigurator
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ERPConfigurator list model.</returns>
        ERPConfiguratorListModel GetERPConfiguratorList(ExpandCollection expands,FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get all ERP Configurator Classes which are not present in database.
        /// </summary>
        /// <returns>Returns ERP Configurator Classes list ViewModel which are not in database.</returns>
        ERPConfiguratorListModel GetAllERPConfiguratorClassesNotInDatabase();

        /// <summary>
        /// Create ERPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorModel">ERPConfigurator Model.</param>
        /// <returns>Returns created ERPConfigurator Model.</returns>
        ERPConfiguratorModel Create(ERPConfiguratorModel eRPConfiguratorModel);

        /// <summary>
        /// Get eRPConfigurator on the basis of eRPConfigurator id.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfiguratorId to get eRPConfigurator details.</param>
        /// <returns>Returns ERPConfiguratorModel.</returns>
        ERPConfiguratorModel GetERPConfigurator(int eRPConfiguratorId);

        /// <summary>
        /// Update eRPConfigurator data.
        /// </summary>
        /// <param name="eRPConfiguratorModel">ERPConfigurator model to update.</param>
        /// <returns>Returns updated eRPConfigurator model.</returns>
        ERPConfiguratorModel Update(ERPConfiguratorModel eRPConfiguratorModel);

        /// <summary>
        /// Delete ERPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel eRPConfiguratorId);

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
        /// <returns></returns>
        string GetActiveERPClassName();

        /// <summary>
        /// Get the class name of ERP defined by user. 
        /// </summary>
        /// <returns></returns>
        string GetERPClassName();
    }
}
