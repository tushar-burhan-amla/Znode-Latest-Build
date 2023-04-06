using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface ILocaleAgent
    {

        /// <summary>
        /// Get the list of all Locales.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Locale List View Model</returns>
        LocaleListViewModel GetLocales(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Method to Update Locale in database.
        /// </summary>
        /// <param name="model">DefaultGlobalConfigViewModel model</param>
        /// <returns>Returns true/false</returns>
        bool UpdateLocale(DefaultGlobalConfigViewModel model, out string message);

        /// <summary>
        /// Get the select list of locales.
        /// </summary>
        /// <returns>Returns select list of locales.</returns>
        List<SelectListItem> GetLocalesList(int localeId = 0);
    }
}