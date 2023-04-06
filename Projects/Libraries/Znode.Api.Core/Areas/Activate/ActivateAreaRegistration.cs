using System.Web.Mvc;

namespace Znode.Engine.Api.Areas.Activate
{
    public class ActivateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Activate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Activate_default",
                "Activate/{action}/{id}",
                 new { controller = "Activate", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
