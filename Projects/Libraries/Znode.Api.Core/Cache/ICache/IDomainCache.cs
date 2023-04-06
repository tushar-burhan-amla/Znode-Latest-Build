using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Api.Cache
{
    public interface IDomainCache
    {
        /// <summary>
        /// Get domain from specified domainId.
        /// </summary>
        /// <param name="domainId">int domainId to get the domain.</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns>return in string format</returns>
        string GetDomain(int domainId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Get domain. 
        /// </summary>
        /// <param name="routeUri">URI to Route.</param>
        /// <returns>ZNodeDomain.</returns>
        ZnodeDomain GetDomain(string routeUri);

        /// <summary>
        /// Get list of domain.
        /// </summary>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns>return in string format</returns>
        string GetDomains(string routeUri, string routeTemplate);
    }
}
