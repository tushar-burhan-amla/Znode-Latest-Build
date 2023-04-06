using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IStoreUnitAgent
    {
        /// <summary>
        /// Get StoreUnit for specified portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get Portal Unit.</param>
        /// <returns>StoreUnit ViewModel.</returns>
        PortalUnitViewModel GetStoreUnit(int portalId);

        /// <summary>
        ///  Update StoreUnit associated to portal.
        /// </summary>
        /// <param name="portalUnitViewModel">PortalUnitViewModel to update StoreUnit.</param>
        /// <returns>StoreUnit ViewModel.</returns>
        PortalUnitViewModel CreateUpdateStoreUnit(PortalUnitViewModel portalUnitViewModel);

        /// <summary>
        /// Get Currency information by currency id.
        /// </summary>
        /// <param name="currencyId">currencyId to get information of specified id.</param>
        /// <param name="oldCurrency">currencyId before update.</param>
        /// <param name="currencySuffix">currency suffix.</param>
        /// <returns>returns currency info.</returns>
        PortalUnitViewModel GetCurrencyInformationByCurrencyId(int currencyId, int oldCurrency, int cultureId = 0);

        /// <summary>
        /// Get Currency List.
        /// </summary>
        /// <param name="expands">expand collection accross currency.</param>
        /// <param name="filters">filter list accross currency.</param>
        /// <param name="sorts">sort collection for currency.</param>
        /// <returns></returns>
        List<SelectListItem> GetCurrencyList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null);

        /// <summary>
        /// Get Weight Unit List.
        /// </summary>
        /// <param name="expands">expand collection accross weight.</param>
        /// <param name="filters">filter list accross weight</param>
        /// <param name="sorts">sort collection for weight</param>
        /// <returns></returns>
        List<SelectListItem> GetWeightUnits(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null);

        /// <summary>
        /// Get Dimension Unit List.
        /// </summary>
        List<SelectListItem> GetDimensionUnits();

        /// <summary>
        /// Get Currency information by currency id.
        /// </summary>
        /// <param name="currencyId">currencyId to get information of specified id.</param>
        /// <param name="oldCurrency">currencyId before update.</param>
        /// <param name="currencySuffix">currency suffix.</param>
        /// <returns>returns currency info.</returns>
        PortalUnitViewModel GetCurrencyInformationByCultureCode(int currencyId, int cultureId);
    }
}
