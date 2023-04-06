using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Znode.Engine.Api
{
    public static class KlaviyoApiConfig
    {
        //Klaviyo Routes
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("klaviyo-updateklaviyosetting", "klaviyo/updateklaviyosetting", new { controller = "klaviyo", action = "updateklaviyosetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("klaviyo-getklaviyosetting", "klaviyo/GetKlaviyo/{portalId}", new { controller = "klaviyo", action = "getklaviyo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("klaviyo-trackklaviyosetting", "klaviyo/track", new { controller = "klaviyo", action = "track" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("klaviyo-identifyklaviyosetting", "klaviyo/identify", new { controller = "klaviyo", action = "identify" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("klaviyo-getemailproviderlist", "klaviyo/GetEmailProviderList", new { controller = "klaviyo", action = "getemailproviderlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
        }
    }
}
