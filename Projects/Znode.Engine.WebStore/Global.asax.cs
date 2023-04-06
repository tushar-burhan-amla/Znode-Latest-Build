using log4net.Config;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Controllers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.Caching;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.WebStore.Caching.Core;
using Znode.WebStore.Caching.Helpers;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore
{
    public class MvcApplication : HttpApplication
    {
        private static ICacheEventPoller cacheEventPoller;

        protected void Application_Start()
        {
            StartUpTasks.RegisterDependencies();
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(APIConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            XmlConfigurator.Configure();
            AutoMapperConfig.Execute();
            //To register the custom ActionSessionStateAttribute 
            ControllerBuilder.Current.SetControllerFactory(typeof(SessionStateBehaviorControllerFactory));
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemedViewEngine());
            MvcHandler.DisableMvcResponseHeader = true;

            bool minifiedJsonResponse = ZnodeWebstoreSettings.MinifiedJsonResponse;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = minifiedJsonResponse ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
            };

            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            // at least for dev and QA, trust any certificate
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            // sets the default representation to be used in serialization of Guids to Standard
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            cacheEventPoller = new WebStoreCacheEventPoller(Convert.ToInt32(ConfigurationManager.AppSettings["CacheEventProcessingDelayInMilliseconds"]));
            OutputCacheHelper.RegisterMemoryCache();
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");           //Remove Server Header   
            Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            try
            {
                HelperUtility.ReplaceProxyToClientIp();
                cacheEventPoller?.PollIfNecessary();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Polling for cache events threw an unexpected exception.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error);
            }

            string currentUrl = $"~{Request.RawUrl.ToLower()}";
            SetTradeCentricCookies();
            UrlRedirectViewModel redirectUrl = UrlRedirectAgent.Has301Redirect(currentUrl);
            if (HelperUtility.IsNotNull(redirectUrl))
                Context.Response.RedirectPermanent($"~/{redirectUrl.RedirectTo.Replace($"http://{HttpContext.Current.Request.Url.Authority}/", string.Empty)}");
        }

        // parameter key
        private static readonly string ReturnUrlParameter = "ReturnUrl";

        //This method validate Query String for Return URL
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.IsRequestBeingRedirected)
            {
                Uri redirectUrl;
                if (Uri.TryCreate(Response.RedirectLocation, UriKind.RelativeOrAbsolute, out redirectUrl))
                {
                    redirectUrl = MakeAbsoluteUriIfNecessary(redirectUrl);
                    Uri currentUrl = Request.Url;
                    var currentQueryParameters =
                            HttpUtility.ParseQueryString(HttpUtility.UrlDecode(currentUrl.Query));
                    // the parameter is present in the current url already
                    if (currentQueryParameters[ReturnUrlParameter] != null)
                    {
                        UriBuilder builder = new UriBuilder(redirectUrl);
                        builder.Query =
                                HttpUtility.UrlDecode(builder.Query)
                                    .Replace(Request.Url.Query, string.Empty).TrimStart('?');

                        Response.RedirectLocation =
                                Request.Url.MakeRelativeUri(builder.Uri).ToString();
                        if (Response.RedirectLocation == string.Empty) Response.RedirectLocation = "Login";

                    }
                }
            }
        }
        //Creating Absolute URL
        private Uri MakeAbsoluteUriIfNecessary(Uri url)
        {
            if (url.IsAbsoluteUri)
            {
                return url;
            }
            else
            {
                Uri currentUrl = Request.Url;
                UriBuilder builder = new UriBuilder(
                        currentUrl.Scheme,
                        currentUrl.Host,
                        currentUrl.Port
                    );

                return new Uri(builder.Uri, url);
            }
        }

        //Redirect to cart page if user added any cart item from from browser.
        private void Session_Start(object sender, EventArgs e)
        {
            IWebstoreHelper sessionStartHelper = GetService<IWebstoreHelper>();
            sessionStartHelper.Session_Start(sender, e);
        }
        //Create cookies for client localization.
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            SetGlobalLoggingSetting();
        }

        //Application Error.
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = null;
            exception = Server.GetLastError().GetBaseException();
            try
            {
                if (exception.GetType() == typeof(ZnodeException))
                {
                    Response.Clear();
                    switch ((exception as ZnodeException).ErrorCode)
                    {
                        case ErrorCodes.StoreNotPublished:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write(WebStore_Resources.ErrorStoreNotPublished);
                            break;
                        case ErrorCodes.InvalidElasticSearchConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                        case ErrorCodes.InvalidDomainConfiguration:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write($"<p style=\"text-align:center;font-size:30;\">{ exception.Message }</p>");
                            break;
                        default:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write(WebStore_Resources.GenericConfigurationError);
                            break;
                    }
                    Server.ClearError();
                }
                else
                {
                    RedirectToErrorPage(sender, exception);
                }
                LogMessage(exception);
            }
            catch (Exception ex) { LogMessage(ex); }
        }

        // This method is used get the parameter for the Vary By Custom
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                if (arg.Equals(WebStoreConstants.RobotsTxtPortalIdFullPageCacheKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    return GetRobotTxtVaryByCustomString();
                }
            }
            return base.GetVaryByCustomString(context, arg);
        }

        #region Private Methods
        private void RedirectToErrorPage(Object sender, Exception exception)
        {
            var controller = new ErrorPageController();
            var httpContext = ((MvcApplication)sender).Context;

            var routeData = new RouteData();
            httpContext.ClearError();
            httpContext.Response.Clear();

            SetErrorPage(exception, routeData);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));

        }

        private void SetErrorPage(Exception exception, RouteData routeData)
        {
            routeData.Values.Add("controller", "ErrorPage");
            routeData.Values.Add("action", "PageNotFound");
            routeData.Values.Add("exception", exception);

            if (exception.GetType().Equals(typeof(HttpException)))
            {

                Response.StatusCode = ((HttpException)exception).GetHttpCode();
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else
            {
                Response.StatusCode = 500;
                routeData.Values.Add("statusCode", 500);
            }

            Response.TrySkipIisCustomErrors = true;
        }

        //Log error message.
        private void LogMessage(Exception exception)
        {
            ZnodeLogging.LogMessage(exception, "WebstoreApplicationError", TraceLevel.Error);
        }
        //Set Global Logging Settings.
        private void SetGlobalLoggingSetting()
        {
            //Store Global Default Logging Setting in cache
            if (HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache] == null)
                ZnodeDependencyResolver.GetService<IPortalAgent>()?.SetGlobalLoggingSetting();
        }

        // This method is used get the parameter of the Vary By Custom for Home/GetRobotsTxt1 method
        private string GetRobotTxtVaryByCustomString()
        {
            return PortalAgent.CurrentPortal.PortalId.ToString();
        }
        //Set cookies if the request originates from the TradeCentric portal.
        private void SetTradeCentricCookies()
        {
            if (Request?.Headers["sec-fetch-dest"] == "iframe" && GlobalAttributeHelper.IsEnableTradeCentric() && Request.Cookies.Count > 0)
            {
                HttpContext.Current.Response.AddOnSendingHeaders(context =>
                {
                    var cookies = context.Response.Cookies;
                    for (var i = 0; i < cookies.Count; i++)
                    {
                        var cookie = cookies[i];
                        cookie.SameSite = SameSiteMode.None;
                        cookie.Secure = true;
                    }
                });
            }
        }
        #endregion
    }
}
