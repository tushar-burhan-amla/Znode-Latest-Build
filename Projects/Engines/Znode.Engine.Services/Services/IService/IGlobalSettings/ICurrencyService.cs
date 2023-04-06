using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICurrencyService
    {
        /// <summary>
        /// Gets a list of currencies.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <returns>Currency list model.</returns>
        CurrencyListModel GetCurrencies(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets currency as per filter passed.
        /// </summary>
        /// <param name="filters">Filter to be applied to get currency.</param>
        /// <returns>Currency model.</returns>
        CurrencyModel GetCurrency(FilterCollection filters);

        /// <summary>
        /// Updates Currency.
        /// </summary>
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateCurrency(DefaultGlobalConfigListModel globalConfigListModel);

        /// <summary>
        /// Gets a list of Culture.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <returns>Culture list model.</returns>
        CultureListModel GetCulture(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets Culture as per filter passed.
        /// </summary>
        /// <param name="filters">Filter to be applied to get Culture.</param>
        /// <returns>Currency model.</returns>
        CultureModel GetCultureCode(FilterCollection filters);

        /// <summary>
        /// Gets a list of currencies.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <returns>Currency list model.</returns>
        CurrencyListModel CurrencyCultureList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get currency detail for portal
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        CurrencyModel GetCurrencyDetail(int portalId);

    }
}
