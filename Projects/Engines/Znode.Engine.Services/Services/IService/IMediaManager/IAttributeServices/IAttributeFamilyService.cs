using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAttributeFamilyService
    {
        /// <summary>
        /// Get the list of all attribute families.
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sort</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of attribute families</returns>
        AttributeFamilyListModel GetAttributeFamilyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This method create new attribute Family
        /// </summary>
        /// <param name="model">AttributeFamilyModel</param>
        /// <returns>AttributeFamilyModel</returns>
        AttributeFamilyModel Create(AttributeFamilyModel model);

        /// <summary>
        /// Get the list of all attribute groups which are assigned to attribute families.
        /// </summary>
        /// <param name="expands">Collection of Expands</param>
        /// <param name="filters">list of filter tuples</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of attribute groups</returns>
        AttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute List Model</param>
        /// <returns>boolean value true/false</returns>
        bool AssignAttributeGroups(FamilyGroupAttributeListModel listModel);

        /// <summary>
        /// UnAssociate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">Family Group Attribute Model</param>
        /// <returns>boolean value true/false</returns>
        bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId);

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

        /// <summary>
        /// Get list of unassigned attribute groups.
        /// </summary>
        /// <param name="filters">Filter tuple.</param>
        /// <param name="sorts">NameValueCollection sorts</param>
        /// <param name="page">NameValueCollection page</param>
        /// <returns>Returns list of unassigned attribute groups.</returns>
        AttributeGroupListModel GetUnAssignedAttributeGroups(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

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
        /// Get attributes by attribute group id.
        /// </summary>
        /// <param name="attributeGroupId">Attribute Group Id.</param>
        /// <returns>Returns list of attributes.</returns>
        AttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId);
        #endregion
    }
}
