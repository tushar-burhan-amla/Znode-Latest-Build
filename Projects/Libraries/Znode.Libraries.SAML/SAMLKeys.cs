using System;
using System.Configuration;


namespace Znode.Libraries.SAML
{
    /// <summary>
    /// SAML Settings
    /// </summary>
    public class SAMLKeys : ConfigurationElement
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
        /// Get SAML Settings defined in web.config file for given key
        /// </summary>
        /// <returns>Return SAML setting </returns>
        public static SAMLKeys GetSettingsByMappingKey(string Key)
        {
            SAMLKeys cacheDurationSettings = null;
            SAMLSettingSection samlSection = ConfigurationManager.GetSection("SAMLSettingSection") as SAMLSettingSection;

            if (!Equals(samlSection, null))
            {
                foreach (SAMLKeys cacheDurationSetting in samlSection.SAMLSettings)
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