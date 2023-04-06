using System.Configuration;

namespace Znode.Engine.WebStore
{
    /// <summary>
    /// Cache Duration Section
    /// </summary>
    public class CacheDurationSection : ConfigurationSection
    {
        #region CacheDurationSettings
        [ConfigurationProperty("CacheDurationSettings", IsDefaultCollection = true)]
        public CacheDurationSettingsCollection CacheDurationSettings
        {
            get { return (CacheDurationSettingsCollection)this["CacheDurationSettings"]; }
            set { this["CacheDurationSettings"] = value; }
        }
        #endregion
    }
}