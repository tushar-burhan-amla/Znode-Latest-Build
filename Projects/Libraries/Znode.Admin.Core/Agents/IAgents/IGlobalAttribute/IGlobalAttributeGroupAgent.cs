using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IGlobalAttributeGroupAgent
    {
        /// <summary>
        ///Get global attribute group list.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Return global attribute group list view model.</returns>  
        GlobalAttributeGroupListViewModel GetGlobalAttributeGroups(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int entityId = 0, string entityType = null);

        /// <summary>
        /// Create attribute group with locales.
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns true if successfully created</returns>
        GlobalAttributeGroupViewModel Create(BindDataModel model);

        /// <summary>
        /// Get the global attribute group.
        /// </summary>
        /// <param name="globalAttributeGroupId">global attribute group id to get the record.</param>
        /// <returns>Returns the global attribute group id.</returns>
        GlobalAttributeGroupViewModel Get(int globalAttributeGroupId);

        /// <summary>
        /// Update Attribute Group
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns updated model.</returns>
        GlobalAttributeGroupViewModel UpdateAttributeGroup(BindDataModel model);

        /// <summary>
        /// Gets the tab structure.
        /// </summary>
        /// <param name="globalAttributeGroupId">Global attribute group id.</param>
        /// <returns>Returns tab structure.</returns>
        TabViewListModel GetTabStructure(int globalAttributeGroupId);

        /// <summary>
        /// Get the list of locale.
        /// </summary>
        /// <returns>Returns list of locales.</returns>
        List<LocaleDataModel> GetLocales(int globalAttributeGroupId);

        /// <summary>
        /// Check whether the group code already exists or not.
        /// </summary>
        /// <param name="groupCode">Group Code</param>
        /// <param name="globalAttributeGroupId">Global Attribute group id.</param>
        /// <returns>Return true or false.</returns>
        bool CheckGroupCodeExist(string groupCode, int globalAttributeGroupId);

        /// <summary>
        /// Delete Global Attribute group.
        /// </summary>
        /// <param name="globalAttributeGroupId">global attributes ids</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns true or false.</returns>
        bool Delete(string globalAttributeGroupId, out string errorMessage);

        /// <summary>
        /// Get assigned attributes list.
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="expands">Expands</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns assigned attributes list.</returns>
        GlobalAttributeGroupMapperListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="globalAttributeGroupId">Global attribute group id.</param>
        /// <returns>Returns unassigned attributes.</returns>
        List<BaseDropDownList> GetUnAssignedAttributes(int globalAttributeGroupId);

        /// <summary>
        /// Associate attributes.
        /// </summary>
        /// <param name="attributeIds">Attributes id to associate.</param>
        /// <param name="attributeGroupId">Group Id to associate attribute.</param>
        /// <param name="message">Error message</param>
        /// <returns>Returns true false.</returns>
        bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message);

        /// <summary>
        /// Remove association. 
        /// </summary>
        /// <param name="globalAttributeGroupId">from group to remove association</param>
        /// <param name="globalAttributeId">attributes to remove</param>
        /// <returns>Returns true false</returns>
        bool RemoveAssociatedAttribute(int globalAttributeGroupId, int globalAttributeId, out string message);

        /// <summary>
        ///Update Attribute Display Order
        /// </summary>
        /// <param name="model">Model to extract data from.</param>
        /// <returns>Returns updated model.</returns>
        GlobalAttributeViewModel UpdateAttributeDisplayOrder(int globalAttributeId, string data);

        /// <summary>
        /// Get Global Entity types
        /// </summary>
        /// <returns>List Of Global Entities</returns>
        List<SelectListItem> GetGlobalEntityTypes();
    }
}
