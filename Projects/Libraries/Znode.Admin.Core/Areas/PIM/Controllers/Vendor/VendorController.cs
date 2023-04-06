using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class VendorController : BaseController
    {
        #region Private Variables

        private IVendorAgent _vendorAgent;
        private readonly string _CreateEdit = "~/Areas/PIM/Views/Vendor/CreateEdit.cshtml";
        private readonly string _List = "~/Areas/PIM/Views/Vendor/List.cshtml";

        #endregion

        #region Public Constructor

        public VendorController(IVendorAgent vendorAgent)
        {
            _vendorAgent = vendorAgent;
        }
        #endregion

        #region Public Methods

        //Get list of vendors
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleVendor", Key = "Vendor", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimVendor.ToString(), model);

            VendorListViewModel vendorList = _vendorAgent.GetVendorList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            vendorList.GridModel = FilterHelpers.GetDynamicGridModel(model, vendorList?.Vendors, GridListType.ZnodePimVendor.ToString(), string.Empty, null, true, true, vendorList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            vendorList.GridModel.TotalRecordCount = vendorList.TotalResults;
            return ActionView(_List, vendorList);
        }

        //GET: Create vendor
        [HttpGet]
        public virtual ActionResult Create()
           => ActionView(_CreateEdit, new VendorViewModel() { VendorCodeList = _vendorAgent.GetVendorCodeList(), Address = new WarehouseAddressViewModel() { Countries = _vendorAgent.GetCountries() } });

        //POST: Create new vendor
        [HttpPost]
        public virtual ActionResult Create(VendorViewModel vendorViewModel)
        {
            if (ModelState.IsValid)
            {
                vendorViewModel = _vendorAgent.CreateVendor(vendorViewModel);
                if (HelperUtility.IsNotNull(vendorViewModel))
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.CreateMessage));
                    return RedirectToAction<VendorController>(x => x.Edit(vendorViewModel.PimVendorId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorFailedToCreate));
            return ActionView(AdminConstants.CreateEdit, new VendorViewModel());
        }

        //Get vendor by pimVendorId
        public virtual ActionResult Edit(int pimVendorId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            return ActionView(_CreateEdit, _vendorAgent.GetVendor(pimVendorId));
        }
        //Update vendor
        [HttpPost]
        public virtual ActionResult Edit(VendorViewModel vendorViewModel)
        {
            if (ModelState.IsValid)
            {
                vendorViewModel = _vendorAgent.UpdateVendor(vendorViewModel);
                if (!vendorViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.UpdateMessage));
                    return RedirectToAction<VendorController>(x => x.Edit(vendorViewModel.PimVendorId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.UpdateErrorMessage));
            return ActionView(_CreateEdit, vendorViewModel);
        }

        //Delete Vendor
        public virtual JsonResult Delete(string pimVendorId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(pimVendorId))
            {
                status = _vendorAgent.DeleteVendor(pimVendorId, out message);

                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Active/Inactive Vendor
        public virtual ActionResult ActiveInactiveVendor(string vendorIds, bool isActive)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(vendorIds))
            {
                status = _vendorAgent.ActiveInactiveVendor(vendorIds, isActive);

                message = status ? (status && isActive) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        #region Association of products
        //Action for vendor product list.
        public virtual ActionResult AssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimVendorId = 0, string vendorCode = null, string vendorName = null)
        => ActionView("_AssociatedProducts", _vendorAgent.AssociatedProductList(model, pimVendorId, vendorCode, vendorName));


        //Action for get product list. 
        public virtual ActionResult UnAssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string vendorCode = null)
          => ActionView("ProductPopupList", _vendorAgent.UnAssociatedProductList(model, vendorCode));


        // Action for associate vendor product
        public virtual JsonResult AssociateVendorProducts(string vendorCode, string productIds)
        {
            string errorMessage = PIM_Resources.ErrorAssociateProductsToVendor;
            bool status = false;
            if (IsNotNull(productIds) && IsNotNull(vendorCode))
            {
                status = _vendorAgent.AssociateVendorProduct(vendorCode, productIds);
                errorMessage = status ? PIM_Resources.SuccessProductAssociation : PIM_Resources.SuccessProductUnAssociation;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated products.
        public virtual ActionResult UnAssociateVendorProducts(string pimProductId, string attributeValue)
        {
            string errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if (IsNotNull(pimProductId))
            {
                status = _vendorAgent.UnAssociateVendorProduct(attributeValue, pimProductId);
                errorMessage = status ? PIM_Resources.ProductUnassociatedSuccessfully : PIM_Resources.Unabletounassociateproduct;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }
        //Check vendor name already exists or not.
        [HttpPost]
        public virtual JsonResult IsVendorNameExist(string VendorCode, int PimVendorId = 0)
        => Json(!_vendorAgent.CheckVendorNameExist(VendorCode, PimVendorId), JsonRequestBehavior.AllowGet);
        #endregion
        #endregion
    }
}