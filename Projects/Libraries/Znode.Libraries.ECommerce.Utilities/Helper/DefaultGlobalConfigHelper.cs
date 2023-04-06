using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class DefaultGlobalConfigHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeGlobalSetting> _defaultGlobalConfigRepository;
        #endregion

        #region  Public constructor
        public DefaultGlobalConfigHelper()
        {
            //Initialize instance of repository.
            _defaultGlobalConfigRepository = new ZnodeRepository<ZnodeGlobalSetting>();
        }
        #endregion

        #region Public Method
        // Get global configuration setting value by key name.
        public string GetDefaultGlobalFeatures(string key)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            string featureValues = string.Empty;

            List<ZnodeGlobalSetting> defaultGlobalConfigList = Equals(HttpRuntime.Cache["DefaultGlobalSettingCache"], null)
             ? GetDefaultGlobalConfigSettings()
             : (List<ZnodeGlobalSetting>)HttpRuntime.Cache.Get("DefaultGlobalSettingCache");

            ZnodeLogging.LogMessage("GetDefaultGlobalFeatures list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, defaultGlobalConfigList?.Count());
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (defaultGlobalConfigList?.Count > 0)
            {
                ZnodeGlobalSetting globalConfigData = defaultGlobalConfigList?.FirstOrDefault(x => x.FeatureName == key);
                if (HelperUtility.IsNotNull(globalConfigData))
                {
                    featureValues = Convert.ToString(globalConfigData.FeatureValues);
                }

            }

            return featureValues;
        }
        #endregion

        #region Private Method
        // Get global configuration settings list and add into Cache.
        private List<ZnodeGlobalSetting> GetDefaultGlobalConfigSettings()
        {
            List<ZnodeGlobalSetting> defaultGlobalConfigList = new List<ZnodeGlobalSetting>(_defaultGlobalConfigRepository.Table);
            if (HelperUtility.IsNotNull(defaultGlobalConfigList))
            {
                ZnodeCacheDependencyManager.Insert("DefaultGlobalSettingCache", defaultGlobalConfigList, "ZnodeGlobalSetting");
            }
            return defaultGlobalConfigList;
        }

        #endregion

        // Get global configuration settings value for allowed Promotions.
        public static string AllowedPromotions
        {
            get
            {
                DefaultGlobalConfigHelper defaultGlobalConfigHelper = new DefaultGlobalConfigHelper();
                return defaultGlobalConfigHelper.GetDefaultGlobalFeatures(GlobalSettingEnum.AllowedPromotions.ToString());
            }
        }

        // Get global configuration setting value for CalculateTaxAfterDiscount flag.
        public static bool IsCalculateTaxAfterDiscount
        {
            get
            {
                DefaultGlobalConfigHelper defaultGlobalConfigHelper = new DefaultGlobalConfigHelper();
                bool isCalculateTaxAfterDiscount = false;
                Boolean.TryParse(defaultGlobalConfigHelper.GetDefaultGlobalFeatures(GlobalSettingEnum.IsCalculateTaxAfterDiscount.ToString()), out isCalculateTaxAfterDiscount);
                return isCalculateTaxAfterDiscount;
            }
        }

        // Get configuration setting value for IsAllowWithOtherPromotionsAndCoupons flag.
        public static bool IsAllowWithOtherPromotionsAndCoupons
        {
            get
            {
                DefaultGlobalConfigHelper defaultGlobalConfigHelper = new DefaultGlobalConfigHelper();
                bool isAllowWithOtherPromotionsAndCoupons = false;
                Boolean.TryParse(defaultGlobalConfigHelper.GetDefaultGlobalFeatures(GlobalSettingEnum.IsAllowWithOtherPromotionsAndCoupons.ToString()), out isAllowWithOtherPromotionsAndCoupons);
                return isAllowWithOtherPromotionsAndCoupons;
            }
        }
    }
}
