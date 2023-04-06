using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;

namespace Znode.Engine.Admin.Agents
{
    public interface IPIMAttributeFamilyAgent
    {
        /// <summary>
        /// Get PIM Attribute family list.
        /// </summary>
        /// <returns>Returns List<SelectListItem> for PIM attribute family.</returns>
        List<SelectListItem> GetPIMAttributeFamilyList(string isCategory);

        /// <summary>
        /// Create attribute family.
        /// </summary>
        /// <param name="model">Model with key value pair.</param>
        /// <returns>Return Family model with data.</returns>
        PIMAttributeFamilyViewModel Save(BindDataModel model);

        /// <summary>
        /// Get PIM Attribute Family model on the basis of PIM Attribute Family Id.
        /// </summary>
        /// <param name="pimAttributeFamilyId">PIM Attribute Family Id.</param>
        /// <returns>Returns PIMAttributeFamilyViewModel against PIM Attribute Family Id.</returns>
        PIMAttributeFamilyViewModel GetPIMAttributeFamily(int pimAttributeFamilyId);

        /// <summary>
        /// Get Assigned PIM Attribute Groups.
        /// </summary>
        /// <param name="pimAttributeFamilyId">ID of PIM Attribute Family.</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Returns PIMAttributeGroupListViewModel.</returns>
        PIMAttributeGroupListViewModel GetAssignedPIMAttributeGroups(int pimAttributeFamilyId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get UnAssigned PIMAttribute Groups.
        /// </summary>
        /// <param name="pimAttributeFamilyId">ID of PIM Attribute Family.</param>
        /// <param name="isCategory">Is Category flag.</param>
        /// <returns>Returns List of BaseDropDownList model.</returns>
        List<BaseDropDownList> GetUnAssignedPIMAttributeGroups(int pimAttributeFamilyId, bool isCategory);

        /// <summary>
        /// Method to create tab structure.
        /// </summary>
        /// <param name="pimAttributeFamilyId">ID of PIM Attribute Family.</param>
        /// <param name="isCategory">Is Category flag.</param>
        /// <returns>Returns TabViewListModel.</returns>
        TabViewListModel CreateTabStructure(int pimAttributeFamilyId, bool isCategory);

        /// <summary>
        /// Assign attribute groups.
        /// </summary>
        /// <param name="attributeGroupIds">Attribute Group Ids which have to be assigned.</param>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool AssignAttributeGroups(string attributeGroupIds, int attributeFamilyId, out string message);

        /// <summary>
        /// Un Assign Attribute Groups.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <param name="attributeGroupId">Attribute Group Ids which have to be unassigned.</param>
        /// <param name="isCategory">Identification flag for category and product.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory, out string message);

        /// <summary>
        ///Get PIMAttributeFamilies list.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Return PIMAttributeFamilyListViewModel </returns>
        PIMAttributeFamilyListViewModel GetPIMAttributeFamilies(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get locales list.
        /// </summary>
        /// <param name="pimAttributeFamilyId">PIM Attribute Family Id.</param>
        /// <returns>Returns list of LocaleDataModel.</returns>
        List<LocaleDataModel> GetLocales(int pimAttributeFamilyId);

        /// <summary>
        /// Save locales for Attribute Family.
        /// </summary>
        /// <param name="model">BindDataModel.</param>
        /// <returns>Returns true if saved successfully else return false.</returns>
        bool SaveFamilyLocales(BindDataModel model);

        /// <summary>
        /// Delete PIM Attribute Family.
        /// </summary>
        /// <param name="pimAttributeFamilyId">PIM Attribute Family Ids to be deleted.</param>
        /// <param name="errorMessage"></param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeletePIMAttributeFamily(string pimAttributeFamilyId, out string errorMessage);

        /// <summary>
        /// Check Whether the Family Code is already exists.
        /// </summary>
        /// <param name="familyCode">is a family Code</param>
        /// <param name="isCategory">specifies if it category family code.</param>
        /// <returns>return the status in true or false</returns>
        bool CheckFamilyCodeExist(string familyCode, bool isCategory,int pimAttributeFamilyId);

        /// <summary>
        /// Update attribute group display order.
        /// </summary>
        /// <param name="pimAttributeGroupId">pimAttributeGroupId</param>
        /// <param name="pimAttributeFamilyId">pimAttributeFamilyId</param>
        /// <param name="displayOrder">DisplayOrder</param>
        /// <returns>Return PIMAttributeGroupViewModel </returns>
        PIMAttributeGroupViewModel UpdateAttributeGroupDisplayOrder(int pimAttributeGroupId, int pimAttributeFamilyId, int displayOrder);

        #region Attributes
        /// <summary>
        /// Get attributes associated to group.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sortCollection">Sort Collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns attribute list view model.</returns>
        PIMAttributeListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="pimAttributeFamilyId">PIM Attribute Family Id.</param>
        /// <param name="attributeGroupId">Attribute Group Id.</param>
        /// <returns>Returns List of BaseDropDownList model.</returns>
        List<BaseDropDownList> GetUnAssignedPIMAttributes(int pimAttributeFamilyId,int attributeGroupId);

        /// <summary>
        /// Assign attributes.
        /// </summary>
        /// <param name="attributeIds">Attribute Group Ids under which we need to assign.</param>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <param name="attributeGroupId">Attribute Ids to be assigned.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool AssignAttributes(string attributeIds, int attributeFamilyId, int attributeGroupId, out string message);

        /// <summary>
        /// Unassign attribute from group.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <param name="attributeGroupId">Attribute group id.</param>
        /// <param name="attributeId">Attribute id.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool UnAssignAttributes(int attributeFamilyId, int attributeGroupId, int attributeId, out string message); 
        #endregion

    }
}
