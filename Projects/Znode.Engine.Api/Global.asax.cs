using log4net.Config;
using MongoDB.Bson;
using StructureMap;
using System;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Znode.Api.Core;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Caching;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Api.Caching.Core;
using Znode.Cloudflare.Caching.Core;
using System.Configuration;

namespace Znode.Engine.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static bool StartupCompleted = false;
        private readonly string logComponentName = "Diagnostics";
        private ICacheEventPoller cacheEventPoller;
        private ICacheEventPoller cacheCloudFlareEventPoller;

        protected void Application_Start()
        {
            ZnodeLogging.LogMessage("Znode API is starting up", logComponentName, TraceLevel.Verbose);
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            ObjectFactory.Initialize(scanner => scanner.Scan(x =>
            {
                x.AssembliesFromApplicationBaseDirectory(
                    assembly => assembly.FullName.Contains("Znode."));
                x.WithDefaultConventions();
            }));
            PerformRegistrations();
            EnsureStartupCompleted();
        }

        protected void Session_Start()
        {
            //Create scheduler for voucher reminder Email.
            ApiHelper.CreateVoucherReminderEmailScheduler();

            //Create scheduler for clear all user registration attempt detail.
            ApiHelper.CreateClearExistingUserRegistrationAttemptScheduler();

            //Create scheduler for delete expired payment token.
            ApiHelper.CreateDeletePaymentTokenScheduler();

            //Create scheduler for stock notification.
            ApiHelper.SendStockNotification();

            //Create scheduler for delete outdated export files.
            ApiHelper.CreateDeleteExportFileScheduler();
        }

        private void PerformRegistrations()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(KlaviyoApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            XmlConfigurator.Configure();
            AutoMapperConfig.Execute();
            ZnodeLogging.LogMessage("Initial registrations and configuration has been performed.", logComponentName, TraceLevel.Verbose);
        }

        private bool EnsureStartupCompleted()
        {
            if (!StartupCompleted)
            {
                ZnodeDiagnostics.RunDiagnostics();
                DefaultCacheBuilder.TryBuildCaches();
                // sets the default representation to be used in serialization of Guids to Standard
                BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
                StartupCompleted = true;
                ZnodeLogging.LogMessage("API startup has completed successfully.", logComponentName, TraceLevel.Info);
            }

            if (cacheEventPoller == null)
            {
                cacheEventPoller = new ApiCacheEventPoller(Convert.ToInt32(ConfigurationManager.AppSettings["CacheEventProcessingDelayInMilliseconds"]));
            }

            if (cacheCloudFlareEventPoller == null)
            {
                cacheCloudFlareEventPoller = new CloudflareCacheEventPoller(Convert.ToInt32(ConfigurationManager.AppSettings["CloudflareCacheEventProcessingDelayInMilliseconds"]));
            }

            return StartupCompleted;
        }

        private void WriteResponseErrorMessage(ZnodeException exception)
        {
            HttpContext.Current.Response.StatusCode = 404;

            if (HttpContext.Current.Request.Headers.AllKeys.Contains("Znode-DomainName") && !string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Znode-DomainName"]))
            {
                BaseResponse responseBody = new BaseResponse
                {
                    ErrorCode = exception.ErrorCode,
                    ErrorMessage = Api_Resources.ApiNotAvailableMessage,
                    HasError = true
                };

                HttpContext.Current.Response.Write(HelperUtility.ToJSON(responseBody));
            }
            else
            {
                HttpContext.Current.Response.Write($"<p style=\"text-align:center;font-size:30;\">{exception.ErrorMessage}</p>");
            }

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            try
            {
                HelperUtility.ReplaceProxyToClientIp();
                EnsureStartupCompleted();
            }
            catch (ZnodeException ex)
            {
                WriteResponseErrorMessage(ex);
                return;
            }
            try
            {
                cacheEventPoller?.PollIfNecessary();
                cacheCloudFlareEventPoller?.PollIfNecessary();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Polling for cache events threw an unexpected exception.", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error);
            }

            FirstRequestInitialization.Initialize();
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Authorization-Token, X-HTTP-Method-Override,Znode-UserId,Znode-DomainName,Authorization,Token");
                HttpContext.Current.Response.End();
            }
            SetSiteConfig();
        }

        protected void Application_AcquireRequestState()
        {
            ProfessionalDomainIdentifier.SetApiMandate();
            SetGlobalLoggingSetting();
        }

        private void SetSiteConfig()
        {
            var domainName = GetDomainNameFromAuthHeader();

            //Set Domain Config & Site Config details based on domain name.
            SetDomainAndSiteConfigDetails(domainName);

            //Set the API Domain & Site Configuration.
            SetAPISiteConfig();
        }

        private void SetDomainAndSiteConfigDetails(string domainName)
        {
            if (!String.IsNullOrEmpty(domainName))
            {
                if (!ZnodeConfigManager.CheckSiteConfigCache(domainName) || ZnodeConfigManager.GetDomainConfig(domainName) == null)
                {
                    var cache = new DomainCache(new DomainService());
                    var domainConfig = cache.GetDomain(domainName);

                    if (Equals(domainConfig, null) || !domainConfig.IsActive)
                    {
                        ZnodeLogging.LogMessage($"Domain {domainName} has not been configured to work with Znode.", string.Empty, System.Diagnostics.TraceLevel.Warning);

                        // The URL was not found in our config, send out a 404 error
                        HttpContext.Current.Response.StatusCode = 404;

                        BaseResponse responseBody = new BaseResponse
                        {
                            ErrorCode = ErrorCodes.InvalidDomainConfiguration,
                            ErrorMessage = Api_Resources.InvalidDomainConfigurationMessage,
                            HasError = true
                        };

                        HttpContext.Current.Response.Write(HelperUtility.ToJSON(responseBody));
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    ZnodeConfigManager.SetDomainConfig(domainName, domainConfig);

                    //Set below call from Portal Cache.
                    IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
                    ZnodePortal portal = _portalRepository.GetById(domainConfig.PortalId);
                    ZnodeConfigManager.SetSiteConfig(domainName, portal);
                }
            }
        }

        private void SetAPISiteConfig()
        {
            var domainName = GetAPIDomainName();

            //Set Domain Config & Site Config details based on domain name.
            SetDomainAndSiteConfigDetails(domainName);
        }
        private string GetAPIDomainName()
            => Convert.ToBoolean(ZnodeApiSettings.ValidateAuthHeader) ? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim() : string.Empty;

        private string GetDomainNameFromAuthHeader()
        {
            var headers = HttpContext.Current.Request.Headers;

            const string domainHeader = "Znode-DomainName";

            string domain = string.IsNullOrEmpty(headers[domainHeader]) ? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim() : headers[domainHeader].ToString();

            return domain;
        }

        private string DecodeBase64(string encodedValue)
        {
            var encodedValueAsBytes = Convert.FromBase64String(encodedValue);
            return Encoding.UTF8.GetString(encodedValueAsBytes);
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            DisposeContext();
        }


        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = null;
            exception = Server.GetLastError().GetBaseException();
            LogMessage(exception);

            DisposeContext();
            RedirectToErrorPage(sender, exception);
        }

        private void RedirectToErrorPage(Object sender, Exception exception)
        {
            var controller = new Znode.Engine.Api.Controllers.HomeController();
            var httpContext = HttpContext.Current;

            var routeData = new RouteData();
            httpContext.ClearError();
            httpContext.Response.Clear();

            SetErrorPage(exception, routeData);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        private void SetErrorPage(Exception exception, RouteData routeData)
        {
            routeData.Values.Add("controller", "home");
            routeData.Values.Add("action", "index");
        }

        private void DisposeContext()
        {
            //Dispose Entity Framework Context.
            RemoveCurrentContextItems("ocm_");

            //Dispose recommendation DB context.
            RemoveCurrentContextItems("recommendationocm_");

            //Dispose Mongo DB Context.
            RemoveCurrentContextItems("mongo_");

            //Dispose Mongo Log DB Context
            RemoveCurrentContextItems("mongologdb_");

            //Dispose Publish Entity DB Context 
            RemoveCurrentContextItems("publishentityocm_");

        }

        //Log error message.
        private void LogMessage(Exception exception)
        {
            ZnodeLogging.LogMessage(exception, "ApiApplicationError", TraceLevel.Error);
        }

        //Remove the Current Context Items based on the key.
        private void RemoveCurrentContextItems(string key)
        {
            string objectContextKey = key + HttpContext.Current.GetHashCode().ToString("x");
            var context = HttpContext.Current.Items[objectContextKey];

            if (!Equals(context, null))
            {
                HttpContext.Current.Items.Remove(objectContextKey);
            }
        }

        //Set Global Logging Settings.
        private void SetGlobalLoggingSetting()
        {
            //Store Global Default Logging Setting in cache
            if (HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache] == null)
                DefaultGlobalConfigSettingHelper.DefaultLoggingConfigSettingCache();
        }
    }
}
