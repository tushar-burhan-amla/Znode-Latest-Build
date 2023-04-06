using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Helpers
{
    public class AuthenticationHelper : AuthorizeAttribute, IAuthenticationHelper
    {
        private string permission;
        private string actionName = string.Empty;
        private string controllerName = string.Empty;
        private string defaultControllerName = "User";
        private string defaultActionName = "Login";
        private string textReturnUrl = "returnUrl";

        public string PermissionKey
        {
            get
            {
                return permission;
            }
            set
            {
                permission = value;
            }
        }

        //Set Authentication cookied for the logged in user
        public virtual void SetAuthCookie(string userName, bool createPersistantCookie) => FormsAuthentication.SetAuthCookie(userName, createPersistantCookie);

        //Redirect to login view in case user is not authenticate.
        public virtual void RedirectFromLoginPage(string userName, bool createPersistantCookie) => FormsAuthentication.RedirectFromLoginPage(userName, createPersistantCookie);

        //Overloaded method for Authorize attribute, user to authenticate & authorize the user for each action.
        public override void OnAuthorization(AuthorizationContext filterContext) => AuthenticateUser(filterContext);

        //Method Used to Authenticate the user.
        public virtual void AuthenticateUser(AuthorizationContext filterContext)
        {
            var isAuthorized = base.AuthorizeCore(filterContext.HttpContext);

            //skipAuthorization get sets to true when the action has the [AllowAnonymous] attributes, If true then skip authentication.
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                            || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (!skipAuthorization)
            {
                if (!isAuthorized && !filterContext.HttpContext.Request.IsAuthenticated && (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name)))
                    HandleUnauthorizedRequest(filterContext);
                else
                {
                    if (!SessionProxyHelper.IsAdminUser())
                    {
                        if (!AuthorizeRequest(filterContext))
                            HandleUnauthorizedRequest(filterContext);
                    }
                }
            }
        }

        #region HandleUnauthorizedRequest
        //Redirect User to Index page in case the un authorized access.
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string returnUrl = (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                 ? (!Equals(filterContext.RequestContext.HttpContext.Request.UrlReferrer, null))
                     ? filterContext.RequestContext.HttpContext.Request.UrlReferrer.PathAndQuery
                     : string.Empty
                 : (Equals(filterContext.RequestContext.HttpContext.Request.HttpMethod, HttpMethod.Post.ToString()))
                 ? (!Equals(filterContext.RequestContext.HttpContext.Request.UrlReferrer, null))
                     ? filterContext.RequestContext.HttpContext.Request.UrlReferrer.PathAndQuery
                     : string.Empty
                 : filterContext.RequestContext.HttpContext.Request.RawUrl;

            returnUrl = returnUrl.Contains(textReturnUrl) ? filterContext.RequestContext.HttpContext.Request.RawUrl : returnUrl;

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                string routeName = (Equals(filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[AdminConstants.AreaKey], null)) ? string.Empty : Convert.ToString(filterContext.RequestContext.HttpContext.Request.RequestContext.RouteData.DataTokens[AdminConstants.AreaKey]);
                routeName = (string.IsNullOrEmpty(routeName)) ? GetAreaNameFromUrlReferrer(filterContext) : routeName;
                filterContext.RequestContext.HttpContext.Response.StatusDescription = HttpUtility.UrlEncode(returnUrl);
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        ErrorCode = "101",
                        ReturnUrl = HttpUtility.UrlEncode(returnUrl),
                        Area = routeName,
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                };
                filterContext.RequestContext.HttpContext.Response.End();
            }
            else
            {
                filterContext.RequestContext.RouteData.DataTokens[AdminConstants.AreaKey] = string.Empty;
                filterContext.Result = new RedirectToRouteResult(
                          new RouteValueDictionary {
                        { AdminConstants.AreaKey, string.Empty },
                        { AdminConstants.Controller, defaultControllerName },
                        { AdminConstants.Action, defaultActionName },
                        { textReturnUrl, HttpUtility.UrlEncode(returnUrl)}
                          });
            }
        }

        #endregion

        #region CreateKey
        // Retrieves the Current Action & Controller Name.       
        protected virtual void CreateKey(AuthorizationContext filterContext)
        {
            controllerName = filterContext.HttpContext.Request.RequestContext.RouteData.Values[AdminConstants.Controller].ToString();
            actionName = filterContext.HttpContext.Request.RequestContext.RouteData.Values[AdminConstants.Action].ToString();
            PermissionKey = $"{ controllerName}/{actionName}";
        }
        #endregion

        // Get Area name from the current request UrlReferrer
        protected virtual string GetAreaNameFromUrlReferrer(AuthorizationContext filterContext)
        {
            string areaName = string.Empty;
            var fullUrl = filterContext.RequestContext.HttpContext.Request.UrlReferrer.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;
            if (!Equals(questionMarkIndex, -1)) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }
            // Arranges
            var request = new HttpRequest(null, url, queryString);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            return areaName = (Equals(routeData.DataTokens[AdminConstants.AreaKey], null)) ? string.Empty : Convert.ToString(routeData.DataTokens[AdminConstants.AreaKey]);
        }

        protected virtual bool AuthorizeRequest(AuthorizationContext filterContext)
        {
            bool result = false;
            CreateKey(filterContext);
            if (UnrestrictedPermissionKeys().Contains(PermissionKey.ToLower()))
            {
                return true;
            }
            List<RolePermissionViewModel> lstPermissions = SessionProxyHelper.GetUserPermission();
            result = Equals(lstPermissions, null) ? false : lstPermissions.FindIndex(w => string.Equals(PermissionKey, w.RequestUrlTemplate, StringComparison.InvariantCultureIgnoreCase)) != -1;
            return result;
        }

        //List of UnRestricted Permission Keys.
        protected virtual List<string> UnrestrictedPermissionKeys()
        {
            List<string> lstPermission = new List<string>();
            lstPermission.Add("dashboard/dashboard");
            lstPermission.Add("user/changepassword");
            return lstPermission;
        }

    }
}
