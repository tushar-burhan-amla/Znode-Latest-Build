using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Exceptions;
using Znode.Libraries.Resources;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Extensions;
using System.Linq;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using System;
using Newtonsoft.Json;

namespace Znode.Engine.Admin.Agents
{
    public class CountryAgent : BaseAgent, ICountryAgent
    {
        #region Private Variables
        private readonly ICountryClient _countryClient;
        #endregion

        #region Constructor
        public CountryAgent(ICountryClient countryClient)
        {
            _countryClient = GetClient<ICountryClient>(countryClient);
        }
        #endregion

        #region Public Methods

        //Method to get Country list
        public virtual CountryListViewModel GetCountries(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sortCollection:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });
            if (HelperUtility.IsNull(sortCollection))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeCountryEnum.CountryName.ToString(), DynamicGridConstants.ASCKey);
            }

            CountryListModel countryList = _countryClient.GetCountryList(expands, filters, sortCollection, pageIndex, recordPerPage);
            CountryListViewModel listViewModel = new CountryListViewModel { Countries = countryList?.Countries?.ToViewModel<CountryViewModel>().ToList() };
            SetListPagingData(listViewModel, countryList);

            //Set tool menu for country list grid view.
            SetCountryListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return countryList?.Countries?.Count > 0 ? listViewModel : new CountryListViewModel() { Countries = new List<CountryViewModel>() };
        }

        //Update Country
        public virtual bool UpdateCountry(DefaultGlobalConfigViewModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            message = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _countryClient.UpdateCountry(DefaultGlobalConfigViewModelMap.ToGlobalConfigurationListModel(model));
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

        public virtual List<SelectListItem> GetActiveCountryList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            CountryListViewModel countryList = GetCountries(null, filters);

            List<SelectListItem> selectedCountryList = new List<SelectListItem>();
            if (countryList?.Countries?.Count > 0)
                countryList.Countries.ForEach(item => { selectedCountryList.Add(new SelectListItem() { Text = item.CountryName, Value = item.CountryCode }); });
            ZnodeLogging.LogMessage("selectedCountryList count: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, selectedCountryList?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return selectedCountryList;
        }

        #region Private Methods
        //Set tool menu for country list grid view.
        private void SetCountryListToolMenu(CountryListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.IsActive, JSFunctionName = "GlobalConfiguration.prototype.ActiveSubmit('','Country','Update','List')", ControllerName = "Country", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "GlobalConfiguration.prototype.DeActivateSubmit('','Country','Update','List')", ControllerName = "Country", ActionName = "List" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = GlobalSetting_Resources.IsDefault, JSFunctionName = "GlobalConfiguration.prototype.DefaultSubmit('','Country','Update','List')", ControllerName = "Country", ActionName = "List" });
            }
        }
        #endregion

        #endregion
    }
}