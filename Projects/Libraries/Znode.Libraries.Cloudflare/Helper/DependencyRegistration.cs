using Autofac;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Cloudflare
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {            
            builder.RegisterType<ZnodeCloudflare>().As<IZnodeCloudflare>().InstancePerRequest();
            builder.RegisterType<DataHelper>().As<DataHelper>().InstancePerRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
