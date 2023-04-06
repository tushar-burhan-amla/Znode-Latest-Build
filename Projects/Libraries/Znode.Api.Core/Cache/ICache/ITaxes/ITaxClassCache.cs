namespace Znode.Engine.Api.Cache
{
    public interface ITaxClassCache
    {
        #region Taxes

        /// <summary>
        /// Get the list of all tax classes.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>list of all tax classes in string format.</returns>
        string GetTaxClassList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a Tax Class.
        /// </summary>
        /// <param name="taxClassId">taxClassId to get TaxClass.</param>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Returns TaxClass Model in string format.</returns>
        string GetTaxClass(int taxClassId, string routeUri, string routeTemplate);

        #endregion Taxes

        #region Tax Class SKU

        /// <summary>
        /// Get Tax Class SKU list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Tax Class SKU.</returns>
        string GetTaxClassSKUList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Product list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of Product.</returns>
        string GetUnassociatedProductList(string routeUri, string routeTemplate);

        #endregion Tax Class SKU

        #region Tax Rule

        /// <summary>
        /// Get Tax Rule from Cache By TaxRuleId.
        /// </summary>
        /// <param name="taxRuleId">Id to get Tax Rule detail.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Tax Rule on the basis of TaxRuleId.</returns>
        string GetTaxRule(int taxRuleId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Tax Rule list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Tax Rule.</returns>
        string GetTaxRuleList(string routeUri, string routeTemplate);

        #endregion Tax Rule
    }
}