using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IGlobalAttributeGroupService
    {
        /// <summary>
        /// Gets the list of Global Attribute group List.
        /// </summary>
        /// <param name="expands">Expands along with Global Attribute List.</param>
        /// <param name="filters">Filters for Global Attribute list.</param>
        /// <param name="sorts">Sorting for Global Attribute list.</param>
        /// <param name="page">Paging to be applied for Global Attribute Group List.</param>
        /// <returns>List of Global Attribute Group List.</returns>
        GlobalAttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Creates Global Attribute Group.
        /// </summary>
        /// <param name="model">Global Attribute group Model to be created.</param>
        /// <returns>New Global Attribute Group model.</returns>
        GlobalAttributeGroupModel Create(GlobalAttributeGroupModel model);

        /// <summary>
        /// Gets Global Attribute Group according to ID.
        /// </summary>
        /// <param name="id">ID of Global Attribute group list.</param>
        /// <param name="expands">Expands for Global Attribute Group.</param>
        /// <returns>Returns Global Attribute Group Model for the specified ID</returns>
        GlobalAttributeGroupModel GetAttributeGroupById(int Id, NameValueCollection expands);

        /// <summary>
        /// Get attribute group locale.
        /// </summary>
        /// <param name="attributeGroupId">attribute group locale id.</param>
        /// <param name="expands">Expands collection</param>
        /// <returns>Returns attribute group locale.</returns>
        GlobalAttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands);

        /// <summary>
        /// Gets the assigned attributes list.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="page">Page</param>
        /// <returns>Returns list of assigned attributes.</returns>
        GlobalAttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the unassigned attributes list.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="page">Page</param>
        /// <returns>Returns list of unassigned attributes.</returns>
        GlobalAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Update Global Attributed Group Model.
        /// </summary>
        /// <param name="model">Global attribute group model to be updated.</param>
        /// <returns>True or false.</returns>
        bool Update(GlobalAttributeGroupModel model);

        /// <summary>
        /// Associate attributes
        /// </summary>
        /// <param name="model">Global attribute group mapper list model.</param>
        /// <returns>Returns the inserted records.</returns>
        GlobalAttributeGroupMapperListModel AssociateAttributes(GlobalAttributeGroupMapperListModel model);

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="model">Global attribute data model.</param>
        /// <returns></returns>
        bool UpdateAttributeDisplayOrder(GlobalAttributeDataModel model);

        /// <summary>
        /// Remove associated attributes.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns>Returns true if removed otherwise false.</returns>
        bool RemoveAssociatedAttributes(RemoveGroupAttributesModel model);

        /// <summary>
        /// Delete Global Attribute Group.
        /// </summary>
        /// <param name="id">ID of the Global Atribute group to be deleted.</param>
        /// <returns>True or false.</returns>
        bool Delete(ParameterModel attributeGroupIds);
    }
}
