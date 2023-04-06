using System.Web.Mvc;

namespace Znode.Engine.Api.Areas.Ecommerce
{
    public class EcommerceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Ecommerce";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Ecommerce_default",
                "Ecommerce/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}