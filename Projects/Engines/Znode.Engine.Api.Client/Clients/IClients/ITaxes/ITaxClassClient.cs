using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ITaxClassClient : IBaseClient
    {
        #region Tax Class

        /// <summary>
        /// Get the list of tax class.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns Tax Class list model.</returns>
        TaxClassListModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a tax class.
        /// </summary>
        /// <param name="taxClassId">ID of taxClass to get details of perticular TaxClass.</param>
        /// <returns>Returns Tax class model.</returns>
        TaxClassModel GetTaxClass(int taxClassId);

        /// <summary>
        /// Create a new tax class.
        /// </summary>
        /// <param name="taxClassModel">Tax class model to be created.</param>
        /// <returns>Returns newly created tax class model.</returns>
        TaxClassModel CreateTaxClass(TaxClassModel taxClassModel);

        /// <summary>
        /// Update a Tax Class.
        /// </summary>
        /// <param name="taxClassModel">TaxClass Model.</param>
        /// <returns>Returns TaxClass updated model.</returns>
        TaxClassModel UpdateTaxClass(TaxClassModel taxClassModel);

        /// <summary>
        /// Delete a Tax Class.
        /// </summary>
        /// <param name="taxClassIds">ID of tax class to delete.</param>
        /// <returns>Returns true if TaxClass deleted else returns false.</returns>
        bool DeleteTaxClass(ParameterModel taxClassIds);

        #endregion Tax Class

        #region Tax Class SKU

        /// <summary>
        /// Get Tax Class SKU list.
        /// </summary>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns Tax Class SKU List Model.</returns>
        TaxClassSKUListModel GetTaxClassSKUList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Add Tax Class SKU.
        /// </summary>
        /// <param name="taxClassSKUModel">Model to create new Tax Class SKU.</param>
        /// <returns>Returns added Tax Class SKU Model.</returns>
        TaxClassSKUModel AddTaxClassSKU(TaxClassSKUModel taxClassSKUModel);

        /// <summary>
        /// Delete existing Tax Class SKU.
        /// </summary>
        /// <param name="taxClassSKUId">taxClassSKUId to delete Tax Class SKU.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteTaxClassSKU(string taxClassSKUId);

        /// <summary>
        /// Get unassociated product list.
        /// </summary>
        /// <param name="expands">Expands for Product  </param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetUnassociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        #endregion Tax Class SKU

        #region Tax Rule

        /// <summary>
        /// Get Tax Rule list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns Tax Rule List Model.</returns>
        TaxRuleListModel GetTaxRuleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Tax Rule by TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to get tax rule details.</param>
        /// <returns>Tax Rule Model</returns>
        TaxRuleModel GetTaxRule(int taxRuleId);

        /// <summary>
        /// Add Tax Rule.
        /// </summary>
        /// <param name="taxRuleModel">Model to create new Tax Rule.</param>
        /// <returns>Returns added Tax Rule Model.</returns>
        TaxRuleModel AddTaxRule(TaxRuleModel taxRuleModel);

        /// <summary>
        /// Update Tax Rule.
        /// </summary>
        /// <param name="taxRuleModel">Model to update existing Tax Rule.</param>
        /// <returns>Returns updated TaxRuleModel.</returns>
        TaxRuleModel UpdateTaxRule(TaxRuleModel taxRuleModel);

        /// <summary>
        /// Delete existing Tax Rule.
        /// </summary>
        /// <param name="taxRuleId">taxRuleId to delete Tax Rule.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteTaxRule(string taxRuleId);

        #endregion Tax Rule

        #region Avalara
        /// <summary>
        ///  Test the avalara connection.
        /// </summary>
        /// <param name="taxPortalModel">Tax portal model containing credentials.</param>
        /// <returns>Message.</returns>
        string TestAvalaraConnection(TaxPortalModel taxPortalModel);
        #endregion
    }
}