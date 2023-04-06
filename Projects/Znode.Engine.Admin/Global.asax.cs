using log4net.Config;
using MongoDB.Bson;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Znode.Admin.Core;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using System.Web;
using Znode.Libraries.Framework.Business;

using System.Web;
namespace Znode.Engine.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            StartUpTasks.RegisterDependencies();
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Execute();
            XmlConfigurator.Configure();
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
            //Added to register the custom ActionSessionStateAttribute 
            ControllerBuilder.Current.SetControllerFactory(typeof(SessionStateBehaviorControllerFactory));
            DevExpress.Web.Mvc.MVCxWebDocumentViewer.StaticInitialize();
            // sets the default representation to be used in serialization of Guids to Standard
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new AdminViewEngine());
        }

        //Create cookies for client localization.
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            CookieHelper.SetCookie("culture", CultureInfo.CurrentCulture.TwoLetterISOLanguageName, isCookieHttpOnly: false);
            SetGlobalLoggingSetting();
        }

        //Application Error
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = null;            
            try
            {
                exception = Server.GetLastError().GetBaseException();

                if (exception.GetType() == typeof(ZnodeException))
                {
                    Response.Clear();
                    switch ((exception as ZnodeException).ErrorCode)
                    {
                        case ErrorCodes.InvalidElasticSearchConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                        case ErrorCodes.InvalidDomainConfiguration:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write($"<p style=\"text-align:center;font-size:30;\">{ exception.Message }</p>");
                            break;
                        case ErrorCodes.NotPermitted:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write($"<p style=\"text-align:center;font-size:30;\">{ exception.Message }</p>");
                            break;
                        default:
                            Response.ContentType = "text/html;charset=UTF-8";
                            Response.Write($"<p style=\"text-align:center;font-size:30;\">A generic error occurred.</p>");
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
            catch (Exception ex)
            {
                LogMessage(ex);
                RedirectToErrorPage(sender, ex);
            }
        }

        //Log error message.
        private void LogMessage(Exception exception)
        {
            ZnodeLogging.LogMessage(exception, "AdminApplicationError", TraceLevel.Error);
            //We have removed the error logging code from this method.
            //Because due to this we are getting exception as log file gets busy.
            //If required we need to write custom code for error logging.
        }

        //Redirect to error message
        private void RedirectToErrorPage(Object sender, Exception exception)
        {
            DemoController controller = new DemoController();
            var httpContext = ((MvcApplication)sender).Context;

            var routeData = new RouteData();
            httpContext.ClearError();
            httpContext.Response.Clear();

            SetErrorPage(exception, routeData);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }


        private void SetErrorPage(Exception exception, RouteData routeData)
        {
            ZnodeException znodeException = exception as ZnodeException;
            routeData.Values.Add("controller", "demo");

            switch (znodeException?.ErrorCode)
            {
                case ErrorCodes.WebAPIKeyNotFound:
                    routeData.Values.Add("action", "TokenError");
                    break;
                case ErrorCodes.InvalidDomainConfiguration:
                case ErrorCodes.InvalidSqlConfiguration:
                case ErrorCodes.InvalidElasticSearchConfiguration:
                case ErrorCodes.InvalidZnodeLicense:
                    routeData.Values.Add("action", "ConfigurationError");
                    break;
                default:
                    routeData.Values.Add("action", "ConfigurationError");
                    break;
            }

            routeData.Values.Add("exception", exception);
        }
        //Set Global Logging Settings.
        private void SetGlobalLoggingSetting()
        {
            //Store Global Default Logging Setting in cache
            if (HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache] == null)
                ZnodeDependencyResolver.GetService<ILogMessageAgent>()?.SetGlobalLoggingSetting();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HelperUtility.ReplaceProxyToClientIp();
        }
    }
}
