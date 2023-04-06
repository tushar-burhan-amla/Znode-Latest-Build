namespace Znode.Engine.Api.Cache
{
    public interface IPIMAttributeFamilyCache
    {
        /// <summary>
        /// Get the list of all attribute families.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAttributeFamilyList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of all attribute groups assigned to attribute family.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAssignedAttributeGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get detail of attribute family on the basis of attributeFamilyId
        /// </summary>
        /// <param name="attributeFamilyId">To get attribute family details</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Attribute Family Model</returns>
        string GetAttributeFamily(int attributeFamilyId,string routeUri, string routeTemplate);

        /// <summary>
        /// Get UnAssigned Attribute Groups.
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns unassigned attribute groups. </returns>
        string GetUnAssignedAttributeGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Family locale by family id.
        /// </summary>
        /// <param name="attributeFamilyId">PIM Attribute Family Id.</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of family locales.</returns>
        string GetFamilyLocale(int attributeFamilyId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of all attributes assigned to attribute group.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Response in string format.</returns>
        string GetAssignedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of unassigned attribute.</returns>
        string GetUnAssignedAttributes(string routeUri, string routeTemplate);
    }
}
