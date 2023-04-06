using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeApiSettings
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
        public static string AdminWebsiteUrl
        {
            get
            {
                return Convert.ToString(settings["AdminWebsiteUrl"]);
            }
        }
        public static string LicensePath
        {
            get
            {
                return Convert.ToString(settings["LicensePath"]);
            }
        }
        public static string EnableFileLogging
        {
            get
            {
                return Convert.ToString(settings["EnableFileLogging"]);
            }
        }
        public static string EnableDBLogging
        {
            get
            {
                return Convert.ToString(settings["EnableDBLogging"]);
            }
        }
        public static string ValidateAuthHeader
        {
            get
            {
                return Convert.ToString(settings["ValidateAuthHeader"]);
            }
        }
        public static string MaxInvalidPasswordAttempts
        {
            get
            {
                return Convert.ToString(settings["MaxInvalidPasswordAttempts"]);
            }
        }
        public static string ResetPasswordLinkExpirationDuration
        {
            get
            {
                return Convert.ToString(settings["ResetPasswordLinkExpirationDuration"]);
            }

        }
        public static string PasswordExpiration
        {
            get
            {
                return Convert.ToString(settings["PasswordExpiration"]);
            }
        }
        public static string FedExGatewayURL
        {
            get
            {
                return Convert.ToString(settings["FedExGatewayURL"]);
            }
        }
        public static string UPSGatewayURL
        {
            get
            {
                return Convert.ToString(settings["UPSGatewayURL"]);
            }
        }
        public static string UPSAddressValidationURL
        {
            get
            {
                return Convert.ToString(settings["UPSAddressValidationURL"]);
            }
        }

        public static string USPSShippingAPIURL
        {
            get
            {
                return Convert.ToString(settings["USPSShippingAPIURL"]);
            }
        }
        public static string USPSWebToolsUserID
        {
            get
            {
                return Convert.ToString(settings["USPSWebToolsUserID"]);
            }
        }
        public static string USPSWeightLimitInLBS
        {
            get
            {
                return Convert.ToString(settings["USPSWeightLimitInLBS"]);
            }
        }
        public static string SiteMapNameSpace
        {
            get
            {
                return Convert.ToString(settings["SiteMapNameSpace"]);
            }
        }
        public static string GoogleProductFeedNameSpace
        {
            get
            {
                return Convert.ToString(settings["GoogleProductFeedNameSpace"]);
            }
        }
        public static string ProductFeedRecordCount
        {
            get
            {
                return Convert.ToInt32(settings["ProductFeedRecordCount"]) == 0 ? "50000" : Convert.ToString(settings["ProductFeedRecordCount"]);
            }
        }
        public static string ZnodeReportFolderName
        {
            get
            {
                return Convert.ToString(settings["ZnodeReportFolderName"]);
            }
        }
        public static string ZnodeApiUriKeyValueSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriKeyValueSeparator"]);
            }
        }
        public static string ZnodeApiUriItemSeparator
        {
            get
            {
                return Convert.ToString(settings["ZnodeApiUriItemSeparator"]);
            }
        }

        public static string ReportServerDynamicReportFolderName
        {
            get
            {
                return Convert.ToString(settings["ReportServerDynamicReportFolderName"]);
            }
        }

        public static string WebstoreWebsiteName
        {
            get
            {
                return Convert.ToString(settings["WebstoreWebsiteName"]);
            }
        }
        public static string UseSynonym
        {
            get
            {
                return Convert.ToString(settings["UseSynonyms"]);
            }
        }
        public static string UseCustomAnalyzer
        {
            get
            {
                return Convert.ToString(settings["UseCustomAnalyzer"]);
            }
        }
        public static string DefaultTokenFilters
        {
            get
            {
                return Convert.ToString(settings["DefaultTokenFilters"]);
            }
        }
        public static string SearchIndexChunkSize
        {
            get
            {
                return Convert.ToString(settings["SearchIndexChunkSize"]);
            }
        }
        public static string RemovableProductAttributes
        {
            get
            {
                return Convert.ToString(settings["RemovableProductAttributes"]);
            }
        }

        public static string CCHTestTransactionMode
        {
            get
            {
                return Convert.ToString(settings["CCHTestTransactionMode"]);
            }
        }

        public static string CCHEntityId
        {
            get
            {
                return Convert.ToString(settings["CCHEntityId"]);
            }
        }

        public static string CCHDivisionId
        {
            get
            {
                return Convert.ToString(settings["CCHDivisionId"]);
            }
        }

        public static string CCHSourceSystem
        {
            get
            {
                return Convert.ToString(settings["CCHSourceSystem"]);
            }
        }

        public static string CCHTransactionDescription
        {
            get
            {
                return Convert.ToString(settings["CCHTransactionDescription"]);
            }
        }
        public static string ProductPublishMaxTime
        {
            get
            {
                return Convert.ToString(settings["ProductPublishMaxTime"]);
            }
        }
        public static string ProductPublishSleepTime
        {
            get
            {
                return Convert.ToString(settings["ProductPublishSleepTime"]);
            }
        }

        public static string AllowedExtention
        {
            get
            {
                return Convert.ToString(settings["AllowedMediaExtention"]);
            }
        }

        public static string CCHWebServiceURL
        {
            get
            {
                return Convert.ToString(settings["CCHWebServiceURL"]);
            }
        }

        public static string CCHTransactionType
        {
            get
            {
                return Convert.ToString(settings["CCHTransactionType"]);
            }
        }

        public static string CCHCustomerType
        {
            get
            {
                return Convert.ToString(settings["CCHCustomerType"]);
            }
        }

        public static string CCHProviderType
        {
            get
            {
                return Convert.ToString(settings["CCHProviderType"]);
            }
        }

        public static string CCHTaxGroup
        {
            get
            {
                return Convert.ToString(settings["CCHTaxGroup"]);
            }
        }

        public static string CCHTaxItem
        {
            get
            {
                return Convert.ToString(settings["CCHTaxItem"]);
            }
        }

        public static string AvaTaxCode
        {
            get
            {
                return Convert.ToString(settings["AvaTaxCode"]);
            }
        }

        public static string AvaTaxClientHeader
        {
            get
            {
                return Convert.ToString(settings["AvaTaxClientHeader"]);
            }
        }

        public static string FlexFieldsToPassInVertex
        {
            get
            {
                return Convert.ToString(settings["FlexFieldsToPassInVertex"]);
            }
        }

        public static string GenerateRuntimeImages
        {
            get
            {
                return Convert.ToString(settings["GenerateRuntimeImages"]);
            }
        }

        public static string ElasticSearchRootUri
        {
            get
            {
                return Convert.ToString(settings["ElasticSearchRootUri"]);
            }
        }

        public static int TokenExpirationTime
        {
            get
            {
                return Convert.ToInt32(settings["TokenExpirationTime"]);
            }
        }

        public static bool EnableTokenBasedAuthorization
        {
            get
            {
                return Convert.ToBoolean(settings["EnableTokenBasedAuthorization"]);
            }
        }

        public static bool EnableBasicAuthorization
        {
            get
            {
                return Convert.ToBoolean(settings["EnableBasicAuthorization"]);
            }
        }

        public static bool CreateIndexAfterPublish
        {
            get
            {
                string createIndexAfterPublish = settings["CreateIndexAfterPublish"];
                if (string.IsNullOrEmpty(createIndexAfterPublish))
                    return true;
                return Convert.ToBoolean(settings["CreateIndexAfterPublish"]);
            }
        }

        public static string ExemptionNo
        {
            get
            {
                return Convert.ToString(settings["ExemptionNo"]);
            }
        }

        public static bool DisableTaxCommit
        {
            get
            {
                return Convert.ToBoolean(settings["DisableTaxCommit"]);
            }
        }


        public static string UseECertificateERPMode
        {
            get
            {
                return Convert.ToString(settings["UseECertificateERPMode"]);
            }
        }


        public static string UPSTrackingURL
        {
            get
            {
                return Convert.ToString(settings["UPSTrackingUrl"]);
            }
        }

        public static string FedExTrackingURL
        {
            get
            {
                return Convert.ToString(settings["FedExTrackingUrl"]);
            }
        }

        public static string USPSTrackingURL
        {
            get
            {
                return Convert.ToString(settings["USPSTrackingUrl"]);
            }
        }

        public static long ImageQualityFactor
        {
            get
            {
                return Convert.ToInt32(settings["ImageQualityFactor"]);
            }
        }
        public static string IsAllowAddOnQuantity
        {
            get
            {
                return Convert.ToString(settings["IsAllowAddOnQuantity"]);
            }
        }

        public static bool IsCookieHttpOnly
        {
            // In case of IsCookieHttpOnly value explicitly set as false in that case IsCookieHttpOnly will return false value 
            // otherwise for all other condition it will return true
            get
            {
                if (!string.IsNullOrEmpty(settings["IsCookieHttpOnly"]))
                    return Convert.ToBoolean(settings["IsCookieHttpOnly"]);
                else
                    return true;
            }
        }
        public static bool IsCookieSecure
        {
            get
            {
                return Convert.ToBoolean(settings["IsCookieSecure"]);
            }
        }
        public static string ProductPublishChunkSize
        {
            get
            {
                return Convert.ToString(settings["ProductPublishChunkSize"]);
            }
        }

        public static string IndexChunkSizeOnCategoryPublish
        {
            get
            {
                return Convert.ToString(settings["IndexChunkSizeOnCategoryPublish"]);
            }
        }

        public static string CustomAssemblyLookupPrefix
        {
            get
            {
                string customAssemblyLookupPrefix = Convert.ToString(settings["CustomAssemblyLookupPrefix"]);
                return string.IsNullOrEmpty(customAssemblyLookupPrefix) ? string.Empty : customAssemblyLookupPrefix;
            }
        }

        public static string CacheControl
        {
            get
            {
                return Convert.ToString(settings["CacheControl"]);
            }
        }
        public static string DefaultCacheControl
        {
            get
            {
                return Convert.ToString(settings["DefaultCacheControl"]);
            }
        }
        public static string ClientId
        {
            get
            {
                return Convert.ToString(settings["ClientId"]);
            }
        }
        public static string ClientSecret
        {
            get
            {
                return Convert.ToString(settings["ClientSecret"]);
            }
        }
        public static string TenanatId
        {
            get
            {
                return Convert.ToString(settings["TenanatId"]);
            }
        }

        public static string SubscriptionId
        {
            get
            {
                return Convert.ToString(settings["SubscriptionId"]);
            }
        }
        public static string ResourceGroups
        {
            get
            {
                return Convert.ToString(settings["ResourceGroups"]);
            }
        }
        public static string Profiles
        {
            get
            {
                return Convert.ToString(settings["Profiles"]);
            }
        }
        public static string EndPoints
        {
            get
            {
                return Convert.ToString(settings["EndPoints"]);
            }
        }
        public static string RequestTimeout
        {
            get
            {
                return Convert.ToString(settings["RequestTimeout"]);
            }
        }
        public static string ZnodeCommaReplacer
        {
            get
            {
                return Convert.ToString(settings["ZnodeCommaReplacer"]);
            }
        }
        public static string BarcodeScannerLicenseKey
        {
            get
            {
                return Convert.ToString(settings["BarcodeScannerLicenseKey"]);
            }
        }
        public static bool EnableBarcodeSpecificSearch
        {
            get
            {
                string enableBarcodeSpecificSearch = settings["EnableBarcodeSpecificSearch"];
                if (string.IsNullOrEmpty(enableBarcodeSpecificSearch))
                    return false;
                return Convert.ToBoolean(settings["EnableBarcodeSpecificSearch"].ToLower());
            }
        }
        public static bool AllowPublishedEntityLogging
        {
            get
            {
                return Convert.ToBoolean(settings["AllowPublishedEntityLogging"]);
            }
        }
        public static int NoOfPublishedVersionsToBeLogged
        {
            get
            {
                return Convert.ToInt32(settings["NoOfPublishedVersionsToBeLogged"]);
            }
        }

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

        public static bool EnableStopWordsForSearchIndex
        {
            get
            {
                return Convert.ToBoolean(settings["EnableStopWordsForSearchIndex"]);
            }
        }
        public static int RecommendationModelChunkSize
        {
            get
            {
                return Convert.ToInt32(settings["RecommendationModelChunkSize"]);
            }
        }

        public static int RecommendationModelTimeLimit
        {
            get
            {
                return Convert.ToInt32(settings["RecommendationModelTimeLimit"]);
            }
        }

        public static int PublishCatalogConnectionTime
        {
            get
            {
                string publishCatalogConnectionTime = settings["PublishCatalogConnectionTime"];

                int _publishCatalogConnectionTime = 0;
                Int32.TryParse(publishCatalogConnectionTime, out _publishCatalogConnectionTime);

                //If the key is not present in web.config or the key value is empty or zero
                if (_publishCatalogConnectionTime == 0)
                    return 30 * 60;

                return _publishCatalogConnectionTime * 60;
            }
        }

        public static int PublishPortalConnectionTime
        {
            get
            {
                string publishPortalConnectionTime = settings["PublishPortalConnectionTime"];

                int _publishPortalConnectionTime = 0;
                Int32.TryParse(publishPortalConnectionTime, out _publishPortalConnectionTime);

                //If the key is not present in web.config or the key value is empty or zero
                if (_publishPortalConnectionTime == 0)
                    return 5 * 60;

                return _publishPortalConnectionTime * 60;
            }
        }

        public static int PurgePublishCatalogConnectionTime
        {
            get
            {
                int purgePublishCatalogConnectionTime = settings["PurgePublishCatalogConnectionTime"].TryParseInt32();

                /* If the key is not present in web.config or the key value is empty or zero
                 * Return default time limit in minutes i.e. 30 minutes */
                if (purgePublishCatalogConnectionTime == 0)
                    return 30 * 60;

                return purgePublishCatalogConnectionTime * 60;
            }
        }

        public static int ElasticCatalogIndexWaitTimeMinute
        {
            get
            {
                int elasticCatalogIndexWaitTimeMinute = settings["ElasticCatalogIndexWaitTimeMinute"].TryParseInt32();

                /* If the key is not present in web.config or the key value is empty or zero
                 * Return default time limit in minutes i.e. 30 minutes */
                if (elasticCatalogIndexWaitTimeMinute == 0)
                    return 30;

                return elasticCatalogIndexWaitTimeMinute;
            }
        }

        public static int PublishProductFetchChunkSize
        {
            get
            {
                int publishProductFetchChunkSize = settings["PublishProductFetchChunkSize"].TryParseInt32();

                /* If the key is not present in web.config or the key value is empty or zero
                 * Return default chunk size to fetch publish product from database entity during 
                 * elasticsearch indexing process i.e. 10000 */
                if (publishProductFetchChunkSize == 0)
                    return 10000;

                return publishProductFetchChunkSize;
            }
        }

        public static bool MinifiedJsonResponse
        {
            get
            {
                return ZnodeHttpHeaders.GetHeaderValue(ZnodeHttpHeaders.Header_MinifiedJsonResponse).TryParseBoolean();
            }
        }

        public static bool EnableSchedulerForVoucherReminderEmail
        {
            get
            {
                return Convert.ToBoolean(settings["EnableSchedulerForVoucherReminderEmail"]);
            }
        }

        public static string SchedulerFrequencyForVoucherReminderEmail
        {
            get
            {
                return Convert.ToString(settings["SchedulerFrequencyForVoucherReminderEmail"]);
            }
        }

        public static string VoucherEmailSchedulerTriggerTime
        {
            get
            {
                return Convert.ToString(settings["VoucherEmailSchedulerTriggerDateTime"]);
            }
        }

        public static int OrderOverDueAmountRoundOffLength
        {
            get
            {
                int orderOverDueAmountRoundOffLength = settings["OrderOverDueAmountRoundOffLength"].TryParseInt32();

                /* If the key is not present in web.config or the key value is empty or zero
                 * Return default split lenght size 7 */ 
                if (orderOverDueAmountRoundOffLength == 0)
                    return 7;

                return orderOverDueAmountRoundOffLength;
            }
        }        
        
        public static string VoucherEmailSchedulerCronExpression
        {
            get
            {
                return Convert.ToString(settings["VoucherEmailSchedulerCronExpression"]) ?? "*/30 * * * *";
            }
        }

        public static int ConnectionTimeout
        {
            get
            {
                string connectionTimeout = settings["PublishCatalogConnectionTime"];

                int _connectionTimeout = 0;
                Int32.TryParse(connectionTimeout, out _connectionTimeout);

                //If the key is not present in web.config or the key value is empty or zero
                //Default time is set to 10 min
                if (_connectionTimeout == 0)
                    return 10 * 60;

                //To get the 25 % value of the actual key value
                _connectionTimeout = Convert.ToInt32(Math.Round((_connectionTimeout * 0.25), MidpointRounding.AwayFromZero));

                return _connectionTimeout * 60;
            }
        }
        public static string AvalaraRestRootUri
        {
            get
            {
                return Convert.ToString(settings["AvalaraRestRootUri"]);
            }
        }

        public static string BuildVersion
        {
            get
            {
                return Convert.ToString(settings["BuildVersion"]);
            }
        }
        
        public static string AvalaraAppName
        {
            get
            {
                return Convert.ToString(settings["AvalaraAppName"]);
            }
        }
        

        #region User registration attempt schedular setting
        public static bool EnableSchedulerForUserRegistrationAttempt
        {
            get
            {
                return Convert.ToBoolean(settings["EnableSchedulerForUserRegistrationAttempt"]);
            }
        }

        public static string UserRegistrationAttemptSchedulerType
        {
            get
            {
                return Convert.ToString(settings["UserRegistrationAttemptSchedulerType"]);
            }
        }

        public static string UserRegistrationAttemptSchedulerTriggerTime
        {
            get
            {
                return Convert.ToString(settings["UserRegistrationAttemptSchedulerTriggerTime"]);
            }
        }

        public static string UserRegistrationAttemptSchedulerCronExpression
        {
            get
            {
                return Convert.ToString(settings["UserRegistrationAttemptSchedulerCronExpression"]) ?? "0 0 0 * * *";
            }
        }
        #endregion

        #region Hanfire keys for clear payment auth token
        public static bool EnableSchedulerForDeletePaymentAuthToken
        {
            get
            {
                return Convert.ToBoolean(settings["EnableSchedulerForDeletePaymentAuthToken"]);
            }
        }

        public static string DeletePaymentAuthTokenSchedulerType
        {
            get
            {
                return Convert.ToString(settings["DeletePaymentAuthTokenSchedulerType"]);
            }
        }

        public static string PaymentTokenSchedularTriggerTime
        {
            get
            {
                return Convert.ToString(settings["DeletePaymentAuthTokenSchedulerTriggerTime"]);
            }
        }
        public static string DeletePaymentAuthTokenSchedulerCronExpression
        {
            get
            {
                return Convert.ToString(settings["DeletePaymentAuthTokenSchedulerCronExpression"]) ?? "*/30 * * * *";
            }
        }
        #endregion

        #region Hanfire keys for Stock Notification.
        public static bool EnableStockNotice
        {
            get
            {
                return Convert.ToBoolean(settings["EnableStockNotice"]);
            }
        }

        public static string StockNoticeSchedulerType
        {
            get
            {
                return Convert.ToString(settings["StockNoticeSchedulerType"]);
            }
        }

        public static string StockNoticeCronExpression
        {
            get
            {
                return Convert.ToString(settings["StockNoticeCronExpression"]) ?? "0 0 0 * * *";
            }
        }
        #endregion
       
        #region Hanfire keys for clear Export Files.
        public static bool EnableSchedulerForDeleteExportFiles
        {
            get
            {
                return Convert.ToBoolean(settings["EnableSchedulerForDeleteExportFiles"]);
            }
        }
        public static string DeleteExportSchedulerType
        {
            get
            {
                return Convert.ToString(settings["DeleteExportSchedulerType"]);
            }
        }
        public static string ExportSchedulerTriggerTime
        {
            get
            {
                return Convert.ToString(settings["DeleteExportSchedulerTriggerTime"]);
            }
        }
        public static string DeleteExportSchedulerCronExpression
        {
            get
            {
                return Convert.ToString(settings["DeleteExportSchedulerCronExpression"]) ?? "*/30 * * * *";
            }
        }
        public static int ExportFileDeletionDuration
        {
            get
            {
                return Convert.ToInt32(settings["ExportFileDeletionDuration"]);
            }
        }
        #endregion

        #region Elastic credentials
        public static string ElasticSearchUsername
        {
            get
            {
                return Convert.ToString(settings["ElasticSearchUsername"]);
            }
        }

        public static string ElasticSearchPassword
        {
            get
            {
                return Convert.ToString(settings["ElasticSearchPassword"]);
            }
        }
        #endregion
    }
}
