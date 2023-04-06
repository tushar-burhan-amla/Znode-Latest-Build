using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;

namespace Znode.Engine.Admin.Agents
{
    public interface ITaxClassAgent
    {
        /// <summary>
        /// Get the list of tax class.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns TaxClass list ViewModel.</returns>
        TaxClassListViewModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a tax class.
        /// </summary>
        /// <param name="taxClassId">ID of taxClass to get details of perticular TaxClass.</param>
        /// <returns>Returns TaxClass ViewModel.</returns>
        TaxClassViewModel GetTaxClass(int taxClassId);

        /// <summary>
        /// Create a new tax class.
        /// </summary>
        /// <param name="taxClassViewModel">Tax class view model to be created.</param>
        /// <returns>Returns newly created tax class view model.</returns>
        TaxClassViewModel CreateTaxClass(TaxClassViewModel taxClassViewModel);

        /// <summary>
        /// Update a tax class.
        /// </summary>
        /// <param name="taxClassViewModel">TaxClass ViewModel.</param>
        /// <returns>Returns true if TaxClass updated else returns false.</returns>
        TaxClassViewModel UpdateTaxClass(TaxClassViewModel taxClassViewModel);

        /// <summary>
        /// Delete a tax class.
        /// </summary>
        /// <param name="taxClassIds">ID of tax class to delete.</param>
        /// <param name="errorMessage">Error message while delete tax class.</param>
        /// <returns>Returns true if TaxClass deleted else returns false.</returns>
        bool DeleteTaxClass(string taxClassIds, out string errorMessage);

        /// <summary>
        /// Get tax rule types as List of SelectListItem.
        /// </summary>
        /// /// <param name="taxClassId">ID of tax class for filter already exists taxType.</param>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetTaxRuleTypes(int taxClassId);

        /// <summary>
        /// Sets filter for tax class id.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="taxClassId">TaxClassId.</param>
        void SetFilters(FilterCollection filters, int taxClassId);

        /// <summary>
        /// Get tax rule Details
        /// </summary>
        /// <param name="taxClassId">tax class Id.</param>
        /// <returns></returns>
        TaxRuleViewModel GetTaxRuleDetails(int taxClassId);

        #region Tax Class SKU

        /// <summary>
        /// Get Tax Class SKU list from database.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page Number.</param>
        /// <param name="recordPerPage">No. of records to be displayed on single page.</param>
        /// <returns>Returns TaxClassSKUListViewModel</returns>
        TaxClassSKUListViewModel GetTaxClassSKUList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Product Details list from database.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page Number.</param>
        /// <param name="recordPerPage">No. of records to be displayed on single page.</param>
        /// <returns>Returns ProductDetailsListViewModel</returns>
        ProductDetailsListViewModel GetUnassociatedProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Add Tax Class SKU.
        /// </summary>
        /// <param name="viewModel">ViewModel to create Tax Class SKU.</param>
        /// <returns>Returns added Tax Class SKU View Model.</returns>
        TaxClassSKUViewModel AddTaxClassSKU(TaxClassSKUViewModel viewModel);



        /// <summary>
        /// Delete Tax Class SKU on the basis of TaxClassSKUId.
        /// </summary>
        /// <param name="taxClassSKUId">Id to delete tax class sku detail.</param>
        /// <returns>Returns True/False on the basis of delete operation.</returns>
        bool DeleteTaxClassSKU(string taxClassSKUId, out string message);

        #endregion

        #region Tax Rule

        /// <summary>
        /// Get Tax Rule list from database.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="pageIndex">Page Number.</param>
        /// <param name="recordPerPage">No. of records to be displayed on single page.</param>
        /// <returns>Returns TaxRuleListViewModel</returns>
        TaxRuleListViewModel GetTaxRuleList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Tax Rule by TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to get tax rule detail.</param>
        /// <param name="taxClassId">Id to get tax class Id.</param>
        /// <returns>Returns TaxRule ViewModel.</returns>
        TaxRuleViewModel GetTaxRule(int taxRuleId, int taxClassId);

        /// <summary>
        /// Add Tax Rule.
        /// </summary>
        /// <param name="viewModel">ViewModel to create Tax Rule.</param>
        /// <returns>Returns added Tax Rule View Model.</returns>
        TaxRuleViewModel AddTaxRule(TaxRuleViewModel viewModel);

        /// <summary>
        /// Update Tax Rule.
        /// </summary>
        /// <param name="viewModel">TaxRuleViewModel to be updated.</param>
        /// <returns>Returns updated TaxRule ViewModel.</returns>
        TaxRuleViewModel UpdateTaxRule(TaxRuleViewModel viewModel);

        /// <summary>
        /// Delete Tax Rule on the basis of TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to delete tax rule detail.</param>
        /// <returns>Returns True/False on the basis of delete operation.</returns>
        bool DeleteTaxRule(string taxRuleId, out string message);

        /// <summary>
        /// Bind Dropdown Values for country
        /// </summary>
        /// <returns>Returns list of active countries</returns>
        List<SelectListItem> BindCountryList();

        /// <summary>
        /// Bind Dropdown Values for State
        /// </summary>
        /// <param name="countryCode">Selected State Code</param>
        /// <returns>Returns list of states</returns>
        List<SelectListItem> BindStateList(string countryCode);

        /// <summary>
        /// Bind Dropdown Values for City
        /// </summary>
        /// <param name="stateCode">Selected state code</param>
        /// <returns>Returns list of cities</returns>
        List<SelectListItem> BindCityList(string stateCode = "");

        /// <summary>
        /// Get tax
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="taxClassId">taxClassId</param>
        /// <param name="name">name</param>
        /// <returns></returns>
        TaxClassSKUListViewModel GetTaxClassSKUListViewModel(FilterCollectionDataModel model, int taxClassId, string name);
        #endregion

    }
}
