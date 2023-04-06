using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IDomainService
    {
        /// <summary>
        /// Get Domain by Domain Name.
        /// </summary>
        /// <param name="domainName">string domainName to get the domain.</param>
        /// <returns>ZNodeDomain</returns>
        ZnodeDomain GetDomain(string domainName);

        /// <summary>
        ///  Get Domain by Domain Id.
        /// </summary>
        /// <param name="domainId">int domainId to get the domain.</param>
        /// <returns>DomainModel</returns>
        DomainModel GetDomain(int domainId);

        /// <summary>
        ///  Get Domain List.
        /// </summary>
        /// <param name="filters">Collection of filters.</param>
        /// <param name="sorts">Collection of sorts.</param>
        /// <param name="page">Collection of page.</param>
        /// <returns></returns>
        DomainListModel GetDomains(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new Domain.
        /// </summary>
        /// <param name="domainModel">DomainModel model to create new model.</param>
        /// <returns>DomainModel.</returns>
        DomainModel CreateDomain(DomainModel domainModel);

        /// <summary>
        /// Update Domain 
        /// </summary>
        /// <param name="domainModel">DomainModel model to update existing model.</param>
        /// <returns>returns true if Doamin is Updated else returns false</returns>
        bool UpdateDomain(DomainModel domainModel);

        /// <summary>
        ///  Delete Domain. 
        /// </summary>
        /// <param name="domainIds">domainIds to delete the domain</param>
        /// <returns>returns true if Doamin is Deleted else returns false</returns>
        bool DeleteDomain(ParameterModel domainIds);

        /// <summary>
        /// This method will enable/disable the domain.
        /// </summary>
        /// <param name="domainModel">DomainModel model to update existing model.</param>
        /// <returns>Returns true if user account deleted successfully, else return false.</returns>
        bool EnableDisableDomain(DomainModel domainModel);
    }
}
