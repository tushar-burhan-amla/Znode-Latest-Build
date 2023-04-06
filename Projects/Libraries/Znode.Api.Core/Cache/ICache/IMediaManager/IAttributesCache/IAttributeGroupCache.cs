namespace Znode.Engine.Api.Cache
{
    public interface IAttributeGroupCache
    {
        /// <summary>
        /// Get a list of all attribute groups.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAttributeGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute group using attributeGroupId.
        /// </summary>
        /// <param name="attributeGroupId">AttributeGroupId use to retrieve attribute group</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAttributeGroup(int attributeGroupId, string routeUri, string routeTemplate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the attribute group locales.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns list of attribute group locales.</returns>
        string GetAttributeGroupLocales(int attributeGroupId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        string UnAssignedAttributes(int attributeGroupId, string routeUri, string routeTemplate);
    }
}