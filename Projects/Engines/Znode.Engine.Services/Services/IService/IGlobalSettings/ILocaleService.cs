using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ILocaleService
    {
        /// <summary>
        /// Gets a list of Locale.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Locale list.</param>
        /// <param name="filters">Filters to be applied on Locale list.</param>
        /// <param name="sorts">Sorting to be applied on Locale list.</param>
        /// <returns>Returns Locale list model.</returns>
        LocaleListModel GetLocaleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets Locale.
        /// </summary>
        /// <returns>Returns Locale model.</returns>
        LocaleModel GetLocale(FilterCollection filters);

        /// <summary>
        /// Updates Locale.
        /// </summary>
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateLocale(DefaultGlobalConfigListModel globalConfigListModel);
    }
}
