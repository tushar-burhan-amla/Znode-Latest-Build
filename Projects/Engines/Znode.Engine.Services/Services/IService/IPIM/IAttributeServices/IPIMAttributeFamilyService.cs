using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPIMAttributeFamilyService
    {
        /// <summary>
        /// Get the List of all attribute families
        /// </summary>
        /// <param name="expands">collection of Expands</param>
        /// <param name="filters">list of filters tuples</param>
        /// <param name="sort">collection of sorts</param>
        /// <param name="page">collection of paging parameters</param>
        /// <returns>List of Attribute families</returns>
        PIMAttributeFamilyListModel GetAttributeFamilyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new Attribute family
        /// </summary>
        /// <param name="model">PIMAttributeFamilyModel</param>
        /// <returns>PIMAttributeFamilyModel</returns>
        PIMAttributeFamilyModel Create(PIMAttributeFamilyModel model);

        /// <summary>
        /// Get the list of all attribute groups which are assigned to PIM attribute families.
        /// </summary>
        /// <param name="expands">Collection of Expands</param>
        /// <param name="filters">list of filter tuples</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">collection of paging parameters</param>
        /// <returns>List of attribute groups</returns>
        PIMAttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute List Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssignAttributeGroups(PIMFamilyGroupAttributeListModel listModel);

        /// <summary>
        /// UnAssociate attribute groups to PIM attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">attributefamily Id</param>
        /// <param name="attributeGroupId">attribute group to remove.</param>
        /// <param name="isCategory">Identification flag for category and product.</param>
        /// <returns>boolean value true/false</returns>
        bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory);

        /// <summary>
        /// Delete attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute family IDs to delete attribute family.</param>
        /// <returns>Boolean status value true/false.</returns>
        bool DeleteAttributeFamily(ParameterModel attributeFamilyId);

        /// <summary>
        /// Get detail of Attribute family on the basis of pimAttributeFamilyId.
        /// </summary>
        /// <param name="pimAttributeFamilyId">To get pim attribute family details.</param>
        /// <returns>Returns PIMAttributeFamilyModel.</returns>
        PIMAttributeFamilyModel GetAttributeFamily(int pimAttributeFamilyId);

        /// <summary>
        /// Get UnAssigned Attribute Groups.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns PIMAttributeGroupListModel.</returns>
        PIMAttributeGroupListModel GetUnAssignedAttributeGroups(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get family locale by family id.
        /// </summary>
        /// <param name="attributeFamilyId">PIM Attribute family Id.</param>
        /// <returns>Returns PIMFamilyLocaleListModel.</returns>
        PIMFamilyLocaleListModel GetFamilyLocale(int attributeFamilyId);

        /// <summary>
        /// Save locales for Attribute Family.
        /// </summary>
        /// <param name="model">PIMFamilyLocaleListModel Model.</param>
        /// <returns>Returns saved locales.</returns>
        PIMFamilyLocaleListModel SaveLocales(PIMFamilyLocaleListModel model);

        /// <summary>
        /// Get attribute list by attribute group ids.
        /// </summary>
        /// <param name="attributeGroupId">Attribute Group Ids.</param>
        /// <returns>Returns list of attribute group ids.</returns>
        PIMAttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="pimAttributeGroupModel"></param>
        /// <returns></returns>
        bool UpdateAttributeGroupDisplayOrder(PIMAttributeGroupModel pimAttributeGroupModel);

        #region Attributes
        /// <summary>
        /// Get the list of all attributes which are assigned to attribute groups.
        /// </summary>
        /// <param name="expands">Collection of Expands.</param>
        /// <param name="filters">List of filter tuples.</param>
        /// <param name="sorts">Collection of sorts.</param>
        /// <param name="page">collection of paging parameters.</param>
        /// <returns>List of attributes.</returns>
        PIMAttributeListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get unassigned attributes.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns PIMAttributeListModel.</returns>
        PIMAttributeListModel GetUnAssignedAttributes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate attributes to attribute group.
        /// </summary>
        /// <param name="model">Attribute data Model</param>
        /// <returns>Boolean value true/false</returns>
        bool AssignAttributes(AttributeDataModel model);

        /// <summary>
        /// Unassociate attribute from group.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns true or false.</returns>
        bool UnAssignAttributes(AttributeDataModel model);
        #endregion
    }
}
