using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IDomainClient : IBaseClient
    {
        /// <summary>
        /// Get all the domains. 
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sortCollection">Sort collection.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>DomainListModel</returns>
        DomainListModel GetDomains(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get domain from specified domain id.
        /// </summary>
        /// <param name="domainId">domain id to get the domain.</param>
        /// <returns>DomainModel.</returns>
        DomainModel GetDomain(int domainId);

        /// <summary>
        ///  Create New Domain Url.
        /// </summary>
        /// <param name="domainModel">DomainModel to create the new domain.</param>
        /// <returns>returns DomainModel</returns>
        DomainModel CreateDomain(DomainModel domainModel);

        /// <summary>
        /// Update Domain Url.
        /// </summary>
        /// <param name="domainModel">DomainModel to update the existing domain.</param>
        /// <returns>returns true if Domain URL is Updated otherwise returns false</returns>
        DomainModel UpdateDomain(DomainModel domainModel);

        /// <summary>
        /// Delete Domain Url.
        /// </summary>
        /// <param name="domainIds">domain ids to delete the domain.</param>
        /// <returns>returns true if Domain URL is Deleted otherwise returns false.</returns>
        bool DeleteDomain(ParameterModel domainIds);

        /// <summary>
        /// Enable disable domain on the basis of domain id.
        /// </summary>
        /// <param name="domainModel">domainModel</param>
        /// <returns>Returns true/false</returns>
        bool EnableDisableDomain(DomainModel domainModel);
    }
}
