using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ITaxClassService
    {
        /// <summary>
        /// Get the list of all tax classes.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">Collection  of paging parameters.</param>
        /// <returns>List of TaxClass Model.</returns>
        TaxClassListModel GetTaxClassList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get a Tax Class.
        /// </summary>
        /// <param name="taxClassId">ID of taxClass to get details of perticular TaxClass.</param>
        /// <returns>Returns TaxClass Model.</returns>
        TaxClassModel GetTaxClass(int taxClassId);

        /// <summary>
        /// Creates a new Tax Class.
        /// </summary>
        /// <param name="taxClassModel">TaxClass model to be created.</param>
        /// <returns>Returns newly created TaxClass Model.</returns>
        TaxClassModel CreateTaxClass(TaxClassModel taxClassModel);

        /// <summary>
        /// Update a Tax Class.
        /// </summary>
        /// <param name="taxClassModel">TaxClass Model.</param>
        /// <returns>Returns true if TaxClass updated else returns false.</returns>
        bool UpdateTaxClass(TaxClassModel taxClassModel);

        /// <summary>
        /// Delete a Tax Class.
        /// </summary>
        /// <param name="taxClassId">ID of tax class to delete.</param>
        /// <returns>Returns true if TaxClass deleted else returns false.</returns>
        bool DeleteTaxClass(ParameterModel taxClassId);

        #region Tax Class SKU

        /// <summary>
        /// Get Tax Class SKU list.
        /// </summary>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page index and Page size for Associated Item.</param>
        /// <returns>Returns TaxClassSKUListModel</returns>
        TaxClassSKUListModel GetTaxClassSKUList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get product list from database.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns ProductDetailsListModel</returns>
        ProductDetailsListModel GetUnassociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Add Tax Class SKU model.
        /// </summary>
        /// <param name="taxClassSKUModel">Model to add new Tax Class SKU.</param>
        /// <returns>Returns added TaxClassSKU model.</returns>
        TaxClassSKUModel AddTaxClassSKU(TaxClassSKUModel taxClassSKUModel);

        /// <summary>
        /// Delete existing Tax Class SKU.
        /// </summary>
        /// <param name="taxClassSKUId">Id to delete Tax Class SKU.</param>
        /// <returns>Returns true or false on the basis of delete operation.</returns>
        bool DeleteTaxClassSKU(ParameterModel taxClassSKUId);

        #endregion Tax Class SKU

        #region Tax Rule

        /// <summary>
        /// Get Tax Rule list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page index and Page size for Associated Item.</param>
        /// <returns>Returns TaxRuleListModel</returns>
        TaxRuleListModel GetTaxRuleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Tax Rule by TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to get Tax Rule detail.</param>
        /// <returns>Returns TaxRuleModel</returns>
        TaxRuleModel GetTaxRule(int taxRuleId);

        /// <summary>
        /// Add Tax Rule model.
        /// </summary>
        /// <param name="taxRuleModel">Model to add new Tax Rule.</param>
        /// <returns>Returns added TaxRule model.</returns>
        TaxRuleModel AddTaxRule(TaxRuleModel taxRuleModel);

        /// <summary>
        /// Update Tax Rule.
        /// </summary>
        /// <param name="taxRuleModel">Model to update existing Tax Rule.</param>
        /// <returns>Returns true or false on the basis of update operation.</returns>
        bool UpdateTaxRule(TaxRuleModel taxRuleModel);

        /// <summary>
        /// Delete existing Tax Rule.
        /// </summary>
        /// <param name="taxRuleId">Id to delete Tax Rule.</param>
        /// <returns>Returns true or false on the basis of delete operation.</returns>
        bool DeleteTaxRule(ParameterModel taxRuleId);

        #endregion Tax Rule

        #region Avalara
        /// <summary>
        /// Test the avalara connection.
        /// </summary>
        /// <param name="taxportalModel">Tax portal model containing credentials.</param>
        /// <returns>Message.</returns>
        string TestAvalaraConnection(TaxPortalModel taxportalModel);
        #endregion
    }
}