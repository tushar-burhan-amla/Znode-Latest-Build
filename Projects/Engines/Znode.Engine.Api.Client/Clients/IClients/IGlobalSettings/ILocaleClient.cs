using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ILocaleClient : IBaseClient
    {
        /// <summary>
        ///  Get Locale list.
        /// </summary>
        /// <param name="expands">Expands for Locale</param>
        /// <param name="filters">Filters for Locale</param>
        /// <param name="sorts">Sorts for Locale</param>
        /// <returns>Returns LocaleListModel</returns>
        LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Locale list.
        /// </summary>
        /// <param name="expands">Expands for Locale  </param>
        /// <param name="filters">Filters for Locale</param>
        /// <param name="sorts">Sorts for Locale</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>Returns LocaleListModel</returns>
        LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Locale
        /// </summary>
        /// <param name="Filters">Filters for Locale</param>
        /// <returns>Returns Locale Model</returns>
        LocaleModel GetLocale(FilterCollection filters);

        /// <summary>
        /// Updates Locales.
        /// </summary>       
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateLocale(DefaultGlobalConfigListModel globalConfigurationListModel);
    }
}
