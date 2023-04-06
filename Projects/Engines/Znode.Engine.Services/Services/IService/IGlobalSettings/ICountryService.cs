using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICountryService
    {
        /// <summary>
        /// Gets a list of countries.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with country list.</param>
        /// <param name="filters">Filters to be applied on country list.</param>
        /// <param name="sorts">Sorting to be applied on country list.</param>
        /// <returns>Country list model.</returns>
        CountryListModel GetCountries(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets country as per filter passed.
        /// </summary>
        /// <param name="filters">Filter to be applied to get country.</param>
        /// <returns>Country model.</returns>
         CountryModel GetCountry(FilterCollection filters);

        /// <summary>
        /// Updates Country.
        /// </summary>
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateCountry(DefaultGlobalConfigListModel globalConfigListModel);
    }
}
