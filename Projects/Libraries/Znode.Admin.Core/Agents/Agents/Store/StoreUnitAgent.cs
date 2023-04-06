using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using System;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class StoreUnitAgent : BaseAgent, IStoreUnitAgent
    {
        #region Private Variables
        private readonly IPortalUnitClient _portalUnitClient;
        private readonly ICurrencyClient _currencyClient;
        private readonly IWeightUnitClient _weightClient;
        private readonly IDefaultGlobalConfigClient _defaultGlobalConfigClient;
        #endregion

        #region Constructor
        public StoreUnitAgent(IPortalUnitClient portalUnitClient, ICurrencyClient currencyClient, IWeightUnitClient weightClient, IDefaultGlobalConfigClient defaultGlobalConfigClient)
        {
            _portalUnitClient = GetClient<IPortalUnitClient>(portalUnitClient);
            _currencyClient = GetClient<ICurrencyClient>(currencyClient);
            _weightClient = GetClient<IWeightUnitClient>(weightClient);
            _defaultGlobalConfigClient = GetClient<IDefaultGlobalConfigClient>(defaultGlobalConfigClient);
        }
        #endregion

        #region Public Methods
        public virtual PortalUnitViewModel GetStoreUnit(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            PortalUnitViewModel portalUnitViewModel = _portalUnitClient.GetPortalUnit(portalId)?.ToViewModel<PortalUnitViewModel>();
            if (HelperUtility.IsNull(portalUnitViewModel))
            {
                portalUnitViewModel = new PortalUnitViewModel();
                if (Equals(portalUnitViewModel?.CurrencyTypeID, null) && string.IsNullOrEmpty(portalUnitViewModel?.DimensionUnit) && string.IsNullOrEmpty(portalUnitViewModel?.WeightUnit))
                {
                    SetCurrencyDetails(portalUnitViewModel);
                    portalUnitViewModel.WeightUnit = DefaultSettingHelper.DefaultWeightUnit;
                    portalUnitViewModel.DimensionUnit = GetDimensionUnits().Where(x => x.Text == "IN").Select(x => x.Text).SingleOrDefault();

                    FilterCollection filters = new FilterCollection();
                    filters.Add(new FilterTuple(ZnodeCurrencyEnum.CurrencyCode.ToString(), FilterOperators.EndsWith, DefaultSettingHelper.DefaultCurrency?.Split('-')[1]));
                    portalUnitViewModel.CurrencyTypeID = _currencyClient.GetCurrency(filters)?.CurrencyId;
                }
            }

            portalUnitViewModel.CurrencyTypes = GetCurrencyList(null, null, null);
            portalUnitViewModel.DimensionUnits = GetDimensionUnits();
            portalUnitViewModel.WeightUnits = GetWeightUnits(null, null, null);

            if (!Equals(portalUnitViewModel?.CurrencyTypeID, null))
            {
                PortalUnitViewModel currencyInfo = GetCurrencyInformationByCurrencyId(portalUnitViewModel.CurrencyTypeID.GetValueOrDefault(), portalUnitViewModel.CurrencyTypeID.GetValueOrDefault());
                portalUnitViewModel.CurrencyPreview = currencyInfo.CurrencyPreview;
                portalUnitViewModel.CurrencySuffix = currencyInfo.CurrencySuffix;
                portalUnitViewModel.Culture = currencyInfo.Culture;
            }
            ZnodeLogging.LogMessage("PortalUnitId, PortalId, CurrencyTypeID and CurrencyName properties of portalUnitViewModel to be returned: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalUnitViewModel?.PortalUnitId, portalUnitViewModel?.PortalId, portalUnitViewModel?.CurrencyTypeID, portalUnitViewModel?.CurrencyName });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalUnitViewModel;
        }

        public virtual PortalUnitViewModel CreateUpdateStoreUnit(PortalUnitViewModel portalUnitViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                portalUnitViewModel.CultureId = portalUnitViewModel.CultureId == 0 ? null : portalUnitViewModel.CultureId;
                PortalUnitModel portalUnitModel = _portalUnitClient.CreateUpdatePortalUnit(portalUnitViewModel.ToModel<PortalUnitModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(portalUnitModel) ? portalUnitModel?.ToViewModel<PortalUnitViewModel>() : new PortalUnitViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new PortalUnitViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (PortalUnitViewModel)GetViewModelWithErrorMessage(portalUnitViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        public virtual PortalUnitViewModel GetCurrencyInformationByCurrencyId(int currencyId, int oldCurrency, int cultureId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters currencyId, oldCurrency and cultureId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { currencyId, oldCurrency, cultureId });

            PortalUnitViewModel portalUnitViewModel = new PortalUnitViewModel();

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(FilterKeys.CurrencyId, FilterOperators.Equals, currencyId.ToString()));
            filter.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));

            portalUnitViewModel.Culture = GetCultureCodeList(currencyId, null, filter, null);
            cultureId = portalUnitViewModel.Culture?.FindIndex(x => x.Value == cultureId.ToString()) > 0 ? cultureId : Convert.ToInt32(portalUnitViewModel.Culture?.FirstOrDefault()?.Value ?? "0");
            CurrencyModel currencyType = _currencyClient.GetCurrency(filter);
            filter.RemoveAll(x => string.Equals(x.FilterName, ZnodeCurrencyEnum.CurrencyId.ToString(), StringComparison.CurrentCultureIgnoreCase));

            filter.Add(new FilterTuple(ZnodeConstant.CultureId.ToString(), FilterOperators.Equals, cultureId > 0 ? cultureId.ToString() : portalUnitViewModel.CultureId.GetValueOrDefault().ToString()));
            ZnodeLogging.LogMessage("Input parameter filter of method GetCultureCode:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { filter });
            CultureModel culture = _currencyClient.GetCultureCode(filter);

            if (portalUnitViewModel.Culture.Count > 0)
            {
                portalUnitViewModel.CurrencySuffix = currencyType.CurrencyCode;

                CultureInfo info = null;
                try { info = new CultureInfo(culture?.CultureCode); } catch (Exception ex) { ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error); }
                GetCurrencyInfo(info, portalUnitViewModel);
            }
            ZnodeLogging.LogMessage("PortalUnitId, PortalId, OldCurrencyId and CurrencyName properties of portalUnitViewModel to be returned: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalUnitViewModel?.PortalUnitId, portalUnitViewModel?.PortalId, portalUnitViewModel?.OldCurrencyId, portalUnitViewModel?.CurrencyName });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalUnitViewModel;
        }

        public virtual PortalUnitViewModel GetCurrencyInformationByCultureCode(int currencyId, int cultureId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters currencyId and cultureId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { currencyId, cultureId });
            PortalUnitViewModel portalUnitViewModel = new PortalUnitViewModel();

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(FilterKeys.CurrencyId, FilterOperators.Equals, currencyId.ToString()));
            filter.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));
            portalUnitViewModel.Culture = GetCultureCodeList(currencyId, null, filter, null);
            CurrencyModel currencyType = _currencyClient.GetCurrency(filter);
            filter.Add(new FilterTuple(ZnodeConstant.CultureId.ToString(), FilterOperators.Equals, cultureId.ToString()));
            ZnodeLogging.LogMessage("Input parameter filter of method GetCultureCode:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { filter });
            CultureModel culture = _currencyClient.GetCultureCode(filter);
            if (portalUnitViewModel.Culture.Count > 0)
            {
                portalUnitViewModel.CurrencySuffix = currencyType.CurrencyCode;

                CultureInfo info = null;
                try { info = new CultureInfo(culture.CultureCode); } catch (Exception ex) { ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error); }
                GetCurrencyInfo(info, portalUnitViewModel);
            }
            ZnodeLogging.LogMessage("PortalUnitId, PortalId, OldCurrencyId and CurrencyName properties of portalUnitViewModel to be returned: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { portalUnitViewModel?.PortalUnitId, portalUnitViewModel?.PortalId, portalUnitViewModel?.OldCurrencyId, portalUnitViewModel?.CurrencyName });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalUnitViewModel;
        }

        public virtual List<SelectListItem> GetWeightUnits(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
               => PortalUnitViewModelMap.ToSelectListItemsforWeight(_weightClient.GetWeightUnitList(Expands, Filters, Sorts).WeightUnits);

        public virtual List<SelectListItem> GetCurrencyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

            if (HelperUtility.IsNotNull(sorts))
                sorts.Add(ZnodeCurrencyEnum.CurrencyName.ToString(), DynamicGridConstants.ASCKey);
            else
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeCurrencyEnum.CurrencyName.ToString(), DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Input parameters expands, filter and sorts of method GetCurrencyList:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sorts });
            return PortalUnitViewModelMap.ToSelectListItemsforCurrency(_currencyClient.GetCurrencyList(expands, filters, sorts).Currencies);
        }

        public virtual List<SelectListItem> GetCultureCodeList(int currencyId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            if (HelperUtility.IsNotNull(sorts))
                sorts.Add(ZnodeConstant.CultureName.ToString(), DynamicGridConstants.ASCKey);
            else
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeConstant.CultureName.ToString(), DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Input parameters expands, filter and sorts of method GetCultureList:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sorts });
            return PortalUnitViewModelMap.ToSelectListItemsforCurrencyCulture(_currencyClient.GetCultureList(expands, filters, sorts).Culture);
        }

        public virtual List<SelectListItem> GetDimensionUnits()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "IN" , Value = "IN"},
                new SelectListItem() { Text = "CM", Value ="CM"}
            };
        }
        #endregion

        #region Private Methods
        private void SetCurrencyDetails(PortalUnitViewModel portalUnitViewModel)
         => GetCurrencyInfo(new CultureInfo(DefaultSettingHelper.DefaultCulture), portalUnitViewModel);

        private void GetCurrencyInfo(CultureInfo info, PortalUnitViewModel portalUnitViewModel)
        {
            decimal price = 100.12M;
            portalUnitViewModel.CurrencyPreview = price.ToString("c", info?.NumberFormat);

            if (portalUnitViewModel?.CurrencySuffix?.Trim()?.Length > 0)
                portalUnitViewModel.CurrencyPreview = string.Concat(portalUnitViewModel.CurrencyPreview, " (", portalUnitViewModel.CurrencySuffix, ")");
        }
        #endregion
    }
}