using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IAttributeFamilyClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all attribute families.
        /// </summary>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sort</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of attribute families</returns>
        AttributeFamilyListModel GetAttributeFamilyList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This method create new attribute Family
        /// </summary>
        /// <param name="model">AttributeFamilyModel</param>
        /// <returns>AttributeFamilyModel</returns>
        AttributeFamilyModel CreateAttributeFamily(AttributeFamilyModel model);

        /// <summary>
        /// Get the list of all attribute groups which are assigned to attribute families.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of attribute groups</returns>
        AttributeGroupListModel GetAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute List Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssociateAttributeGroups(FamilyGroupAttributeListModel listModel);

        /// <summary>
        /// UnAssociate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute Model</param>
        /// <returns>boolean value true/false</returns>
        bool UnAssociateAttributeGroups(int attributeFamilyId, int attributeGroupId);

        /// <summary>
        /// Delete multiple attribute families.
        /// </summary>
        /// <param name="attributeFamilyId">Parameter model of Attribute Family Ids.</param>
        /// <returns>Returns true is families deleted successfully else return false.</returns>
        bool DeleteAttributeFamily(ParameterModel attributeFamilyId);

        /// <summary>
        /// Get detail of attribute family on the basis of attributeFamilyId
        /// </summary>
        /// <param name="attributeFamilyId">To get attribute family details</param>
        /// <returns>Attribute Family Model</returns>
        AttributeFamilyModel GetAttributeFamily(int attributeFamilyId);

        #region Family Locale
        /// <summary>
        /// Get family locale on the basis of attribute family id.
        /// </summary>
        /// <param name="attributeFamilyId">Media attribute family id.</param>
        /// <returns>Returns FamilyLocaleListModel.</returns>
        FamilyLocaleListModel GetFamilyLocale(int attributeFamilyId);

        /// <summary>
        /// Save locales.
        /// </summary>
        /// <param name="model">FamilyLocaleListModel.</param>
        /// <returns>Returns saved locales.</returns>
        FamilyLocaleListModel SaveLocales(FamilyLocaleListModel model);

        /// <summary>
        /// Get list of unassigned attribute groups.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns list of unassigned attribute groups.</returns>
        AttributeGroupListModel GetUnAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get attributes by attribute group id.
        /// </summary>
        /// <param name="attributeGroupId">Attribute Group Id.</param>
        /// <returns>Returns list of attributes.</returns>
        AttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId);
        #endregion
    }
}
