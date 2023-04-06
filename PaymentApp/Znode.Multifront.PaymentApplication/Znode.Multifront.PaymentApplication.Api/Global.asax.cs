using Swashbuckle.Application;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MongoDB.Bson;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Api.Helpers;
using System.Diagnostics;

namespace Znode.Multifront.PaymentApplication.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        AuthorizationHelper validateAuthorization = new AuthorizationHelper();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            if (ConfigurationManager.AppSettings[PaymentConstants.EnableSwagger].Equals("true", System.StringComparison.OrdinalIgnoreCase))
            {
                string buildVersion = !string.IsNullOrEmpty(ConfigurationManager.AppSettings[PaymentConstants.SwaggerBuildVersion]) ? $"Znode {ConfigurationManager.AppSettings[PaymentConstants.SwaggerBuildVersion]}" : "Znode";
                GlobalConfiguration.Configuration.EnableSwagger(c =>
                {
                    c.SingleApiVersion(buildVersion, "Znode Payment Web API");
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.ResolveConflictingActions(x => x.First());
                }).EnableSwaggerUi(c => { c.DisableValidator(); });
            }
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Execute();
            // sets the default representation to be used in serialization of Guids to Standard
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            EnsureStartupCompleted();
        }

        protected void Application_BeginRequest()
        {
            bool isValidRequest = ValidateRequest();

            if (!isValidRequest)
            {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
                return;
            }
            HelperMethods.ReplaceProxyToClientIp();
            HttpContext.Current.Response.Headers.Remove("Access-Control-Allow-Origin");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", GetDefaultCORSDomain());
            // For every web request the value will updated
            HttpContext.Current.Response.AddHeader("Vary", "Origin");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                AddResponseHeader();
            }
        }

        private bool ValidateRequest()
        {
            if (validateAuthorization.HasValidPrivateKey())
                return true;

            if (ConfigurationManager.AppSettings[PaymentConstants.EnableSwagger].Equals("true", System.StringComparison.OrdinalIgnoreCase))
            {
                if (Request.Url.AbsolutePath.ToLower().Contains("swagger"))
                {
                    return true;
                }
            }

            bool status = AuthenticationSkipUrlValidation();

            if (status)
                return status;

            if (ValidateReferrer() && ValidateUserAgent())
            {
                return true;
            }

            return false;
        }

        // Below urls will be skipped for authentication.
        private bool AuthenticationSkipUrlValidation()
        {
            string[] urlPathArray = { "/home", "/content", "/bundles", "/activate" };

            if (Request.Url.AbsolutePath.Equals("/"))
                return true;

            return urlPathArray.Any(Request.Url.AbsolutePath.ToLower().Contains);
        }

            private bool ValidateReferrer()
        {
            bool isValidReferrer = false;

            var uriReferrer = HttpContext.Current.Request.Headers.Get("Referer");

            if (string.IsNullOrEmpty(uriReferrer))
            {
                return false;
            }

            string corsDomains = Convert.ToString(ConfigurationManager.AppSettings[PaymentConstants.CORS_Domains]);

            if (string.IsNullOrEmpty(corsDomains))
            {
                //Logging.LogMessage($"Error : 'CORS_Domains' key missing in the web.config file.");
                return false;
            }

            string[] corsDomainList = corsDomains.Split(',');

            foreach (var domain in corsDomainList)
            {
                if (uriReferrer.ToLower().Contains(domain.ToLower()))
                {
                    isValidReferrer = true;
                    break;
                }
            }

            if (!isValidReferrer)
            {
                Logging.LogMessage($"Invalid Referer : " + uriReferrer + " is not allowed to access the payment application.", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return isValidReferrer;
        }

        private bool ValidateUserAgent()
        {
            var userAgent = HttpContext.Current.Request.Headers.Get("User-Agent");
            return !string.IsNullOrEmpty(userAgent);
        }

        protected void AddResponseHeader()
        {
            HttpContext.Current.Response.Headers.Remove("Access-Control-Allow-Methods");
            HttpContext.Current.Response.Headers.Remove("Access-Control-Allow-Headers");
            HttpContext.Current.Response.Headers.Remove("Vary");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Authorization-Token, X-HTTP-Method-Override,Authorization");
            HttpContext.Current.Response.End();
        }

        protected string GetDefaultCORSDomain()
        {
            if (validateAuthorization.HasValidPrivateKey())
                return "*";

            var uriOrigin = HttpContext.Current.Request.Headers.Get("Origin");

            if (string.IsNullOrEmpty(uriOrigin))
            {
                //Logging.LogMessage($"Error : 'Origin' missing in the header.");
                return Request.Url.Scheme + "://" + Request.Url.Authority;
            }

            string corsDomains = Convert.ToString(ConfigurationManager.AppSettings[PaymentConstants.CORS_Domains]);

            if (string.IsNullOrEmpty(corsDomains))
            {
                //Logging.LogMessage($"Error : 'CORS_Domains' key missing in the web.config file.");
                return Request.Url.Scheme + "://" + Request.Url.Authority;
            }

            string[] corsDomainList = corsDomains.Split(',');

            foreach (var domain in corsDomainList)
            {
                if (domain.ToLower().Contains(uriOrigin.ToLower()))
                    return uriOrigin;
            }

            Logging.LogMessage($"Invalid access : " + uriOrigin + " is not allowed to access the payment application.", Logging.Components.Payment.ToString(), TraceLevel.Error);
            return Request.Url.Scheme + "://" + Request.Url.Authority;
        }

        private void EnsureStartupCompleted()
        {
            string corsDomains = Convert.ToString(ConfigurationManager.AppSettings[PaymentConstants.CORS_Domains]);
            var privateKey = ConfigurationManager.AppSettings[PaymentConstants.ZnodePrivateKey];

            if (string.IsNullOrEmpty(corsDomains))
                Logging.LogMessage($"Error : 'CORS_Domains' key missing in the web.config file.", Logging.Components.Payment.ToString(), TraceLevel.Error);

            if (string.IsNullOrEmpty(privateKey))
                Logging.LogMessage($"Error : 'privateKey' key missing in the web.config file.", Logging.Components.Payment.ToString(), TraceLevel.Error);
        }

        protected static string GetXmlCommentsPath()
            => $@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\Znode.Multifront.PaymentApplication.Api.XML";
    }
}
