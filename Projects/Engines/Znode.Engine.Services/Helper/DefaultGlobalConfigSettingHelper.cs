using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public static class DefaultGlobalConfigSettingHelper
    {
        public static string GetServerTimeZone() => TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName.ToString()).ToString();

        public static string GetStringDateFormat() => GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString()).FeatureValues;

        public static string GetStringTimeFormat() => GetDefaultGlobalSettingData(GlobalSettingEnum.TimeFormat.ToString()).FeatureValues;

        public static string GetStringDateTimeFormat() => GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString()).FeatureValues + " " + GetDefaultGlobalSettingData(GlobalSettingEnum.TimeFormat.ToString()).FeatureValues;


        //Caches all DefaultGlobalConfigSetting in Cache
        public static void DefaultGlobalConfigSettingCache()
        {
            if (Equals(HttpRuntime.Cache[CachedKeys.DefaultGlobalConfigCache], null))
            {
                GetDefaultGlobalConfigSettings();
            }
        }
        //Caches all DefaultLoggingSettting in cache.
        public static void DefaultLoggingConfigSettingCache()
        {
            if (Equals(HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache], null))
            {
                GetDefaultLoggingConfigSettings();
            }
        }

        #region Public Method
        public static DefaultGlobalConfigListModel GetDefaultGlobalConfigSettings()
        {
            IDefaultGlobalConfigService _service = GetService<IDefaultGlobalConfigService>() ?? new DefaultGlobalConfigService();
            DefaultGlobalConfigListModel globalSettingData = _service.GetDefaultGlobalConfigList();
            ZnodeCacheDependencyManager.Insert(CachedKeys.DefaultGlobalConfigCache, globalSettingData, "ZnodeGlobalSetting");
            return globalSettingData;
        }

        public static Dictionary<string, string> GetDefaultLoggingConfigSettings()
        {
            IDefaultGlobalConfigService _service = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigService>() ?? new DefaultGlobalConfigService();
            Dictionary<string, string> globalSettingData = _service.GetLoggingGlobalConfigList();
            ZnodeCacheDependencyManager.Insert(CachedKeys.DefaultLoggingConfigCache, globalSettingData, "ZnodeGlobalSetting");
            return globalSettingData;
        }
        #endregion


        public static string DefaultTimeZone
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeZone.ToString()).FeatureValues;
            }
        }

       
        public static string Locale
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString()).FeatureValues;
            }
        }

        public static bool AllowGlobalLevelUserCreation
        {
            get
            {
                bool isAllowGlobalLevelUserCreation = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.AllowGlobalLevelUserCreation.ToString()).FeatureValues,out isAllowGlobalLevelUserCreation);
                return isAllowGlobalLevelUserCreation;
            }
        }

        public static GlobalSettingDetail DefaultLocaleSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString());
            }
        }
        public static GlobalSettingDetail DefaultTimeZoneSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeZone.ToString());
            }
        }

        public static string DefaultOrderAttribute
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.SaveOrderAttribute.ToString()).FeatureValues;
            }
        }

        public static string DefaultPriceRoundOff
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PriceRoundOff.ToString()).FeatureValues;
            }
        }

        public static string DefaultInventoryRoundOff
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.InventoryRoundOff.ToString()).FeatureValues;
            }
        }

        public static string DefaultDateFormat
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString()).FeatureValues;
            }
        }

        public static string DefaultCountry
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.Country.ToString()).FeatureValues;
            }
        }

        public static string DefaultCartAttribute
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.CartAttribute.ToString()).FeatureValues;
            }
        }
        public static string DefaultGroupIdProductAttribute
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.GroupIdFormat.ToString()).FeatureValues;
            }
        }
        public static GlobalSettingValues DefaultGroupIdPersonalizeAttribute
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.GroupIdFormat.ToString()).FeatureSubValues;
            }
        }

        public static bool IsAllowWithOtherPromotionsAndCoupons
        {
            get
            {
                bool isAllowWithOtherPromotionsAndCoupons = false;
                bool.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.IsAllowWithOtherPromotionsAndCoupons.ToString()).FeatureValues, out isAllowWithOtherPromotionsAndCoupons);
                return isAllowWithOtherPromotionsAndCoupons;
            }
        }

        //This is added for column encryption setting. For security reason we will add this "'IsColumnEncryptionSetting' = 'Enabled'" in connection string.
        public static bool IsColumnEncryptionSettingEnabled
        {
            get
            {
                bool isColumnEncryptionSettingEnabled = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.IsColumnEncryptionSettingEnabled.ToString()).FeatureValues, out isColumnEncryptionSettingEnabled);
                return isColumnEncryptionSettingEnabled;
            }
        }

        private static bool IsClearLoadBalancerAPICacheEnabled
        {
            get
            {
                bool isClearLoadBalancerAPICacheEnabled = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.ClearLoadBalancerAPICacheIPs.ToString()).FeatureValues, out isClearLoadBalancerAPICacheEnabled);
                return isClearLoadBalancerAPICacheEnabled;
            }
        }

        private static bool IsClearLoadBalancerWebStoreCacheEnabled
        {
            get
            {
                bool isClearLoadBalancerWebStoreCacheEnabled = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.ClearLoadBalancerWebStoreCacheIPs.ToString()).FeatureValues, out isClearLoadBalancerWebStoreCacheEnabled);
                return isClearLoadBalancerWebStoreCacheEnabled;
            }
        }


        public static GlobalSettingValues DefaultClearLoadBalancerAPICacheIPs
        {
            get
            {
                return (IsClearLoadBalancerAPICacheEnabled) ? GetDefaultGlobalSettingData(GlobalSettingEnum.ClearLoadBalancerAPICacheIPs.ToString()).FeatureSubValues : null;
            }
        }

        public static GlobalSettingValues DefaultClearLoadBalancerWebStoreCacheIPs
        {
            get
            {
                return (IsClearLoadBalancerWebStoreCacheEnabled) ? GetDefaultGlobalSettingData(GlobalSettingEnum.ClearLoadBalancerWebStoreCacheIPs.ToString()).FeatureSubValues : null;
            }
        }

        public static bool IsClearOnlyHttpsDomainCache
        {
            get
            {
                bool isClearOnlyHttpsDomainCache = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.ClearOnlyHttpsDomainCache.ToString()).FeatureValues, out isClearOnlyHttpsDomainCache);
                return isClearOnlyHttpsDomainCache;
            }
        }

        //To get the limit of number of products to be displayed in recommended products widget from ZnodeGlobalSetting.
        public static int DefaultProductLimitForRecommendations
        {
            get
            {
                int defaultProductLimit = 0;
                int.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.DefaultProductLimitForRecommendations.ToString()).FeatureValues, out defaultProductLimit);
                return defaultProductLimit;
            }
        }

        
        // Get global setting for allowed promotions
        public static string AllowedPromotions
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.AllowedPromotions.ToString()).FeatureValues;
            }
        }

        // Get global flag based setting for calculate Tax after discount.
        public static bool IsCalculateTaxAfterDiscount
        {
            get
            {
                bool isCalculateTaxAfterDiscount = false;
                bool.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.IsCalculateTaxAfterDiscount.ToString()).FeatureValues, out isCalculateTaxAfterDiscount);
                return isCalculateTaxAfterDiscount;
            }
        }


        //Get searchable attribute for cms page search from global setting
        public static string CMSPageSearchableAttributes
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.CMSPageSearchableAttributes.ToString()).FeatureValues;
            }
        }
        public static string PowerBIApplicationId
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBIApplicationId.ToString()).FeatureValues;
            }
        }

        public static string PowerBITenantId
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBITenantId.ToString()).FeatureValues;
            }
        }

        public static string PowerBIGroupId
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBIGroupId.ToString()).FeatureValues;
            }
        }

        public static string PowerBIReportId
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBIReportId.ToString()).FeatureValues;
            }
        }

        public static string PowerBIUserName
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBIUserName.ToString()).FeatureValues;
            }
        }

        public static string PowerBIPassword
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.PowerBIPassword.ToString()).FeatureValues;
            }
        }

        public static string OldOrderIdentifierOrderId
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.OldOrderIdentifierOrderId.ToString()).FeatureValues;
            }
        }

        public static string DefaultLocale
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString()).FeatureValues; }
        }

        // Delete already sent emails for inventory subscription notification settings.
        public static string DeleteAlreadySentEmails
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DeleteAlreadySentEmails.ToString()).FeatureValues;
            }
        }

        // Delete pending emails for inventory subscription notification settings
        public static string DeletePendingEmails
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DeletePendingEmails.ToString()).FeatureValues;
            }
        }

        #region Private Method
        /// <summary>
        /// Returns the Default Global Setting Values based on the key.
        /// </summary>
        /// <param name="key">Key in Global settings</param>
        /// <returns>Returns the Default Global Setting Values.</returns>
        private static GlobalSettingDetail GetDefaultGlobalSettingData(string key)
        {
            GlobalSettingDetail data = new GlobalSettingDetail();

            DefaultGlobalConfigListModel globalSettings = Equals(HttpRuntime.Cache["DefaultGlobalConfigCache"], null)
               ? DefaultGlobalConfigSettingHelper.GetDefaultGlobalConfigSettings()
               : (DefaultGlobalConfigListModel)HttpRuntime.Cache.Get("DefaultGlobalConfigCache");

            if (globalSettings?.DefaultGlobalConfigs.Count > 0)
            {
                int index = globalSettings.DefaultGlobalConfigs.FindIndex(item => Equals(item.FeatureName, key));
                if (index != -1)
                {
                    DefaultGlobalConfigModel model = globalSettings.DefaultGlobalConfigs[index];
                    data.FeatureValues = Convert.ToString(model.FeatureValues);
                    data.FeatureSubValues = GlobalSettingHelper.SetFeatureValue(model.FeatureSubValues);
                }
            }
            return data;
        }

        #endregion

        #region TimeZone
        //Convert Datetime to timezone which is set in global setting.
        public static DateTime ToGlobalTimeZone(this DateTime dateTime)
        {
            return dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrEmpty(DefaultTimeZone) ? "Central Standard Time" : DefaultTimeZone));
        }

        //Convert to Datetime format which is set in global setting.
        public static string ToTimeZoneDateTimeFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringDateTimeFormat()) : null;
        }

        //Convert to Date format which is set in global setting.
        public static string ToTimeZoneDateFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringDateFormat()) : null;
        }

        //Convert to Time format which is set in global setting.
        public static string ToTimeZoneTimeFormat(this DateTime dateTime)
        {
            return (dateTime != null && dateTime != DateTime.MinValue) ? ToGlobalTimeZone(dateTime).ToString(GetStringTimeFormat()) : null;
        }
        #endregion

    }
}