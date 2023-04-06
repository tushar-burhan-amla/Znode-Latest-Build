using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IGlobalAttributeGroupEntityService
    {
        /// <summary>
        /// Get Global Entity List
        /// </summary>
        /// <returns>Returns GlobalEntityListModel</returns>
        GlobalEntityListModel GetGlobalEntityList();

        /// <summary>
        /// Get the list of all attribute groups which are assigned to Global attribute families.
        /// </summary>
        /// <param name="expands">Collection of Expands</param>
        /// <param name="filters">list of filter tuples</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">collection of paging parameters</param>
        /// <returns>List of attribute groups</returns>
        GlobalAttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get UnAssigned Attribute Groups.
        /// </summary>
        /// <param name="expands">Collection of Expands</param>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns Global AttributeGroupListModel.</returns>
        GlobalAttributeGroupListModel GetUnAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the list of all attributes which are assigned to attribute groups.
        /// </summary>
        /// <param name="expands">Collection of Expands.</param>
        /// <param name="filters">List of filter tuples.</param>
        /// <param name="sorts">Collection of sorts.</param>
        /// <param name="page">collection of paging parameters.</param>
        /// <returns>List of attributes.</returns>
        GlobalAttributeListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="model">GlobalAttributeGroupModel</param>
        /// <returns> returns true/false</returns>
        bool UpdateAttributeGroupDisplayOrder(GlobalAttributeGroupModel model);

        /// <summary>
        /// Assign attribute groups to Entity.
        /// </summary>
        /// <param name="model">Global Attribute Group Entity Mapper Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssignEntityGroups(GlobalAttributeGroupEntityModel model);

        /// <summary>
        /// Unassign global attribute groups from entity.
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        /// <param name="groupId">Group Id to remove.</param>
        /// <returns>boolean value true/false</returns>
        bool UnAssignEntityGroup(int entityId, int groupId);

        /// <summary>
        /// Get Attribute Values based on the Entity Id.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Return the Attribute Values for the Entity.</returns>
        GlobalAttributeEntityDetailsModel GetEntityAttributeDetails(int entityId, string entityType);

        /// <summary>
        /// Save Entity Attribute Details for provided entity
        /// </summary>
        /// <param name="model">Entity Attribute Model</param>
        /// <returns>retrun EntityAttributeModel</returns>
        EntityAttributeModel SaveEntityAttributeDetails(EntityAttributeModel model);

        /// <summary>
        /// Get publish global attributes.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="entityType">Entity Type</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <returns>Returns global attribute entity details model.</returns>
        GlobalSelectedAttributeEntityDetailsModel GetGlobalEntityAttributes(int entityId, string entityType, FilterCollection filters);


        /// <summary>
        /// gets the global attributes based on the passed familyCode for setting the values for default container variant.
        /// </summary>
        /// <param name="familyCode">FamilyCode.</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Return the Attribute Values for the Entity.</returns>
        GlobalAttributeEntityDetailsModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType);


        /// <summary>
        /// Get Global Attribute details on the basis of variantId id and localeid
        /// </summary>
        /// <param name="variantId">variantId.</param>
        /// <param name="localeId">localeId.</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Return the Attribute Values for the Entity.</returns>
        GlobalAttributeEntityDetailsModel GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0);
    }
}
