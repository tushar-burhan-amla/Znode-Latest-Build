using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Znode.Multifront.PaymentApplication.Api.Helpers;

namespace Znode.Multifront.PaymentApplication.Api.Filters
{
    public class TokenBasedAuthorization : AuthorizeAttribute
    {
        //Added DoNotCount property to avoid increasing the token usage count.
        public bool DoNotCount { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ValidateAuthHeader"]))
            {
                bool isAuthorized = new AuthorizationHelper().HasValidAuthorization(DoNotCount);
                
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
            else
                return true;
            
        }
    }
}