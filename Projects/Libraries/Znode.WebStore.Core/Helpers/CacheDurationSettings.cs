using System;
using System.Configuration;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    /// <summary>
    /// Cache Duration Settings
    /// </summary>
    public class CacheDurationSettings : ConfigurationElement
    {
        [ConfigurationProperty("Key", IsRequired = true, IsKey = true)]
        public String Key
        {
            get { return (String)this["Key"]; }
            set { this["Key"] = value; }
        }
        [ConfigurationProperty("Value")]
        public String Value
        {
            get { return (String)this["Value"]; }
            set { this["Value"] = value; }
        }

        #region CacheDurationSettings HelperMethod
        /// <summary>
        /// Get Cache Duration Settings defined in web.config file for given key
        /// </summary>
        /// <returns>Return Cache Duration </returns>
        public static CacheDurationSettings GetCacheDurationSettingsByMappingKey(string Key)
        {
            CacheDurationSettings cacheDurationSettings = null;
            CacheDurationSection cacheDurationSection = ConfigurationManager.GetSection("CacheDurationSection") as CacheDurationSection;

            if (HelperUtility.IsNotNull(cacheDurationSection))
            {
                foreach (CacheDurationSettings cacheDurationSetting in cacheDurationSection.CacheDurationSettings)
                {
                    if (Equals(cacheDurationSetting.Key, Key))
                    {
                        cacheDurationSettings = cacheDurationSetting;
                        break;
                    }
                }
            }
            return cacheDurationSettings;
        }
        #endregion
    }
}