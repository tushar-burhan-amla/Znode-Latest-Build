using Newtonsoft.Json;
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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class InventoryAgent : BaseAgent, IInventoryAgent
    {
        #region Private Variables

        private readonly IInventoryClient _inventoryClient;
        private readonly IWarehouseClient _warehouseClient;
        private readonly IImportClient _importClient;
        private readonly IImportAgent _importAgent;

        #endregion

        #region Constructor

        public InventoryAgent(IInventoryClient inventoryClient, IWarehouseClient warehouseClient, IImportClient importClient)
        {
            _inventoryClient = GetClient<IInventoryClient>(inventoryClient);
            _warehouseClient = GetClient<IWarehouseClient>(warehouseClient);
            _importClient = GetClient<IImportClient>(importClient);
            _importAgent = new ImportAgent(GetClient<ImportClient>(), GetClient<PriceClient>(), GetClient<CountryClient>(), GetClient<PortalClient>(), GetClient<CatalogClient>());
        }

        #endregion

        #region Public Methods

        #region SKU Inventory
        //Method to create sku inventory.
        public virtual InventorySKUViewModel AddSKUInventory(InventorySKUViewModel inventorySKUViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            try
            {
                return _inventoryClient.AddSKUInventory(inventorySKUViewModel?.ToModel<InventorySKUModel>())?.ToViewModel<InventorySKUViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorSkuWarehouseCombination);
                    default:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Error);
                return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get sku inventory list.
        public virtual InventorySKUListViewModel GetSKUInventoryList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeInventoryEnum.InventoryId.ToString(), DynamicGridConstants.DESCKey);
            }
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, Equals(Convert.ToInt32(DefaultSettingHelper.DefaultLocale), 0) ? DefaultSettingHelper.DefaultLocale : DefaultSettingHelper.DefaultLocale));
            InventorySKUListModel inventorySKUList = _inventoryClient.GetSKUInventoryList(expands, filters, sorts, pageIndex, recordPerPage);
            InventorySKUListViewModel inventorySKUListViewModel = new InventorySKUListViewModel { InventorySKUList = inventorySKUList?.InventorySKUList?.ToViewModel<InventorySKUViewModel>().ToList() };
            List<SelectListItem> warehouseCodeList = GetWarehouseList();
            ZnodeLogging.LogMessage("warehouseCodeList count returned from GetWarehouseList method", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new { warehouseCodeListCount = warehouseCodeList?.Count });

            inventorySKUListViewModel.InventorySKUList?.ForEach(item => { item.WarehouseNameList = warehouseCodeList; item.WarehouseDisplayName = item.WarehouseName; item.WarehouseName = item.WarehouseId.ToString(); });
            SetListPagingData(inventorySKUListViewModel, inventorySKUList);

            //Set tool menu for SKU Inventory on grid list view.
            SetSKUInventoryListToolMenu(inventorySKUListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            return inventorySKUList?.InventorySKUList?.Count > 0 ? inventorySKUListViewModel
                : new InventorySKUListViewModel() { InventorySKUList = new List<InventorySKUViewModel>() };
        }

        //Get inventory by SKU from Product page.
        public virtual InventorySKUListViewModel GetInventoryBySKU(FilterCollectionDataModel model, string SKU)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            model.Filters.Add(new FilterTuple(ZnodeInventoryEnum.SKU.ToString(), FilterOperators.Is, SKU));
            ZnodeLogging.LogMessage("Input parameters expands and sorts of method GetSKUInventoryList.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new { filters = model?.Filters, sorts = model?.SortCollection });
            InventorySKUListViewModel inventorySKUListViewModel = GetSKUInventoryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            var wareHouseList = GetWarehouseList();
            ZnodeLogging.LogMessage("WarehouseList count returned from GetWarehouseList method", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new { warehouseListCount = wareHouseList?.Count });

            foreach (var item in inventorySKUListViewModel.InventorySKUList)
            {
                item.WarehouseNameList = wareHouseList;
                item.WarehouseName = item.WarehouseId.ToString();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return inventorySKUListViewModel;
        }

        //Get sku inventory on the basis of inventory id.
        public virtual InventorySKUViewModel GetSKUInventory(int inventoryId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            InventorySKUModel inventorySKUModel = _inventoryClient.GetSKUInventory(inventoryId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            return IsNotNull(inventorySKUModel) ? inventorySKUModel.ToViewModel<InventorySKUViewModel>() : new InventorySKUViewModel();
        }

        //Returning just object of Inventory as model.
        public virtual InventorySKUViewModel GetSKUInventoryBySKU(string sku, int productId, bool isDownloadable)
        {
            return new InventorySKUViewModel()
            {
                SKU = sku,
                ReOrderLevel = null,
                ProductId = productId,
                IsDownloadable = isDownloadable,
                WarehouseNameList = GetWarehouseList()
            };
        }

        //Update sku inventory.
        public virtual InventorySKUViewModel UpdateSKUInventory(InventorySKUViewModel inventorySKUViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            try
            {
                return _inventoryClient.UpdateSKUInventory(inventorySKUViewModel?.ToModel<InventorySKUModel>())?.ToViewModel<InventorySKUViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorSkuWarehouseCombination);
                    case ErrorCodes.DuplicateProductKey:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.ErrorUpdateDownloadableSKU);
                    default:
                        return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Error);
                return (InventorySKUViewModel)GetViewModelWithErrorMessage(inventorySKUViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get List of warehouse.
        public virtual List<SelectListItem> GetWarehouseList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            List<SelectListItem> warehouseSelectList = new List<SelectListItem>();
            WarehouseListModel warehouses = _warehouseClient.GetWarehouseList(null, null, null, null, null);
            ZnodeLogging.LogMessage("WareHouseList count ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { warehouseListCount = warehouses?.WarehouseList?.Count });

            if (warehouses?.WarehouseList?.Count > 0)
            {
                foreach (WarehouseModel warehouse in warehouses?.WarehouseList)
                    warehouseSelectList.Add(new SelectListItem() { Text = warehouse.WarehouseName.ToString(), Value = warehouse.WarehouseId.ToString() });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return warehouseSelectList;
        }

        //Set Import Inventory Details.
        public virtual InventorySKUViewModel SetImportInventoryDetails()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            //Set import head type id and name.
            InventorySKUViewModel inventorySKUViewModel = SetImportHeadData();
            //Get Import Template List.
            inventorySKUViewModel.TemplateTypeList = _importAgent.GetImportTemplateList(inventorySKUViewModel.ImportHeadId, 0);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return inventorySKUViewModel;
        }

        //Delete sku inventory.
        public virtual bool DeleteSKUInventory(string inventoryId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            try
            {
                return _inventoryClient.DeleteSKUInventory(inventoryId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual void SetFilters(FilterCollection filters, string filterName, int id)
        {
            //If id already persent in filters remove it.
            filters.RemoveAll(x => x.Item1 == filterName);

            //Add new Id into filters.
            filters.Add(new FilterTuple(filterName, FilterOperators.Equals, id.ToString()));
        }

        #endregion

        #region Digital Asset
        //Get downloadable product keys.
        public virtual DownloadableProductKeyListViewModel GetDownloadableProductKeys(int productId, string sku, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            //checks if the filter collection null
            if (Equals(filters, null))
                filters = new FilterCollection();

            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts :", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sortCollection = sortCollection });

            if (productId > 0)
            {
                if (!filters.Exists(x => x.Item1 == ZnodePimDownloadableProductEnum.SKU.ToString()))
                    filters.Add(new FilterTuple(ZnodePimDownloadableProductEnum.SKU.ToString(), FilterOperators.Contains, sku.ToString()));
            }
            FilterTuple isUsedFilter = filters?.FirstOrDefault(x => x.FilterName.ToLower() == "isused");
            //Adding single quote to isUsed filter value. 
            UpdateIsUsedFilter(isUsedFilter, filters, "\'" + isUsedFilter?.Item3 + "\'");
            DownloadableProductKeyListModel list = _inventoryClient.GetDownloadableProductKeys(expands, filters, sortCollection, pageIndex, recordPerPage);
            //Removing single quote from isUsed filter value.
            UpdateIsUsedFilter(isUsedFilter, filters, isUsedFilter?.Item3);
            DownloadableProductKeyListViewModel listViewModel = new DownloadableProductKeyListViewModel { DownloadableProductKeys = list?.DownloadableProductKeys?.ToViewModel<DownloadableProductKeyViewModel>().ToList() };
            SetListPagingData(listViewModel, list);

            //Set tool menu for custom field list grid view.
            SetDownloadableProductKeyListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return list?.DownloadableProductKeys?.Count > 0 ? listViewModel
                : new DownloadableProductKeyListViewModel();
        }

        // Add/Save Downloadable Product Keys..
        public virtual DownloadableProductKeyViewModel AddDownloadableProductKeys(DownloadableProductKeyViewModel downloadableProductKeyViewModel, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorAddDownloadableProductKey;
            try
            {
                DownloadableProductKeyModel downloadableProductKeyModel = _inventoryClient.AddDownloadableProductKey(InventoryViewModelMap.ToDownloadableProductKeyListModel(downloadableProductKeyViewModel));
                return !Equals(downloadableProductKeyModel, null) ? downloadableProductKeyModel.ToViewModel<DownloadableProductKeyViewModel>() : new DownloadableProductKeyViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (DownloadableProductKeyViewModel)GetViewModelWithErrorMessage(downloadableProductKeyViewModel, Admin_Resources.DupliacteDownloadableProductKeyErrorMessage);
                    default:
                        return (DownloadableProductKeyViewModel)GetViewModelWithErrorMessage(downloadableProductKeyViewModel, Admin_Resources.ErrorAddDownloadableProductKey);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                return (DownloadableProductKeyViewModel)GetViewModelWithErrorMessage(downloadableProductKeyViewModel, Admin_Resources.ErrorAddDownloadableProductKey);
            }
        }

        //Delete Downloadable Product Keys
        public virtual bool DeleteDownloadableProductKeys(string PimDownloadableProductKeyId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            message = PIM_Resources.CannotDeleteUsedKeys;
            try
            {
                return _inventoryClient.DeleteDownloadableProductKeys(PimDownloadableProductKeyId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                message = PIM_Resources.CannotDeleteUsedKeys;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Error);
                message = PIM_Resources.CannotDeleteUsedKeys;
                return false;
            }
        }

        //Update Downloadable Product Key.
        public virtual bool UpdateDownloadableProductKey(DownloadableProductKeyViewModel downloadableProductKeyViewModel, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            message = PIM_Resources.UpdateErrorMessage;
            try
            {
                return _inventoryClient.UpdateDownloadableProductKey(downloadableProductKeyViewModel.ToModel<DownloadableProductKeyModel>());

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.IsUsed:
                        message = PIM_Resources.KeyIsUsed;
                        return false;
                    case ErrorCodes.AlreadyExist:
                        message = PIM_Resources.ProductKeyAlreadyExists;
                        return false;
                    default:
                        message = PIM_Resources.UpdateErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,  ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Warning);
                message = PIM_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion

        #endregion

        #region Private Methods

        //Set the Tool Menus for Inventory SKU List Grid View.
        private void SetSKUInventoryListToolMenu(InventorySKUListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('InventorySKUDeletePopup')", ControllerName = "Inventory", ActionName = "DeleteSKUInventory" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonImport, JSFunctionName = "Inventory.prototype.ImportTool()", ControllerName = "Inventory", ActionName = "ImportInventoryView" });
            }
        }

        //Set import head type id and name.
        private InventorySKUViewModel SetImportHeadData()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeImportHeadEnum.IsUsedInImport.ToString(), FilterOperators.Equals, "true"));

            ImportTypeListModel importList = _importClient.GetImportTypeList(null, filters, null, null, null)?.ImportTypeList;

            return new InventorySKUViewModel
            {
                ImportHeadId = importList?.ImportTypeList?.Count > 0 ? importList.ImportTypeList.FirstOrDefault(w => w.Name == ImportHeadEnum.Inventory.ToString()).ImportHeadId : 0,
                ImportType = ImportHeadEnum.Inventory.ToString()
            };
        }

        //Set tool menu for downloadable product key list grid view.
        private void SetDownloadableProductKeyListToolMenu(DownloadableProductKeyListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DownloadableProductKeyDeletePopup')", ControllerName = "Products", ActionName = "DeleteDownloadableProductKeys" });
            }
        }
        private void UpdateIsUsedFilter(FilterTuple isUsedFilter, FilterCollection filter, string value)
        {
            if (IsNotNull(isUsedFilter))
            {

                filter.RemoveAll(x => x.Item1.ToLower() == "isused");

                filter.Add(new FilterTuple(isUsedFilter.Item1, isUsedFilter.Item2, value));
            }
        }
        #endregion
    }
}