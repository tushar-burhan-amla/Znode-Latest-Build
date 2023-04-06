using System.Web.Mvc;
using System.Web.Routing;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.WebStore.ViewModels;
using Znode.WebStore.Core.Extensions;

namespace Znode.Engine.WebStore.Controllers
{
    public class ErrorPageController : Controller
    {
        #region "Public Methods"

        // GET: ErrorPage
        public virtual ActionResult Index()
        {
            HttpContext.Response.AddHeader("Location", "/Error");
            return new HttpStatusCodeResult(307);
        }

        [NoCacheAttribute]
        public virtual ActionResult PageNotFound()
        {
            //Getting Seo URL details for 404 seo slug
            SEOUrlViewModel model = GetSeoUrlDetails(WebStoreConstants.ContentPage404);
            
            if (HelperUtility.IsNotNull(model) && model.SEOId > 0 && model.IsActive)
            {
                //setting 404 response from CMS content page from passing 404 contagepageId
                Set404Response(model.SEOId);

                Response.StatusCode = 404;

                Response.TrySkipIisCustomErrors = true;

                Response.End();
            }
            else
            {
                Response.ContentType = "text/html;charset=UTF-8";
                return View("ErrorPage");              
            }
           
            return null;          
        }

        [AllowAnonymous]
        public virtual ActionResult UnAuthorizedErrorRequest()=>
            View("UnAuthorizedRequest");

        #endregion

        #region "Protected Methods"

        //Get Seo Details by seo slug
        protected virtual SEOUrlViewModel GetSeoUrlDetails(string seoSlug)
        {
            IWebstoreHelper helper = ZnodeDependencyResolver.GetService<IWebstoreHelper>();

            return helper.GetSeoUrlDetail(seoSlug);
        }

        //Set 404 content page response of current portal
        protected virtual void Set404Response(int seoId)
        {
            var routeData = new RouteData();

            routeData.Values.Add("controller", "ContentPage");
            routeData.Values.Add("action", "ContentPage");
            routeData.Values.Add("contentPageId", seoId);

            var controller = ZnodeDependencyResolver.GetService<ContentPageController>();

            ((IController)controller).Execute(new RequestContext(HttpContext, routeData));
        }
        #endregion
    }
}