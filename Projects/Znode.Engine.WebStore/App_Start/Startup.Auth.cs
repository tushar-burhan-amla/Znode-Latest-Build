using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using Owin.Security.Providers.Amazon;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Znode.Engine.WebStore.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public partial class Startup
    {       
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "200509957420-qquqb6c3enhkbudgjf28dt4hmd8bsvp8.apps.googleusercontent.com",
            //    ClientSecret = "-uHCP3dnIjVDqrMpjmeukwaJ"
            //});

            app.UseOAuthBearerToken();

            var model = DefaultSettingHelper.GetSocialLoginProviders();
            if (model?.SocialDomainList?.Count > 0)
            {
                foreach (var domain in model.SocialDomainList)
                {
                    app.MapWhen(ctx => ctx.Request.Headers.Get("Host").Equals(domain.DomainName), app2 =>
                    {
                        foreach (var item in domain.SocialTypeList)
                        {
                            if (!string.IsNullOrEmpty(item.ProviderName))
                            {
                                switch (item.ProviderName.ToLower())
                                {
                                    case ZnodeConstant.Facebook:
                                        FacebookAuthenticationOptions facebookOptions = new FacebookAuthenticationOptions()
                                        {
                                            AppId = item.Key,
                                            AppSecret = item.SecretKey,
                                            Provider = new FacebookAuthenticationProvider()
                                            {
                                                OnAuthenticated = (context) =>
                                                {
                                                    context.Identity.AddClaim(new Claim(ZnodeConstant.FacebookAccessToken, context.AccessToken, "XmlSchemaString", "Facebook"));
                                                    return Task.FromResult(0);

                                                }
                                            }
                                        };
                                        //Way to specify additional scopes
                                        facebookOptions.Scope.Add("email");
                                        app2.UseFacebookAuthentication(facebookOptions);
                                        break;
                                    case ZnodeConstant.Google:
                                        app2.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
                                        {
                                            ClientId = item.Key,
                                            ClientSecret = item.SecretKey
                                        });
                                        break;
                                    case ZnodeConstant.Amazon:
                                        var amazonOptions = new AmazonAuthenticationOptions
                                        {
                                            ClientId = item.Key,
                                            ClientSecret = item.SecretKey,
                                            Provider = new AmazonAuthenticationProvider()
                                            {
                                                OnAuthenticated = (context) =>
                                                {
                                                    context.Identity.AddClaim(new Claim(ZnodeConstant.Amazon, context.AccessToken, "XmlSchemaString", "Amazon"));
                                                    return Task.FromResult(0);
                                                }
                                            }
                                        };
                                        app2.UseAmazonAuthentication(amazonOptions);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    });
                }

            }
        }
    }
}