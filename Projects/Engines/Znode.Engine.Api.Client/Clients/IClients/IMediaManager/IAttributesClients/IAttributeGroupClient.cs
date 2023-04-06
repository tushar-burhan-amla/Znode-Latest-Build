using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IAttributeGroupClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Attribute groups.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Attribute group list.</param>
        /// <param name="filters">Filters to be applied on Atribute group list.</param>
        /// <param name="sorts">Sorting to be applied on attribute group list.</param>
        /// <returns>List of attribute group models.</returns>
        AttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Attribute groups.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Attribute group list.</param>
        /// <param name="filters">Filters to be applied on Atribute group list.</param>
        /// <param name="sorts">Sorting to be applied on attribute group list.</param>
        /// <param name="pageIndex">Start page index of the attribute group list.</param>
        /// <param name="pageSize">page size of attribute group list.</param>
        /// <returns>Attribute group list model.</returns>
        AttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Creates an attribute group.
        /// </summary>
        /// <param name="model">Attribute group model to be created.</param>
        /// <returns>Newly created attribute group.</returns>
        AttributeGroupModel CreateAttributeGroup(AttributeGroupModel model);

        /// <summary>
        /// Gets the attribute group by attribute group ID.
        /// </summary>
        /// <param name="attributeGroupId">ID of the attribute group to be retrieved.</param>
        /// <returns>Attribute group model.</returns>
        AttributeGroupModel GetAttributeGroup(int attributeGroupId);

        /// <summary>
        /// Updates an attribute group.
        /// </summary>
        /// <param name="model">Updated model of an attribute group.</param>
        /// <returns>Updated attribute group.</returns>
        AttributeGroupModel UpdateAttributeGroup(AttributeGroupModel model);

        /// <summary>
        /// Delete attribute groups.
        /// </summary>
        /// <param name="attributeGroupIds">IDs of an attribute group to be deleted.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteAttributeGroup(ParameterModel attributeGroupIds);

        /// <summary>
        /// Assigns a list of attribute to Attribute Group.
        /// </summary>
        /// <param name="attributeGroupMapperList">List of attribute and group mapper.</param>
        /// <returns>True/False value according the status of assignment operation.</returns>
        bool AssignAttributes(AttributeGroupMapperListModel attributeGroupMapperList);

        /// <summary>
        /// Gets associated attributes.
        /// </summary>
        /// <param name="expands">Expands to be included in attribute group mapper list.</param>
        /// <param name="filters">Filters to be applied on attribute group mapper list.</param>
        /// <param name="sorts">Sorting of attribute group mapper list.</param>
        /// <returns>List of associated attributes to attribute group.</returns>
        AttributeGroupMapperListModel GetAssociatedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of associated attributes to the attribute group.
        /// </summary>
        /// <param name="expands">Expands to be included in attribute group mapper list.</param>
        /// <param name="filters">Filters to be applied on attribute group mapper list.</param>
        /// <param name="sorts">Sorting of attribute group mapper list.</param>
        /// <param name="pageIndex">Start page index of attribute group mapper list.</param>
        /// <param name="pageSize">Page size of attribute group mapper list.</param>
        /// <returns>List of associated attributes to attribute group.</returns>
        AttributeGroupMapperListModel GetAssociatedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Deletes an association of attribute and attribute group.
        /// </summary>
        /// <param name="attributeGroupMapperId">Attribute group mapper ID to be deleted.</param>
        /// <returns>Bool value according to the state of delete operation.</returns>
        bool DeleteAssociatedAttribute(int attributeGroupMapperId);

        /// <summary>
        /// Updates an attribute group mapper.
        /// </summary>
        /// <param name="model">Attribute group mapper model to be updated.</param>
        /// <returns>Updated attribute group mapper model.</returns>
        AttributeGroupMapperModel UpdateAttributeGroupMapper(AttributeGroupMapperModel model);

        /// <summary>
        /// Gets the list of attribute group locales.
        /// </summary>
        /// <param name="attributeGroupLocaleId">Attribute Group Locale Id</param>
        /// <returns>Returns list of attribute group locales.</returns>
        AttributeGroupLocaleListModel GetAttributeGroupLocales(int attributeGroupLocaleId);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="attributegroupId">Attribute group id to get the attributes.</param>
        /// <param name="expands">Expands</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        AttributesListDataModel GetUnAssignedAttributes(int attributegroupId, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
