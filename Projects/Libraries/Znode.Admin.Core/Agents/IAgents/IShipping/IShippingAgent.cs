using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IShippingAgent
    {
        #region Shipping


        /// <summary>
        /// Get shipping list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with shipping list.</param>
        /// <param name="filters">Filters to be applied on shipping list.</param>
        /// <param name="sorts">Sorting to be applied on shipping list.</param>
        /// <param name="pageIndex">Start page index of shipping list.</param>
        /// <param name="pageSize">Records per page in shipping list.</param>
        /// <returns>Returns shipping list view model.</returns>
        ShippingListViewModel GetShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create shipping.
        /// </summary>
        /// <param name="model">Shipping view model to create.</param>
        /// <returns>Returns created shipping view model.</returns>
        ShippingViewModel CreateShipping(ShippingViewModel model);

        /// <summary>
        /// Get shipping on the basis of shipping id.
        /// </summary>
        /// <param name="shippingId">Shipping Id to get shipping details.</param>
        /// <returns>Returns ShippingViewModel.</returns>
        ShippingViewModel GetShippingById(int shippingId);

        /// <summary>
        /// Update shipping model.
        /// </summary>
        /// <param name="model">Shipping  view model to update.</param>
        /// <returns>Returns updated shipping view model.</returns>
        ShippingViewModel UpdateShipping(ShippingViewModel model);

        /// <summary>
        /// Delete shipping by shipping Ids.
        /// </summary>
        /// <param name="shippingId">Shipping Ids to be deleted.</param>
        /// <returns>True if shipping deleted successfully; false if shipping fails to delete.</returns>
        bool DeleteShipping(string shippingId, out string errorMessage);

        /// <summary>
        /// Get shipping service code as List of SelectListItem.
        /// </summary>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetShippingServiceCodeList(string className);

        /// <summary>
        /// Sets filter for shippingId.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="shippingId">shippingId.</param>
        void SetFilters(FilterCollection filters, int shippingId);

        /// <summary>
        /// Bind Dropdown values for ShippingType, Profile, Country
        /// </summary>
        /// <param name="viewModel">ShippingViewModel</param>
        ///  <param name="isEditMode">isEditMode</param>
        void BindDropdownValues(ShippingViewModel viewModel, bool isEditMode);

        /// <summary>
        /// Get State list by country code.
        /// </summary>
        /// <param name="countryCode">country code</param>
        /// <returns>Returns list of state.</returns>
        List<SelectListItem> GetStateListByCountryCode(string countryCode);

        /// <summary>
        /// Get city list by state code.
        /// </summary>
        /// <param name="stateCode">state code</param>
        /// <returns></returns>
        List<SelectListItem> GetCityListByStateCode(string stateCode);

        /// <summary>
        /// Set filter for shippingRuleId
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="shippingRuleId"></param>
        void SetShippingRuleFilters(FilterCollection filters, int shippingRuleId);

        /// <summary>
        /// Check whether the shipping name already exists.
        /// </summary>
        /// <param name="shippingName">shippingName</param>
        /// <param name="shippingId">shippingId</param>
        /// <returns>returns true if exists else false.</returns>
        bool CheckShippingNameExist(string shippingName, int shippingId);

        /// <summary>
        /// Get active currency list.
        /// </summary>
        /// <returns>bind active currency list in select list item.</returns>
        List<SelectListItem> GetActiveCurrency();

        /// <summary>
        /// For check shipping method.
        /// </summary>
        /// <param name="className">className</param>
        /// <returns>returns true if exists else false.</returns>
        bool CheckShippingMethod(string className);
        #endregion

        #region Shipping SKU

        /// <summary>
        /// Get Shipping SKU list from database.
        /// </summary>
        /// <param name="expands">Expands collection.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page Number.</param>
        /// <param name="recordPerPage">No. of records to be displayed on single page.</param>
        /// <returns>Returns ShippingSKUListViewModel</returns>
        ShippingSKUListViewModel GetShippingSKUList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Add Shipping SKU.
        /// </summary>
        /// <param name="viewModel">ViewModel to create Shipping SKU.</param>
        /// <returns>Returns added Shipping SKU View Model.</returns>
        ShippingSKUViewModel AddShippingSKU(ShippingSKUViewModel viewModel);

        /// <summary>
        /// Delete Shipping SKU on the basis of ShippingSKUId.
        /// </summary>
        /// <param name="shippingSKUId">Id to delete shipping sku detail.</param>
        /// <returns>Returns True/False on the basis of delete operation.</returns>
        bool DeleteShippingSKU(string shippingSKUId, out string message);

        /// <summary>
        /// Get shipping SKU list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel </param>
        /// <param name="shippingId">shippingId</param>
        /// <param name="shippingRuleId">shippingRuleId</param>
        /// <returns>Returns ShippingSKUListViewModel.</returns>
        ShippingSKUListViewModel GetShippingSKUListViewModel(FilterCollectionDataModel model, int shippingId, int shippingRuleId, string shippingRuleType);

        #endregion

        #region Shipping Rule

        /// <summary>
        /// Get Shipping Rule list from database.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page Number.</param>
        /// <param name="recordPerPage">No. of records to be displayed on single page.</param>
        /// <returns>Returns ShippingRuleListViewModel</returns>
        ShippingRuleListViewModel GetShippingRuleList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Shipping Rule by ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to get shipping rule detail.</param>
        /// <returns>Returns Shipping Rule ViewModel.</returns>
        ShippingRuleViewModel GetShippingRule(int shippingRuleId);

        /// <summary>
        /// Add Shipping Rule.
        /// </summary>
        /// <param name="viewModel">ViewModel to create Shipping Rule.</param>
        /// <returns>Returns added Shipping Rule View Model.</returns>
        ShippingRuleViewModel AddShippingRule(ShippingRuleViewModel viewModel);

        /// <summary>
        /// Update Shipping Rule.
        /// </summary>
        /// <param name="viewModel">ShippingRuleViewModel to be updated.</param>
        /// <returns>Returns updated Shipping Rule ViewModel.</returns>
        ShippingRuleViewModel UpdateShippingRule(ShippingRuleViewModel viewModel);

        /// <summary>
        /// Delete Shipping Rule on the basis of ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to delete shipping rule detail.</param>
        /// <returns>Returns True/False on the basis of delete operation.</returns>
        bool DeleteShippingRule(string shippingRuleId, out string message);

        /// <summary>
        /// Get Shipping rule types as List of SelectListItem.
        /// </summary>
        /// <param name="shippingId">Id to bind shipping rule list.</param>
        /// <param name="isEditMode">mode</param>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetShippingRuleTypes(int shippingId, bool isEditMode);

        /// <summary>
        /// Get shipping rule list with other property.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel Model</param>
        /// <param name="shippingId">shipping Id</param>
        /// <param name="shippingTypeId">shippingTypeId</param>
        /// <returns></returns>
        ShippingRuleListViewModel ShippingRuleList(FilterCollectionDataModel model, int shippingId, int shippingTypeId);
        #endregion

        /// <summary>
        /// Bind shipping option list.
        /// </summary>
        /// <param name="expands">Expands for Store.</param>
        /// <param name="filters">Filters for Store.</param>
        /// <param name="sorts">Sorts for Store.</param>
        /// <param name="profileList">customer associated profile</param>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Get filter based List of all shipping option</returns>
        ShippingListViewModel BindShippingList(CreateOrderViewModel createOrderViewModel, ProfileListModel profileList, int userId = 0, int portalId = 0);

        /// <summary>
        /// Get shipping Address from session.
        /// </summary>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetShippingAddress();

        /// <summary>
        /// For getting Shipping list.
        /// </summary>
        /// <param name="omsOrderId"> omsOrderId</param>
        /// <param name="profileList"> profileList </param>
        /// <returns>ShippingListViewModel </returns>
        ShippingListViewModel BindManageShippingList(int omsOrderId, ProfileListModel profileList);

        /// <summary>
        /// Set filter, expand and sorts for getting shipping list.
        /// </summary>
        /// <param name="expands">expand</param>
        /// <param name="sorts">sorts</param>
        /// <returns></returns>
        FilterCollection SetFilterDataForShippingList(UserAddressDataViewModel addressModel, ref ExpandCollection expands, ref SortCollection sorts);

        /// <summary>
        /// Get filters for manage order page.
        /// </summary>
        /// <param name="orderModel">orderModel</param>
        /// <returns>filters.</returns>
        FilterCollection GetFilterForShipping(OrderModel orderModel);

        
    }
}
