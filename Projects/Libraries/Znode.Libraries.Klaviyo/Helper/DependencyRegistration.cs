using Autofac;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Klaviyo
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            builder.RegisterType<ZnodeklaviyoService>().As<IZnodeklaviyoService>().InstancePerRequest();
            builder.RegisterType<ConnectionHelper>().As<IConnectionHelper>().InstancePerDependency();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
