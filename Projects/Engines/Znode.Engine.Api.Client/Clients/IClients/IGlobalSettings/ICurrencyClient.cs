using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ICurrencyClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Currencies.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <returns>Currency list model.</returns>
        CurrencyListModel GetCurrencyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Currencies.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <param name="pageIndex">Start page index of currency list.</param>
        /// <param name="pageSize">Page size of currency list.</param>
        /// <returns>Currency list model.</returns>
        CurrencyListModel GetCurrencyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a Currency as per the filter passed.
        /// </summary>
        /// <param name="filters">Filters to be applied to get currency.</param>
        /// <returns>Currency model.</returns>
        CurrencyModel GetCurrency(FilterCollection filters);

        /// <summary>
        /// Update Currencies.
        /// </summary>       
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateCurrency(DefaultGlobalConfigListModel globalConfigurationListModel);

        /// <summary>
        /// Gets the list of Culture.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Culture list.</param>
        /// <param name="filters">Filters to be applied on Culture list.</param>
        /// <param name="sorts">Sorting to be applied on Culture list.</param>
        /// <returns>Culture list model.</returns>
        CultureListModel GetCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Culture.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Culture list.</param>
        /// <param name="filters">Filters to be applied on Culture list.</param>
        /// <param name="sorts">Sorting to be applied on Culture list.</param>
        /// <param name="pageIndex">Start page index of Culture list.</param>
        /// <param name="pageSize">Page size of Culture list.</param>
        /// <returns>Culture list model.</returns>
        CultureListModel GetCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a Culture as per the filter passed.
        /// </summary>
        /// <param name="filters">Filters to be applied to get culture code.</param>
        /// <returns>Culture model.</returns>
        CultureModel GetCultureCode(FilterCollection filters);

        /// <summary>
        /// Gets the list of Currencies.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with currency list.</param>
        /// <param name="filters">Filters to be applied on currency list.</param>
        /// <param name="sorts">Sorting to be applied on currency list.</param>
        /// <param name="pageIndex">Start page index of currency list.</param>
        /// <param name="pageSize">Page size of currency list.</param>
        /// <returns>Currency list model.</returns>
        CurrencyListModel GetCurrencyCultureList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
