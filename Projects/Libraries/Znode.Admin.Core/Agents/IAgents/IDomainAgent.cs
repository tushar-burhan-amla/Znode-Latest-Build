using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IDomainAgent
    {
        /// <summary>
        /// Get all the domains from specified portal id
        /// </summary>
        /// <param name="portalId">portal id to get the associated domains</param>
        /// <param name="filters">filter list across Domains</param>
        /// <param name="sortCollection">sort collection for domains.</param>
        /// <param name="pageIndex">pageIndex for Domain record </param>
        /// <param name="recordPerPage">paging domain record per page</param>
        /// <returns></returns>
        DomainListViewModel GetDomains(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create New Domain Url.
        /// </summary>
        /// <param name="model">Domain ViewModel.</param>
        /// <returns>Returns DomainViewModel.</returns>
        DomainViewModel CreateDomainUrl(DomainViewModel domainModel);

        /// <summary>
        /// Get Existing Domain by domain id.
        /// </summary>
        /// <param name="portalId">domain id to get the domain.</param>
        /// <returns>Returns DomainViewModel</returns>
        DomainViewModel GetDomain(int domainId);

        /// <summary>
        /// Update Existing Domain url.
        /// </summary>
        /// <param name="viewModel">DomainViewModel</param>
        /// <returns>Returns true if Domain Updated else returns false.</returns>
        DomainViewModel UpdateDomainUrl(DomainViewModel viewModel);

        /// <summary>
        /// Delete Existing Domain Url by domain Id.
        /// </summary>
        /// <param name="domainId">domain Id of domain to be deleted.</param>
        /// <param name="message">returns message domain is deleted nor not</param>
        /// <returns>Returns true if Domain Deleted else returns false.</returns>
        bool DeleteDomainUrl(string domainId, out string message);

        /// <summary>
        /// Bind Application types to dropdown.
        /// </summary>
        /// <returns>List<SelectListItem>.</returns>
        List<SelectListItem> GetApplicationTypes();

        /// <summary>
        /// Check whether the domain name is already exists or not.
        /// </summary>
        /// <param name="domainName">Name of the Domain</param>
        /// <param name="domainId">Id for the Domain</param>
        /// <returns>return true or false</returns>
        bool CheckDomainNameExist(string domainName, int domainId);

        /// <summary>
        /// Validate Domain against Default Value
        /// </summary>
        /// <param name="PortalId"></param>
        /// <param name="ApplicationType"></param>
        /// <param name="DomainId"></param>
        /// <param name="DefaultChecked"></param>
        /// <returns></returns>
        bool ValidateDomainIsDefault(int PortalId, string ApplicationType, int DomainId, bool DefaultChecked);

        /// <summary>
        /// Enable Disable customer domains.
        /// </summary>
        /// <param name="domainId">User Ids whose domain has to be enabled or disabled.</param>
        /// <param name="IsActive">IsActive</param>
        /// <param name="portalId">portalId</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false.</returns>
        bool EnableDisableDomain(string domainId, int portalId, bool IsActive, out string errorMessage);

        /// <summary>
        /// Set filters for domain.
        /// </summary>
        /// <param name="filters">filter collection.</param>
        /// <param name="filterKey">filterKey</param>
        /// <param name="filterOperator">filterOperator</param>
        /// <param name="filterValue">filterValue</param>
        void SetFilters(FilterCollection filters, string filterKey, string filterOperator, string filterValue);

        /// <summary>
        /// Validate the Domain Name URl in Manage Url for edit function inline with table data.
        /// </summary>
        /// <param name="domainViewModel"></param>
        /// <returns></returns>
        bool ValidateDomainUrl(DomainViewModel domainViewModel);
    }
}

