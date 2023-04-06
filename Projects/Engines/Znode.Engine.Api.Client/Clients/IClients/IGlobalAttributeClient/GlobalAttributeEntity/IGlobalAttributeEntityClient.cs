using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGlobalAttributeEntityClient : IBaseClient
    {
        /// <summary>
        /// Get global entity list.
        /// </summary>
        /// <returns>GlobalEntityListModel</returns>
        GlobalEntityListModel GetGlobalEntity();

        /// <summary>
        /// Get assigned entity attribute groups.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>GlobalAttributeGroupListModel</returns>
        GlobalAttributeGroupListModel GetAssignedEntityAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? pageSize);


        /// <summary>
        /// Get unassigned entity attribute groups.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>GlobalAttributeGroupListModel</returns>
        GlobalAttributeGroupListModel GetUnAssignedEntityAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate group to entity.
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel"></param>
        /// <returns>true/false</returns>
        bool AssociateAttributeEntityToGroups(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel);

        /// <summary>
        /// Un Associate groups from entity.
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>true/false</returns>
        bool UnAssociateEntityGroups(int entityId, int groupId);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="attributeGroupModel"></param>
        /// <returns></returns>
        GlobalAttributeGroupModel UpdateAttributeGroupDisplayOrder(GlobalAttributeGroupModel attributeGroupModel);

        /// <summary>
        /// Get Entity Associated Attribute Details
        /// </summary>
        /// <param name="entityId">id of the entity</param>
        /// <param name="entityType">type of the entity</param>
        /// <returns>return the Associated Attribute Details.</returns>
        GlobalAttributeEntityDetailsModel GetEntityAttributeDetails(int entityId, string entityType);

        /// <summary>
        /// Save Entity Attribute Details
        /// </summary>
        /// <param name="model">EntityAttributeModel</param>
        /// <returns>return the Associated Attribute Details.</returns>
        EntityAttributeModel SaveEntityAttributeDetails(EntityAttributeModel model);

        /// <summary>
        /// Get publish global attributes.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="entityType">Entity Type.</param>
        /// <param name="filters">Filters for group code.</param>
        /// <returns>Return global attribute details.</returns>
        GlobalSelectedAttributeEntityDetailsModel GetGlobalEntityAttributes(int entityId, string entityType, FilterCollection filters);

        /// <summary>
        /// gets the global attributes based on the passed familyCode for setting the values for default container variant. 
        /// </summary>
        /// <param name="familyCode">family Code</param>
        /// <param name="entityType">entity type</param>
        /// <returns></returns>
        GlobalAttributeEntityDetailsModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType);

        /// <summary>
        /// Get Global Attribute details on the basis of Variant id and localeid
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="entityType">entity type</param>
        /// <returns></returns>
        GlobalAttributeEntityDetailsModel GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0);
    }
}
