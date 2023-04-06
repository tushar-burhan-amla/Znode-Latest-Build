using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAttributeGroupService
    {
        /// <summary>
        /// Gets a list of Attribute groups.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Attribute group list.</param>
        /// <returns>List of attribute group model.</returns>
        AttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
      
        /// <summary>
        /// Gets attribute group using attributeGroupIs passed.
        /// </summary>
        /// <param name="attributeGroupId">Attribute group ID to be retrieved.</param>
        /// <returns>Attribute group model.</returns>
        AttributeGroupModel GetAttributeGroup(int attributeGroupId);

        /// <summary>
        /// Creates an attribute group.
        /// </summary>
        /// <param name="model">Attribute group model to be created.</param>
        /// <returns>Newly created attribute group model.</returns>
        AttributeGroupModel CreateAttributeGroup(AttributeGroupModel model);

        /// <summary>
        /// Updates an attribute group.
        /// </summary>
        /// <param name="model">Updated model of an attribute group.</param>
        /// <returns>Bool value according the status of update operation..</returns>
        bool UpdateAttributeGroup(AttributeGroupModel model);

        /// <summary>
        /// Deletes an attribute group.
        /// </summary>
        /// <param name="attributeGroupId">ID of an attribute group to be deleted.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteAttributeGroup(ParameterModel attributeGroupIds);

        /// <summary>
        /// Assigns a list of attribute to Attribute Group.
        /// </summary>
        /// <param name="attributeGroupMapperList">List of attribute and group mapper.</param>
        /// <returns>Returns associated attribute groups mapper model.</returns>
        AttributeGroupMapperListModel AssignAttributes(AttributeGroupMapperListModel attributeGroupMapperList);

        /// <summary>
        /// Gets list of attributes assigned to an attribute group.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with attribute group mapper model list.</param>
        /// <param name="filters">Filters to be applied on attribute group mapper model list.</param>
        /// <param name="sorts">Sorts to be applied on attribute group mapper model list.</param>
        /// <param name="page">Paging information for attribute group mapper model list.</param>
        /// <returns>List of attribute group mapper list model.</returns>
        AttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Deletes an attribute to attribute group association.
        /// </summary>
        /// <param name="attributeGroupMapperId">Attribute group mapper ID to be deleted.</param>
        /// <returns>Bool value according the status of delete operation.</returns>
        bool DeleteAssociatedAttribute(int attributeGroupMapperId);

        /// <summary>
        /// Updates an attribute group mapper.
        /// </summary>
        /// <param name="model">Attribute group mapper model to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateAttributeGroupMapper(AttributeGroupMapperModel model);

        /// <summary>
        /// Get attribute group locale.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <param name="expands">Expand collection.</param>
        /// <returns>Returns attribute group locale list.</returns>
        AttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands);

        /// <summary>
        /// Gets unassign attributes.
        /// </summary>
        /// <param name="expands">Expand collection.</param>
        /// <param name="filters">Filters to be applied on list.</param>
        /// <param name="sorts">Sorts to be applied on model list.</param>
        /// <param name="page">Paging information for attribute model list.</param>
        /// <returns>Returns attributes list data model.</returns>
        AttributesListDataModel GetUnAssignedAttributes(int attributeGroupId, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page);
    }
}
