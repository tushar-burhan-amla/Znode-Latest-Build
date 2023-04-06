using System.Web.Mvc;

namespace Znode.Engine.Admin.Areas.PIM
{
    public class PIMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PIM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PIM_default",
                "PIM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}