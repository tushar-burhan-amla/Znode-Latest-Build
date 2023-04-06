using MvcSiteMapProvider;
using System;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Newtonsoft.Json;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class WarehouseController : BaseController
    {
        #region Private Variables

        private readonly IWarehouseAgent _warehouseAgent;

        #endregion

        #region Public Constructor

        public WarehouseController(IWarehouseAgent warehouseAgent)
        {
            _warehouseAgent = warehouseAgent;
        }

        #endregion

        #region Public Methods

        // Get warehouse list.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,HeaderWarehouse", Key = "WareHouse", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeWarehouse.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeWarehouse.ToString(), model);
            //Get warehouse list.
            WarehouseListViewModel warehouseList = _warehouseAgent.GetWarehouseList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            warehouseList.GridModel = FilterHelpers.GetDynamicGridModel(model, warehouseList?.WarehouseList, GridListType.ZnodeWarehouse.ToString(), string.Empty, null, true, true, warehouseList?.GridModel?.FilterColumn?.ToolMenuList);
            warehouseList.GridModel.TotalRecordCount = warehouseList.TotalResults;

            //Returns the warehouse list.
            return ActionView(warehouseList);
        }

        //Get:Create warehouse.
        public virtual ActionResult Create()
           => View(AdminConstants.CreateEdit, new WarehouseViewModel { Address = new WarehouseAddressViewModel { Countries = _warehouseAgent.GetCountries() } });

        //Post:Create warehouse.
        [HttpPost]
        public virtual ActionResult Create(WarehouseViewModel warehouseViewModel)
        {
            ModelState.Remove("Address.FirstName");
            ModelState.Remove("Address.LastName");
            if (ModelState.IsValid)
            {
                warehouseViewModel = _warehouseAgent.Create(warehouseViewModel);
                if (!warehouseViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<WarehouseController>(x => x.Edit(warehouseViewModel.WarehouseId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(warehouseViewModel.ErrorMessage));
            warehouseViewModel.Address.Countries = _warehouseAgent.GetCountries();
            return View(AdminConstants.CreateEdit, warehouseViewModel);
        }

        //Get:Edit warehouse.
        [HttpGet]
        public virtual ActionResult Edit(int warehouseId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            WarehouseViewModel warehouseViewModel = _warehouseAgent.GetWarehouse(warehouseId);
            if (HelperUtility.IsNull(warehouseViewModel?.Address))
                warehouseViewModel.Address = new WarehouseAddressViewModel() { Countries = _warehouseAgent.GetCountries() };
            else
                warehouseViewModel.Address.Countries = _warehouseAgent.GetCountries();
            return ActionView(AdminConstants.CreateEdit, warehouseViewModel);
        }

        //Post:Edit warehouse.
        [HttpPost]
        public virtual ActionResult Edit(WarehouseViewModel warehouseViewModel)
        {
            ModelState.Remove("Address.FirstName");
            ModelState.Remove("Address.LastName");
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_warehouseAgent.Update(warehouseViewModel).HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));

                return RedirectToAction<WarehouseController>(x => x.Edit(warehouseViewModel.WarehouseId));
            }
            warehouseViewModel.Address.Countries = _warehouseAgent.GetCountries();
            return View(AdminConstants.CreateEdit, warehouseViewModel);
        }

        //Delete warehouse.
        public virtual JsonResult Delete(string warehouseId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(warehouseId))
            {
                status = _warehouseAgent.DeleteWarehouse(warehouseId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Associate inventory
        //Get associated inventory list.
        public virtual ActionResult GetAssociatedInventoryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int warehouseId, string warehouseName)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeWarehouseInventory.ToString(), model);
            _warehouseAgent.SetFilters(model.Filters, warehouseId);
            InventoryWarehouseMapperListViewModel inventoryWarehouseList = _warehouseAgent.GetAssociatedInventoryList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            inventoryWarehouseList.WarehouseId = warehouseId;
            inventoryWarehouseList.WarehouseName = string.IsNullOrEmpty(warehouseName) ? HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[AdminConstants.WarehouseName] : warehouseName;

            inventoryWarehouseList.GridModel = FilterHelpers.GetDynamicGridModel(model, inventoryWarehouseList?.InventoryWarehouseMapperList, GridListType.ZnodeWarehouseInventory.ToString(), string.Empty, null, true, true, inventoryWarehouseList?.GridModel?.FilterColumn?.ToolMenuList);
            inventoryWarehouseList.GridModel.TotalRecordCount = inventoryWarehouseList.TotalResults;

            return ActionView(inventoryWarehouseList);
        }

        //Update SKU inventory.
        public virtual ActionResult EditSKUInventory(int inventoryId, int warehouseId, string data)
        {
            if (ModelState.IsValid && IsNotNull(data))
            {
                InventorySKUViewModel inventorySKUViewModel = JsonConvert.DeserializeObject<InventorySKUViewModel[]>(data)[0];

                InventorySKUViewModel inventorySKU = _warehouseAgent.UpdateSKUInventory(inventorySKUViewModel);
                if (!inventorySKU.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = IsNotNull(inventorySKU.ErrorMessage) ? inventorySKU.ErrorMessage : PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete sku inventory.
        public virtual JsonResult DeleteSKUInventory(string inventoryId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(inventoryId))
            {
                status = _warehouseAgent.DeleteSKUInventory(inventoryId);

                message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}