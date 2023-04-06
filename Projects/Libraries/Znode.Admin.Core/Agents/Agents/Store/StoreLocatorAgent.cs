using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using System.Web.Mvc;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class StoreLocatorAgent : BaseAgent, IStoreLocatorAgent
    {
        #region Private Variables.
        private readonly IStoreLocatorClient _storeLocatorClient;
        private readonly IPortalClient _portalClient;
        #endregion

        #region public Constructor.
        public StoreLocatorAgent(IStoreLocatorClient storeLocatorClient, IPortalClient portalClient)
        {
            _storeLocatorClient = GetClient<IStoreLocatorClient>(storeLocatorClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
        }
        #endregion

        //Get Store List for location.
        public virtual StoreLocatorListViewModel GetStoreLocatorList(FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (IsNull(model.Expands))
            {
                model.Expands = new ExpandCollection();
                model.Expands.Add(ZnodePortalAddressEnum.ZnodeAddress.ToString());
            }

            if (IsNull(model.SortCollection))
            {
                model.SortCollection = new Api.Client.Sorts.SortCollection();
                model.SortCollection.Add(ZnodePortalAddressEnum.DisplayOrder.ToString(), "Asc");
            }
            //Get Store List for locationfrom client.
            StoreLocatorListModel list = _storeLocatorClient.GetStoreLocatorList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            StoreLocatorListViewModel listViewModel = new StoreLocatorListViewModel { StoreLocatorList = list?.StoreLocatorList?.ToViewModel<StoreLocatorDataViewModel>().ToList() };

            SetListPagingData(listViewModel, list);

            //Set tool menu for store locator list grid view.
            SetStoreLocatorListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return listViewModel?.StoreLocatorList?.Count > 0 ? listViewModel
                : new StoreLocatorListViewModel { StoreLocatorList = new List<StoreLocatorDataViewModel>() };
        }

        //Get:data for create view.  
        public virtual StoreLocatorDataViewModel Create()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            StoreLocatorDataViewModel viewModel = new StoreLocatorDataViewModel();
            viewModel.PortalList = ThemeViewModelMap.ToPortalList(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);
            viewModel.CountryList = HelperMethods.GetCountries();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return viewModel;
        }

        //Save store data for store location.
        public virtual StoreLocatorDataViewModel SaveStore(StoreLocatorDataViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                StoreLocatorDataModel model = _storeLocatorClient.SaveStore(viewModel.ToModel<StoreLocatorDataModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(model) ? model.ToViewModel<StoreLocatorDataViewModel>() : new StoreLocatorDataViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new StoreLocatorDataViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (StoreLocatorDataViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.SaveErrorMessage);
            }
        }

        //Update an existing store data for store location.
        public virtual StoreLocatorDataViewModel Update(StoreLocatorDataViewModel viewmodel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                StoreLocatorDataModel model = _storeLocatorClient.Update(viewmodel.ToModel<StoreLocatorDataModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                return HelperUtility.IsNotNull(model) ? model.ToViewModel<StoreLocatorDataViewModel>() : new StoreLocatorDataViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new StoreLocatorDataViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (StoreLocatorDataViewModel)GetViewModelWithErrorMessage(viewmodel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get Store data for location by id
        public virtual StoreLocatorDataViewModel GetStoreLocator(int portalAddressId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            //Get expand for to have data from another table.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodePortalAddressEnum.ZnodeAddress.ToString());

            StoreLocatorDataModel model = _storeLocatorClient.GetStoreLocator(portalAddressId, expands);

            StoreLocatorDataViewModel viewModel = HelperUtility.IsNotNull(model) ? model.ToViewModel<StoreLocatorDataViewModel>() : new StoreLocatorDataViewModel();
            if (viewModel?.PortalAddressId > 0)
            {
                viewModel.PortalList = ThemeViewModelMap.ToPortalList(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);
                viewModel.CountryList = HelperMethods.GetCountries();
            }
            foreach (SelectListItem portalDetail in viewModel.PortalList)
            {
                if (Equals(portalDetail.Value, model.PortalId.ToString()))
                {
                    viewModel.PortalName = portalDetail.Text;
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    return viewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return viewModel;
        }

        //Get Store data for location by storeLocationCode
        public virtual StoreLocatorDataViewModel GetStoreLocator(string storeLocationCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            //Get expand for to have data from another table.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodePortalAddressEnum.ZnodeAddress.ToString());

            StoreLocatorDataModel model = _storeLocatorClient.GetStoreLocator(storeLocationCode, expands);

            StoreLocatorDataViewModel viewModel = HelperUtility.IsNotNull(model) ? model.ToViewModel<StoreLocatorDataViewModel>() : new StoreLocatorDataViewModel();
            ZnodeLogging.LogMessage("PortalAddressId: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, viewModel?.PortalAddressId);

            if (viewModel?.PortalAddressId > 0)
            {
                viewModel.PortalList = ThemeViewModelMap.ToPortalList(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);
                viewModel.CountryList = HelperMethods.GetCountries();
            }
            foreach (SelectListItem portalDetail in viewModel.PortalList)
            {
                if (Equals(portalDetail.Value, model.PortalId.ToString()))
                {
                    viewModel.PortalName = portalDetail.Text;
                    return viewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return viewModel;
        }

        //Delete an store by id or Store Location Code.
        public virtual bool DeleteStoreLocator(string storeLocatorIds, bool isDeleteByCode)
        {
            if (!string.IsNullOrEmpty(storeLocatorIds))
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    return _storeLocatorClient.DeleteStoreLocator(new ParameterModel { Ids = storeLocatorIds }, isDeleteByCode);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        #region Private Method.
        //Set tool menu for store locator list grid view.
        private void SetStoreLocatorListToolMenu(StoreLocatorListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreLocatorDeletePopUp')", ControllerName = "StoreLocator", ActionName = "Delete" });
            }
        }
        #endregion
    }
}