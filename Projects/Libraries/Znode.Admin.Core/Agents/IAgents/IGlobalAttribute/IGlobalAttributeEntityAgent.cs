using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IGlobalAttributeEntityAgent
    {
        /// <summary>
        /// Get global entity
        /// </summary>
        /// <returns>GlobalAttributeEntityViewModel</returns>
        GlobalAttributeEntityViewModel GetGlobalEntity();

        /// <summary>
        /// Get assigned entity group list.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>AssignedEntityGroupListViewModel</returns>
        AssignedEntityGroupListViewModel GetAssignedEntityAttributeGroups(int entityId);

        /// <summary>
        /// Get unassigned entity attribute group.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        List<BaseDropDownList> GetUnAssignedEntityAttributeGroups(int entityId);

        /// <summary>
        /// Assign entity to group.
        /// </summary>
        /// <param name="attributeGroupIds"></param>
        /// <param name="entityId"></param>
        /// <param name="message"></param>
        /// <returns>true/false</returns>
        bool AssignAttributeEntityToGroups(string attributeGroupIds, int entityId, out string message);

        /// <summary>
        /// Un assign group from entity
        /// </summary>
        /// <param name="entityId">Entity Id </param>
        /// <param name="groupId">Group Id</param>
        /// <param name="message">error message</param>
        /// <returns>returns true/false</returns>
        bool UnAssignEntityGroups(int entityId, int groupId, out string message);

        /// <summary>
        /// Update attribute group display order.
        /// </summary>
        /// <param name="globalattributeGroupId">globalattributeGroupId</param>
        /// <param name="globalAttributeEntityId">globalAttributeEntityId</param>
        /// <param name="displayOrder">DisplayOrder</param>
        /// <returns>Return GlobalAttributeGroupViewModel </returns>
        GlobalAttributeGroupViewModel UpdateAttributeGroupDisplayOrder(int globalattributeGroupId, int globalAttributeEntityId, int displayOrder);

        /// <summary>
        /// Get Entity Associated Attribute Details
        /// </summary>
        /// <param name="entityId">id of the entity</param>
        /// <param name="entityType">type of the entity</param>
        /// <returns>return the Associated Attribute Details.</returns>
        GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int entityId, string entityType);

        /// <summary>
        /// Save Entity Attributes value
        /// </summary>
        /// <param name="model">BindDataModel</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns Entity Attribute View Model</returns>
        EntityAttributeViewModel SaveEntityAttributeDetails(BindDataModel model, out string errorMessage);

        /// <summary>
        /// Method to create tab structure.
        /// </summary>
        /// <param name="globalEntityId">Global Entity id.</param>
        /// <returns>Returns TabViewListModel.</returns>
        TabViewListModel CreateTabStructure(int globalEntityId);

        /// <summary>
        /// gets the global attributes based on the passed familyCode for setting the values for default container variant. 
        /// </summary>
        /// <param name="familyCode">family Code</param>
        /// <param name="entityType">entity type</param>
        /// <returns></returns>
        GlobalAttributeEntityDetailsViewModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType);

        /// <summary>
        /// Get Global Attribute details on the basis of variantId id and localeid. 
        /// </summary>
        /// <param name="variantId">Variant ID</param>
        /// <param name="localeId">localeId</param>
        /// <returns></returns>
        ContainerVariantDataViewModel GetGlobalAttributesForAssociatedVariant(int variantId, int localeId = 0);
    }
}
