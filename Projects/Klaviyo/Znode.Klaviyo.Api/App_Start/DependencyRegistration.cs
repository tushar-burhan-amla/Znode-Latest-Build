using Autofac;
using System;
using Znode.Engine.Klaviyo.Services;
using Znode.Libraries.Framework.Business;
using Autofac.Integration.WebApi;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.klaviyo.Cache;

namespace Znode.Libraries.Klaviyo
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            ////Register Assemblies Includes the API Controller.
            builder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());
            builder.RegisterType<KlaviyoService>().As<IKlaviyoService>().InstancePerLifetimeScope();
            builder.RegisterType<ZnodeklaviyoService>().As<IZnodeklaviyoService>().InstancePerRequest();
            builder.RegisterType<ConnectionHelper>().As<IConnectionHelper>().InstancePerDependency();
            builder.RegisterType<KlaviyoCache>().As<IKlaviyoCache>().InstancePerLifetimeScope();

        }

        public int Order
        {
            get { return 1; }
        }
    }
}
