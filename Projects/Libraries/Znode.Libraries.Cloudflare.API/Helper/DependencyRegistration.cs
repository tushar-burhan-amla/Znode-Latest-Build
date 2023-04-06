using Autofac;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Cloudflare.API
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            builder.RegisterType<CloudflareRequest>().As<ICloudflareRequest>().InstancePerRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}