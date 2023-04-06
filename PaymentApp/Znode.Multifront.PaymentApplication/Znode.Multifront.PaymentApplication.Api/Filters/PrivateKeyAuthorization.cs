using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Znode.Multifront.PaymentApplication.Api.Helpers;

namespace Znode.Multifront.PaymentApplication.Api.Filters
{
    public class PrivateKeyAuthorization : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = new AuthorizationHelper().HasValidPrivateKey();
            if (!isAuthorized)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                HttpContext.Current.Response.StatusDescription = "The requested operation is forbidden.";
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            return isAuthorized;
        }
    }
}