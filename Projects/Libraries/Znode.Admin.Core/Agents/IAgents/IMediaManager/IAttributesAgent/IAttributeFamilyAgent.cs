using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IAttributeFamilyAgent
    {
        /// <summary>
        /// Get the list of all attribute families.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Attribute Family List View Model</returns>
        AttributeFamilyListViewModel GetAttributeFamilies(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the list of all attribute families in Key-Value pair.
        /// </summary>
        /// <returns>SelectListItem of attribute families.</returns>
        List<SelectListItem> GetAttributeFamilyList();

        /// <summary>
        /// Create new attribute family.
        /// </summary>
        /// <param name="viewModel">AttributeFamilyViewModel</param>
        /// <returns>AttributeFamilyViewModel</returns>
        AttributeFamilyViewModel CreateAttributeFamily(AttributeFamilyViewModel viewModel);

        /// <summary>
        /// Get the list attribute groups which are assigned to attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">ID of attribute family</param>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Family Group Attribute List View Model</returns>
        AttributeGroupListViewModel GetAssignedAttributeGroups(int attributeFamilyId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Assign attribute groups to attribute family.
        /// </summary>
        /// <param name="viewModel">Family Group Attribute View Model</param>
        /// <returns>Returns true/False</returns>
        bool AssignAttributeGroups(string attributeGroupIds, int attributeFamilyId, out string message);

        /// <summary>
        /// UnAssign attribute groups from attribute family.
        /// </summary>
        /// <param name="viewModel">Family Group Attribute View Model</param>
        /// <returns>Returns true/False</returns>
        bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, out string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeFamilyId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool DeleteAttributeFamily(string attributeFamilyId, out string errorMessage);

        /// <summary>
        /// Get Attribute Group list which are not associated with any attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">attribute family id to attribute groups</param>
        /// <returns>Attribute Group List View Model</returns>
        List<BaseDropDownList> GetUnAssignedAttributeGroups(int attributeFamilyId);

        /// <summary>
        /// Get detail of attribute family on the basis of attributeFamilyId
        /// </summary>
        /// <param name="attributeFamilyId">To get attribute family details</param>
        /// <returns>Attribute Family View Model</returns>
        AttributeFamilyViewModel GetAttributeFamily(int attributeFamilyId);

        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="mediaAttributeFamilyId">Media attribute family id.</param>
        /// <returns>Returns TabViewListModel.</returns>
        TabViewListModel CreateTabStructure(int mediaAttributeFamilyId);

        /// <summary>
        /// Get family locale on the basis of attribute family id.
        /// </summary>
        /// <param name="attributeFamilyId">Media attribute family id.</param>
        /// <returns>Returns list of LocaleDataModel.</returns>
        List<LocaleDataModel> GetLocales(int attributeFamilyId);

        /// <summary>
        /// Save locales.
        /// </summary>
        /// <param name="model">BindDataModel</param>
        /// <returns>Returns true if saved successfully else return false.</returns>
        bool SaveFamilyLocales(BindDataModel model);

        /// <summary>
        /// Get View Mode type.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <returns>Returns list of view mode types.</returns>
        List<SelectListItem> GetViewModes(FilterCollectionDataModel model);
    }
}
