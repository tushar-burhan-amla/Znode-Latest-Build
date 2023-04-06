using System.Configuration;

namespace Znode.Libraries.SAML
{
    /// <summary>
    /// SAML setting Section
    /// </summary>
    public class SAMLSettingSection : ConfigurationSection
    {
        #region SAMLSettings
        [ConfigurationProperty("SAMLSettings", IsDefaultCollection = true)]
        public SAMLSettingsCollection SAMLSettings
        {
            get { return (SAMLSettingsCollection)this["SAMLSettings"]; }
            set { this["SAMLSettings"] = value; }
        }
        #endregion
    }
}