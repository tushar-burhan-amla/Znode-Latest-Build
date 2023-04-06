using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStorePortalEndpoint : BaseEndpoint
    {
        //Endpoint to get details of portal by portal Id.
        public static string GetPortal(int portalId) => $"{ApiRoot}/webstoreportal/getportal/{portalId}";

        //Endpoint to get details of portal by portal Id, Locale Id and Content state.
        public static string GetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType) => $"{ApiRoot}/webstoreportal/getportal/{portalId}/{localeId}/{applicationType}";

        //Endpoint to get details of portal by domainName.
        public static string GetPortal(string domainName) => $"{ApiRoot}/webstoreportal/getportal/{domainName}";
    }
}
