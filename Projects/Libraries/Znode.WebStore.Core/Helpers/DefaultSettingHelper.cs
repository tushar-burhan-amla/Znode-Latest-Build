using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.WebStore
{
    public static class DefaultSettingHelper
    {
        #region Private Variables
        private static readonly IDefaultGlobalConfigClient _defaultGlobalConfigClient = new DefaultGlobalConfigClient();
        #endregion

        #region Public Method

        //Get date fromat which is set in global setting.
        public static string DefaultDateFormat
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString()).FeatureValues;
            }
        }

        //Get time fromat which is set in global setting.
        public static string DefaultTimeFormat
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeFormat.ToString()).FeatureValues;
            }
        }

        //Get time zone which is set in global setting.
        public static string DefaultTimeZone
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.TimeZone.ToString()).FeatureValues;
            }
        }

        //Get display units which is set in global setting.
        public static string DefaultDisplayUnit
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.DisplayUnit.ToString()).FeatureValues; }
        }

        //Get locale which is set in global setting.
        public static string DefaultLocale
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Locale.ToString()).FeatureValues; }
        }

        //Get weight units which is set in global setting.
        public static string DefaultWeightUnit
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.WeightUnit.ToString()).FeatureValues; }
        }

        //Get currency which is set in global setting.
        public static string DefaultCurrency
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Currency.ToString()).FeatureValues; }
        }

        public static string DefaultCulture
        {
            get { return GetDefaultGlobalSettingData(GlobalSettingEnum.Culture.ToString()).FeatureValues; }
        }

        public static GlobalSettingDetail DefaultDateFormatSetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.DateFormat.ToString());
            }
        }

        //Returns the Default Global Setting Values based on the key.
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

        public static GlobalSettingDetail DefaultCurrencySetting
        {
            get
            {
                return GetDefaultGlobalSettingData(GlobalSettingEnum.Currency.ToString());
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
                return GetCurrentEnvironment();
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

        public static string CurrentPublishState
        {
            get { return GetPublishState().ToString(); }
        }

        //Get the social login providers.
        public static SocialViewModel GetSocialLoginProviders()
        {
            try
            {
                IWebstoreHelper helper = GetService<IWebstoreHelper>();
                SocialModel model = helper.GetLoginProviders();
                if (model?.SocialDomainList?.Count > 0)
                {
                    List<SocialDomainViewModel> SocialDomainList = new List<SocialDomainViewModel>();
                    foreach (SocialDomainModel domainModel in model.SocialDomainList)
                    {
                        if (HelperUtility.IsNotNull(domainModel))
                        {
                            //Maps the social type model to social type view model.
                            var socialTypeViewModelList = domainModel.SocialTypeList.Select(x => new SocialTypeViewModel() { Key = x.Key, ProviderName = x.ProviderName, SecretKey = x.SecretKey }).ToList();
                            //Maps the data into domain view model.
                            SocialDomainList.Add(new SocialDomainViewModel { DomainId = domainModel.DomainId, DomainName = domainModel.DomainName, PortalId = domainModel.PortalId, SocialTypeList = socialTypeViewModelList });
                        }
                    }
                    return new SocialViewModel { SocialDomainList = SocialDomainList };
                }
                return null;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
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

        /// <summary>
        /// Returns the Default Global Setting Values based on the key.
        /// </summary>
        /// <param name="key">Key in Global settings</param>
        /// <returns>Returns the Default Global Setting Values.</returns>
        public static GlobalSettingDetail GetDefaultGlobalSettingData(string key)
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

        //Get default global setting from cache.
        public static DefaultGlobalConfigListModel GetGlobalConfigSettingDetails()
        {
            DefaultGlobalConfigListModel data =
                Helper.GetFromCache<DefaultGlobalConfigListModel>(CachedKeys.DefaultGlobalConfigCache);
            if (HelperUtility.IsNull(data))
            {
                data = _defaultGlobalConfigClient.GetDefaultGlobalConfigList();

                if (data?.DefaultGlobalConfigs?.Count > 0)
                {
                    Helper.AddIntoCache(data, CachedKeys.DefaultGlobalConfigCache, "CurrentPortalCacheDuration");
                }
            }

            return Helper.GetFromCache<DefaultGlobalConfigListModel>(WebStoreConstants.DefaultGlobalSettingCacheKey);
        }

        //Re initialize global setting data.
        public static void ReInitializeGlobalSettingData()
        {
            //remove the data from cache.
            Helper.ClearCache(WebStoreConstants.DefaultGlobalSettingCacheKey);
            GetGlobalConfigSettingDetails();
        }
        #endregion

        #region Private Method

        private static ZnodePublishStatesEnum GetPublishState()
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            return helper.GetCurrentPublishState();
        }



        public static LocaleListModel GetActiveLocaleList()
        {
            LocaleListModel activeLocales = null;

            //ToDo: Need to correct with proper solution for NULL session
            if (SessionHelper.IsSessionObjectPresent())
            {
                //Incase Session is null then call the API to fill the Session.           
                if (Equals(SessionHelper.GetDataFromSession<LocaleListModel>(WebStoreConstants.ActiveLocale), null))
                {
                    activeLocales = GetLocaleList();
                    SessionHelper.SaveDataInSession<LocaleListModel>(WebStoreConstants.ActiveLocale, activeLocales);
                }
                else
                {
                    //Set the list of Locales to Session Variable.                    
                    activeLocales = SessionHelper.GetDataFromSession<LocaleListModel>(WebStoreConstants.ActiveLocale);
                }
            }
            else
            {
                activeLocales = GetLocaleList();
            }
            return activeLocales;
        }

        private static LocaleListModel GetLocaleList()
        {
            LocaleListModel activeLocales = null;
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            FilterCollection filters = new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") };
            activeLocales = helper.LocaleClient().GetLocaleList(null, filters, null, null, null);

            return activeLocales;
        }

        private static string GetCurrentEnvironment()
        {
            string currentEnvironment = string.Empty;
            currentEnvironment = Helper.GetFromCache<DefaultGlobalConfigListModel>("DefaultGlobalConfigCache")?.DefaultGlobalConfigs?.Where(x => x.FeatureName == GlobalSettingEnum.CurrentEnvironment.ToString()).FirstOrDefault()?.FeatureValues;
            if (string.IsNullOrEmpty(currentEnvironment))
            {
                currentEnvironment = GetDefaultGlobalSettingData(GlobalSettingEnum.CurrentEnvironment.ToString()).FeatureValues;
            }
            return currentEnvironment;

        }

        #endregion

    }
}
