namespace Znode.Engine.Api.Cache
{
    public interface IAttributeFamilyCache
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
        string GetAttributeFamily(int attributeFamilyId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get family locale on the basis of attribute family id.
        /// </summary>
        /// <param name="attributeFamilyId">Media attribute family id.</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns family locale.</returns>
        string GetFamilyLocale(int attributeFamilyId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassigned attribute groups.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns list of unassigned attribute groups.</returns>
        string GetUnAssignedAttributeGroups(string routeUri, string routeTemplate);
    }
}
