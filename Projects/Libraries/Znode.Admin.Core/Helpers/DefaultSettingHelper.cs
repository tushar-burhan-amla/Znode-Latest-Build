using System;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Helpers
{
    public static class DefaultSettingHelper
    {
        #region Private Variables
        private static readonly IDefaultGlobalConfigClient _defaultGlobalConfigClient = new DefaultGlobalConfigClient();
        private static readonly ILocaleClient _localeClient = new LocaleClient();
        #endregion

        #region Public Method

        public static string DefaultDateFormat
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString()).FeatureValues;
            }
        }

        public static string DefaultTimeZone
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeZone.ToString()).FeatureValues;
            }
        }
        public static string DefaultTimeFormat
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeFormat.ToString()).FeatureValues;
            }
        }
        public static string DefaultDisplayUnit
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.DisplayUnit.ToString()).FeatureValues; }
        }

        public static string DefaultLocale
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString()).FeatureValues; }
        }

        public static string DefaultWeightUnit
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.WeightUnit.ToString()).FeatureValues; }
        }

        public static string DefaultCurrency
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Currency.ToString()).FeatureValues; }
        }

        public static string DefaultCulture
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Culture.ToString()).FeatureValues; }
        }

        public static string DefaultCountry
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Country.ToString()).FeatureValues; }
        }

        public static GlobalSettingDetail DefaultDateFormatSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString());
            }
        }

        public static GlobalSettingDetail DefaultDisplayUnitSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DisplayUnit.ToString());
            }
        }

        public static GlobalSettingDetail DefaultLocaleSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString());
            }
        }

        public static GlobalSettingDetail DefaultWeightUnitSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.WeightUnit.ToString());
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

        public static string DefaultCurrentEnvironment
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.CurrentEnvironment.ToString()).FeatureValues;
            }
        }

        public static bool AllowGlobalLevelUserCreation
        {
            get
            {
                bool isAllowGlobalLevelUserCreation = false;
                Boolean.TryParse(GetDefaultGlobalSettingData(GlobalSettingEnum.AllowGlobalLevelUserCreation.ToString()).FeatureValues, out isAllowGlobalLevelUserCreation);
                return isAllowGlobalLevelUserCreation;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Returns the Default Global Setting Values based on the key.
        /// </summary>
        /// <param name="key">Key in Global settings</param>
        /// <returns>Returns the Default Global Setting Values.</returns>
        private static GlobalSettingDetail GetDefaultGlobalSettingData(string key)
        {
            GlobalSettingDetail data = new GlobalSettingDetail();

            //Get the Default Global Setting Details.
            DefaultGlobalConfigListModel globalSettings = GetGlobalConfigSettingDetails();

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

        //get global setting details.
        private static DefaultGlobalConfigListModel GetGlobalConfigSettingDetails()
        {
            //Check data is present in cache.
            if (HelperUtility.IsNull(HelperMethods.GetFromCache<DefaultGlobalConfigListModel>(AdminConstants.DefaultGlobalSettingCacheKey)))
            {
                DefaultGlobalConfigListModel globalSettings = _defaultGlobalConfigClient.GetDefaultGlobalConfigList();

                if (globalSettings?.DefaultGlobalConfigs?.Count > 0)
                {
                    HelperMethods.AddIntoCache(globalSettings, AdminConstants.DefaultGlobalSettingCacheKey, ZnodeAdminSettings.GlobalSettingCacheDuration);
                }
            }
            return HelperMethods.GetFromCache<DefaultGlobalConfigListModel>(AdminConstants.DefaultGlobalSettingCacheKey);
        }

        public static LocaleListModel GetActiveLocaleList()
        {
            LocaleListModel activeLocales = SessionHelper.GetDataFromSession<LocaleListModel>(AdminConstants.ActiveLocale);


            //Incase Session is null then call the API to fill the Session.           
            if (Equals(activeLocales, null))
            {
                activeLocales = GetLocaleList();
                SessionHelper.SaveDataInSession<LocaleListModel>(AdminConstants.ActiveLocale, activeLocales);
            }

            return activeLocales;
        }

        private static LocaleListModel GetLocaleList()
        {
            LocaleListModel activeLocales = null;
            FilterCollection filters = new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") };
            activeLocales = _localeClient.GetLocaleList(null, filters, null, null, null);

            return activeLocales;
        }

        //Gets the Data Separation Allowed for ApplicationType.
        public static bool IsDataSeparationAllowedforAppType() => ZnodeAdminSettings.IsDataSeparationAllowedforAppType;

        //Gets the value of Data Separation Allowed for ApplicationType of int type.
        public static int GetCurrentOrDefaultAppType(ZnodePublishStatesEnum currentAppType)
        {
            if (IsDataSeparationAllowedforAppType() && currentAppType > 0)
                return (int)currentAppType;
            else
                return (int)ZnodePublishStatesEnum.PRODUCTION;
        }

        //Gets the value of Data Separation Allowed for ApplicationType of string type.
        public static string GetCurrentOrDefaultAppType(string currentAppType)
        {
            if (IsDataSeparationAllowedforAppType() && !string.IsNullOrEmpty(currentAppType))
                return currentAppType;
            else
                currentAppType = ZnodePublishStatesEnum.PRODUCTION.ToString();
            return currentAppType;
        }
        #endregion
    }
}
