using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class WarehouseAgent : BaseAgent, IWarehouseAgent
    {
        #region Private Variables

        private readonly IWarehouseClient _warehouseClient;
        private readonly ICountryClient _countryClient;
        private readonly IInventoryClient _inventoryClient;

        #endregion

        #region Constructor

        public WarehouseAgent(IWarehouseClient warehouseClient, ICountryClient countryClient, IInventoryClient inventoryClient)
        {
            _warehouseClient = GetClient<IWarehouseClient>(warehouseClient);
            _countryClient = GetClient<ICountryClient>(countryClient);
            _inventoryClient = GetClient<IInventoryClient>(inventoryClient);
        }

        #endregion

        #region Public Methods

        #region Warehouse

        //Gets the list of Warehouse.
        public virtual WarehouseListViewModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetWarehouseList(expands, filters, sorts, null, null);

        //Gets the list of Warehouse.
        public virtual WarehouseListViewModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sortCollection.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new object[] { expands, filters, sorts });
            SortCollection sortlist = new SortCollection();
            sortlist.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);

            WarehouseListModel warehouseList = _warehouseClient.GetWarehouseList(expands, filters, sorts, pageIndex, pageSize);
            WarehouseListViewModel listViewModel = new WarehouseListViewModel { WarehouseList = warehouseList?.WarehouseList?.ToViewModel<WarehouseViewModel>().ToList() };
            SetListPagingData(listViewModel, warehouseList);

            //Set tool menu for warehouse list grid view.
            SetWarehouseListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            return warehouseList?.WarehouseList?.Count > 0 ? listViewModel : new WarehouseListViewModel() { WarehouseList = new List<WarehouseViewModel>() };
        }

        //Get List of Active Countries.
        public virtual List<SelectListItem> GetCountries()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            List<SelectListItem> countriesSelectList = new List<SelectListItem>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodeCountryEnum.CountryName.ToString(), DynamicGridConstants.ASCKey);
            ZnodeLogging.LogMessage("Filters and sortCollection.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new object[] { filters, sorts });

            CountryListModel countries = _countryClient.GetCountryList(null, filters, sorts);

            if (countries?.Countries?.Count > 0)
            {
                //Set default country on top in dropdown as per in Global setting.
                SetDefaultCountry(countries);
                foreach (CountryModel country in countries.Countries)
                    countriesSelectList.Add(new SelectListItem() { Text = country.CountryName, Value = country.CountryCode });
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            return countriesSelectList;
        }


        //Create warehouse.
        public virtual WarehouseViewModel Create(WarehouseViewModel warehouseViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                WarehouseModel warehouseModel = _warehouseClient.CreateWarehouse(warehouseViewModel.ToModel<WarehouseModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                return IsNotNull(warehouseModel) ? warehouseModel.ToViewModel<WarehouseViewModel>() : new WarehouseViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (WarehouseViewModel)GetViewModelWithErrorMessage(warehouseViewModel, Admin_Resources.AlreadyExistCode);
                    default:
                        return (WarehouseViewModel)GetViewModelWithErrorMessage(warehouseViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                return (WarehouseViewModel)GetViewModelWithErrorMessage(warehouseViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get warehouse by warehouse id.
        public virtual WarehouseViewModel GetWarehouse(int warehouseId)
            => _warehouseClient.GetWarehouse(warehouseId).ToViewModel<WarehouseViewModel>();

        //Update warehouse.
        public virtual WarehouseViewModel Update(WarehouseViewModel warehouseViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                WarehouseModel warehouseModel = _warehouseClient.UpdateWarehouse(warehouseViewModel.ToModel<WarehouseModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                return IsNotNull(warehouseModel) ? warehouseModel.ToViewModel<WarehouseViewModel>() : (WarehouseViewModel)GetViewModelWithErrorMessage(new WarehouseViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                return (WarehouseViewModel)GetViewModelWithErrorMessage(warehouseViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete warehouse.
        public virtual bool DeleteWarehouse(string warehouseId, out string errorMessage)
        {
            errorMessage = Admin_Resources.ErrorFailedToDelete;

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                return _warehouseClient.DeleteWarehouse(new ParameterModel { Ids = warehouseId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDeleteWarehouse;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        public virtual void SetFilters(FilterCollection filters, int warehouseId)
        {
            ZnodeLogging.LogMessage("Input parameters filters, warehouseId.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new object[] { filters, warehouseId });

            if (!Equals(filters, null))
            {
                //Checking For WarehouseId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodeWarehouseEnum.WarehouseId.ToString()))
                {
                    //If WarehouseId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodeWarehouseEnum.WarehouseId.ToString());

                    //Add New WarehouseId Into filters
                    filters.Add(new FilterTuple(ZnodeWarehouseEnum.WarehouseId.ToString(), FilterOperators.Equals, warehouseId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeWarehouseEnum.WarehouseId.ToString(), FilterOperators.Equals, warehouseId.ToString()));
            }
        }
        #endregion

        #region Associate inventory
        //Get Associated Inventory List.
        public virtual InventoryWarehouseMapperListViewModel GetAssociatedInventoryList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            InventoryWarehouseMapperListModel ListModel = _warehouseClient.GetAssociatedInventoryList(expands, filters, sorts, pageIndex, recordPerPage);
            InventoryWarehouseMapperListViewModel inventoryWarehouseMapperListViewModel = new InventoryWarehouseMapperListViewModel { InventoryWarehouseMapperList = ListModel?.InventoryWarehouseMapperList?.ToViewModel<InventoryWarehouseMapperViewModel>().ToList() };
            SetListPagingData(inventoryWarehouseMapperListViewModel, ListModel);
            //Set tool menu for SKU Inventory on grid list view.
            SetSKUInventoryListToolMenu(inventoryWarehouseMapperListViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return ListModel?.InventoryWarehouseMapperList?.Count > 0 ? inventoryWarehouseMapperListViewModel
                : new InventoryWarehouseMapperListViewModel() { InventoryWarehouseMapperList = new List<InventoryWarehouseMapperViewModel>() };
        }

        //Get inventory.
        public virtual InventorySKUViewModel GetSKUInventory(int inventoryId, int warehouseId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            InventorySKUModel inventorySKUModel = _inventoryClient.GetSKUInventory(inventoryId);
            WarehouseModel warehouse = _warehouseClient.GetWarehouse(warehouseId);
            inventorySKUModel.WarehouseName = warehouse.WarehouseName;
            inventorySKUModel.WarehouseCode = warehouse.WarehouseCode;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return IsNotNull(inventorySKUModel) ? inventorySKUModel.ToViewModel<InventorySKUViewModel>() : new InventorySKUViewModel();
        }

        //Update sku inventory.
        public virtual InventorySKUViewModel UpdateSKUInventory(InventorySKUViewModel inventorySKUViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                inventorySKUViewModel.IsFromWarehouse = true;
                return _inventoryClient.UpdateSKUInventory(inventorySKUViewModel?.ToModel<InventorySKUModel>())?.ToViewModel<InventorySKUViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DuplicateProductKey:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorUpdateDownloadableSKU);
                    default:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.UpdateErrorMessage);
            }

        }

        //Delete sku inventory.
        public virtual bool DeleteSKUInventory(string inventoryId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
                return _inventoryClient.DeleteSKUInventory(inventoryId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #endregion

        #region Private Methods.
        //Set tool menu for warehouse list grid view.
        private void SetWarehouseListToolMenu(WarehouseListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('WarehouseDeletePopup')", ControllerName = "Warehouse", ActionName = "Delete" });
            }
        }

        //Set the Tool Menus for Inventory SKU List Grid View.
        private void SetSKUInventoryListToolMenu(InventoryWarehouseMapperListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('InventorySKUDeletePopup')", ControllerName = "Warehouse", ActionName = "DeleteSKUInventory" });
            }
        }

        //Method to set default country on top in dropdown as per in Global setting.
        private static void SetDefaultCountry(CountryListModel countries)
        {
            int defaultCountryId = countries.Countries.FindIndex(x => x.CountryId == countries.Countries.FirstOrDefault(i => i.IsDefault == true)?.CountryId);
            ZnodeLogging.LogMessage("set default country with defaultCountryId on top in dropdown as per in Global setting.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new object[] { defaultCountryId });

            CountryModel defaultCountry = countries.Countries[defaultCountryId];
            countries.Countries[defaultCountryId] = countries.Countries[0];
            countries.Countries[0] = defaultCountry;
        }

        #endregion
    }
}