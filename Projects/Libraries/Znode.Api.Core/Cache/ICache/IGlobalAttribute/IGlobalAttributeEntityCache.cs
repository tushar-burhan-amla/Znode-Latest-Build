namespace Znode.Engine.Api.Cache
{
    public interface IGlobalAttributeEntityCache
    {
        /// <summary>
        /// Get all entity entity list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAllEntityList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get assigned entity attribute groups.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAssignedEntityAttributeGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassigned entity group attribute groups.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetUnAssignedEntityAttributeGroups(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Attribute Values based on the Entity Id.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="entityType">Entity Type.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetEntityAttributeDetails(int entityId, string entityType, string routeUri, string routeTemplate);

        /// <summary>
        /// Get publish global attributes.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="entityType">Entity Type.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Get publish attributes.</returns>
        string GetGlobalEntityAttributes(int entityId, string entityType, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the global attributes based on the passed familyCode for setting the values for default container variant.
        /// </summary>
        /// <param name="familyCode">Family Code</param>
        /// <param name="entityType">Entity Type.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Global Attribute details on the basis of variantId id and localeid
        /// </summary>
        /// <param name="variantId">variantId.</param>
        /// <param name="localeId">localeId.</param>
        /// <param name="entityType">Entity Type.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, string routeUri, string routeTemplate, int localeId = 0);
        
    }
}
