using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPIMAttributeFamilyClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all attribute families.
        /// </summary>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sort</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of attribute families</returns>
        PIMAttributeFamilyListModel GetAttributeFamilyList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This method create new attribute Family
        /// </summary>
        /// <param name="model">AttributeFamilyModel</param>
        /// <returns>AttributeFamilyModel</returns>
        PIMAttributeFamilyModel CreateAttributeFamily(PIMAttributeFamilyModel model);

        /// <summary>
        /// Get the list of all attribute groups which are assigned to attribute families.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of attribute groups</returns>
        PIMAttributeGroupListModel GetAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute List Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssociateAttributeGroups(PIMFamilyGroupAttributeListModel listModel);

        /// <summary>
        /// UnAssociate attribute groups to attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">attributefamily Id</param>
        /// <param name="attributeGroupId">attribute group to remove.</param>
        /// <param name="isCategory">Identification flag for category and product.</param>
        /// <returns>boolean value true/false</returns>
        bool UnAssociateAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory);

        /// <summary>
        /// Delete PIM Attribute Family.
        /// </summary>
        /// <param name="attributeFamilyId">String of Family Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAttributeFamily(ParameterModel attributeFamilyId);

        /// <summary>
        /// Get detail of attribute family on the basis of attributeFamilyId
        /// </summary>
        /// <param name="attributeFamilyId">To get attribute family details</param>
        /// <returns>Attribute Family Model</returns>
        PIMAttributeFamilyModel GetAttributeFamily(int attributeFamilyId);

        /// <summary>
        /// Get unassigned attribute groups.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns PIMAttributeGroupListModel.</returns>
        PIMAttributeGroupListModel GetUnAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Family locale by family id.
        /// </summary>
        /// <param name="attributeFamilyId">PIM attribute Family Id.</param>
        /// <returns>Returns PIMFamilyLocaleListModel.</returns>
        PIMFamilyLocaleListModel GetFamilyLocale(int attributeFamilyId);

        /// <summary>
        /// Save Locales for PIM Attribute Family.
        /// </summary>
        /// <param name="model">PIMFamilyLocaleListModel Model.</param>
        /// <returns>Returns saved locales.</returns>
        PIMFamilyLocaleListModel SaveLocales(PIMFamilyLocaleListModel model);

        /// <summary>
        /// Get attribute group list by multiple group ids.
        /// </summary>
        /// <param name="attributeGroupId">Attribute Group Ids.</param>
        /// <returns>Return list of attribute ids.</returns>
        PIMAttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId);

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="attributeGroupModel"></param>
        /// <returns></returns>
        PIMAttributeGroupModel UpdateAttributeGroupDisplayOrder(PIMAttributeGroupModel attributeGroupModel);

        #region Attributes.
        /// <summary>
        /// Get the list of all attributes which are assigned to attribute groups.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Collection of filters.</param>
        /// <param name="sorts">Collection of sorts.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>List of attributes.</returns>
        PIMAttributeListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of unassigned attributes.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Collection of filters.</param>
        /// <param name="sorts">Collection of sorts.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>List of unassigned attributes.</returns>
        PIMAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate attributes to attribute group.
        /// </summary>
        /// <param name="model">Attribute data model.</param>
        /// <returns>Boolean value true/false</returns>
        bool AssignAttributes(AttributeDataModel model);

        /// <summary>
        /// Unassign attribute from group.
        /// </summary>
        /// <param name="model">Attribute model with required data.</param>
        /// <returns>Returns true if unassigned successfully else return false.</returns>
        bool UnAssignAttributes(AttributeDataModel model); 
        #endregion
    }
}
