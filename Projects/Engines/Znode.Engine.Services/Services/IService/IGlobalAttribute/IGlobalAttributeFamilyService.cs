using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IGlobalAttributeFamilyService
    {
        /// <summary>
        /// Gets the List of Attribute Family
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>GlobalAttributeFamilyListModel</returns>
        GlobalAttributeFamilyListModel List(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// To Create a new Attribute Family
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>GlobalAttributeFamilyModel</returns>
        GlobalAttributeFamilyModel Create(GlobalAttributeFamilyCreateModel model);

        /// <summary>
        /// Update Global Attribute Family
        /// </summary>
        /// <param name="model">GlobalAttributeFamilyModel model</param>
        /// <returns>status if update</returns>
        GlobalAttributeFamilyModel Update(GlobalAttributeFamilyUpdateModel model);

        /// <summary>
        /// To Delete Global Attribute Family
        /// </summary>
        /// <param name="globalAttributeFamilyIds">comma separated globalAttributeFamilyIds</param>
        /// <returns>status of Deletion</returns>
        bool Delete(ParameterModel globalAttributeFamilyIds);

        /// <summary>
        /// To Delete Global Attribute Family by Family Code
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of Deletion</returns>
        bool DeleteFamilyByCode(string familyCode);

        /// <summary>
        /// Returns the List of Groups that are associated to a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeGroupListModel</returns>
        GlobalAttributeGroupListModel GetAssignedAttributeGroups(string familyCode);

        /// <summary>
        /// Returns the List of Unassigned groups of a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeGroupListModel</returns>
        GlobalAttributeGroupListModel GetUnassignedAttributeGroups(string familyCode);

        /// <summary>
        /// Assign Attribute Groups to a Family
        /// </summary>
        /// <param name="attributeGroupIds">comma separated attributeGroupIds</param>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of assignment</returns>
        bool AssignAttributeGroups(string attributeGroupIds, string familyCode);

        /// <summary>
        /// Assign Attribute Groups to a Family by group code
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="familyCode">familyCode</param>
        /// <returns>status of assignment</returns>
        bool AssignAttributeGroupsByGroupCode(string groupCode, string familyCode);

        /// <summary>
        /// Unassign attribute Groups from a family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <param name="groupCode">groupCode</param>
        /// <returns>status of unassigned</returns>
        bool UnassignAttributeGroups(string groupCode, string familyCode);

        /// <summary>
        /// Get the Attribute Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyModel model</returns>
        GlobalAttributeFamilyModel GetAttributeFamily(string familyCode);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="familyCode">familyCode</param>
        /// <param name="displayOrder">displayOrder</param>
        /// <returns>status of Update</returns>
        bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder);

        /// <summary>
        /// Get the Attribute Family Locale
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyLocaleListModel</returns>
        GlobalAttributeFamilyLocaleListModel GetAttributeFamilyLocale(string familyCode);

        /// <summary>
        /// check if attribute family exists
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>true if the code exists</returns>
        bool IsAttributeFamilyExist(string familyCode);




    }
}
