using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStorePortalClient : IBaseClient
    {
        /// <summary>
        /// Get Portal information on the basis of Portal Id.
        /// </summary>
        /// <param name="portalId">Id of the portal to get portal information.</param>
        /// <param name="expands">Expands to be fetched along with Portal.</param>
        /// <returns>Returns WebStorePortalModel containing portal information.</returns>
        WebStorePortalModel GetPortal(int portalId, ExpandCollection expands);

        /// <summary>
        /// Get Portal information on the basis of Portal Id.
        /// </summary>
        /// <param name="portalId">Id of the portal to get portal information.</param>
        /// <param name="localeId">Locale Id to get the portal information for.</param>
        /// <param name="applicationType">Application type of the portal which has to be picked.</param>
        /// <param name="expands">Expands to be fetched along with Portal.</param>
        /// <returns>Returns WebStorePortalModel containing portal information.</returns>
        WebStorePortalModel GetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, ExpandCollection expands);

        /// <summary>
        /// Get Portal information on the basis of domainname.
        /// </summary>
        /// <param name="domainName">domainname of the portal to get portal information.</param>
        /// <param name="expands">Expands to be fetched along with Portal.</param>
        /// <returns>Returns WebStorePortalModel containing portal information.</returns>
        WebStorePortalModel GetPortal(string domainName, ExpandCollection expands);
    }
}
