using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Http;
using System.Net;
using System.Web.Security;
using Znode.Engine.WebStore.Agents;
using System.Linq;
using Znode.Libraries.ECommerce.Utilities;
using Znode.WebStore.Core.Extensions;

namespace Znode.Engine.WebStore
{
    public class AuthenticationHelper : AuthorizeAttribute, IAuthenticationHelper
    {
        private readonly string textReturnUrl = "returnUrl";
        private readonly string defaultControllerName = "User";
        private readonly string defaultActionName = "Login";


        //Set Authorization cookie for the logged in user
        public virtual void SetAuthCookie(string userName, bool createPersistantCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistantCookie);
            SessionProxyHelper.SetAuthenticatedUserName(userName);
        }
        //Redirect to login view in case user is not authenticate.
        public virtual void RedirectFromLoginPage(string userName, bool createPersistantCookie) => FormsAuthentication.RedirectFromLoginPage(userName, createPersistantCookie);


        //Overloaded method for Authorize attribute, user to authenticate & authorize the user for each action.
        public override void OnAuthorization(AuthorizationContext filterContext) => AuthenticateUser(filterContext);

        //Method Used to Authenticate the user.
        public virtual void AuthenticateUser(AuthorizationContext filterContext)
        {
            //This OR condition to check authorization for only those action which are annotate with AuthorizeAttribute
            if (Convert.ToBoolean(IsAuthorizationMandatory()) || IsAuthorizeAttribute(filterContext))
            {
                bool isAuthorized = base.AuthorizeCore(filterContext.HttpContext);

                //skipAuthorization get sets to true when the action has the [AllowAnonymous] attributes, If true then skip authentication.
                bool skipAuthorization = (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
                                || (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
                                || (filterContext.ActionDescriptor.IsDefined(typeof(TradeCentricAuthorize), inherit: true))
                                || (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(TradeCentricAuthorize), inherit: true));
                if (!skipAuthorization)
                {
                    if (!isAuthorized && !filterContext.HttpContext.Request.IsAuthenticated && (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name)))
                        HandleUnauthorizedRequest(filterContext);
                    else
                    {
                        if (!SessionProxyHelper.IsLoginUser())
                            HandleUnauthorizedRequest(filterContext);
                    }
                }
            }
            //Validate the User Session/Cookie information based on the Forms Authentication Cookie, and User details stored in Session.
            //In Znode, Login used Forms Authentication to validate the login user, which is a cookie based, and apart from this, User details are also 
            //available with in the Session Variables. So there are two source of data for the logged in user. 
            //This code check for the Session Variable data, and also check whether the Forms authentication cookie is expired or not. 
            //In case Cookie gets expire but Session data persists, then it will gets redirect user to the Login page, where based on the same condition
            ValidateUserSession(filterContext);
        }

        //Handle the Unauthorized Request.
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            HttpRequestBase requestData = filterContext.RequestContext.HttpContext.Request;
            string returnUrl = string.Empty;

            if (requestData.IsAjaxRequest())
            {
                returnUrl = GetReturnUrl(requestData);
            }
            else if (Equals(requestData.HttpMethod, HttpMethod.Post.ToString()))
            {
                returnUrl = GetReturnUrl(requestData);
            }
            else
                returnUrl = requestData.RawUrl;

            returnUrl = returnUrl.Contains(textReturnUrl) ? requestData.RawUrl : returnUrl;

            if (requestData.IsAjaxRequest())
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                string routeName = (Equals(requestData.RequestContext.RouteData.DataTokens["area"], null)) ? string.Empty : Convert.ToString(requestData.RequestContext.RouteData.DataTokens["area"]);
                routeName = (string.IsNullOrEmpty(routeName)) ? GetAreaNameFromUrlReferrer(filterContext) : routeName;
                filterContext.RequestContext.HttpContext.Response.StatusDescription = HttpUtility.UrlEncode(returnUrl);
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        ErrorCode = "101",
                        ReturnUrl = returnUrl,
                        Area = routeName,
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                };
                filterContext.RequestContext.HttpContext.Response.End();
            }
            else
            {
                filterContext.RequestContext.RouteData.DataTokens[WebStoreConstants.AreaKey] = string.Empty;
                filterContext.Result = new RedirectToRouteResult(
                  new RouteValueDictionary {
                        { WebStoreConstants.AreaKey, string.Empty },
                        { WebStoreConstants.Controller, defaultControllerName },
                        { WebStoreConstants.Action, defaultActionName },
                        { textReturnUrl, returnUrl}
                  });
            }
        }

        // Get Area name from the current request UrlReferrer
        protected virtual string GetAreaNameFromUrlReferrer(AuthorizationContext filterContext)
        {
            string fullUrl = filterContext.RequestContext.HttpContext.Request.UrlReferrer.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;
            if (!Equals(questionMarkIndex, -1)) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }
            // Arranges
            HttpRequest request = new HttpRequest(null, url, queryString);
            HttpResponse response = new HttpResponse(new StringWriter());
            HttpContext httpContext = new HttpContext(request, response);
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            return (Equals(routeData.DataTokens[WebStoreConstants.AreaKey], null)) ? string.Empty : Convert.ToString(routeData.DataTokens[WebStoreConstants.AreaKey]);
        }

        // This method is used to check whether authentication is mandatory or not for the current portal(Login Required Flag set from store setting).
        public virtual string IsAuthorizationMandatory()
            => PortalAgent.CurrentPortal?.GlobalAttributes?.Attributes?.FirstOrDefault(x => x.AttributeCode == WebStoreConstants.LoginRequired)?.AttributeValue;

        //Check action is annotate with AuthorizeAttribute
        public virtual bool IsAuthorizeAttribute(AuthorizationContext filterContext)
            => filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), inherit: true);

        protected virtual void ValidateUserSession(AuthorizationContext filterContext)
        {
            if (!Helper.IsUserSessionsValid())
            {
                //Clear logged in user session if user has invalid Session and invalid Auth Cookie. 
                FormsAuthentication.SignOut();
                HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
                SessionProxyHelper.RemoveAuthenticatedUserSession();
                HandleUnauthorizedRequest(filterContext);
            }
        }
        protected virtual string GetReturnUrl(HttpRequestBase requestData)
        {
            return (!Equals(requestData.UrlReferrer, null))
                     ? requestData.UrlReferrer.PathAndQuery
                     : string.Empty;
        }
    }
}
