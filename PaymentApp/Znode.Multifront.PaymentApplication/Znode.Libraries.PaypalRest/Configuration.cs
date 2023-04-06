using PayPal.Api;
using System.Collections.Generic;
using System.Configuration;

namespace Znode.Libraries.PaypalRest
{
    /// <summary>
    /// This is the class used for the Configuration settings.
    /// </summary>
    public static class Configuration
    {
        #region read only keys
        //these variables will store the clientID and clientSecret
        //by reading them from the web.config
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        #endregion

        #region Constructor
        static Configuration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method will get the properties from the web.config
        /// </summary>
        /// <returns>returns the key value pair of all configurations.</returns>
        public static Dictionary<string, string> GetConfig()
        {
            var defaultConfig = new Dictionary<string, string>();
            // Default connection timeout in milliseconds
            defaultConfig[BaseConstants.HttpConnectionTimeoutConfig] = ConfigurationManager.AppSettings["PaypalConnectionTimeout"];
            defaultConfig[BaseConstants.HttpConnectionRetryConfig] = "3";
            defaultConfig[BaseConstants.ClientId] = ConfigurationManager.AppSettings["PaypalClientId"];
            defaultConfig[BaseConstants.ClientSecret] = ConfigurationManager.AppSettings["PaypalClientSecret"];

            defaultConfig["mode"] = ConfigurationManager.AppSettings["PaypalMode"];
            return defaultConfig;
        }

        /// <summary>
        /// This method will set the API Context.
        /// </summary>
        /// <returns>returns the API context</returns>
        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();            
            return apiContext;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method will create the access token
        /// </summary>
        /// <returns></returns>
        private static string GetAccessToken() => new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
        #endregion
    }
}