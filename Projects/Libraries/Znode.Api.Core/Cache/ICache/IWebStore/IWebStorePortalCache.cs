using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public interface IWebStorePortalCache
    {
        /// <summary>
        /// Get Portal information by Portal Id.
        /// </summary>
        /// <param name="portalId">Id of the portal to get portal information.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns portal information by Portal Id.</returns>
        string GetPortal(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Portal information by Portal Id.
        /// </summary>
        /// <param name="portalId">Id of the portal to get portal information.</param>
        /// <param name="localeId">Id of the locale to get portal information.</param>
        /// <param name="applicationType">Application Type of the portal which has to be picked.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns portal information by Portal Id.</returns>
        string GetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Portal information by domainName.
        /// </summary>
        /// <param name="domainName">Id of the portal to get portal information.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns portal information by Portal Id.</returns>
        string GetPortal(string domainName, string routeUri, string routeTemplate);
    }
}
