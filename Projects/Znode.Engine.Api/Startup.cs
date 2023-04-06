using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;

using Hangfire;
using Hangfire.Dashboard;

using Microsoft.Owin;

using Owin;

using Swashbuckle.Application;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

[assembly: OwinStartup(typeof(Znode.Engine.Api.Startup))]

namespace Znode.Engine.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Register the Dependencies for the API
            var container = StartUpTasks.RegisterDependencies();

            ConfigureAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            if (ConfigurationManager.AppSettings["EnableSwagger"].Equals("true", System.StringComparison.OrdinalIgnoreCase))
            {
                string buildVersion = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SwaggerBuildVersion"]) ? $"Znode {ConfigurationManager.AppSettings["SwaggerBuildVersion"]}" : "Znode";
                config.EnableSwagger(c =>
                {
                    c.SingleApiVersion(buildVersion, Api_Resources.APITitle);
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.SchemaId(x => x.FullName);
                    c.ResolveConflictingActions(x => x.First());
                }).EnableSwaggerUi(c => { c.DisableValidator(); });
            }

            // Create an assign a dependency resolver for Web API to use.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

            app.UseHangfireAspNet(GetHangfireServers);

            if (ZnodeHangfireSettings.EnableHangfireDashboard)
            {
                app.UseHangfireDashboard(ZnodeConstant.HangfireDashboardEndpoint, new DashboardOptions
                {
                    AppPath = null,
                    StatsPollingInterval = ZnodeHangfireSettings.HangfireStatsPollingInterval,
                    IsReadOnlyFunc = ((DashboardContext _) => ZnodeHangfireSettings.MakeHangfireDashboardReadOnly),
                    Authorization = new IDashboardAuthorizationFilter[]
                        {
                            new BasicAuthAuthorizationFilter(
                                new BasicAuthAuthorizationFilterOptions
                                {
                                    LoginCaseSensitive = true,
                                    Users = GetAuthorizedUserCredentials()
                                })
                        }
                });
            }
        }

        private BasicAuthAuthorizationUser[] GetAuthorizedUserCredentials()
        {
            List<BasicAuthAuthorizationUser> credentials = new List<BasicAuthAuthorizationUser>();
            try
            {
                Dictionary<string, string> credsFromConfig = ZnodeHangfireSettings.HangfireDashboardCredentials;

                foreach (var credPair in credsFromConfig)
                {
                    credentials.Add(new BasicAuthAuthorizationUser()
                    {
                        Login = credPair.Key,
                        PasswordClear = credPair.Value
                    });
                }
                return credentials.ToArray();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Hangfire", System.Diagnostics.TraceLevel.Info);
                return credentials.ToArray();
            }


        }

        protected static string GetXmlCommentsPath()
        {
            return string.Format(@"{0}\bin\Znode.Api.Core.XML",
                    AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
