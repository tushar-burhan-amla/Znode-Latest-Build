using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeAdminSettings
    {
        private static NameValueCollection settings = ConfigurationManager.AppSettings;

        public static void SetConfigurationSettingSource(NameValueCollection settingSource)
        {
            settings = settingSource;
        }

        public static string ZnodeApiRootUri
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiRootUri"]);
            }
        }
        public static string ZnodeAdminUri
        {
            get
            {
                return Convert.ToString(settings["ZnodeAdminUri"]);
            }
        }
        public static string ZnodeApiUriItemSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriItemSeparator"]);
            }
        }
        public static string CookieExpiresValue
        {
            get
            {
                return Convert.ToString(settings["CookieExpiresValue"]);
            }
        }
        public static string CookieExpiresValueForFilter
        {
            get
            {
                return Convert.ToString(settings["CookieExpiresValue"]);
            }
        }
        public static string ZnodeApiUriKeyValueSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriKeyValueSeparator"]);
            }
        }
        public static string MediaFilePath
        {
            get
            {
                return Convert.ToString(settings["MediaFilePath"]);
            }
        }
        public static string FolderJsonPath
        {
            get
            {
                return Convert.ToString(settings["FolderJsonPath"]);
            }
        }
        public static string GridPagingStartValue
        {
            get
            {
                return Convert.ToString(settings["GridPagingStartValue"]);
            }

        }


        public static string SessionWarningTime
        {
            get
            {
                return Convert.ToString(settings["SessionWarningTime"]);
            }
        }
        public static string MediaUploadFolderName
        {
            get
            {
                return Convert.ToString(settings["MediaUploadFolderName"]);
            }
        }


        public static string PaymentApplicationUrl
        {
            get
            {
                return Convert.ToString(settings["PaymentApplicationUrl"]);
            }
        }

        public static string DefaultImagePath
        {
            get
            {
                return Convert.ToString(settings["DefaultImagePath"]);
            }
        }

        public static string DefaultUserImagePath
        {
            get
            {
                return Convert.ToString(settings["DefaultUserImagePath"]);
            }
        }
        
        public static string MaxFileSize
        {
            get
            {
                return Convert.ToString(settings["MaxFileSize"]);
            }
        }

        public static string MaxSizeForVideo
        {
            get
            {
                return Convert.ToString(settings["MaxSizeForVideo"]);
            }
        }

        public static string NotificationMessagesIsFadeOut
        {
            get
            {
                return Convert.ToString(settings["NotificationMessagesIsFadeOut"]);
            }
        }

        public static string AccessType
        {
            get
            {
                return Convert.ToString(settings["AccessType"]);
            }
        }
        public static string ZnodeReportFolderName
        {
            get
            {
                return Convert.ToString(settings["ZnodeReportFolderName"]);
            }
        }

        public static string ReportServerDynamicReportFolderName
        {
            get
            {
                return Convert.ToString(settings["ReportServerDynamicReportFolderName"]);
            }
        }

        public static bool IsDebugMode
        {
            get
            {
                return Convert.ToBoolean(settings["IsDebugMode"]);
            }
        }

        public static bool EnableScriptOptimizations
        {
            get
            {
                return Convert.ToBoolean(settings["EnableScriptOptimizations"]);
            }
        }
        public static string ProductUpdateSampleCSVPath
        {
            get
            {
                return Convert.ToString(settings["ProductUpdateSampleCSVPath"]);
            }
        }

        public static int USPSWebRequestTimeOutMs
        {
            get
            {
                int uspsWebRequestTimeOut = 0;
                //Default 30 seconds if key is not available in config file.
                if (!(int.TryParse(settings["USPSWebRequestTimeOutMs"], out uspsWebRequestTimeOut)))
                    uspsWebRequestTimeOut = 30000;

                return uspsWebRequestTimeOut;
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

        public static bool EnableCMSPreview
        {
            get
            {
                return Convert.ToBoolean(settings["IsEnableCMSPreview"]);
            }
        }

       /// <summary>
        /// Product export chunk size setting present in web.config file (Key Name: ProductExportChunkSize).
        /// If key not added in web.config then 50 will be the chunk size
        /// </summary>
        public static int ProductExportChunkSize
        {
            get
            {
                int productExportChunkSize = 0;
                //Default 50 if key is not available in config file.
                if (!(int.TryParse(settings["ProductExportChunkSize"], out productExportChunkSize)))
                    productExportChunkSize = 50;

                return productExportChunkSize;

            }
        }

        /// <summary>
        /// Enable or disable route based on web.config value.
        /// </summary>
        public static bool DisableRoutesForStaticFile
        {
            get
            {
                string disableRoutesForStaticFile = settings["DisableRoutesForStaticFile"];
                if (string.IsNullOrEmpty(disableRoutesForStaticFile))
                    return false;

                return Convert.ToBoolean(disableRoutesForStaticFile);
            }
        }

        public static string GlobalSettingCacheDuration
        {
            get
            {
                return Convert.ToString(settings["GlobalSettingCacheDuration"]);
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

        public static int GenericParserImportMaxBufferSize
        {
            get
            {
                int var = 0;
                int genericParserImportMaxBufferSize = Convert.ToInt32(settings["GenericParserImportMaxBufferSize"]);

                /* If the key is not present in web.config or the key value is empty or zero
                 * Return default value i.e. 4096 */
                if (genericParserImportMaxBufferSize == 0)
                    return 4096;

                return genericParserImportMaxBufferSize;
            }
        }
        public static string ImportFileUploadLocation
        {
            get
            {
                return Convert.ToString(settings["ImportFileUploadLocation"]);
            }
        }
    }
}
