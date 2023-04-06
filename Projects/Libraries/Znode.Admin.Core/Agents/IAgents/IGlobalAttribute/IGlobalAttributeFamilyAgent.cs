using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{ 
    public interface IGlobalAttributeFamilyAgent
    {
        /// <summary>
        /// Gets the List of Attribute Family
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <param name="entityId">entityId</param>
        /// <param name="entityType">entityType</param>
        /// <returns>GlobalAttributeFamilyListViewModel model</returns>
        GlobalAttributeFamilyListViewModel List(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int entityId = 0, string entityType = null);

        /// <summary>
        /// Create a new Attribute Family
        /// </summary>
        /// <param name="attributeFamilyModel">GlobalAttributeFamilyModel model</param>
        /// <returns>GlobalAttributeFamilyViewModel model</returns>
        GlobalAttributeFamilyViewModel Create(BindDataModel model);


        /// <summary>
        /// Get the Attribute Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeFamilyViewModel model</returns>
        GlobalAttributeFamilyViewModel Edit(string familyCode);

        /// <summary>
        /// Update Attribute Family
        /// </summary>
        /// <param name="model">BindDataModel model</param>
        /// <returns>GlobalAttributeFamilyViewModel model</returns>
        GlobalAttributeFamilyViewModel UpdateAttributeFamily(BindDataModel model);
        bool DeleteAttributeFamily(string globalAttributeFamilyIds, out string errorMessage);

        /// <summary>
        /// Get the Tab Structure
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>TabViewListModel model</returns>
        TabViewListModel GetTabStructure(string familyCode);

        /// <summary>
        /// Get locales
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>List of LocaleDataModel</returns>
        List<LocaleDataModel> GetLocales(string familyCode);

        /// <summary>
        ///  Get Global Entity Types
        /// </summary>
        /// <returns>List</returns>
        List<SelectListItem> GetGlobalEntityTypes(bool IsCreate = false);

        /// <summary>
        /// Returns the List of Unassigned groups of a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>List</returns>
        List<BaseDropDownList> GetUnassignedAttributeGroups(string familyCode);

        /// <summary>
        /// Returns the List of Groups that are associated to a Family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns>GlobalAttributeGroupListViewModel model</returns>
        GlobalAttributeGroupListViewModel GetAssignedAttributeGroups(string familyCode);

        /// <summary>
        /// Assign Attribute Groups to a Family
        /// </summary>
        /// <param name="attributeGroupIds">attributeGroupIds</param>
        /// <param name="familyCode">familyCode</param>
        /// <param name="message">message</param>
        /// <returns>status of assignment</returns>
        bool AssignAttributeGroups(string attributeGroupIds, string familyCode, out string message);

        /// <summary>
        /// Unassign attribute Groups from a family
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <param name="groupCode">groupCode</param>
        /// <param name="message">message</param>
        /// <returns>status of unassign</returns>
        bool UnassignAttributeGroups(string groupCode, string familyCode, out string message);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="groupCode">groupCode</param>
        /// <param name="globalAttributeFamilyId">globalAttributeFamilyId</param>
        /// <param name="DisplayOrder">DisplayOrder</param>
        /// <returns>status of update</returns>
        bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int DisplayOrder);

        /// <summary>
        /// Verify if the family code exists
        /// </summary>
        /// <param name="familyCode">familyCode</param>
        /// <returns></returns>
        bool IsFamilyCodeExist(string familyCode);


    }
}
