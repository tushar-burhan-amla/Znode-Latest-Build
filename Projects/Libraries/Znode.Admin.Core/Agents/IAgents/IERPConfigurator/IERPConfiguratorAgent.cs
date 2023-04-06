using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IERPConfiguratorAgent
    {
     
        #region ERP Configurator
        /// <summary>
        /// Get the list of ERP Configurator.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ERPConfigurator list ViewModel.</returns>
        ERPConfiguratorListViewModel GetERPConfiguratorList(ExpandCollection expands,FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get all ERP Configurator Class which are not present in database.
        /// </summary>
        /// <returns>Returns ERP Configurator Class list ViewModel which are not in database.</returns>
        List<SelectListItem> GetAllERPConfiguratorClassesNotInDatabase();

        /// <summary>
        /// Get eRPConfigurator list by ERPConfiguratorId.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator list Id</param>
        /// <returns>Returns ERPConfiguratorViewModel.</returns>
        ERPConfiguratorViewModel GetERPConfigurator(int eRPConfiguratorId);

        /// <summary>
        /// Create new ERP Configurator Class.
        /// </summary>
        /// <param name="eRPConfiguratorModel">ERPConfigurator ViewModel.</param>
        /// <returns>Returns true if ERPConfigurator created else returns false.</returns>
        ERPConfiguratorViewModel Create(ERPConfiguratorViewModel eRPConfiguratorViewModel);

        /// <summary>
        /// Update eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorViewModel">ERPConfigurator view model to update.</param>
        /// <returns>Returns updated eRPConfigurator model.</returns>
        ERPConfiguratorViewModel Update(ERPConfiguratorViewModel eRPConfiguratorViewModel);

        /// <summary>
        /// Delete eRPConfigurator.
        /// </summary>
        /// <param name="eRPConfiguratorId">ERPConfigurator Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(string eRPConfiguratorId, out string errorMessage);

        /// <summary>
        /// Enable Disable ERP Configurator.
        /// </summary>
        /// <param name="eRPConfiguratorId">eRPConfigurator Id which ERP Configurator has to be enabled or disabled.</param>
        /// <param name="isActive">To isActive or deActive ERPConfigurator .</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false.</returns>
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
        #endregion
    }
}
