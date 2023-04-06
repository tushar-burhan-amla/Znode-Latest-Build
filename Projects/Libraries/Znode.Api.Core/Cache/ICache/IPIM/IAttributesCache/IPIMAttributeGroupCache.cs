namespace Znode.Engine.Api.Cache
{
    public interface IPIMAttributeGroupCache
    {
        /// <summary>
        /// Gets the list of PIM Attribute Group list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>String response.</returns>
        string GetAttributeGroupList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets PIM Attribute group by ID.
        /// </summary>
        /// <param name="id">PIM attribute group ID.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>String response.</returns>
        string GetAttributeGroup(int id, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets assigned attributes list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string AssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets unassigned attributes list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string UnAssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets attribute group locales list.
        /// </summary>
        /// <param name="attributeGroupId">PIM attribute group ID.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Returns response.</returns>
        string GetAttributeGroupLocales(int attributeGroupId, string routeUri, string routeTemplate);
    }
}