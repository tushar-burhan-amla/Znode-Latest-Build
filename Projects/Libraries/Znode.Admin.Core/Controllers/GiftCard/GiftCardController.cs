using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class GiftCardController : BaseController
    {
        #region Private Variables

        private readonly IGiftCardAgent _giftCardAgent;
        private readonly IOrderAgent _orderAgent;
        private readonly IStoreAgent _storeAgent;

        private readonly string storeListAsidePanelPopup = "_AsideStorelistPanelPopup";
        private readonly string GiftCardList = "~/Views/GiftCard/List.cshtml";

        #endregion

        #region Public Constructor

        public GiftCardController(IGiftCardAgent giftCardAgent, IOrderAgent orderAgent, IStoreAgent storeAgent)
        {
            _giftCardAgent = giftCardAgent;
            _orderAgent = orderAgent;
            _storeAgent = storeAgent;
        }

        #endregion

        #region Public Methods
        // Get GiftCard list.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, bool isExcludeExpired = false)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeGiftCard.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGiftCard.ToString(), model);
            //Get GiftCard list.
            GiftCardListViewModel giftCardList = _giftCardAgent.GetGiftCardList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, isExcludeExpired);

            //Get the grid model.
            giftCardList.GridModel = FilterHelpers.GetDynamicGridModel(model, giftCardList?.GiftCardList, GridListType.ZnodeGiftCard.ToString(), string.Empty, null, true, true, giftCardList?.GridModel?.FilterColumn?.ToolMenuList);
            giftCardList.GridModel.TotalRecordCount = giftCardList.TotalResults;

            //Returns the GiftCard list.
            return ActionView(giftCardList);
        }

        //Get:Create GiftCard.
        public virtual ActionResult Create(int userId = 0)
        {
            GiftCardViewModel giftCardViewModel = new GiftCardViewModel();
            if (userId > 0)
            {
                UserViewModel userViewModel = _orderAgent.GetUserDetailsByUserId(userId);
                if (IsNotNull(userViewModel))
                {
                    giftCardViewModel.UserId = userViewModel.UserId;
                    giftCardViewModel.CustomerName = string.Concat(userViewModel.UserName?.Trim(), "|", userViewModel.FullName?.Trim());
                }
            }
            giftCardViewModel.CardNumber = _giftCardAgent.GetRandomGiftCardNumber();
            giftCardViewModel.ExpirationDate = null;
            return View(AdminConstants.CreateEdit, giftCardViewModel);
        }

        //Post:Create GiftCard.
        [HttpPost]
        public virtual ActionResult Create(GiftCardViewModel giftCardViewModel)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (IsNotNull(giftCardViewModel.RmaRequestModel))
                {
                    if (IsNotNull(giftCardViewModel) && (IsNotNull(giftCardViewModel.UserId)))
                    {
                        int accountId = giftCardViewModel.UserId.GetValueOrDefault();

                        //Create gift card
                        GiftCardViewModel giftCard = _giftCardAgent.Create(giftCardViewModel);

                        if (IsNotNull(giftCard) && giftCard.GiftCardId > 0)
                        {
                            giftCardViewModel.GiftCardId = giftCard.GiftCardId;

                            //Update RMA 
                            if (_giftCardAgent.UpdateRMA(giftCardViewModel, out message))
                            {
                                SetNotificationMessage(string.IsNullOrEmpty(message) ? GetSuccessNotificationMessage(Admin_Resources.VoucherCreationSuccessMessage) : GetErrorNotificationMessage(giftCardViewModel.ErrorMessage));
                                return RedirectToAction<GiftCardController>(x => x.Edit(giftCard.GiftCardId));
                            }
                        }
                        SetNotificationMessage((IsNotNull(giftCard) && giftCard.GiftCardId > 0) ? GetSuccessNotificationMessage(giftCardViewModel.SuccessMessage) : GetErrorNotificationMessage(giftCardViewModel.ErrorMessage));
                        return RedirectToAction(GiftCardList, "GiftCard");
                    }
                }
                else
                {
                    giftCardViewModel = _giftCardAgent.Create(giftCardViewModel);

                    if (!giftCardViewModel.HasError)
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.VoucherCreationSuccessMessage));
                        return RedirectToAction<GiftCardController>(x => x.Edit(giftCardViewModel.GiftCardId));
                    }
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(giftCardViewModel.ErrorMessage));
            return View(AdminConstants.CreateEdit, giftCardViewModel);
        }

        //Get:Edit GiftCard.
        [HttpGet]
        public virtual ActionResult Edit(int giftCardId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("EditVoucher", _giftCardAgent.GetGiftCard(giftCardId));
        }

        //Post:Edit GiftCard.
        [HttpPost]
        public virtual ActionResult Edit(GiftCardViewModel giftCardViewModel)
        {
            if (giftCardViewModel.RemainingAmount!= null)
            {
                if (ModelState.IsValid)
                {
                    SetNotificationMessage(_giftCardAgent.Update(giftCardViewModel).HasError
                    ? GetErrorNotificationMessage(Admin_Resources.VoucherUpdateFailureMessage)
                    : GetSuccessNotificationMessage(Admin_Resources.VoucherUpdateSuccessMessage));
                    return RedirectToAction<GiftCardController>(x => x.Edit(giftCardViewModel.GiftCardId));
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.VoucherUpdateFailureMessage));
                }
            }
            return View("EditVoucher", giftCardViewModel);
        }

        //Delete GiftCard.      
        public virtual JsonResult Delete(string giftCardId)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(giftCardId))
                status = _giftCardAgent.DeleteGiftCard(giftCardId);

            return Json(new { status = status, message = status ? Admin_Resources.VoucherDeleteMessage : Admin_Resources.AssociatedVoucherDeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Check entered Customer id already exists or not.
        [HttpPost]
        public virtual JsonResult IsUserIdExist(int UserId, int portalId = 0)
          => Json(_giftCardAgent.CheckIsUserIdExist(UserId, portalId), JsonRequestBehavior.AllowGet);

        //Get Customer list based on portal.
        public virtual ActionResult GetCustomerList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGiftCardCustomer.ToString(), model);
            //Get the list of Customers.    
            CustomerListViewModel customerList = _orderAgent.GetCustomerList(portalId, 0, false, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            customerList.GridModel = FilterHelpers.GetDynamicGridModel(model, customerList.List, GridListType.ZnodeGiftCardCustomer.ToString(), string.Empty, null, true, true, null);

            //Set portalId and IsAccountCustomer.
            customerList.PortalId = portalId;

            //Set the total record count.
            customerList.GridModel.TotalRecordCount = customerList.TotalResults;

            return ActionView(AdminConstants.CustomerListView, customerList);
        }

        //Get active currency.
        public virtual JsonResult GetActiveCurrencyToStore(int portalId)
        {
            CurrencyViewModel currencyViewModel = new CurrencyViewModel();
            currencyViewModel = _giftCardAgent.GetActiveCurrency(portalId);
            return Json(new { currencyViewModel = currencyViewModel }, JsonRequestBehavior.AllowGet);
        }

        //Get currency details by code.
        public virtual JsonResult GetCurrencyDetailsByCode(string currencyCode)
        {
            CurrencyViewModel currencyViewModel = new CurrencyViewModel();
            currencyViewModel = _giftCardAgent.GetCurrencyDetailsByCode(currencyCode);
            return Json(new { currencyViewModel = currencyViewModel }, JsonRequestBehavior.AllowGet);
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);

            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeListAsidePanelPopup, storeList);
        }

        //Activate/De-Activate Vouchers in bulk
        [HttpPost]
        public virtual ActionResult ActivateDeactivateVouchers(string giftCardId, bool isActive)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(giftCardId))
            {
                status = _giftCardAgent.ActivateDeactivateVouchers(giftCardId, isActive);
                message = status ? (status && isActive) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Activate/De-Activate Voucher 
        [HttpGet]
        public virtual ActionResult ActiveDeactiveSingleVoucher(string giftCardId, bool isActive)
        {
            string ReturnUrl = System.Convert.ToString(Request?.UrlReferrer?.AbsoluteUri);
            bool status = false;
            if (!string.IsNullOrEmpty(giftCardId))
            {
                status = _giftCardAgent.ActivateDeactivateVouchers(giftCardId, !isActive);
                SetNotificationMessage(status ? (status && !isActive) ? 
                    GetSuccessNotificationMessage(Admin_Resources.SuccessMessageStatusActive) :
                    GetSuccessNotificationMessage(Admin_Resources.SuccessMessageStatusInactive) : 
                    GetErrorNotificationMessage(Admin_Resources.ErrorMessageFailedStatus));
            }

            if (!string.IsNullOrEmpty(ReturnUrl))
                return Redirect(ReturnUrl);
            else
                return RedirectToAction("List");
        }

        // Get Voucher history List.
        public virtual ActionResult GetVoucherHistoryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int voucherId, int portalId )
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeVoucherHistory.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeVoucherHistory.ToString(), model);
            //Get Voucher list.
            VoucherHistoryListViewModel VoucherList = _giftCardAgent.GetVoucherHistoryList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, voucherId, portalId);

            //Get the grid model.
            VoucherList.GridModel = FilterHelpers.GetDynamicGridModel(model, VoucherList.GiftCardHistoryList, GridListType.ZnodeVoucherHistory.ToString(), string.Empty, null, true, true, VoucherList?.GridModel?.FilterColumn?.ToolMenuList);
            VoucherList.GridModel.TotalRecordCount = VoucherList.TotalResults;
            VoucherList.PortalId = portalId;
            VoucherList.GiftCardId = voucherId;
            //Returns the Voucher list.
            return ActionView("_GetVoucherHistoryList", VoucherList);
        }
        #endregion
    }
}