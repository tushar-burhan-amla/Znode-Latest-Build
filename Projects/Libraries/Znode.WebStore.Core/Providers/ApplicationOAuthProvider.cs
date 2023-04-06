using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private AuthenticationProperties _authGrantTypeProperty;

        #region Validate Client authentication override method

        /// <summary>
        /// Validate Client authentication override method
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>Returns validation of client authentication</returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);
            _authGrantTypeProperty = GetGrantType(context.Parameters);
            if (GlobalAttributeHelper.IsEnableTradeCentric() && clientId.Contains(ZnodeWebstoreSettings.TradeCentricAccessKeyword, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Validated(clientId);
            }
            return base.ValidateClientAuthentication(context);
        }


        // Setting user authentication.
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            var ticket = new AuthenticationTicket(oAuthIdentity, _authGrantTypeProperty ?? new AuthenticationProperties());
            context.Validated(ticket);
            return base.GrantClientCredentials(context);
        }
        #endregion

        private AuthenticationProperties GetGrantType(IReadableStringCollection parameters)
        {
            string typeName = "grant_type";
            IList<string> values = parameters.GetValues(typeName);
            if (values?.Count > 0)
            {
                IDictionary<string, string> grantProperty = new Dictionary<string, string>
                {
                    { typeName, values[0] }
                };
                return new AuthenticationProperties(grantProperty);
            }
            return new AuthenticationProperties();
        }
    }
}