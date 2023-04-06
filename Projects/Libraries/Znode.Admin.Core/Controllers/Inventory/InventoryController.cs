using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Controllers
{
    public class InventoryController : BaseController
    {
        #region Private Variables

        private readonly IInventoryAgent _inventoryAgent;
        private readonly IProductAgent _productAgent;
        private readonly IImportAgent _importAgent;
        private const string copyInventoryView = "CopyInventory";
        private const string AddSKUInventoryView = "AddSKUInventory";
        private const string inventoryBySKU = "_InventoryBySKU";
        private const string WareHouseId = "inventorySKUViewModel.WarehouseId";
        #endregion

        #region Public Constructor

        public InventoryController(IInventoryAgent inventoryAgent, IProductAgent productAgent, IImportAgent importAgent)
        {
            _inventoryAgent = inventoryAgent;
            _productAgent = productAgent;
            _importAgent = importAgent;
        }

        #endregion

        #region Public Methods


        #region SKU Inventory

        //Create sku inventory.
        [HttpGet]
        public virtual ActionResult AddSKUInventory()
        {
            InventorySKUViewModel inventorySKUViewModel = new InventorySKUViewModel();
            inventorySKUViewModel.WarehouseNameList = _inventoryAgent.GetWarehouseList();
            return View(inventorySKUViewModel);
        }

        //Import Inventory View.
        public virtual ActionResult ImportInventoryView()
            => PartialView("_ImportInventory", _inventoryAgent.SetImportInventoryDetails());

        //Create sku inventory.
        [HttpPost]
        public virtual ActionResult AddSKUInventory(InventorySKUViewModel inventorySKUViewModel)
        {
            bool isDownloadable = false;
            if (ModelState.IsValid)
            {
                isDownloadable = inventorySKUViewModel.IsDownloadable;
                inventorySKUViewModel = _inventoryAgent.AddSKUInventory(inventorySKUViewModel);
                if (!inventorySKUViewModel.HasError)
                {
                    if (inventorySKUViewModel?.InventoryId > 0 && inventorySKUViewModel.IsDownloadable)
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DownloadableProductSuccessMessage));
                        return RedirectToAction<InventoryController>(x => x.EditSKUInventory(inventorySKUViewModel.InventoryId));
                    }
                    else
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                        return RedirectToAction<InventoryController>(x => x.InventorySKUList(null));
                    }
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(inventorySKUViewModel.ErrorMessage));
            inventorySKUViewModel.WarehouseNameList = _inventoryAgent.GetWarehouseList();
            inventorySKUViewModel.IsDownloadable = isDownloadable;
            return View(inventorySKUViewModel);
        }

        //Add Update Product Inventory Details from Pim product page.
        [HttpPost]
        public virtual ActionResult AddUpdateSKUInventoryProduct(InventorySKUViewModel inventorySKUViewModel)
        {
            bool IsUpdate = (inventorySKUViewModel.InventoryId > 0);
            bool _status = false;
            string message = string.Empty;
            if (inventorySKUViewModel.IsDownloadable)
                ModelState.Remove(WareHouseId);

            if (ModelState.IsValid)
            {
                inventorySKUViewModel = IsUpdate ? _inventoryAgent.UpdateSKUInventory(inventorySKUViewModel) : _inventoryAgent.AddSKUInventory(inventorySKUViewModel);
                if (!inventorySKUViewModel.HasError)
                {
                    _status = true;
                    message = ((inventorySKUViewModel?.InventoryId > 0 && inventorySKUViewModel.IsDownloadable) ?
                    Admin_Resources.DownloadableProductSuccessMessage :
                    (IsUpdate ? Admin_Resources.UpdateMessage : Admin_Resources.RecordCreationSuccessMessage));
                }
                else
                    message = inventorySKUViewModel?.ErrorMessage;
            }

            return Json(new { status = _status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get SKU inventory list.
        public virtual ActionResult InventorySKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeInventory.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeInventory.ToString(), model);
            //SKU inventory list.
            InventorySKUListViewModel inventorySKUList = _inventoryAgent.GetSKUInventoryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            inventorySKUList.GridModel = FilterHelpers.GetDynamicGridModel(model, inventorySKUList?.InventorySKUList, GridListType.ZnodeInventory.ToString(), string.Empty, null, true, true, inventorySKUList?.GridModel?.FilterColumn?.ToolMenuList);
            inventorySKUList.GridModel.TotalRecordCount = inventorySKUList.TotalResults;
            return ActionView(inventorySKUList);
        }

        public virtual JsonResult GetInventoryDetail(string sku)
        {
            FilterCollectionDataModel model = new FilterCollectionDataModel() { Page = 1, RecordPerPage = 100, Filters = new FilterCollection() };
            model.Filters.Add(new FilterTuple(ZnodeInventoryEnum.SKU.ToString(), FilterOperators.Is, Convert.ToString(sku)));
            var inventorySKU = _inventoryAgent.GetSKUInventoryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage).InventorySKUList.FirstOrDefault();
            return Json(new { data = inventorySKU }, JsonRequestBehavior.AllowGet);
        }

        //Get inventory by SKU from Product page.
        public virtual ActionResult InventoryBySKU([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string SKU)
        {
            InventorySKUListViewModel inventorySKUList = _inventoryAgent.GetInventoryBySKU(model, SKU);
            inventorySKUList.GridModel = FilterHelpers.GetDynamicGridModel(model, inventorySKUList?.InventorySKUList, GridListType.ZnodePimInventory.ToString(), string.Empty, null, true, true, inventorySKUList?.GridModel?.FilterColumn?.ToolMenuList);
            inventorySKUList.GridModel.TotalRecordCount = inventorySKUList.TotalResults;
            return ActionView(inventoryBySKU, inventorySKUList);
        }

        [HttpGet]
        public virtual JsonResult InventoryBySKUAndWarehouseId(string sku, int warehouseId)
        {
            //creating dummy model object for FilterCollectionDataModel.
            var model = new FilterCollectionDataModel() { Page = 1, RecordPerPage = 100, Filters = new FilterCollection() };
            model.Filters.Add(new FilterTuple(ZnodeInventoryEnum.WarehouseId.ToString(), FilterOperators.Is, Convert.ToString(warehouseId)));

            InventorySKUViewModel inventorySkuModel = _inventoryAgent.GetInventoryBySKU(model, sku)?.InventorySKUList?.FirstOrDefault();

            bool _status = IsNotNull(inventorySkuModel);
            return Json(new
            {
                status = _status,
                quantity = inventorySkuModel?.Quantity,
                reOrderLevel = inventorySkuModel?.ReOrderLevel,
                inventoryId = inventorySkuModel?.InventoryId,
                backOrderQuantity = inventorySkuModel?.BackOrderQuantity,
                backOrderExpectedDate = Convert.ToString(inventorySkuModel?.BackOrderExpectedDate.ToDateTimeFormat())
            }, JsonRequestBehavior.AllowGet);
        }

        //Edit SKU Price.
        [HttpGet]
        public virtual ActionResult EditSKUInventory(int InventoryId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            InventorySKUViewModel inventorySKUViewModel = new InventorySKUViewModel();
            inventorySKUViewModel = _inventoryAgent.GetSKUInventory(InventoryId);
            inventorySKUViewModel.WarehouseNameList = _inventoryAgent.GetWarehouseList();
            return ActionView("AddSKUInventory", inventorySKUViewModel);
        }

        //Update Grid Product Inventory Details (Inline Edit)
        [HttpGet]
        public virtual JsonResult UpdateGridSKUPIMInventory(string data)
        {
            bool _status = false;
            string _message = string.Empty;

            var dSerializedData = JsonConvert.DeserializeObject<InventorySKUViewModel[]>(data)[0];
            var skuInventoryDetails = _inventoryAgent.GetSKUInventory(dSerializedData.InventoryId);

            if (IsNotNull(skuInventoryDetails))
            {
                skuInventoryDetails.ReOrderLevel = dSerializedData.ReOrderLevel;
                skuInventoryDetails.Quantity = dSerializedData.Quantity;

                //update Inventory with Re-order and Quantity values.
                InventorySKUViewModel inventorySKU = _inventoryAgent.UpdateSKUInventory(skuInventoryDetails);
                _status = !inventorySKU.HasError;
                _message = _status ? Admin_Resources.UpdateMessage : inventorySKU.ErrorMessage;
            }
            else
                _message = Admin_Resources.InventoryNotFoundForUpdate;

            return Json(new
            {
                status = _status,
                message = _message
            }, JsonRequestBehavior.AllowGet);
        }

        //Update SKU inventory.
        public virtual ActionResult EditSKUInventory(InventorySKUViewModel inventorySKUViewModel)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                InventorySKUViewModel inventorySKU = inventorySKUViewModel?.InventoryId > 0 ? _inventoryAgent.UpdateSKUInventory(inventorySKUViewModel) : _inventoryAgent.AddSKUInventory(inventorySKUViewModel);

                SetNotificationMessage(inventorySKU.HasError
                                        ? GetErrorNotificationMessage(inventorySKU.ErrorMessage)
                                        : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<InventoryController>(x => x.EditSKUInventory(inventorySKU.InventoryId));

            }

            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<InventoryController>(x => x.EditSKUInventory(inventorySKUViewModel.InventoryId));

        }

        //Delete sku inventory.
        public virtual JsonResult DeleteSKUInventory(string inventoryId)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(inventoryId))
            {
                status = _inventoryAgent.DeleteSKUInventory(inventoryId);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Get Product SKU list for Inventry.
        public virtual ActionResult GetProductSKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SKUList.ToString(), model);
            //GetSkuProductListBySKU methods gets product sku list for inventory if pricelistId equals 0.
            _inventoryAgent.SetFilters(model.Filters, AdminConstants.PriceListId, 0);
            PIMProductAttributeValuesListViewModel skuList = _productAgent.GetSkuProductListBySKU(AdminConstants.SKU, model);
            skuList.GridModel = FilterHelpers.GetDynamicGridModel(model, skuList?.ProductAttributeValues, GridListType.SKUList.ToString(), string.Empty, null, true);
            skuList.GridModel.TotalRecordCount = skuList.TotalResults;
            return ActionView(AdminConstants.ProductSKUListView, skuList);
        }
        #endregion

        #region Digital Asset 

        //Create new keys for Downloadable Product.
        public virtual ActionResult AddDownloadableProductKeys(int productId, string sku)
        => PartialView("AddDownloadableProductKeys", new DownloadableProductKeyViewModel() { ProductId = productId, SKU = sku });

        [HttpGet]
        public virtual ActionResult ProductInventory(string SKU, int productId, bool isDownloadable = false)
        => ActionView(AdminConstants.productInventory, _inventoryAgent.GetSKUInventoryBySKU(SKU, productId, isDownloadable));

        //Get list of all downloadable product keys.
        public virtual ActionResult GetDownloadableProductKeys([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int productId, string sku, int inventoryId = 0)
        {
            //Get the list of all Downloadable Product keys.
            DownloadableProductKeyListViewModel downloadableProductKeys = _inventoryAgent.GetDownloadableProductKeys(productId, sku, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            downloadableProductKeys.PimProductId = productId;
            downloadableProductKeys.InventoryId = inventoryId;
            downloadableProductKeys.SKU = sku;
            downloadableProductKeys.IsDownloadable = true;
            //Get the grid model.
            downloadableProductKeys.GridModel = FilterHelpers.GetDynamicGridModel(model, downloadableProductKeys.DownloadableProductKeys, GridListType.ZnodePimDownloadableProductKeyForInventory.ToString(), string.Empty, null, true, true, downloadableProductKeys?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            downloadableProductKeys.GridModel.TotalRecordCount = downloadableProductKeys.TotalResults;

            return ActionView(inventoryId == 0 ? "_DownloadableProductKeyList" : "DownloadableProductKeyList", downloadableProductKeys);
        }

        //Add downloadable product keys.
        [HttpPost]
        public virtual JsonResult AddDownloadableProductKeys(DownloadableProductKeyViewModel model)
        {
            string message = string.Empty;
            DownloadableProductKeyViewModel downloadableProductKeyViewModel = _inventoryAgent.AddDownloadableProductKeys(model, out message);
            if (downloadableProductKeyViewModel.DownloadableProductKeyList.Any(x => x.IsDuplicate == true) || downloadableProductKeyViewModel.HasError)
                return Json(new { status = false, list = downloadableProductKeyViewModel.DownloadableProductKeyList, hasError = downloadableProductKeyViewModel.HasError, message = message }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = true, list = downloadableProductKeyViewModel.DownloadableProductKeyList, message = Admin_Resources.SaveMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete downloadable product keys.
        public virtual ActionResult DeleteDownloadableProductKeys(string pimDownloadableProductKeyId)
        {
            string errorMessage = string.Empty;
            if (IsNotNull(pimDownloadableProductKeyId))
            {
                bool isDeleted = _inventoryAgent.DeleteDownloadableProductKeys(pimDownloadableProductKeyId, out errorMessage);

                return Json(new { status = isDeleted, message = isDeleted ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Update Downloadable Product Key
        public virtual JsonResult UpdateDownloadableProductKey(int PimDownloadableProductKeyId, string data)
        {
            bool status = false;
            string message = PIM_Resources.UpdateErrorMessage;
            if (IsNotNull(data))
            {
                DownloadableProductKeyViewModel downloadableProductKeyViewModel = JsonConvert.DeserializeObject<DownloadableProductKeyViewModel[]>(data)[0];

                if (ModelState.IsValid)
                    status = _inventoryAgent.UpdateDownloadableProductKey(downloadableProductKeyViewModel, out message);
            }
            return Json(new { status = status, message = status ? PIM_Resources.UpdateMessage : message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Download Template.
        public virtual ActionResult DownloadTemplate(int downloadImportHeadId, string downloadImportName, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0)
        {
            _importAgent.DownloadTemplate(downloadImportHeadId, downloadImportName, downloadImportFamilyId, downloadImportPromotionTypeId, Response);
            if (Request.IsAjaxRequest())
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            else
                return new EmptyResult();
        }
        #endregion
    }
}