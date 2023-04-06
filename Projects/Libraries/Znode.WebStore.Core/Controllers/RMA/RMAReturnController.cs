using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Controllers
{
    [Authorize]
    public partial class RMAReturnController : BaseController
    {
        #region Private Read-only members
        private readonly IRMAReturnAgent _rmaReturnAgent;
        private readonly IUserAgent _userAgent;
        #endregion

        #region Public Constructor        
        public RMAReturnController(IRMAReturnAgent rmaReturnAgent, IUserAgent userAgent)
        {
            _rmaReturnAgent = rmaReturnAgent;
            _userAgent = userAgent;
        }
        #endregion

        #region Public Methods
        //Get Select Order Return List
        [HttpGet]
        public virtual ActionResult GetOrderDetailsForReturn(string orderNumber = null)
        {
            if (GlobalAttributeHelper.EnableReturnRequest() == false)
                return Redirect("/404");

            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = _rmaReturnAgent.GetOrderDetailsForReturn(orderNumber);

            return ActionView("../RMAReturn/SelectReturnItems", rmaReturnOrderDetailViewModel);
        }

        //Get return list
        public virtual ActionResult GetReturnList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (GlobalAttributeHelper.EnableReturnRequest() == false)
                return Redirect("/404");

            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnListViewModel returnList = _rmaReturnAgent.GetReturnList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            returnList.GridModel = FilterHelpers.GetDynamicGridModel(model, returnList?.ReturnList, WebStoreConstants.ZnodeWebstoreReturn, string.Empty, null, true, true, returnList?.GridModel?.FilterColumn?.ToolMenuList);
            returnList.GridModel.TotalRecordCount = returnList.TotalResults;
            return ActionView("ReturnHistoryList", returnList);
        }

        //Check if the order is eligible for return
        [HttpGet]
        public virtual ActionResult CheckOrderEligibilityForReturn(string orderNumber)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            bool isOrderEligibleForReturn = _rmaReturnAgent.IsOrderEligibleForReturn(orderNumber);

            if (isOrderEligibleForReturn)
                return RedirectToAction<RMAReturnController>(x => x.GetOrderDetailsForReturn(orderNumber));
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorOrderNotEligibleForReturn));
                return RedirectToAction<UserController>(x => x.History(null));
            }
        }

        //Get order return details by return number
        [HttpGet]
        public virtual ActionResult GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            if (GlobalAttributeHelper.EnableReturnRequest() == false)
                return Redirect("/404");

            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnViewModel returnViewModel = _rmaReturnAgent.GetReturnDetails(returnNumber, isReturnDetailsReceipt);

            return ActionView("../RMAReturn/ReturnOrderReceipt", returnViewModel);
        }

        //Insert or update order return details.
        [HttpPost]
        public virtual ActionResult SaveOrderReturn(RMAReturnViewModel returnViewModel)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.SaveOrderReturn(returnViewModel);
            if (!rmaReturnViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(rmaReturnViewModel.ErrorMessage));

            return Json(new { hasError = rmaReturnViewModel.HasError, errorMessage = rmaReturnViewModel.ErrorMessage, returnNumber = rmaReturnViewModel.ReturnNumber }, JsonRequestBehavior.AllowGet);
        }

        //Delete order return on basis of return number.
        [HttpPost]
        public virtual ActionResult DeleteOrderReturn(string returnNumber)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            bool status = false;
            if (!string.IsNullOrEmpty(returnNumber))
            {
                status = _rmaReturnAgent.DeleteOrderReturn(returnNumber);
                if (status)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.SuccessDeleteReturn));
                    return Json(new { status = status, message = WebStore_Resources.SuccessDeleteReturn }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = status, message = WebStore_Resources.ErrorDeleteReturn }, JsonRequestBehavior.AllowGet);
        }

        //Submit order return
        [HttpPost]
        public virtual ActionResult SubmitOrderReturn(RMAReturnViewModel returnViewModel)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.SubmitOrderReturn(returnViewModel);

            if (!rmaReturnViewModel.HasError)
                SetNotificationMessage(GetSuccessNotificationMessage(rmaReturnViewModel.ErrorMessage));

            return Json(new { hasError = rmaReturnViewModel.HasError, errorMessage = rmaReturnViewModel.ErrorMessage, returnNumber = rmaReturnViewModel.ReturnNumber }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Perform calculations for an order return line item.
        public virtual ActionResult CalculateOrderReturn(RMAReturnCalculateViewModel calculateOrderReturnModel)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnCalculateViewModel returnCalculateViewModel = _rmaReturnAgent.CalculateOrderReturn(calculateOrderReturnModel);

            return Json(new
            {
                html = RenderRazorViewToString("../RMAReturn/_ReturnCalculation", returnCalculateViewModel),
                hasError = returnCalculateViewModel.HasError,
                errorMessage = returnCalculateViewModel.ErrorMessage,
                calculateLineItemList = returnCalculateViewModel.ReturnCalculateLineItemList,
            }, JsonRequestBehavior.AllowGet);
        }

        //Manage order return
        [HttpGet]
        public virtual ActionResult ManageOrderReturn(string returnNumber)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = _rmaReturnAgent.ManageOrderReturn(returnNumber);

            return ActionView("../RMAReturn/SelectReturnItems", rmaReturnOrderDetailViewModel);
        }

        //Get print return receipt
        [HttpGet]
        public virtual ActionResult PrintReturnReceipt(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return ActionView("PrintReturnReceipt", _rmaReturnAgent.GetReturnDetails(returnNumber, isReturnDetailsReceipt));
        }
        #endregion

        #region Private Methods
        
        #endregion
    }
}