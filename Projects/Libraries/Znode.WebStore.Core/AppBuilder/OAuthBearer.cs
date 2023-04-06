using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public static class OAuthBearer
    {
        #region Public /Protected Properties.

        /// <summary>
        /// OAUTH options property.
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        #endregion

        public static void UseOAuthBearerToken(this IAppBuilder app)
        {
            // Configure the application for OAuth based flow.
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/AccessToken"),
                Provider = new ApplicationOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(ZnodeWebstoreSettings.AccessTokenExpireTimeSpan),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
                AllowInsecureHttp = false 

            };
            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
