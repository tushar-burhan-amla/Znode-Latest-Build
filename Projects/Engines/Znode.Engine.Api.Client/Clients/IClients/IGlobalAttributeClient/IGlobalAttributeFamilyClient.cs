using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGlobalAttributeFamilyClient :  IBaseClient
    {
        /// <summary>
        /// Gets the List of Attribute Family
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>GlobalAttributeFamilyListModel</returns>
        GlobalAttributeFamilyListModel GetAttributeFamilyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create a new Attribute Family
        /// </summary>
        /// <param name="attributeFamilyModel">GlobalAttributeFamilyModel model</param>
        /// <returns>GlobalAttributeFamilyModel model</returns>
        GlobalAttributeFamilyModel CreateAttributeFamily(GlobalAttributeFamilyCreateModel attributeFamilyModel);

        /// <summary>
        ///  Update Global Attribute Family.
        /// </summary>
        /// <param name="attributeFamilyModel">GlobalAttributeFamilyModel model</param>
        /// <returns>GlobalAttributeFamilyModel model</returns>
        GlobalAttributeFamilyModel UpdateAttributeFamily(GlobalAttributeFamilyUpdateModel attributeFamilyModel);

        /// <summary>
        /// Delete Global Attribute Family
        /// </summary>
        /// <param name="globalAttributeFamilyIds">comma separated globalAttributeFamilyIds</param>
        /// <returns>status of deletion</returns>
        bool DeleteAttributeFamily(ParameterModel globalAttributeFamilyIds);

        /// <summary>
        /// Get the Attribute Family
        /// </summary>
        /// <param name="familyCode>familyCode</param>
        /// <returns>GlobalAttributeFamilyModel model</returns>
        GlobalAttributeFamilyModel GetAttributeFamily(string familyCode);

        /// <summary>
        /// Returns the List of Groups that are associated to a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeGroupListModel model</returns>
        GlobalAttributeGroupListModel GetAssignedAttributeGroups(string familyCode);

        /// <summary>
        /// Returns the List of Unassigned groups of a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeGroupListModel model</returns>
        GlobalAttributeGroupListModel GetUnassignedAttributeGroups(string familyCode);

        /// <summary>
        /// Assign Attribute Groups to a Family
        /// </summary>
        /// <param name="attributeGroupIds">comma separated attributeGroupIds</param>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of assignment</returns>
        bool AssignAttributeGroups(string attributeGroupIds, string familyCode);

        /// <summary>
        /// Unassign attribute Groups from a family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <param name="groupCode">groupCode</param>
        /// <returns>status of Unassign</returns>
        bool UnassignAttributeGroups(string groupCode, string familyCode);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="familyCode">familyCode</param>
        /// <param name="displayOrder">displayOrder</param>
        /// <returns>status of update</returns>
        bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder);

        /// <summary>
        /// Get the Attribute Family Locale
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyLocaleListModel model</returns>
        GlobalAttributeFamilyLocaleListModel GetGlobalAttributeFamilyLocales(string familyCode);

        /// <summary>
        /// Check if the family code exists
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>true if exists</returns>
        bool IsFamilyCodeExist(string familyCode);

    }
}
