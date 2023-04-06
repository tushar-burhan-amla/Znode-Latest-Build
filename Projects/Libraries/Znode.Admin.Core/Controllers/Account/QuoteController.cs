using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class QuoteController : BaseController
    {
        #region Private ReadOnly members
        private readonly IAccountQuoteAgent _accountQuoteAgent;
        private readonly IOrderAgent _orderAgent;
        private readonly ICartAgent _cartAgent;
        private readonly IQuoteAgent _quoteAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly IPaymentAgent _paymentAgent;

        private const string quoteListView = "QuoteList";
        private const string manageQuoteView = "ManageQuote";
        private const string manageQuoteStatusView = "_ManageQuoteStatus";
        private const string manageNotes = "_ManageNotes";
        private const string quoteLineItemView = "_QuoteLineItemList";
        private const string manageShoppingCartView = "ManageShoppingCart";
        private const string manageCustomerAddressView = "_ManageCustomerAddress";
        private const string manageCustomerView = "_ManageCustomerInformation";
        private const string manageTotalTableView = "_ManageTotalTable";
        private const string manageTotaltableView = "_ManageTotalTable";
        private const string printQuoteDetail = "PrintQuoteDetails";
        private readonly string paymentOptionList = "PaymentOptionList";
        private readonly string checkoutReceiptView = "CheckoutReceipt";
        #endregion

        #region Public Constructor
        public QuoteController(IOrderAgent orderAgent, IAccountQuoteAgent accountQuoteAgent, ICartAgent cartAgent, IQuoteAgent quoteAgent, IStoreAgent storeAgent, IPaymentAgent paymentAgent)
        {
            _orderAgent = orderAgent;
            _accountQuoteAgent = accountQuoteAgent;
            _cartAgent = cartAgent;
            _quoteAgent = quoteAgent;
            _storeAgent = storeAgent;
            _paymentAgent = paymentAgent;
        }
        #endregion

        #region Account Quote
        //Get account note list.
        public virtual ActionResult AccountQuoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeOmsQuote.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOmsQuote.ToString(), model);
            AccountQuoteListViewModel quoteList = _accountQuoteAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, 0, 0);
            quoteList.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteList?.AccountQuotes, GridListType.ZnodeOmsQuote.ToString(), string.Empty, null, true, true, null);
            quoteList.GridModel.TotalRecordCount = quoteList.TotalResults;
            return ActionView(quoteList);
        }

        // Checkout receipt after successfull order submit.
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult CheckoutReceipt(int orderId, string ReceiptHtml, bool? IsEmailSend)
        {
            //if order is not present then redirect to list page.
            if (orderId > 0)
            {
                CreateOrderViewModel model = new CreateOrderViewModel();
                //Decode the checkout receipt html.
                model.ReceiptHtml = WebUtility.HtmlDecode(ReceiptHtml);

                //Set order receipt Message if unable to send email receipt.
                if (!Convert.ToBoolean(IsEmailSend))
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToSendOrderReceipt));

                return View("CheckoutReceipt", model);
            }
            return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
        }

        // The index page for create quote.
        [HttpGet]
        public virtual ActionResult CreateQuote(int portalId = 0)
        {

            CreateOrderViewModel createQuote = _orderAgent.GetCreateOrderDetails(portalId);
            createQuote.IsQuote = true;

            return ActionView(createQuote);
        }

        //Get Pending Payment List
        public virtual ActionResult PendingPaymentList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeOmsPendingPayment.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOmsPendingPayment.ToString(), model);

            AccountQuoteListViewModel quoteList = _accountQuoteAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, 0, 0, true);
            quoteList.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteList?.AccountQuotes, GridListType.ZnodeOmsPendingPayment.ToString(), string.Empty, null, true, true, quoteList?.GridModel?.FilterColumn?.ToolMenuList);
            quoteList.GridModel.TotalRecordCount = quoteList.TotalResults;
            return ActionView(quoteList);
        }

        //Convert To Order
        [HttpPost]
        public virtual ActionResult ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel, int omsQuoteId = 0)
        {
            if (omsQuoteId > 0)
                accountQuoteViewModel.OmsQuoteId = omsQuoteId;
            OrderViewModel order = _accountQuoteAgent.ConvertToOrder(accountQuoteViewModel);

            if(order.OrderLineItems.Count < 1)
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageForDisableProduct));
            }
            if (HelperUtility.IsNull(order))
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorUnablePlaceOrder));
            }
            if (!order.HasError)
            {
                order.ReceiptHtml = WebUtility.HtmlEncode(order.ReceiptHtml);
                return new JsonResult { Data = order, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            if (!Equals(order, null) && order.HasError)
            {
                if (string.IsNullOrEmpty(order.ErrorMessage))
                    order.ErrorMessage = Admin_Resources.OrderSubmitError;
                SetNotificationMessage(GetErrorNotificationMessage(order.ErrorMessage));
            }
            return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
        }
        #endregion

        #region Quote
        // Get list of all the Quote.
        public virtual ActionResult QuoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
          => ActionView(quoteListView, _quoteAgent.GetQuoteList(model));

        //Get Quote Details
        public virtual ActionResult GetQuoteDetails(int omsQuoteId)
        {
            QuoteResponseViewModel quoteViewModel = _quoteAgent.GetQuoteDetails(omsQuoteId);
            // This method is call to get payment Api header as store on the view
            ViewBag.PaymentApiRequestHeader = _paymentAgent.GetPaymentAuthToken(HttpContext.User.Identity.Name.ToString(), true);

            if (IsNotNull(TempData["SuccessMessage"]))
                SetNotificationMessage(GetSuccessNotificationMessage(Convert.ToString(TempData["SuccessMessage"])));

            return ActionView(manageQuoteView, quoteViewModel);
        }

        //Get the List of Quote Status.
        public virtual ActionResult ManageQuoteStatus(string quoteStatus, int omsQuoteId)
           => ActionView(manageQuoteStatusView, _quoteAgent.BindManageQuoteStatus(quoteStatus, omsQuoteId));

        //Update Quote Status in session.
        [HttpPost]
        public virtual JsonResult UpdateQuoteStatus(OrderStatusList model)
            => Json(_quoteAgent.UpdateQuoteStatus(model), JsonRequestBehavior.AllowGet);

        // Get Quote status Edit value.
        public virtual ActionResult GetQuoteStateValueById(int omsQuoteStateId)
            => Json(new { isEdit = _quoteAgent.GetQuoteStateValueById(omsQuoteStateId) }, JsonRequestBehavior.AllowGet);

        //Get Additional Notes.
        public virtual ActionResult GetAdditionalNotes()
          => ActionView(manageNotes, new QuoteResponseViewModel());

        //Update CustomerShipping's Account Number.
        [HttpPost]
        public virtual JsonResult UpdateShippingAccountNumber(int omsQuoteId, string accountNumber)
        {
            if (IsNotNull(accountNumber) && omsQuoteId > 0)
            {
                bool status = _quoteAgent.UpdateShippingAccountNumber(omsQuoteId, accountNumber);
                return Json(new { status = status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, ErrorMessage = Admin_Resources.UnableToUpdateAccountNumber }, JsonRequestBehavior.AllowGet);
        }

        //Update Customer Shipping's Method Name.
        [HttpPost]
        public virtual JsonResult UpdateShippingMethod(int omsQuoteId, string shippingMethod)
        {
            if (IsNotNull(shippingMethod) && omsQuoteId > 0)
            {
                bool status = _quoteAgent.UpdateShippingMethod(omsQuoteId, shippingMethod);
                return Json(new { status = status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, ErrorMessage = Admin_Resources.UnableToUpdateShippingMethod }, JsonRequestBehavior.AllowGet);
        }

        //Update In Hand Date of Quote.
        [HttpPost]
        public virtual JsonResult UpdateInHandDate(int omsQuoteId, DateTime inHandDate)
        {
            if (IsNotNull(inHandDate) && omsQuoteId > 0)
            {
                bool status = _quoteAgent.UpdateInHandDate(omsQuoteId, inHandDate);
                return Json(new { status = status }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, ErrorMassage = Admin_Resources.UnableToUpdateInHandDate }, JsonRequestBehavior.AllowGet);
        }

        //Update Quote Expiration Date.
        [HttpPost]
        public virtual JsonResult UpdateQuoteExpirationDate(int omsQuoteId, DateTime quoteExpirationDate)
        {
            if (IsNotNull(quoteExpirationDate) && omsQuoteId > 0)
            {
                bool status = _quoteAgent.UpdateQuoteExpirationDate(omsQuoteId, quoteExpirationDate);
                return Json(new { status = status , ErrorMassage = Admin_Resources.UnableToUpdateQuoteExpirationDate }, JsonRequestBehavior.AllowGet);
            }     
            else
                return Json(new { status = false, ErrorMassage = Admin_Resources.UnableToUpdateQuoteExpirationDate }, JsonRequestBehavior.AllowGet);
        }

        //Get Shipping List for Manage Quote.
        public virtual ActionResult GetQuoteShippingList(int omsQuoteId)
        {
            ShippingListViewModel shippingList = _quoteAgent.GetShippingListForManage(omsQuoteId);
            return Json(new { status = shippingList }, JsonRequestBehavior.AllowGet);
        }

        //To Calculate Shipping Charges.
        [HttpPost]
        public virtual JsonResult CalculateShippingInManage(int omsQuoteId, int shippingId)
        {
            QuoteCartViewModel cartViewModel = _quoteAgent.GetShippingChargesForManage(omsQuoteId, shippingId);

            if (!string.IsNullOrEmpty(cartViewModel.ShippingErrorMessage))
            {
                return Json(new
                {
                    shippingErrorMessage = cartViewModel.ShippingErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    CartView = RenderRazorViewToString(quoteLineItemView, cartViewModel)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //Update shipping and Handling Charges
        [HttpPost]
        public virtual ActionResult UpdateShippingHandling(int omsQuoteId, string ShippingCost)
        {
            QuoteCartViewModel cartViewModel = _quoteAgent.UpdateShippingHandling(omsQuoteId, ShippingCost);

            if(IsNotNull(cartViewModel))
            {
                if (!string.IsNullOrEmpty(cartViewModel.ShippingErrorMessage)) { SetNotificationMessage(GetErrorNotificationMessage(cartViewModel.ShippingErrorMessage)); }
                return ActionView(manageShoppingCartView, cartViewModel);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCalculateCart));
                return ActionView(manageShoppingCartView, _quoteAgent.GetQuoteShoppingCart(omsQuoteId));
            }
        }

        //Get User Address By AddressId
        public virtual ActionResult GetUserAddressForManageById(int selectedAddressId, int omsQuoteId, int userId, int shippingAddressId, int billingAddressId, int portalId, string fromBillingShipping = "")
        {
            ManageAddressModel addressDetail = new ManageAddressModel();
            addressDetail.SelectedAddressId = selectedAddressId;
            addressDetail.OmsQuoteId = omsQuoteId;
            addressDetail.ShippingAddressId = shippingAddressId;
            addressDetail.BillingAddressId = billingAddressId;
            addressDetail.PortalId = portalId;
            addressDetail.FromBillingShipping = fromBillingShipping;
            addressDetail.UserId = userId;
            return ActionView(manageCustomerAddressView, _quoteAgent.GetUserAddressForManageQuote(addressDetail));
        }

        //Update Address for Manage Quote
        [HttpPost]
        public virtual ActionResult ManageCustomerAddress(AddressViewModel addressViewModel)
        {
            if (IsNotNull(addressViewModel) && ModelState.IsValid)
            {
                QuoteSessionModel quoteViewModel = _quoteAgent.UpdateCustomerAddressAndCalculate(addressViewModel);

                if (!string.IsNullOrEmpty(quoteViewModel.CartInformation.ShippingErrorMessage))
                {
                    return Json(new
                    {
                        shippingErrorMessage = quoteViewModel.CartInformation.ShippingErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (quoteViewModel.HasError)
                {
                    return Json(new
                    {
                        shippingErrorMessage = quoteViewModel.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        totalView = RenderRazorViewToString(manageShoppingCartView, quoteViewModel.CartInformation),
                        addressView = RenderRazorViewToString(manageCustomerView, _quoteAgent.MapUpdatedCustomerAddress(addressViewModel.OmsQuoteId)),
                        quoteTotal = RenderRazorViewToString(manageTotalTableView, quoteViewModel.CartInformation)
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            return GetAddressWithValidation(addressViewModel);
        }

        //Get Address with Validation
        public virtual ActionResult GetAddressWithValidation(AddressViewModel addressViewModel)
        {
            if (IsNotNull(addressViewModel))
            {
                AddressViewModel defaultAddressData = _quoteAgent.GetAddressDefaultData(new ManageAddressModel
                {
                    SelectedAddressId = addressViewModel.AddressId,
                    OmsQuoteId = addressViewModel.OmsQuoteId,
                    PortalId = addressViewModel.PortalId,
                    UserId = addressViewModel.UserId
                },
                                          addressViewModel.AccountId);
                addressViewModel.UsersAddressNameList = defaultAddressData.UsersAddressNameList;
                addressViewModel.Countries = defaultAddressData.Countries;
                return Json(new
                {
                    addressView = RenderRazorViewToString(manageCustomerAddressView, addressViewModel)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorMessage = Admin_Resources.ErrorAddressModelNull }, JsonRequestBehavior.AllowGet);

        }

        //Removes cart item from shopping cart.
        [HttpPost]
        public virtual ActionResult RemoveQuoteCartItem(int omsQuoteId, string guid)
        {
            if (_quoteAgent.RemoveQuoteCartItem(omsQuoteId, guid))
            {
                return GetShoppingCartItems(omsQuoteId);
            }
            GetErrorNotificationMessage(Admin_Resources.UnableToDeleteSingleItem);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // Get Shopping Cart
        public virtual ActionResult GetShoppingCartItems(int omsQuoteId)
        {
            QuoteCartViewModel cartViewModel = _quoteAgent.GetQuoteShoppingCart(omsQuoteId);

            if (IsNotNull(cartViewModel))
            {
                return Json(new
                {
                    html = RenderRazorViewToString(quoteLineItemView, cartViewModel),
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToLoadCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        //Calculate Shopping Cart
        public virtual ActionResult CalculateShoppingCart(int omsQuoteId)
        {
            QuoteCartViewModel cartdata = _quoteAgent.CalculateShoppingCart(omsQuoteId);

            if (IsNotNull(cartdata))
            {
                return Json(new
                {
                    html = RenderRazorViewToString(manageTotalTableView, cartdata),
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCalculateCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        //Update Quote CartItem
        [HttpPost]
        public virtual ActionResult UpdateQuoteCartItem(ManageOrderDataModel quoteDataModel)
        {
            QuoteCartViewModel cartViewModel = _quoteAgent.UpdateQuoteLineItemDetails(quoteDataModel);
            ShoppingCartItemModel cartItem = cartViewModel?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == quoteDataModel.Guid);
            if (IsNotNull(cartItem) && IsNotNull(cartViewModel))
            {
                return Json(new
                {
                    totalView = RenderRazorViewToString(manageTotaltableView, cartViewModel),
                    unitPrice = HelperMethods.FormatPriceWithCurrency(cartItem.UnitPrice, cartViewModel.CultureCode),
                    extendedPrice = HelperMethods.FormatPriceWithCurrency(cartItem.ExtendedPrice, cartViewModel.CultureCode),
                    quantity = cartItem?.GroupProducts?.Count > 0 ? (cartItem.GroupProducts.FirstOrDefault()?.Quantity).GetValueOrDefault() : cartItem.Quantity,
                    shippingCost = HelperMethods.FormatPriceWithCurrency(cartItem.ShippingCost, cartViewModel.CultureCode),
                    hasError = cartViewModel.HasError,
                    errorMessage = cartViewModel.ErrorMessage,
                    lineItemShipping = GetShippingCost(quoteDataModel, cartViewModel)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                totalView = RenderRazorViewToString(manageTotaltableView, cartViewModel)
            }, JsonRequestBehavior.AllowGet);
        }

        //Update Tax on Manage Quote
        [HttpPost]
        public virtual ActionResult UpdateTaxExemptForManage(int omsQuoteId, bool isTaxExempt)
        {
            QuoteCartViewModel cartViewModel = _quoteAgent.UpdateTaxExemptForManage(omsQuoteId, isTaxExempt);
            if (IsNotNull(cartViewModel) && !cartViewModel.HasError)
            {
                return ActionView(manageTotalTableView, cartViewModel);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCalculateCart));
            return ActionView(manageTotalTableView, _quoteAgent.GetQuoteShoppingCart(omsQuoteId));
        }

        //Print Quote Details for Manage Quote
        public virtual ActionResult PrintManageQuote(int omsQuoteId)
        {
            if (omsQuoteId > 0)
            {
                return Json(new
                {
                    html = RenderRazorViewToString(printQuoteDetail, _quoteAgent.GetQuoteDetails(omsQuoteId)),
                    success = true,
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorQuoteIdNullOrZero);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        //Update Quote Shipping Constraint
        [HttpPost]
        public virtual JsonResult UpdateShippingConstraintCode(int omsQuoteId, string shippingConstraintCode)
        {
            if (IsNotNull(shippingConstraintCode) && omsQuoteId > 0)
            {
                bool status = _quoteAgent.UpdateShippingConstraintCode(omsQuoteId, shippingConstraintCode);
                return Json(new { status = status, ErrorMassage = Admin_Resources.UnableToUpdateShippingConstraintCode }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, ErrorMassage = Admin_Resources.UnableToUpdateShippingConstraintCode }, JsonRequestBehavior.AllowGet);
        }

        //Update Quote Job Name
        [HttpPost]
        public virtual JsonResult UpdateJobName(int omsQuoteId, string jobName)
        {

            if (omsQuoteId > 0 && IsNotNull(jobName))
            {
                bool status = _quoteAgent.UpdateJobName(omsQuoteId, jobName);
                return Json(new { status = status, ErrorMassage = Admin_Resources.UnableToUpdateJobName }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, ErrorMassage = Admin_Resources.UnableToUpdateJobName }, JsonRequestBehavior.AllowGet);
        }

        // Update manage Quote.
        [HttpPost]
        public virtual ActionResult UpdateQuote(int omsQuoteId, string additionalNote)
        {
            BooleanModel isQuoteUpdated = _quoteAgent.UpdateQuote(omsQuoteId, additionalNote);
            if (!isQuoteUpdated.IsSuccess)
            {
                return Json(new { status = isQuoteUpdated.IsSuccess, errorMessage = isQuoteUpdated.ErrorMessage });
            }
            else
            {
                TempData["SuccessMessage"] = Admin_Resources.SuccessQuoteUpdate;
                return Json(new { status = isQuoteUpdated.IsSuccess, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public virtual ActionResult CreateQuoteRequest(int portalId = 0)
        {
            QuoteCreateViewModel viewModel = _quoteAgent.GetCreateQuoteDetails(portalId);
            StoreListViewModel storeList = _storeAgent.GetStoreList(null, null, null, null);
            viewModel.IsQuote = true;
            if (storeList?.StoreList?.Count() == 1)
            {
                StoreViewModel storeViewModel = storeList.StoreList.FirstOrDefault();
                viewModel.StoreName = storeViewModel?.StoreName;
                viewModel.PortalId = storeViewModel.PortalId;
            }

            return ActionView(viewModel);
        }

        /// <summary>
        /// Submit order.
        /// </summary>
        /// <param name="model">QuoteCreateViewModel</param>
        /// <returns>Returns View.</returns>
        [HttpPost]
        public virtual ActionResult SubmitQuote(QuoteCreateViewModel model)
        {
            //create quote request.
            QuoteCreateViewModel quote = _quoteAgent.SubmitQuote(model);
            if (IsNull(quote) || (quote.HasError && !string.IsNullOrEmpty(quote.ErrorMessage)))
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorSubmitQuote));
            else
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessQuoteRequest));

            return new JsonResult { Data = quote, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // Get payment options
        public virtual ActionResult GetPaymentMethods(int userId, int portalId)
        => ActionView(paymentOptionList, _orderAgent.GetPaymentMethods(portalId, userId));

        //Convert Quote to an order
        [HttpPost]
        public virtual ActionResult SaveAndConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderViewModel)
        {
            if (HelperUtility.IsNotNull(convertToOrderViewModel))
            {
                OrdersViewModel order = _quoteAgent.SaveAndConvertQuoteToOrder(convertToOrderViewModel);
                if (order.OrderId > 0)
                {
                    order.ReceiptHtml = WebUtility.HtmlEncode(order.ReceiptHtml);
                    return new JsonResult { Data = order, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                // Return error message, if payment through "Credit Card" raises any error.
                if (IsCreditCardPayment(convertToOrderViewModel?.PaymentDetails.PaymentType))
                {
                    return Json(new { error = order.ErrorMessage });
                }
                if (order.HasError)
                {
                    return Json(new { error = Admin_Resources.ConvertQuoteToOrderErrorMessage });
                }
            }
            return RedirectToAction<QuoteController>(x => x.GetQuoteDetails(convertToOrderViewModel.OmsQuoteId));
        }


        //Update cart item quantity.
        [HttpPost]
        public virtual ActionResult UpdateQuoteCartItemPrice(string guid, decimal unitPrice, int productId = 0, int shippingId = 0, bool isQuote = true, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.UpdateCartItemPrice(guid, unitPrice, productId, shippingId, userId);
            cartViewModel.IsQuote = isQuote;
            if (isQuote)
            {
                return RedirectToAction<OrderController>(x => x.GetShoppingCartItems(cartViewModel.UserId, cartViewModel.PortalId, 0, isQuote));
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToUpdateCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // Checkout receipt after successfull order submit.
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult QuoteCheckoutReceipt(int orderId, string ReceiptHtml)
        {
            //if order is not present then redirect to list page.
            if (orderId > 0 && !string.IsNullOrEmpty(ReceiptHtml))
            {
                CreateOrderViewModel model = new CreateOrderViewModel();
                //Decode the checkout receipt html.
                model.ReceiptHtml = WebUtility.HtmlDecode(ReceiptHtml);

                return View(checkoutReceiptView, model);
            }
            return RedirectToAction<QuoteController>(x => x.QuoteList(null));
        }
        #endregion

        #region private method
        //check if payment is Credit Card type
        private bool IsCreditCardPayment(string paymentType)
        {
            return !string.IsNullOrEmpty(paymentType) && Equals(paymentType.ToLower(), ZnodeConstant.CreditCard.ToLower());
        }

        #region protected method
        protected virtual System.Collections.Generic.List<Tuple<string, string>> GetShippingCost(ManageOrderDataModel quoteDataModel, QuoteCartViewModel cartViewModel)
        {
            if (quoteDataModel.IsShippingEdit)
                return null;

            return cartViewModel?.ShoppingCartItems?.Where(x => x.ExternalId != quoteDataModel.Guid)?.Select(y => new Tuple<string, string>(y.ExternalId, HelperMethods.FormatPriceWithCurrency(y.ShippingCost, cartViewModel.CultureCode))).ToList();
        }
        #endregion
        #endregion
    }
}