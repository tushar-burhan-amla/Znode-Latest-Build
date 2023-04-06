namespace Znode.Engine.Api.Cache
{
    public interface IGlobalAttributeGroupCache
    {
        /// <summary>
        /// Gets the list of global attribute Group list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>String response.</returns>
        string GetAttributeGroupList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets global attribute group by ID.
        /// </summary>
        /// <param name="id">Global attribute group ID.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>String response.</returns>
        string GetAttributeGroup(int id, string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute group locales list.
        /// </summary>
        /// <param name="attributeGroupId">Global attribute group ID.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string GetAttributeGroupLocales(int attributeGroupId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get assigned attributes list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string AssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned attributes list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string UnAssignedAttributes(string routeUri, string routeTemplate);
    }
}
