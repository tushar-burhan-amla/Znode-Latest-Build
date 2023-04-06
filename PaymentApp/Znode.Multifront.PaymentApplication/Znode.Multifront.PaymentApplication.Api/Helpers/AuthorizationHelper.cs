using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Znode.Multifront.PaymentApplication.Data.Service;

namespace Znode.Multifront.PaymentApplication.Api.Helpers
{
    public class AuthorizationHelper
    {
        #region Authorization Property


        public virtual string AuthTokenHeader
        {
            get
            {
                var athorizationHeader = HttpContext.Current.Request.Headers;
                return athorizationHeader.AllKeys.Contains(PaymentConstants.Authorization) ? athorizationHeader[PaymentConstants.Authorization] : String.Empty;
            }
        }

        public virtual bool IsAjaxCall
        {
            get
            {
                return HasAjaxRequest(); // check authorization
            }
        }

        #endregion

        #region Private Key Verification

        public virtual bool HasValidPrivateKey()
        {
            var privateKeyFromHeader = HttpContext.Current.Request.Headers.Get(PaymentConstants.Znode_PrivateKey);
            var privateKey = ConfigurationManager.AppSettings[PaymentConstants.ZnodePrivateKey];

            if (!string.IsNullOrEmpty(privateKeyFromHeader) && privateKeyFromHeader.Equals(privateKey))
                return true;

            return false;
        }

        #endregion

        #region Auth token verification based on GUID
        // This process is works for loading payment section and card verification.
        // doNotCount is allow us to load payment section without increasing token attempt.
        protected virtual bool HasValidAuthToken(bool doNotCount = false)
        {
            string authToken = AuthTokenHeader;

            if (!string.IsNullOrEmpty(authToken))
            {
                AuthTokenService authTokenService = new AuthTokenService();
                return authTokenService.ValidateAuthToken(authToken, doNotCount);
            }

            return false;
        }

        #endregion


        #region Check valid authorizartion

        public virtual bool HasValidAuthorization( bool DoNotCount = false)
        {
            if (HasValidPrivateKey())
                return true;

            return HasValidAuthToken(DoNotCount);
        }

        #endregion
        protected virtual bool HasAjaxRequest()
        {
            return HttpContext.Current.Request.Headers[PaymentConstants.AjaxHeaderKey] == PaymentConstants.AjaxHeaderValue;
        }
    }
}