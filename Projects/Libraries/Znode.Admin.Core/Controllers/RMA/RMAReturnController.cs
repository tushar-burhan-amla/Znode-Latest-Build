using System;
using System.Web.Mvc;

using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class RMAReturnController : BaseController
    {
        #region Private Read-only members
        private readonly IRMAReturnAgent _rmaReturnAgent;
        #endregion

        #region Public Constructor        
        public RMAReturnController(IRMAReturnAgent rmaReturnAgent)
        {
            _rmaReturnAgent = rmaReturnAgent;
        }
        #endregion

        #region Public Methods
        //Get returns list
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, string portalName = null)
        {
            //Remove DateTimeRange Filter From Cookie.
            DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.ZnodeReturn.ToString(), model);

            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeReturn.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeReturn.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of Returns
            RMAReturnListViewModel returnList = _rmaReturnAgent.GetReturnList(model, portalId, portalName);

            //Get the grid model
            returnList.GridModel = FilterHelpers.GetDynamicGridModel(model, returnList?.ReturnList, GridListType.ZnodeReturn.ToString(), string.Empty, null, true, true, returnList?.GridModel?.FilterColumn?.ToolMenuList);
            returnList.GridModel.TotalRecordCount = returnList.TotalResults;
            return ActionView("List", returnList);
        }

        //Manage return details
        public virtual ActionResult ManageReturn(string returnNumber)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnAgent.ManageReturn(returnNumber);
            if (IsNotNull(TempData["SuccessMessage"]))
                SetNotificationMessage(GetSuccessNotificationMessage(Convert.ToString(TempData["SuccessMessage"])));
            else if (IsNotNull(TempData["ErrorMessage"]))
                SetNotificationMessage(GetErrorNotificationMessage(Convert.ToString(TempData["ErrorMessage"])));
            return ActionView("ManageReturn", returnViewModel);
        }

        //Fetch the List of return status
        public virtual ActionResult ManageReturnStatus(string returnStatus)
           => ActionView("_ManageReturnStatus", _rmaReturnAgent.GetReturnStatusList(returnStatus));

        //Get Additional ReturnNotes
        public virtual ActionResult GetAdditionalReturnNotes()
                 => ActionView("_ManageReturnNotes", new RMAReturnViewModel());

        //Update Order Return Line Item.
        [HttpPost]
        public virtual ActionResult UpdateOrderReturnLineItem(RMAReturnLineItemViewModel orderReturnLineItemModel, string returnNumber)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnAgent.UpdateOrderReturnLineItem(orderReturnLineItemModel, returnNumber);
            return Json(new
            {
                hasError = returnViewModel.HasError,
                message = returnViewModel.ErrorMessage,
                returnLineItems = RenderRazorViewToString("ManageReturnLineItemList", returnViewModel),
            }, JsonRequestBehavior.AllowGet);
        }

        //Update Order Return Status.
        [HttpPost]
        public virtual ActionResult UpdateOrderReturnStatus(int returnStatusCode, string returnNumber)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnAgent.UpdateOrderReturnStatus(returnStatusCode, returnNumber);
            return Json(new
            {
                hasError = returnViewModel.HasError,
                message = returnViewModel.ErrorMessage,
                returnLineItems = RenderRazorViewToString("ManageReturnLineItemList", returnViewModel),
                returnStateId = returnViewModel.RmaReturnStateId,
            }, JsonRequestBehavior.AllowGet);
        }

        //Submit order return
        [HttpPost]
        public virtual ActionResult SubmitOrderReturn(string returnNumber, string notes)
        {
            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.SubmitOrderReturn(returnNumber, notes);

            if (!rmaReturnViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(rmaReturnViewModel.ErrorMessage));

            return Json(new { hasError = rmaReturnViewModel.HasError, message = rmaReturnViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Print order return receipt by return number
        [HttpGet]
        public virtual ActionResult PrintReturn(string returnNumber)
        {
            return ActionView("PrintReturnReceipt", _rmaReturnAgent.PrintReturnReceipt(returnNumber));
        }

        //Check if the order is eligible for return
        [HttpGet]
        public virtual ActionResult CheckOrderEligibleForReturn(string orderNumber, int userId, int portalId, int omsOrderId, bool isManageOrder)
        {
            bool isOrderEligibleForReturn = _rmaReturnAgent.IsOrderEligibleForReturn(orderNumber, userId, portalId);

            if (isOrderEligibleForReturn)
                return RedirectToAction<RMAReturnController>(x => x.GetOrderDetailsForReturn(orderNumber, userId));
            else
            {
                if (isManageOrder)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorOrderEligibleForReturnOrNot));
                    return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, string.Empty));
                }
                
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorOrderEligibleForReturnOrNot));
                return RedirectToAction<OrderController>(x => x.List(null, 0, 0, 0, null));

            }
        }

        //Get Select Order Return List
        [HttpGet]
        public virtual ActionResult GetOrderDetailsForReturn(string orderNumber, int userId)
        {
            RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = _rmaReturnAgent.GetOrderDetailsForReturn(orderNumber, userId);

            return ActionView("CreateReturn", rmaReturnOrderDetailViewModel);
        }

        [HttpPost]
        //Perform calculations for an order return line item.
        public virtual ActionResult CalculateOrderReturn(RMAReturnCalculateViewModel calculateOrderReturnModel)
        {
            RMAReturnCalculateViewModel returnCalculateViewModel = _rmaReturnAgent.CalculateOrderReturn(calculateOrderReturnModel);

            return Json(new
            {
                html = RenderRazorViewToString("_ReturnCalculation", returnCalculateViewModel),
                hasError = returnCalculateViewModel.HasError,
                errorMessage = returnCalculateViewModel.ErrorMessage,
                calculateLineItemList = returnCalculateViewModel.ReturnCalculateLineItemList,
            }, JsonRequestBehavior.AllowGet);
        }

        //Submit order return
        [HttpPost]
        public virtual ActionResult SubmitCreateReturn(RMAReturnViewModel returnViewModel)
        {
            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.SubmitCreateReturn(returnViewModel);

            if (!rmaReturnViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(rmaReturnViewModel.ErrorMessage));

            return Json(new { hasError = rmaReturnViewModel.HasError, errorMessage = rmaReturnViewModel.ErrorMessage, returnNumber = rmaReturnViewModel.ReturnNumber }, JsonRequestBehavior.AllowGet);
        }

        //Get order return details by return number
        [HttpGet]
        public virtual ActionResult GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnAgent.GetReturnDetails(returnNumber, isReturnDetailsReceipt);

            return ActionView("ReturnOrderReceipt", returnViewModel);
        }

        [HttpGet]
        public virtual ActionResult PrintCreateReturnReceipt(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            RMAReturnViewModel returnViewModel = _rmaReturnAgent.GetReturnDetails(returnNumber, isReturnDetailsReceipt);

            return ActionView("PrintCreateReturnReceipt", returnViewModel);
        }
        
        //validate orderlinitem for create return.
        [HttpPost]
        public virtual ActionResult IsValidReturnItems(RMAReturnViewModel returnViewModel)
        {
            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.IsValidReturnItems(returnViewModel);

            if (!rmaReturnViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(rmaReturnViewModel.ErrorMessage));

            return Json(new { hasError = rmaReturnViewModel.HasError, orderLineItems = rmaReturnViewModel.ReturnLineItems}, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}