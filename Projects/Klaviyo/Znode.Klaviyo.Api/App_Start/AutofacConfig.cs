using Autofac;
using Autofac.Integration.WebApi;

using System.Reflection;
using System.Web.Http;

using Znode.Engine.Klaviyo.Services;
using Znode.Libraries.Klaviyo;

namespace Znode.Engine.Api
{
    public static class AutofacConfig
    {
        public static IContainer Container;
        public static void RegisterDependencies(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));

        }
        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ZnodeklaviyoService>().As<IZnodeklaviyoService>().InstancePerRequest();
            builder.RegisterType<ConnectionHelper>().As<IConnectionHelper>().InstancePerDependency();

            builder.RegisterType<KlaviyoService>().As<IKlaviyoService>().InstancePerDependency();
            
            

            //Set the dependency resolver to be Autofac.  
            Container = builder.Build();

            return Container;
        }
    }
}