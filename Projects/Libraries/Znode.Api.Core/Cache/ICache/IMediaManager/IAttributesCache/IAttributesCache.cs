namespace Znode.Engine.Api.Cache
{
    public interface IAttributesCache
    {
        /// <summary>
        /// Get attribute list from database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns attributes string</returns>
        string GetAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute by attribute id
        /// </summary>
        /// <param name="attributeId">media attribute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns attribute data</returns>
        string GetAttribute(int attributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute type list from database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns attribute type string</returns>
        string GetAttributeTypes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute inputvalidations from database.
        /// </summary>
        /// <param name="attributeTypeId">attribute type id</param>
        /// <param name="attributeId">attrubute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns attribute inputvalidations string</returns>
        string GetInputValidations(int attributeTypeId, int attributeId, string routeUri, string routeTemplate);

        #region Attribute Locale

        /// <summary>
        /// Get attribute locale  details by attribute id.
        /// </summary>
        /// <param name="attributeId">media attribute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAttributeLocale(int attributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Default Value By attribute Id
        /// </summary>
        /// <param name="AttributeId">media attribute id</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetDefaultValues(int AttributeId, string routeUri, string routeTemplate);
        #endregion
    }
}
