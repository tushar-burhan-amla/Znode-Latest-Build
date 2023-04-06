using System.Web.Http;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (ZnodeApiSettings.DisableRoutesForStaticFile)
            {
               context.Routes.IgnoreRoute("help/css");
            }

            context.MapRoute(
                "HelpPage_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}