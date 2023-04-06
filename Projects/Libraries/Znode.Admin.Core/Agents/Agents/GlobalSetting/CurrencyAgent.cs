using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using System;

namespace Znode.Engine.Admin.Agents
{
    public class CurrencyAgent : BaseAgent, ICurrencyAgent
    {
        #region Private Variables
        private readonly ICurrencyClient _currencyClient;
        #endregion

        #region Constructor
        public CurrencyAgent(ICurrencyClient currencyClient)
        {
            _currencyClient = GetClient<ICurrencyClient>(currencyClient);
        }
        #endregion

        #region public Methods

        //Method to get Currency list
        public virtual CurrencyListViewModel GetCurrencies(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sortCollection: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });

            if (HelperUtility.IsNull(sortCollection))   
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeCurrencyEnum.CurrencyName.ToString(), DynamicGridConstants.ASCKey);
            }
            CurrencyListModel currencyList = _currencyClient.GetCurrencyCultureList(expands, filters, sortCollection, pageIndex, recordPerPage);
            CurrencyListViewModel listViewModel = new CurrencyListViewModel { Currencies = currencyList?.Currencies?.ToViewModel<CurrencyViewModel>().ToList() };
            SetListPagingData(listViewModel, currencyList);

            //Set tool menu for currency list grid view.
            SetCurrencyListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return currencyList?.Currencies?.Count > 0 ? listViewModel : new CurrencyListViewModel() { Currencies = new List<CurrencyViewModel>() };
        }       

        //Update Currency
        public virtual bool UpdateCurrency(DefaultGlobalConfigViewModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            message = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _currencyClient.UpdateCurrency(DefaultGlobalConfigViewModelMap.ToGlobalConfigurationListModel(model));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                switch (ex.ErrorMessage)
                {
                    case "ErrorDefault":
                        message = GlobalSetting_Resources.ErrorDefault;
                        return false;
                    case "ErrorDeactivate":
                        message = GlobalSetting_Resources.ErrorDeactivate;
                        return false;
                    default:
                        message = Admin_Resources.UpdateErrorMessage;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        #endregion

        #region Private Methods
        //Set tool menu for Currency list grid view.
        private void SetCurrencyListToolMenu(CurrencyListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.IsActive, JSFunctionName = "GlobalConfiguration.prototype.ActiveSubmit('','Currency','Update','List')", ControllerName = "Currency", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "GlobalConfiguration.prototype.DeActivateSubmit('','Currency','Update','List')", ControllerName = "Currency", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = GlobalSetting_Resources.IsDefault, JSFunctionName = "GlobalConfiguration.prototype.DefaultSubmit('','Currency','Update','List')", ControllerName = "Currency", ActionName = "List" });
            }
        }
        #endregion
    }
}