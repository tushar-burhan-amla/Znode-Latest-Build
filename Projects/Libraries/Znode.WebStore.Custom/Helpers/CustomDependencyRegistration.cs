using Autofac;
using Znode.Engine.WebStore.Controllers;
using Znode.Libraries.Framework.Business;
namespace Znode.Engine.WebStore
{
    public class CustomDependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            builder.RegisterType<CustomUserController>().As<UserController>().InstancePerDependency();
        }
        public int Order
        {
            get { return 1; }
        }
    }
}