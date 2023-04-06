using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeWebstoreSettings
    {
        private static NameValueCollection settings = ConfigurationManager.AppSettings;
        private static NameValueCollection section = ConfigurationManager.GetSection("TradeCentricConfigSection") as NameValueCollection;

        public static void SetConfigurationSettingSource(NameValueCollection settingSource)
        {
            settings = settingSource;
            section = settingSource;
        }

        public static string ZnodeApiRootUri
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiRootUri"]);
            }
        }

        public static string PaymentApplicationUrl
        {
            get
            {
                return Convert.ToString(settings["PaymentApplicationUrl"]);
            }
        }

        public static string ZnodeApiUriItemSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriItemSeparator"]);
            }
        }

        public static string ZnodeApiUriKeyValueSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriKeyValueSeparator"]);
            }
        }

        public static string TotalBrandCount
        {
            get
            {
                return Convert.ToString(settings["TotalBrandsCount"]);
            }
        }

        public static string ZnodeWebStoreUri
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority);
            }
        }

        public static string ZnodeWebStoreUriSocialLogin
        {
            get
            {
                return Convert.ToString(settings["ZnodeWebStoreUriSocialLogin"]);
            }
        }

        public static string ZnodeGoogleMapKey
        {
            get
            {
                return Convert.ToString(settings["ZnodeGoogleMapKey"]);
            }
        }

        public static string ZnodeGocoderGoogleAPI
        {
            get
            {
                return Convert.ToString(settings["ZnodeGocoderGoogleAPI"]);
            }
        }

        public static string ZnodeGoogleAPIDomainKey
        {
            get
            {
                return Convert.ToString(settings["ZnodeGoogleAPIDomainKey"]);
            }
        }

        /// <summary>
        /// Disable the default startup page navigation to the portal selection page while executing in debug mode.
        /// NULL/"False" : Navigation to the portal selection page in DEBUG mode over Url:"/Dev/PortalSelection" or [Startup] will be enabled.
        /// "True" : Navigation to the portal selection page on [Startup] will be disabled. Default navigation to "/Home/Index" will be avaialble.
        /// </summary>
        public static string DisablePortalSelection
        {
            get
            {
                return Convert.ToString(settings["DisablePortalSelection"]);
            }
        }
        public static string SEOSlugToSkip
        {
            get
            {
                return Convert.ToString(settings["SEOSlugToSkip"]);
            }
        }

        public static bool RunAllManagedModules
        {
            get
            {
                return Convert.ToBoolean(settings["RunAllManagedModules"]);
            }
        }
        public static bool IsDataSeparationAllowedforAppType
        {
            get
            {
                return Convert.ToBoolean(settings["AllowDataSeparationforApptype"]);
            }
        }
        public static bool EnableTokenBasedAuthorization
        {
            get
            {
                return Convert.ToBoolean(settings["EnableTokenBasedAuthorization"]);
            }
        }

        public static bool IsCalculatePromotionForShippingEstimate
        {
            get
            {
                return Convert.ToBoolean(settings["IsCalculatePromotionForShippingEstimate"]);
            }
        }

        public static string HomeController
        {
            get
            {
                if (string.IsNullOrEmpty(settings["CustomHomePageUrl"]))
                    return "Home";
                string[] controllerActionValue = settings["CustomHomePageUrl"].Split(',');
                return string.IsNullOrEmpty(controllerActionValue[0]) ? "Home" : controllerActionValue[0];
            }
        }
        public static string HomeAction
        {
            get
            {
                if (string.IsNullOrEmpty(settings["CustomHomePageUrl"]))
                    return "Index";
                string[] controllerActionValue = settings["CustomHomePageUrl"].Split(',');
                return string.IsNullOrEmpty(controllerActionValue[1]) ? "Index" : controllerActionValue[1];
            }
        }
        public static string LoginPageUrl
        {
            get
            {
                return string.IsNullOrEmpty(settings["CustomLoginPageUrl"]) ?
                             "/User/Login" : settings["CustomLoginPageUrl"];
            }
        }

        public static bool MinifiedJsonResponse
        {
            get
            {
                return settings["MinifiedJsonResponseFromAPI"].TryParseBoolean();
            }
        }

        public static string EnableEnhancedAddToCartNotificationForThemes
        {
            get
            {
                return Convert.ToString(settings["EnableEnhancedAddToCartNotificationForThemes"]);
            }
        }

        public static string PowerBIAuthorityUrl
        {
            get
            {
                return Convert.ToString(settings["PowerBIAuthorityUrl"]);
            }
        }

        public static string PowerBIResourceUrl
        {
            get
            {
                return Convert.ToString(settings["PowerBIResourceUrl"]);
            }
        }

        public static string PowerBIApiUrl
        {
            get
            {
                return Convert.ToString(settings["PowerBIApiUrl"]);
            }
        }

        public static bool EnableScriptOptimizations
        {
            get
            {
                return Convert.ToBoolean(settings["EnableScriptOptimizations"]);
            }
        }

        public static string TradeCentricAccessKeyword
        {
            get
            {
                return Convert.ToString(section["TradeCentricAccessKeyword"]);
            }
        }

        public static int AccessTokenExpireTimeSpan
        {
            get
            {
                return Convert.ToInt32(section["AccessTokenExpireTimeSpan"]);
            }
        }

        public static int RefreshTokenExpireTimeSpan
        {
            get
            {
                return Convert.ToInt32(section["RefreshTokenExpireTimeSpan"]);
            }
        }

    }
}