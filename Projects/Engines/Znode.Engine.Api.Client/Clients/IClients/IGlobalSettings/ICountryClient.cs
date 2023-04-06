using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ICountryClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of countries.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with country list.</param>
        /// <param name="filters">Filters to be applied on country list.</param>
        /// <param name="sorts">Sorting to be applied on country list.</param>
        /// <returns>Country list model.</returns>
        CountryListModel GetCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of countries.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with country list.</param>
        /// <param name="filters">Filters to be applied on country list.</param>
        /// <param name="sorts">Sorting to be applied on country list.</param>
        /// <param name="pageIndex">Start page index of country list.</param>
        /// <param name="pageSize">Page size of country list.</param>
        /// <returns>Country list model.</returns>
        CountryListModel GetCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a country as per the filter passed.
        /// </summary>
        /// <param name="filters">Filters to be applied to get country.</param>
        /// <returns>Country model.</returns>
        CountryModel GetCountry(FilterCollection filters);

        /// <summary>
        /// Update Countries.
        /// </summary>       
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateCountry(DefaultGlobalConfigListModel globalConfigurationListModel);
    }
}
