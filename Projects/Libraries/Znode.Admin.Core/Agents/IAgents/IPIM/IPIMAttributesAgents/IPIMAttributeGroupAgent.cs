using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IPIMAttributeGroupAgent
    {
        /// <summary>
        /// Get the list of locale.
        /// </summary>
        /// <returns>Returns list of locales.</returns>
        List<LocaleDataModel> GetLocales(int pimAttributeGroupId);

        /// <summary>
        /// Create attribute group with locales.
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns true if successfully created</returns>
        PIMAttributeGroupViewModel Create(BindDataModel model);

        /// <summary>
        /// Get Assigned attributes list.
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="expands">Expands</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns assigned attributes list.</returns>
        PIMAttributeGroupMapperListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Gets the tab structure.
        /// </summary>
        /// <param name="pimAttributeGroupId">PIM attribute group id.</param>
        /// <param name="isCategory">Is category.</param>
        /// <returns>Returns tab structure.</returns>
        TabViewListModel GetTabStructure(int pimAttributeGroupId, bool isCategory);

        /// <summary>
        /// Get the pim attribute group.
        /// </summary>
        /// <param name="pimAttributeGroupId">pim attribute group id to get the record.</param>
        /// <returns>Returns the pim attribute group id.</returns>
        PIMAttributeGroupViewModel Get(int pimAttributeGroupId);

        /// <summary>
        /// Sets the filters.
        /// </summary>
        /// <param name="filters">Filter to set.</param>
        /// <param name="pimAttributeGroupId">pim attribute group is to set/</param>
        void SetFilters(FilterCollection filters, int pimAttributeGroupId);

        /// <summary>
        ///Get PIMAttributeGroup list.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Return PIMAttributeGroupListViewModel </returns>  
        PIMAttributeGroupListViewModel GetPIMAttributeGroups(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="pimAttributeGroupId">pim attribute group id.</param>
        /// <param name="isCategory">Is category.</param>
        /// <returns>Returns unassigned attributes.</returns>
        List<BaseDropDownList> GetUnAssignedAttributes(int pimAttributeGroupId, bool isCategory);

        /// <summary>
        /// Associate attributes
        /// </summary>
        /// <param name="attributeIds">Attributes id to associate</param>
        /// <param name="attributeGroupId">group to associate attribute</param>
        /// <param name="message">error message</param>
        /// <returns>returns true false</returns>
        bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message);

        /// <summary>
        /// Remove association 
        /// </summary>
        /// <param name="pimAttributeGroupId">from group to remove association</param>
        /// <param name="pimAttributeId">attributes to remove</param>
        /// <param name="isCategory">Identification flag for category and product.</param>
        /// <returns>returns true false</returns>
        bool RemoveAssociatedAttribute(int pimAttributeGroupId, int pimAttributeId, bool isCategory, out string message);

        /// <summary>
        /// delete attribute group.
        /// </summary>
        /// <param name="pimAttributeGroupIds">pim attributes ids</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns true or false.</returns>
        bool Delete(string pimAttributeGroupIds, out string errorMessage);

        /// <summary>
        /// Update Attribute Group
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns updated model.</returns>
        PIMAttributeGroupViewModel UpdateAttributeGroup(BindDataModel model);

        /// <summary>
        /// Check Whether the Group Code is already exists.
        /// </summary>
        /// <param name="groupCode">is a group Code</param>
        /// <param name="isCategory">specifies if it category group code.</param>
        /// <returns>return the status in true or false</returns>
        bool CheckGroupCodeExist(string groupCode, bool isCategory, int pimAttributeGroupId);

        /// <summary>
        ///Update Attribute Display Order
        /// </summary>
        /// <param name="model">model to extract data from.</param>
        /// <returns>Returns updated model.</returns>
        PIMAttributeDataViewModel UpdateAttributeDisplayOrder(int pimAttributeId, string data);
    }
}
