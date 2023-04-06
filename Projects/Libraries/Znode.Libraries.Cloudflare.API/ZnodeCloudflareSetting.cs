using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Znode.Libraries.Cloudflare.API
{
    public static class ZnodeCloudflareSetting
    {
        private static NameValueCollection settings = ConfigurationManager.GetSection("CloudflareConfigSection") as NameValueCollection;

        public static void SetConfigurationSettingSource(NameValueCollection settingSource)
        {
            settings = settingSource;
        }

        /// <summary>
        /// Get cloudflare URL from web.config.
        /// </summary>
        public static string CloudflareUrl
        {
            get
            {
                return Convert.ToString(settings["CloudflareUrl"]);
            }
        }

        /// <summary>
        /// Get cloudflare API key from web.config.
        /// </summary>
        public static string CloudflareApiKey
        {
            get
            {
                return Convert.ToString(settings["CloudflareApiKey"]);
            }
        }

        /// <summary>
        /// Get cloudflare email account from web.config.
        /// </summary>
        public static string CloudflareEmailAccount
        {
            get
            {
                return Convert.ToString(settings["CloudflareEmailAccount"]);
            }
        }
    }
}
