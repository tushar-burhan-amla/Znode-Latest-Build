
using Znode.Libraries.Abstract.Client.Endpoints;

namespace Znode.Engine.klaviyo.Client.Endpoints
{
    public class KlaviyoEndpoint : BaseEndpoint
    {
        //Get Klaviyo Endpoint
        public static string Get(int portalId) => $"{ApiRoot}/klaviyo/GetKlaviyo/{portalId}";

        //Update Klaviyo Endpoint
        public static string Update() => $"{ApiRoot}/klaviyo/updateklaviyosetting";

        //Get Track Api Endpoint
        public static string Track() => $"{ApiRoot}/klaviyo/track";

        //Get Identify Api Endpoint
        public static string Identify() => $"{ApiRoot}/klaviyo/identify";

        //Get Email Provider List Endpoint
        public static string GetEmailProviderList() => $"{ApiRoot}/klaviyo/GetEmailProviderList";

    }
}
