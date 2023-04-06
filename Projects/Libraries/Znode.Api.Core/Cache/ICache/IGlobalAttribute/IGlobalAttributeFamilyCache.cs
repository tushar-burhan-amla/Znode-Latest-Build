namespace Znode.Engine.Api.Cache
{
    public interface IGlobalAttributeFamilyCache
    {
        /// <summary>
        /// Get global attribute family list
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>attribute family list</returns>
        string List(string routeUri, string routeTemplate);

        /// <summary>
        /// Get assigned attribute groups
        /// </summary>
        /// <param name="attributeFamilyId">attributeFamilyId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>assigned attribute groups</returns>
        string GetAssignedAttributeGroups(string familyCode, string routeUri, string routeTemplate);

        /// <summary>
        ///  Get unassigned attribute groups
        /// </summary>
        /// <param name="attributeFamilyId">attributeFamilyId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string GetUnassignedAttributeGroups(string familyCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets global attribute family
        /// </summary>
        /// <param name="familyId">familyId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string GetAttributeFamily(string familyCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get attribute family locale list.
        /// </summary>
        /// <param name="attributeFamilyId">attributeFamilyId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string GetAttributeFamilyLocales(string familyCode, string routeUri, string routeTemplate);

    }
}
