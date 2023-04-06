using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Znode.Engine.WebStore.Startup))]
namespace Znode.Engine.WebStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
