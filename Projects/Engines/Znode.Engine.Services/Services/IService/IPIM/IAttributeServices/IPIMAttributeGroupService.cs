using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPIMAttributeGroupService
    {
        /// <summary>
        /// Gets the list of PIM Attribute group List.
        /// </summary>
        /// <param name="expands">Expands along with PIM Attribute List.</param>
        /// <param name="filters">Filters for PIM Attribute list.</param>
        /// <param name="sorts">Sorting for PIM Attribute list.</param>
        /// <param name="page">Paging to be applied for PIM Attribute Group List.</param>
        /// <returns>List of PIM Attribute Group List.</returns>
        PIMAttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets PIM Attribute Group according to ID.
        /// </summary>
        /// <param name="pimAttributeGroupId">ID of PIM Attribute group list.</param>
        /// <param name="expands">Expands for PIM Attribute Group.</param>
        /// <returns>Returns PIM Attribute Group Model for the specified ID</returns>
        PIMAttributeGroupModel GetAttributeGroupById(int pimAttributeGroupId, NameValueCollection expands);

        /// <summary>
        /// Update PIM Attributed Group Model.
        /// </summary>
        /// <param name="pimAttributeGroupModel">PIM attribute group model to be updated.</param>
        /// <returns>True or false.</returns>
        bool UpdateAttributeGroup(PIMAttributeGroupModel pimAttributeGroupModel);

        /// <summary>
        /// Creates PIM Attribute Group.
        /// </summary>
        /// <param name="pimAttributeGroupModel">PIM Attribute group Model to be created.</param>
        /// <returns>New PIM Attribute Group model.</returns>
        PIMAttributeGroupModel Create(PIMAttributeGroupModel pimAttributeGroupModel);

        /// <summary>
        /// Delete PIM Attribute Group.
        /// </summary>
        /// <param name="id">ID of the PIMAtribute group to be deleted.</param>
        /// <returns>True or false.</returns>
        bool Delete(ParameterModel attributeGroupIds);

        /// <summary>
        /// Gets the assigned attributes list.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="page">Page</param>
        /// <returns>Returns list of assigned attributes.</returns>
        PIMAttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the unassigned attributes list.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="page">Page</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        PIMAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get attribute group locale.
        /// </summary>
        /// <param name="attributeGroupId">attribute group locale id.</param>
        /// <param name="expands">Expands collection</param>
        /// <returns>Returns attribute group locale.</returns>
        PIMAttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands);

        /// <summary>
        /// Save the locales value.
        /// </summary>
        /// <param name="model">Model to save.</param>
        /// <returns>Updated model.</returns>
        PIMAttributeGroupLocaleListModel SaveAttributeGroupLocales(PIMAttributeGroupLocaleListModel pimAttributeGroupLocaleListModel);

        /// <summary>
        /// Associate attributes
        /// </summary>
        /// <param name="model">PIM attribute group mapper list model.</param>
        /// <returns>Returns the inserted records.</returns>
        PIMAttributeGroupMapperListModel AssociateAttributes(PIMAttributeGroupMapperListModel model);

        /// <summary>
        /// Remove associated attributes.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns>Returns true if removed otherwise false.</returns>
        bool RemoveAssociatedAttributes(RemoveAssociatedAttributesModel model);

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="attributeDataModel"></param>
        /// <returns></returns>
        bool UpdateAttributeDisplayOrder(PIMAttributeDataModel attributeDataModel);
    }
}
