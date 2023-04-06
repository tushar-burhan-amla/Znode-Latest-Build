using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Client.Expands;

namespace Znode.Engine.Api.Client
{
    public interface IShippingClient : IBaseClient
    {
        #region Shipping

        /// <summary>
        /// Get shipping on the basis of shipping id.
        /// </summary>
        /// <param name="shippingId">Shipping Id to get shipping details.</param>
        /// <returns>Returns ShippingModel.</returns>
        ShippingModel GetShipping(int shippingId);

        /// <summary>
        /// Gets the list of Shipping.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with shipping list.</param>
        /// <param name="filters">Filters to be applied on shipping list.</param>
        /// <param name="sorts">Sorting to be applied on shipping list.</param>
        /// <param name="pageIndex">Start page index of shipping list.</param>
        /// <param name="pageSize">Page size of shipping list.</param>
        /// <returns>Returns shipping list.</returns>
        ShippingListModel GetShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create shipping.
        /// </summary>
        /// <param name="model">Shipping model to create.</param>
        /// <returns>Returns created shipping model.</returns>
        ShippingModel CreateShipping(ShippingModel model);

        /// <summary>
        /// Update shipping model.
        /// </summary>
        /// <param name="model">Shipping model to update.</param>
        /// <returns>Returns updated shipping model.</returns>
        ShippingModel UpdateShipping(ShippingModel model);

        /// <summary>
        /// Delete shipping by shipping Ids.
        /// </summary>
        /// <param name="shippingId">Shipping Ids to be deleted.</param>
        /// <returns>True if shipping deleted successfully; false if shipping fails to delete.</returns>
        bool DeleteShipping(ParameterModel shippingId);
        #endregion

        #region Shipping SKU

        /// <summary>
        /// Get Shipping SKU list.
        /// </summary>
        /// <param name="expands">Expands for associated item.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns Shipping SKU List Model.</returns>
        ShippingSKUListModel GetShippingSKUList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Add Shipping SKU. 
        /// </summary>
        /// <param name="shippingSKUModel">Model to create new Shipping SKU.</param>
        /// <returns>Returns added Shipping SKU Model.</returns>
        ShippingSKUModel AddShippingSKU(ShippingSKUModel shippingSKUModel);


        /// <summary>
        /// Delete existing Shipping SKU.
        /// </summary>
        /// <param name="shippingSKUId">shippingSKUId to delete Shipping SKU.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteShippingSKU(string shippingSKUId);

        #endregion

        #region Shipping Service Code

        /// <summary>
        /// Get shipping service code by Id.
        /// </summary>
        /// <param name="shippingId">Id of shipping service code.</param>
        /// <returns>Returns ShippingServiceCodeModel.</returns>
        ShippingServiceCodeModel GetShippingServiceCode(int shippingServiceCodeId);

        /// <summary>
        /// Get shipping service code list 
        /// </summary>
        /// <param name="filters">Collection of filter.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Return paged shipping service code list. </returns>
        ShippingServiceCodeListModel GetShippingServiceCodeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        #endregion

        #region Shipping Rule

        /// <summary>
        /// Get Shipping Rule list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns Shipping Rule List Model.</returns>
        ShippingRuleListModel GetShippingRuleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Shipping Rule by ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to get shipping rule details.</param>
        /// <returns>Shipping Rule Model</returns>
        ShippingRuleModel GetShippingRule(int shippingRuleId);

        /// <summary>
        /// Add Shipping Rule. 
        /// </summary>
        /// <param name="shippingRuleModel">Model to create new Shipping Rule.</param>
        /// <returns>Returns added Shipping Rule Model.</returns>
        ShippingRuleModel AddShippingRule(ShippingRuleModel shippingRuleModel);

        /// <summary>
        /// Update Shipping Rule.
        /// </summary>
        /// <param name="shippingRuleModel">Model to update existing Shipping Rule.</param>
        /// <returns>Returns updated ShippingRuleModel.</returns>
        ShippingRuleModel UpdateShippingRule(ShippingRuleModel shippingRuleModel);

        /// <summary>
        /// Delete existing Shipping Rule.
        /// </summary>
        /// <param name="shippingRuleId">shippingRuleId to delete Shipping Rule.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteShippingRule(string shippingRuleId);

        #endregion

        #region Shipping Rule Type

        /// <summary>
        /// Get shipping rule type list 
        /// </summary>
        /// <param name="filters">Collection of filter.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <returns>Return shipping rule type list. </returns>
        ShippingRuleTypeListModel GetShippingRuleTypeList(FilterCollection filters, SortCollection sorts);

        #endregion

        #region Portal/Profile Shipping
        /// <summary>
        /// Get associated shipping list for Portal/Profile.
        /// </summary>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Portal/Profile shipping .</returns>
        ShippingListModel GetAssociatedShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get list of unassociated shipping for Portal/Profile.
        /// </summary>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated shipping list.</returns>
        ShippingListModel GetUnAssociatedShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

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
        /// Check shipping address is valid or not.
        /// </summary>
        /// <param name="model">AddressModel</param>
        /// <returns>Returns model with status true / false.</returns>
        BooleanModel IsShippingAddressValid(AddressModel model);

        /// <summary>
        /// Get recommended address list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>AddressListModel</returns>
        AddressListModel RecommendedAddress(AddressModel model);

        /// <summary>
        /// Update profile shipping
        /// </summary>
        /// <param name="model">Portal Profile Shipping Model</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateProfileShipping(PortalProfileShippingModel model);

        /// <summary>
        /// Update Shipping To Portal.
        /// </summary>
        /// <param name="portalProfileShippingModel">portalProfileShippingModel</param>
        /// <returns>portalProfileShippingModel</returns>
        bool UpdateShippingToPortal(PortalProfileShippingModel portalProfileShippingModel);
    }
}
