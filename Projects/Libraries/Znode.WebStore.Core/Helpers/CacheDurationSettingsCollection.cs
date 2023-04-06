using System.Configuration;

namespace Znode.Engine.WebStore
{
    [ConfigurationCollection(typeof(CacheDurationSettings))]
    public class CacheDurationSettingsCollection : ConfigurationElementCollection
    {
        #region ConfigurationElementCollection Methods

        /// <summary>
        /// creates a new System.Configuration.ConfigurationElement.
        /// </summary>
        /// <returns>Return CacheDurationSettings </returns>
        protected override ConfigurationElement CreateNewElement()
        => new CacheDurationSettings();

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden
        /// </summary>
        /// <returns>Return CacheDurationSettings element key</returns>
        protected override object GetElementKey(ConfigurationElement element)
        => ((CacheDurationSettings)element).Key;
        #endregion
    }
}