using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Znode.Engine.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //Create a single instance of db context per request.
            app.CreatePerOwinContext(Znode.Engine.Services.ApplicationDbContext.Create);

            //Create a single instance of UserManager per request.
            app.CreatePerOwinContext<Znode.Engine.Services.ApplicationUserManager>(Znode.Engine.Services.ApplicationUserManager.Create);

            //Create a single instance of SignInManager per request.
            app.CreatePerOwinContext<Znode.Engine.Services.ApplicationSignInManager>(Znode.Engine.Services.ApplicationSignInManager.Create);

            //Create a single instance of SignInManager per request.
            app.CreatePerOwinContext<Znode.Engine.Services.ApplicationRoleManager>(Znode.Engine.Services.ApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider            
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
        }
    }
}
