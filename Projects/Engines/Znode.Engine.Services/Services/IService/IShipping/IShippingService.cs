using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using System.Collections.Generic;

namespace Znode.Engine.Services
{
    public interface IShippingService
    {
        #region Shipping 

        /// <summary>
        /// Get Shipping on the basis of shipping id.
        /// </summary>
        /// <param name="shippingId">Shipping Id.</param>
        /// <returns>Returns Shipping Model.</returns>
        ShippingModel GetShipping(int shippingId);

        /// <summary>
        /// Gets the list of Shipping.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with shipping list.</param>
        /// <param name="filters">Filters to be applied on shipping list.</param>
        /// <param name="sorts">Sorting to be applied on shipping list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of shipping.</returns>
        ShippingListModel GetShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        ///  Get All shipping list by userId and portalId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns>Returns shipping list.</returns>
        List<ShippingModel> GetShippingListByUserDetails(int userId, int portalId);

        /// <summary>
        /// Create Shipping.
        /// </summary>
        /// <param name="model">Shipping Model.</param>
        /// <returns>Returns created Shipping Model.</returns>
        ShippingModel CreateShipping(ShippingModel model);

        /// <summary>
        /// Update Shipping data.
        /// </summary>
        /// <param name="model">Shipping model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateShipping(ShippingModel model);

        /// <summary>
        /// Delete shipping as per shipping Ids.
        /// </summary>
        /// <param name="shippingId">Shipping Ids to be deleted.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteShipping(ParameterModel shippingId);
        #endregion 

        #region Shipping SKU

        /// <summary>
        /// Get Shipping SKU list.
        /// </summary>
        /// <param name="expands">Expands for Associated Item.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page index and Page size for Associated Item.</param>
        /// <returns>Returns ShippingSKUListModel</returns>
        ShippingSKUListModel GetShippingSKUList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        #endregion

        #region Shipping ServiceCode

        /// <summary>
        /// Get shipping service code list.
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filter list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>Returns ShippingServiceCodeListModel </returns>
        /// <returns></returns>
        ShippingServiceCodeListModel GetShippingServiceCodeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get shipping service code by Id.
        /// </summary>
        /// <param name="shippingServiceCodeId">Id of shipping service code.</param>
        /// <returns>Returns ShippingServiceCodeModel</returns>
        ShippingServiceCodeModel GetShippingServiceCode(int shippingServiceCodeId);

        #endregion

        #region Shipping Rule

        /// <summary>
        /// Get Shipping Rule list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page index and Page size for Associated Item.</param>
        /// <returns>Returns ShippingRuleListModel</returns>
        ShippingRuleListModel GetShippingRuleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Shipping Rule by ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to get Shipping Rule detail.</param>
        /// <returns>Returns ShippingRuleModel</returns>
        ShippingRuleModel GetShippingRule(int shippingRuleId);

        /// <summary>
        /// Add Shipping Rule model. 
        /// </summary>
        /// <param name="shippingRuleModel">Model to add new Shipping Rule.</param>
        /// <returns>Returns added ShippingRule model.</returns>
        ShippingRuleModel AddShippingRule(ShippingRuleModel shippingRuleModel);

        /// <summary>
        /// Update Shipping Rule.
        /// </summary>
        /// <param name="shippingRuleModel">Model to update existing Shipping Rule.</param>
        /// <returns>Returns true or false on the basis of update operation.</returns>
        bool UpdateShippingRule(ShippingRuleModel shippingRuleModel);

        /// <summary>
        /// Delete existing Shipping Rule.
        /// </summary>
        /// <param name="shippingRuleId">Id to delete Shipping Rule.</param>
        /// <returns>Returns true or false on the basis of delete operation.</returns>
        bool DeleteShippingRule(ParameterModel shippingRuleId);

        #endregion

        #region Shipping Rule Type

        /// <summary>
        /// Get shipping rule type list.
        /// </summary>
        /// <param name="filters">filter list</param>
        /// <param name="sorts">sort list</param>
        /// <returns>Returns ShippingRuleTypeListModel </returns>
        /// <returns></returns>
        ShippingRuleTypeListModel GetShippingRuleTypeList(FilterCollection filters, NameValueCollection sorts);

        #endregion


        #region Portal/Profile Shipping 
        /// <summary>
        ///  Get associated shipping list for Portal/Profile.
        /// </summary>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of shipping associated to Portal/Profile.</returns>
        ShippingListModel GetAssociatedShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated shipping to Portal/Profile.
        /// </summary>
        /// <param name="filters">Filters for shipping.</param>
        /// <param name="sorts">Sorts for for shipping.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of unassociated shipping list.</returns>
        ShippingListModel GetUnAssociatedShippingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate shipping to Portal/Profile.
        /// </summary>
        /// <param name="portalProfileShippingModel">PortalProfileShippingModel</param>
        /// <returns>Returns true if shipping associated successfully else return false.</returns>
        bool AssociateShipping(PortalProfileShippingModel portalProfileShippingModel);

        /// <summary>
        /// Remove associated shipping from Portal/Profile.
        /// </summary>
        /// <param name="portalProfileShippingModel">portalProfileShippingModel</param>
        /// <returns>Returns true if shipping unassociated successfully else return false.</returns>
        bool UnAssociateAssociatedShipping(PortalProfileShippingModel portalProfileShippingModel);
        #endregion

        /// <summary>
        /// Check whether the address is valid or not.
        /// </summary>
        /// <param name="address">Address object to validate using USPS.</param>
        /// <returns>Returns model with IsSuccess flag which if true then address is valid otherwise false.</returns>
        BooleanModel IsShippingAddressValid(AddressModel addressModel);

        /// <summary>
        /// Get recommended address list.
        /// </summary>
        /// <param name="addressModel"></param>
        /// <returns>AddressListModel</returns>
        AddressListModel RecommendedAddress(AddressModel addressModel);

        /// <summary>
        /// Update profile shipping.
        /// </summary>
        /// <param name="model">Portal Profile Shipping Model</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateProfileShipping(PortalProfileShippingModel model);

        /// <summary>
        /// Update Shipping To Portal
        /// </summary>
        /// <param name="portalProfileShippingModel">portalProfileShippingModel</param>
        /// <returns>true or false</returns>
        bool UpdateShippingToPortal(PortalProfileShippingModel portalProfileShippingModel);

        /// <summary>
        /// Validating Model 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true or false</returns>
        bool ValidateRecommendedAddressModel(AddressModel model);
    }
}
