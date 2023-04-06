using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class LocaleAgent : BaseAgent, ILocaleAgent
    {
        #region Private Variables
        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public LocaleAgent(ILocaleClient localeClient)
        {
            _localeClient = GetClient<ILocaleClient>(localeClient);
        }
        #endregion

        #region public Methods
        //Method to get Locale list
        public virtual LocaleListViewModel GetLocales(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sortCollection: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });

            LocaleListModel localeList = _localeClient.GetLocaleList(expands, filters, sortCollection, pageIndex, recordPerPage);
            LocaleListViewModel listViewModel = new LocaleListViewModel { Locales = localeList?.Locales?.ToViewModel<LocaleViewModel>().ToList() };
            SetListPagingData(listViewModel, localeList);

            //Set tool menu for locale list grid view.
            SetLocaleListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return localeList?.Locales?.Count > 0 ? listViewModel : new LocaleListViewModel() { Locales = new List<LocaleViewModel>() };
        }
        

        //Update Locale
        public virtual bool UpdateLocale(DefaultGlobalConfigViewModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            message = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _localeClient.UpdateLocale(DefaultGlobalConfigViewModelMap.ToGlobalConfigurationListModel(model));
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
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        //Get the select list of locales.
        public virtual List<SelectListItem> GetLocalesList(int localeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter localeId: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { localeId = localeId });

            LocaleListModel localeList = _localeClient.GetLocaleList(null, new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") }, null, null, null);

            if (localeList?.Locales?.Count > 0)
            {
                if (localeId == 0)
                    localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return localeList.Locales.Select(x => new SelectListItem { Text = x.Name, Value = x.LocaleId.ToString(), Selected = Equals(x.LocaleId, localeId) }).ToList();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return new List<SelectListItem>();
        }

        #endregion

        #region Private Methods
        //Set tool menu for Locale list grid view.
        private void SetLocaleListToolMenu(LocaleListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.IsActive, JSFunctionName = "GlobalConfiguration.prototype.ActiveSubmit('','Locale','Update','List')", ControllerName = "Locale", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "GlobalConfiguration.prototype.DeActivateSubmit('','Locale','Update','List')", ControllerName = "Locale", ActionName = "List" });
            }
        }
        #endregion
    }
}