using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IUrlManagementAgent
    {
        /// <summary>
        /// Get the list of Admin and Api Domains
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Domain list view model</returns>
        DomainListViewModel GetDomainList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Existing Domain by domain id.
        /// </summary>
        /// <param name="portalId">domain id to get the domain.</param>
        /// <returns>Returns DomainViewModel</returns>
        DomainViewModel GetDomain(int domainId);

        /// <summary>
        /// Bind Admin and API types to dropdown
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> GetAdminAPIApplicationTypes();

        /// <summary>
        /// Create New Domain Url.
        /// </summary>
        /// <param name="model">Domain ViewModel.</param>
        /// <returns>Returns DomainViewModel.</returns>
        DomainViewModel CreateDomainUrl(DomainViewModel domainViewModel);
        
        /// <summary>
        /// Enable Disable customer domains.
        /// </summary>
        /// <param name="domainId">User Ids whose domain has to be enabled or disabled.</param>
        /// <param name="IsActive">IsActive</param>
        /// <param name="portalId">portalId</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false.</returns>
        bool EnableDisableDomain(string domainId, bool isActive, out string errorMessage);

        /// <summary>
        /// Check whether the domain name is already exists or not.
        /// </summary>
        /// <param name="domainName">Name of the Domain</param>
        /// <param name="domainId">Id for the Domain</param>
        /// <returns>return true or false</returns>
        bool CheckDomainNameExist(string domainName, int domainId);

        /// <summary>
        /// Update Existing Domain url.
        /// </summary>
        /// <param name="viewModel">DomainViewModel</param>
        /// <returns>Returns true if Domain Updated else returns false.</returns>
        DomainViewModel UpdateDomainUrl(DomainViewModel domainViewModel);
        
        /// <summary>
        /// Delete Existing Domain Url by domain Id.
        /// </summary>
        /// <param name="domainId">domain Id of domain to be deleted.</param>
        /// <param name="message">returns message domain is deleted nor not</param>
        /// <returns>Returns true if Domain Deleted else returns false.</returns>
        bool DeleteDomainUrl(string domainId, out string message);
    }
}
