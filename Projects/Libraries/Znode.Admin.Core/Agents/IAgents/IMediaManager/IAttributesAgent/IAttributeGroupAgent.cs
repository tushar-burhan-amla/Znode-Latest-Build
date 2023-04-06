using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;

namespace Znode.Engine.Admin.Agents
{
    public interface IAttributeGroupAgent
    {
        /// <summary>
        /// Get the list of all attribute groups.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Attribute Group List View Model</returns>
        AttributeGroupListViewModel GetAttributeGroups(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the list of all attribute groups in Key-Value pair.
        /// </summary>
        /// <returns>SelectListItem of attribute groups.</returns>
        List<SelectListItem> GetAttributeGroupList();

        /// <summary>
        /// Get attribute group.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <returns>Returns attribute group.</returns>
        AttributeGroupViewModel GetAttributeGroup(int attributeGroupId);

        /// <summary>
        /// Update attribute group.
        /// </summary>
        /// <param name="viewModel">view model to update.</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        bool UpdateAttributeGroup(BindDataModel viewModel);

        /// <summary>
        /// Delete existing attribute group.
        /// </summary>
        /// <param name="attributeGroupId">attribute group ids to delete.</param>
        /// <param name="message"></param>
        /// <returns>Returns true or false.</returns>
        bool DeleteAttributeGroup(string attributeGroupId, out string message);

        /// <summary>
        /// Gets list of assigned attributes.
        /// </summary>
        /// <param name="filters">Filters to be applied on list.</param>
        /// <param name="sortCollection">Sorting to be applied on the list.</param>
        /// <param name="pageIndex">Page index of the list.</param>
        /// <param name="recordPerPage">Page size of the list.</param>
        /// <returns>List pf attribute group mapper list.</returns>
        AttributeGroupMapperListViewModel GetAssignedAttributes( FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands= null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Deletes association of attribute to an attributegroup.
        /// </summary>
        /// <param name="attributeGroupMapperId">Attribute group mapper ID to be deleted.</param>
        /// <returns>Return true or false according to the status of delete operation.</returns>
        bool RemoveAssociatedAttribute(int attributeGroupMapperId);

        /// <summary>
        /// Returns drop down list of unassociated attributes.
        /// </summary>
        /// <param name="attributeGroupId">Attributed Group accordint which attribute lis twill be filtered.</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        List<BaseDropDownList> GetUnAssignedAttributes(int attributeGroupId);

        /// <summary>
        /// GEts the view mode.
        /// </summary>
        /// <param name="model">Filter collection model.</param>
        /// <returns>Returns list of select list item.</returns>
        List<SelectListItem> GetViewModes(FilterCollectionDataModel model);

        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="mediaAttributeGroupId">media Attribute Group Id</param>
        /// <returns>Returns tab view list model</returns>
        TabViewListModel CreateGroupTabStructure(int mediaAttributeGroupId);

        /// <summary>
        /// Associates list of attribute to an attribute Group.
        /// </summary>
        /// <param name="attributeIds">attribute ids to associate.</param>
        /// <param name="attributeGroupId">group to associate.</param>
        /// <param name="message">error message.</param>
        /// <returns>Returns true or false.</returns>
        bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message);

        /// <summary>
        /// Create the attribute.
        /// </summary>
        /// <param name="model">model to save.</param>
        /// <returns>Returns created model.</returns>
        AttributeGroupViewModel Create(BindDataModel model);

        /// <summary>
        /// Get locales.
        /// </summary>
        /// <param name="pimAttributeGroupId">group id to get locales.</param>
        /// <returns></returns>
        List<LocaleDataModel> GetLocales(int pimAttributeGroupId);

        /// <summary>
        /// Set filters for attribute group id.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="attributeGroupId">Attribute Group Id.</param>
        void SetFilters(FilterCollection filters, int attributeGroupId);

        /// <summary>
        /// Check Whether the Group Code is already exists.
        /// </summary>
        /// <param name="groupCode">Group Code</param>
        /// <param name="mediaAttributeGroupId">mediaAttributeGroupId</param>
        /// <returns>return the status in true or false</returns>
        bool CheckGroupCodeExist(string groupCode, int mediaAttributeGroupId);
    }
}