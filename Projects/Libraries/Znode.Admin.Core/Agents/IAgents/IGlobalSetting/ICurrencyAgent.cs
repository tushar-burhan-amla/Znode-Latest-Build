using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface ICurrencyAgent
    {
        /// <summary>
        /// Get the list of all Currencies.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns> Currency List View Model</returns>
        CurrencyListViewModel GetCurrencies(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Method to Update Currency in database.
        /// </summary>
        /// <param name="model">DefaultGlobalConfigViewModel model</param>
        /// <returns>Returns true/false</returns>
        bool UpdateCurrency(DefaultGlobalConfigViewModel model, out string message);
    }
}