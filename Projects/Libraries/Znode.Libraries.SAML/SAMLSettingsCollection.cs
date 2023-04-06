using System.Configuration;

namespace Znode.Libraries.SAML
{
    [ConfigurationCollection(typeof(SAMLKeys))]
    public class SAMLSettingsCollection : ConfigurationElementCollection
    {
        #region ConfigurationElementCollection Methods

        /// <summary>
        /// creates a new System.Configuration.ConfigurationElement.
        /// </summary>
        /// <returns>Return SAMLSettings </returns>
        protected override ConfigurationElement CreateNewElement()
        => new SAMLKeys();

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden
        /// </summary>
        /// <returns>Return Settings element key</returns>
        protected override object GetElementKey(ConfigurationElement element)
        => ((SAMLKeys)element).Key;
        #endregion
    }
}