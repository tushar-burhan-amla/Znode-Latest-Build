using System;
using System.Linq;
using System.Web.Mvc;

using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Extensions;
using Znode.WebStore.Core.ViewModels;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Controllers
{
    [NoCacheAttribute]
    public class CheckoutController : BaseController
    {
        protected readonly string createSchedulerView = "~/Views/Themes/B2B/Views/Checkout/AddPayment.cshtml";

        #region protected Read-only members
        protected readonly IUserAgent _userAgent;
        protected readonly ICheckoutAgent _checkoutAgent;
        protected readonly ICartAgent _cartAgent;
        protected readonly IPaymentAgent _paymentAgent;
        protected readonly bool IsEnableSinglePageCheckout = PortalAgent.CurrentPortal.IsEnableSinglePageCheckout;
        protected readonly string TotalTableView = "_TotalTable";
        protected readonly string CheckoutReceipt = "CheckoutReceipt";
        #endregion

        #region Public Constructor
        public CheckoutController(IUserAgent userAgent, ICheckoutAgent checkoutAgent, ICartAgent cartAgent, IPaymentAgent paymentAgent)
        {
            _userAgent = userAgent;
            _checkoutAgent = checkoutAgent;
            _cartAgent = cartAgent;
            _paymentAgent = paymentAgent;
        }
        #endregion
        // GET: Checkout
        public virtual ActionResult Index(bool IsSinglePage = true)
        {
            string userOrSessionId = string.Empty;
            CartViewModel accountQuoteViewModel = null;
            if (!Equals(Request.QueryString["QuoteId"], null))
            {
                accountQuoteViewModel = _cartAgent.SetQuoteCart(Convert.ToInt32(Request.QueryString["QuoteId"]));
            }

            if (!Equals(Request.QueryString["ShippingId"], null))
            {
                _cartAgent.AddEstimatedShippingIdToCartViewModel(int.Parse(Convert.ToString(Request.QueryString["ShippingId"])));
            }
            //TODO: Manage guest mode token generation process current session based process is not secured.
            if ((User.Identity.IsAuthenticated || Convert.ToString(Request.QueryString["mode"]) == "guest"))
            {
                //passing UserOrSessionId value for token generation call
                if (User.Identity.IsAuthenticated)
                    userOrSessionId = User.Identity.Name;
                else
                    userOrSessionId = Request.Cookies["ASP.NET_SessionId"].Value;

                // This method is call to get payment Api header as store on the view
                ViewBag.PaymentApiResponseHeader = _paymentAgent.GetPaymentAuthToken(userOrSessionId, false);

                return _cartAgent.GetCartCount() < 1 ? RedirectToAction<HomeController>(x => x.Index()) : IsEnableSinglePageCheckout ? View("SinglePage", _checkoutAgent.GetUserDetails(accountQuoteViewModel?.UserId ?? 0)) : View("MultiStepCheckout", _checkoutAgent.GetBillingShippingAddress());
            }

            return RedirectToAction("Login", "User", new { returnUrl = "~/checkout", isSinglePageCheckout = IsEnableSinglePageCheckout });
        }

        //Account address
        public virtual ActionResult AccountAddress(int userid = 0, int quoteId = 0,int addressId = 0,string addressType = "", bool isQuoteRequest = false, bool isAddressFromSession = false)
        {
            if(quoteId > 0)
            return PartialView("_BillingShippingAddress", _checkoutAgent.GetBillingShippingAddress(userid, quoteId > 0, addressType, addressId, 0, false, isQuoteRequest, false));
            else
            {
                return PartialView("_BillingShippingAddress", _checkoutAgent.GetBillingShippingAddress(userid, isAddressFromSession, addressType, addressId, 0, false, isQuoteRequest, false));
            }
        }

        public virtual ActionResult RefreshAddressOptions(string addressType, bool isCalculateCart = true)
        {
            // we are refreshing alternate dropdown that ' why we set billing and shipping
            ViewData["addressType"] = addressType == "billing" ? "shipping" : "billing";
            return PartialView("_AddressOptions", _checkoutAgent.GetBillingShippingAddress(0, false, string.Empty, 0, 0, false, false, isCalculateCart));
        }
        

        //Get Shipping options
        public virtual ActionResult ShippingOptions(bool isQuote = false)
        {
            return CheckoutActionView("_ShippingOptions", _checkoutAgent.GetShippingOptions(null, isQuote));
        }

        //get Payment options
        public virtual ActionResult PaymentOptions(bool isQuote = false, bool isPendingOrder = false)
        {
            var model = _checkoutAgent.PaymentOptions(false, isQuote);
            ViewData["isQuote"] = isQuote;
            if (isQuote)
            {
                //passing userOrSessionId value for token generation call
                string userOrSessionId = string.Empty;
                if (User.Identity.IsAuthenticated)
                    userOrSessionId = User.Identity.Name;
                else
                    userOrSessionId = Request.Cookies["ASP.NET_SessionId"].Value;

                // This method is call to get payment Api header as store on the view
                ViewBag.PaymentApiResponseHeader = _paymentAgent.GetPaymentAuthToken(userOrSessionId, false);
            }
            if (isPendingOrder)
                model.RemoveAll(x => x.Type.ToLower().Equals(ZnodeConstant.PAYPAL_EXPRESS.ToLower()));
            return CheckoutActionView("_PaymentOptions", model);
        }

        public virtual ActionResult GetOrderAndPaymentDetails(int omsOrderId, int portalId)
        {

            OrdersViewModel ordersViewModel = _userAgent.GetOrderDetails(omsOrderId, portalId);
            ordersViewModel.IsUserForOfflinePayment = true;
            var paymentModel = _checkoutAgent.PaymentOptions(true);

            string userOrSessionId = string.Empty;
            if (User.Identity.IsAuthenticated)
                userOrSessionId = User.Identity.Name;
            else
                userOrSessionId = Request.Cookies["ASP.NET_SessionId"].Value;

            // This method is call to get payment Api header as store on the view
            ordersViewModel.PaymentApiResponseHeader = _paymentAgent.GetPaymentAuthToken(userOrSessionId, false);
            string ordersView = RenderRazorViewToString(createSchedulerView, ordersViewModel);
            string paymentOptionView = RenderRazorViewToString("_PaymentOptions", paymentModel);

            return Json(new
            {
                orderHtml = ordersView,
               paymentOptionHtml = paymentOptionView
            }, JsonRequestBehavior.AllowGet);
        }
        //Get logged in user cart to review
        public virtual ActionResult CartReview(int? shippingOptionId, int? shippingAddressId, string shippingCode, string additionalInstruction = "", bool isQuoteRequest = false, bool isCalculateCart = true, bool isPendingOrderRequest = false, string jobName = "")
        {
            return PartialView("_CartReview", _cartAgent.CalculateShipping(shippingOptionId.GetValueOrDefault(), shippingAddressId.GetValueOrDefault(), shippingCode, additionalInstruction, isQuoteRequest, isCalculateCart, isPendingOrderRequest, jobName));
        }

        //get promossion and coupons 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ApplyDiscount(string discountCode, bool isGiftCard = false)
        {
            CartViewModel cartViewModel = _cartAgent.ApplyDiscount(discountCode, isGiftCard);
            string totalView = RenderRazorViewToString(TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons,
                vouchers = cartViewModel.Vouchers,
                giftCardCode = discountCode,
                isGiftCard = isGiftCard,
                message = cartViewModel.SuccessMessage,
                freeshipping = !isGiftCard ? cartViewModel.FreeShipping : false,
                encryptedTotalAmount = Encryption.EncryptPaymentToken(Convert.ToString(cartViewModel?.Total).Replace(",", "."))
            }, JsonRequestBehavior.AllowGet);
        }

        //Get address by address id while change address
        [ValidateAntiForgeryToken]
        public virtual ActionResult GetAddress(int AddressId, int OtherAddressId, string addressType = null)
        {
            AddressViewModel model = _userAgent.GetAddress(AddressId);
            if (HelperUtility.IsNull(model))
            {
                //if model is null then associate it by checking if it's associated to the order 
                //It will handle editing one time address
                AddressListViewModel addressListModel = _checkoutAgent.GetBillingShippingAddress(addressType, AddressId, OtherAddressId);
                model = addressListModel?.AddressList
                                        ?.FirstOrDefault(o => o.AddressId == AddressId);
            }
            ViewBag.AddressType = addressType;

            //set countries for address
            if (HelperUtility.IsNull(model))
            {
                model = new AddressViewModel();
            }

            model.OtherAddressId = OtherAddressId;
            model.Countries = _userAgent.GetCountries();

            return PartialView("_EditAddress", model);
        }

        public virtual ActionResult EditAddress(int AddressId, int otherAddressId, string type, string mode = null, int UserId = 0, bool IsFromEdit = false)
        {
            //Address type
            ViewBag.AddressType = type;
            ViewBag.HideDisplayName = (mode == "guest") || !(HttpContext.Request.LogonUserIdentity.IsAuthenticated) ? true : false;
            //Get address list
            AddressListViewModel model = _checkoutAgent.GetBillingShippingAddress(type, AddressId, otherAddressId, UserId, false, IsFromEdit);
            
            if(model.HasError == true && model.ErrorMessage == WebStore_Resources.HttpCode_401_AccessDeniedMsg)
            {
                return Redirect("/404");
            }
            return PartialView("_AccountAddress", model);
        }

        //Change address Billing and shipping.
        public virtual ActionResult ChangeAddress(int AddressId, int otherAddressId, string type, string mode = null)
        {
            //Address type
            ViewBag.AddressType = type;
            ViewBag.HideDisplayName = (mode == "guest") || !(HttpContext.Request.LogonUserIdentity.IsAuthenticated) ? true : false;
            //Get address list
            AddressListViewModel model = _checkoutAgent.GetBillingShippingAddress(type, AddressId, otherAddressId);
            return PartialView("_AccountAddress", model);
        }

        //Update address if user change address at checkout time.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateAddress(AddressViewModel model, bool formChange, string addressType)
        {          
            if (formChange)
            {
                AddressListViewModel allAddresses = new AddressListViewModel();                
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    Helper.ClearCache($"UserAccountAddressList");
                    SessionHelper.RemoveDataFromSession(WebStoreConstants.UserAccountKey);
                    if (HelperUtility.IsNotNull(addressType))
                    {
                        model.IsDefaultShipping = Equals(addressType, WebStoreConstants.ShippingAddressType);
                        model.IsDefaultBilling = Equals(addressType, WebStoreConstants.BillingAddressType);
                        model.UseSameAsShippingAddress = model.IsBothBillingShipping;
                    }
                }
                else
                {
                    allAddresses = _userAgent.GetAddressList();

                    if (HelperUtility.IsNull(allAddresses?.AddressList) || allAddresses.AddressList.Count < 1)
                    {
                        if (HelperUtility.IsNotNull(addressType))
                        {
                            model.IsDefaultShipping = Equals(addressType, WebStoreConstants.ShippingAddressType);
                            model.IsDefaultBilling = Equals(addressType, WebStoreConstants.BillingAddressType);
                        }
                        else
                        {
                            model.IsDefaultShipping = true;
                            model.IsDefaultBilling = true;
                        }
                    }
                }
                if (Equals(addressType?.ToLower(), WebStoreConstants.BillingAddressType))
                {
                    model.OtherAddressId = Convert.ToInt32(allAddresses?.ShippingAddress?.AddressId);                    
                }
                else if (Equals(addressType?.ToLower(), WebStoreConstants.ShippingAddressType))
                {
                    model.OtherAddressId = Convert.ToInt32(allAddresses?.BillingAddress?.AddressId);                  
                }
                //Update Address
                model.CityName = model.CityName?.Trim();
                model.Address1 = model.Address1?.Trim();
                model.PostalCode = model.PostalCode?.Trim();
                model.FirstName = model.FirstName?.Trim();
                model.LastName = model.LastName?.Trim();
                model.PhoneNumber = model.PhoneNumber?.Trim();
                model.DisplayName = model.DisplayName?.Trim();
                model.Address2 = model.Address2?.Trim();
                AddressViewModel resultModel = _userAgent.CreateUpdateAddress(model, addressType);
                             
                if (!string.IsNullOrEmpty(resultModel.ErrorMessage))
                    return Json(new { status = true, error = resultModel.ErrorMessage, addressType = addressType }, JsonRequestBehavior.AllowGet);

                //Set New billing shipping address
                if (resultModel?.AddressId > 0)
                {
                    _checkoutAgent.SetBillingShippingAddress(resultModel.AddressId, model.OtherAddressId, resultModel.UserId, addressType);
                }

                return Json(new { status = resultModel.HasError, error = resultModel.ErrorMessage, addressType = addressType }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Set New billing shipping address
                _checkoutAgent.SetBillingShippingAddress(model.AddressId, model.OtherAddressId, model.UserId, addressType);
                return Json(new { status = false, error = "", addressType = addressType }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual ActionResult UpdateSearchAddress(AddressViewModel addressViewModel)
        {
            AddressViewModel addViewModel = _checkoutAgent.UpdateSearchAddress(addressViewModel);
            string htmlContent = string.Empty;

            return Json(new
            {
                html = htmlContent,
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult DisplayShippingAddress(int userId = 0)
        {
            return PartialView("_DisplayAddress", _checkoutAgent.GetBillingShippingAddress(userId, false, null, 0, 0).ShippingAddress);
        }


        //TODO
        public virtual ActionResult GetPaymentProvider(string paymentType, int paymentSettingId)
        {
            switch (paymentType)
            {
                case "purchase_order":
                    //Recalculations will be skipped in case of pending order
                    return ActionView("_PurchaseOrder", _paymentAgent.GetPaymentDetails(paymentSettingId,"", true));
                default:
                    break;
            }
            return View();
        }
       
        //Submit order (work with both checkout Single/Multistep check out)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubmitOrder(SubmitOrderViewModel submitOrderViewModel)
        {
            if (!TempData.ContainsKey(ZnodeConstant.BrowserRefresh))
            {
                TempData.Add(ZnodeConstant.BrowserRefresh, true);

                OrdersViewModel order = _checkoutAgent.SubmitOrder(submitOrderViewModel);

                if (HelperUtility.IsNotNull(order) && !order.HasError)
                {
                    order.Total = submitOrderViewModel.Total;
                    order.SubTotal = submitOrderViewModel.SubTotal;

                    // Below code is used for "PayPal Express" to redirect payment website.
                    if (!string.IsNullOrEmpty(submitOrderViewModel.PayPalReturnUrl) && !string.IsNullOrEmpty(submitOrderViewModel.PayPalCancelUrl) && HelperUtility.Equals(submitOrderViewModel.PaymentType?.ToLower(), ZnodeConstant.PayPalExpress.ToLower()))
                    {
                        //Check User Authentication
                        CheckUserAuthentication();
                        TempData["Error"] = order.HasError;
                        TempData["ErrorMessage"] = order.ErrorMessage;
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { responseText = order.PayPalExpressResponseText });
                    }

                    // Below code is used for "Amazon Pay" to redirect payment website.
                    if (!string.IsNullOrEmpty(submitOrderViewModel.AmazonPayReturnUrl) && !string.IsNullOrEmpty(submitOrderViewModel.AmazonPayCancelUrl) && HelperUtility.Equals(submitOrderViewModel.PaymentType?.ToLower(), ZnodeConstant.AmazonPay.ToLower()))
                    {
                        //Check User Authentication
                        CheckUserAuthentication();
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { responseText = Convert.ToString(order.PaymentStatus), responseToken = order.TrackingNumber });
                    }

                    // Below code is used, after payment success from "PayPal Express" return to "return url" of "PayPal Express" i.e. "SubmitPaypalOrder".
                    if (submitOrderViewModel.IsFromPayPalExpress)
                    {
                        TempData["Order"] = order;
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { success = true });
                    }
                    // Below code is used, after payment success from "Amazon Pay" return to "return url" of "AmazonPay" i.e. "SubmitAmazonPayOrder".
                    if (submitOrderViewModel.IsFromAmazonPay)
                    {
                        TempData["Order"] = order;
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { success = true });
                    }

                    if (!User.Identity.IsAuthenticated)
                    {
                        _userAgent.RemoveGuestUserSession();
                    }

                    _cartAgent.RemoveAllCartItems(order.OmsOrderId);

                    // Below code is used, for after successfully payment from "Credit Card" return receipt.
                    if (Equals(submitOrderViewModel?.PaymentType?.ToLower(), ZnodeConstant.CreditCard.ToLower()))
                    {
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { receiptHTML = RenderRazorViewToString(CheckoutReceipt, order), omsOrderId = order.OmsOrderId });
                    }

                    if (Equals(submitOrderViewModel?.PaymentType?.ToLower(), ZnodeConstant.ACH.ToLower()))
                    {
                        TempData.Remove(ZnodeConstant.BrowserRefresh);
                        return Json(new { receiptHTML = RenderRazorViewToString(CheckoutReceipt, order), omsOrderId = order.OmsOrderId });
                    }

                    TempData.Remove(ZnodeConstant.BrowserRefresh);
                    TempData["OrderId"] = order.OmsOrderId;
                    return RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
                }

                //Return error message, if payment through "Paypal Express" raises any error.
                if (order.HasError)
                {
                    TempData["Error"] = order.HasError;
                    TempData["ErrorMessage"] = order?.ErrorMessage;
                }

                // Return error message, if payment through "Credit Card" raises any error.
                if (Equals(submitOrderViewModel?.PaymentType?.ToLower(), ZnodeConstant.CreditCard.ToLower()))
                {
                    TempData.Remove(ZnodeConstant.BrowserRefresh);
                    return Json(new { error = order.ErrorMessage });
                }

                // Return error message, if payment through "PayPal Express" raises any error.
                if (!string.IsNullOrEmpty(submitOrderViewModel.PayPalReturnUrl) && !string.IsNullOrEmpty(submitOrderViewModel.PayPalCancelUrl))
                {
                    TempData.Remove(ZnodeConstant.BrowserRefresh);
                    return Json(new { error = order.ErrorMessage, responseText = order.PaymentStatus });
                }

                // Return error message, if payment through "AmazonPay" raises any error.
                if (!string.IsNullOrEmpty(submitOrderViewModel.AmazonPayReturnUrl) && !string.IsNullOrEmpty(submitOrderViewModel.AmazonPayCancelUrl))
                {
                    TempData.Remove(ZnodeConstant.BrowserRefresh);
                    return Json(new { error = order.ErrorMessage, responseText = order.PaymentStatus });
                }
                SetNotificationMessage(GetErrorNotificationMessage(order.ErrorMessage));
            }

            TempData.Remove(ZnodeConstant.BrowserRefresh);
            return RedirectToAction<CheckoutController>(x => x.Index(true));
        }

        [HttpGet]
        public virtual ActionResult SubmitOrder(OrdersViewModel order)
        {
            if (!Equals(order, null) && order.OmsOrderId > 0)
            {
                return ActionView("CheckoutReceipt", order);
            }

            return RedirectToAction<HomeController>(x => x.Index());

        }

        //Remove applied coupon.
        [HttpGet]
        public virtual ActionResult RemoveCoupon(string couponCode)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveCoupon(couponCode);
            cartViewModel.ErrorMessage = string.IsNullOrEmpty(couponCode) ? WebStore_Resources.RequiredCouponCode : WebStore_Resources.ErrorCouponCode;
            string totalView = RenderRazorViewToString(TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons,
                giftCardCode = couponCode,
                isGiftCard = false,
                message = cartViewModel.SuccessMessage,
                encryptedTotalAmount = Encryption.EncryptPaymentToken(Convert.ToString(cartViewModel?.Total).Replace(",", ".")),
                isShippingBasedCoupon = cartViewModel.IsShippingBasedCoupon
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetPaymentDetails(int paymentSettingId, bool isUsedForOfflinePayment = false, decimal remainingOrderAmount = 0)
        {
            PaymentDetailsViewModel paymentDetailsViewModel = _paymentAgent.GetPaymentDetails(paymentSettingId, null, false , isUsedForOfflinePayment, remainingOrderAmount);
            CartViewModel cartViewModel = null;
            string totalView = string.Empty;
            if (HelperUtility.IsNotNull(paymentDetailsViewModel))
            {
                cartViewModel = _paymentAgent.GetCartViewModel();
                totalView = RenderRazorViewToString(TotalTableView, cartViewModel);
            }

            return Json(new
            {
                html = totalView,
                Response = paymentDetailsViewModel,
                coupons = cartViewModel?.Coupons,
                vouchers = cartViewModel?.Vouchers,
                total = Helper.FormatPriceWithCurrency(cartViewModel?.Total, cartViewModel?.CultureCode),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetPaymentDetailsForQuotes(int paymentSettingId, string quoteNumber)
        {
            PaymentDetailsViewModel paymentDetailsViewModel = _paymentAgent.GetPaymentDetails(paymentSettingId, quoteNumber);
            return Json(new
            {
                Response = paymentDetailsViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetPaymentCreditCardDetails(string customerGUID)
        {
            return Json(_paymentAgent.GetPaymentCreditCardDetails(customerGUID), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetSaveCreditCardCount(string customerGUID)
        {
            return Json(!string.IsNullOrEmpty(customerGUID) ? _paymentAgent.GetSaveCreditCardCount(customerGUID) : 0, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetBillingAddressDetail(int portalId, int billingAddressId = 0, int shippingAddressId = 0)
            => Json(new
            {
                data = _userAgent.GetBillingAddressDetail(billingAddressId, shippingAddressId),
                orderNumber = _checkoutAgent.GenerateOrderNumber(portalId)
            }, JsonRequestBehavior.AllowGet);

        [HttpGet]
        public virtual ActionResult SubmitPaypalOrder(int shippingAddressId, int billingAddressId, int shippingOptionId, int paymentSettingId, string additionalInstruction, string token, string paymentCode, string orderNumber, string inHandDate = "", string jobName = "", string shippingConstraintCode = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorUnablePlaceOrder));
                return RedirectToAction<CheckoutController>(x => x.Index(true));
            }

            SubmitOrderViewModel submitOrderViewModel = _checkoutAgent.SetPayPalToken(token, shippingAddressId, billingAddressId, shippingOptionId, paymentSettingId, additionalInstruction, paymentCode, orderNumber, inHandDate, jobName, shippingConstraintCode);
            TempData.Remove(ZnodeConstant.BrowserRefresh);
            var status = SubmitOrder(submitOrderViewModel);
            //Added PayPalToken and Identity in tempdata to be used in further action method.

            bool hasError = HelperUtility.IsNotNull(TempData["Error"]) ? Convert.ToBoolean(TempData["Error"]) : false;
            string ErrorMessage = HelperUtility.IsNotNull(TempData["ErrorMessage"]) ? Convert.ToString(TempData["ErrorMessage"]) : null;
            OrdersViewModel order = (OrdersViewModel)TempData["Order"];
            if (!hasError)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _userAgent.RemoveGuestUserSession();
                }

                _cartAgent.RemoveAllCartItems();

                TempData["OrderId"] = (order?.OmsOrderId).GetValueOrDefault();
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                SetNotificationMessage(GetErrorNotificationMessage(ErrorMessage));
            }

            int omsOrderId = (order?.OmsOrderId).GetValueOrDefault();
            return hasError ? RedirectToAction<CheckoutController>(x => x.Index(true)) : RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
        }


        public virtual ActionResult OrderCheckoutReceipt()
        {
            int omsOrderId = Convert.ToInt32(_checkoutAgent.GetOrderIdFromCookie());
            if (omsOrderId > 0)
            {
                GetService<IWebstoreHelper>().SaveDataInCookie(WebStoreConstants.UserOrderReceiptOrderId, null, 1);

                OrdersViewModel order = _checkoutAgent.GetOrderViewModel(omsOrderId);
                return HelperUtility.IsNotNull(order) ? View(CheckoutReceipt, order) : RedirectToAction<HomeController>(x => x.Index());
            }
            return User.Identity.IsAuthenticated ? RedirectToAction<UserController>(x => x.History(null)) : RedirectToAction<HomeController>(x => x.Index());
        }

        //to get locale wise amount
        protected virtual string GetLocaleWiseAmount(decimal amount)
        {
            string formattedAmount = Convert.ToString(amount);
            if (!string.IsNullOrEmpty(formattedAmount) && formattedAmount.Contains(","))
            {
                formattedAmount = formattedAmount.Replace(",", ".");
            }

            return formattedAmount;
        }

        //Quote receipt after successful quote creation.
        public virtual ActionResult QuoteReceipt(int quoteId, bool isPendingPayment = false)
        {
            ViewData["QuoteId"] = quoteId;
            ViewData["IsPendingPayment"] = isPendingPayment;

            if (!User.Identity.IsAuthenticated)
            {
                _userAgent.RemoveGuestUserSession();
                return RedirectToAction<HomeController>(x => x.Index());
            }

            if (quoteId > 0)
            {
                AccountQuoteViewModel accountQuoteViewModel = _userAgent.GetQuoteView(quoteId);
                return HelperUtility.IsNotNull(accountQuoteViewModel) ? View("_QuoteReceipt", accountQuoteViewModel) : RedirectToAction<HomeController>(x => x.Index());
            }
            return RedirectToAction<UserController>(x => x.QuoteHistory(null));
        }

        //Get Ajax headers.
        [PreventAccess]
        public virtual JsonResult GetAjaxHeaders()
        {
            AjaxHeadersModel response = _checkoutAgent.GetAppHeader();

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //Get Payment App Header
        public virtual JsonResult GetPaymentAppHeader()
        {
            AjaxHeadersModel response = _checkoutAgent.GetPaymentAPIHeader();
            return Json(new
            {
                response.Authorization
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult GetshippingBillingAddress(int portalId ,int shippingId=0,int billingId=0)
        {
            AddressListViewModel addressListViewModel = _userAgent.GetshippingBillingAddress();
            return Json(new
            {
                Shipping = _checkoutAgent.SetAddressByAddressType(WebStoreConstants.ShippingAddressType, shippingId, addressListViewModel),
                Billing = _checkoutAgent.SetAddressByAddressType(WebStoreConstants.BillingAddressType, billingId, addressListViewModel),
                orderNumber = _checkoutAgent.GenerateOrderNumber(portalId)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //Get list of search billing locations.
        public JsonResult GetSearchBillingLocation(string query)
        {
            return Json(_checkoutAgent.GetSearchLocation(query, PortalAgent.CurrentPortal.PortalId, WebStoreConstants.BillingAddressType), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //Get list of search shipping locations.
        public JsonResult GetSearchShippingLocation(string query)
        {
            return Json(_checkoutAgent.GetSearchLocation(query, PortalAgent.CurrentPortal.PortalId, WebStoreConstants.ShippingAddressType), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //Set recipient name in the respective address type of cart.
        public JsonResult SetAddressRecipientNameInCart(string firstName, string lastName, string addressType)
        {
            return Json(_checkoutAgent.SetAddressRecipientNameInCart(firstName, lastName, addressType), JsonRequestBehavior.AllowGet);
        }

        //Get address details on the basis of address id.
        public JsonResult GetAddressById(int addressId, string addressType = "", bool isCalculateCart = true)
        {
            if (addressId > 0)
            {
                AddressViewModel model = _checkoutAgent.GetAddressById(addressId, addressType, isCalculateCart);
                model.Countries = _userAgent.GetCountries();

                if(model.HasError)
                {
                    return Json(new
                    {
                        Error = model.ErrorMessage,
                        StatusCode=model.StateCode
                    }
                  , JsonRequestBehavior.AllowGet);
                }

                string address = _checkoutAgent.CheckAndAppendAlternateAddress(model);
                if (model.HasError)
                    return Json(new { model.ErrorMessage }, JsonRequestBehavior.AllowGet);

                if (model.HasError)
                    return Json(new { ErrorMessage = model.ErrorMessage }, JsonRequestBehavior.AllowGet);

                if (PortalAgent.CurrentPortal.EnableAddressValidation)
                {
                    AddressListViewModel listViewModel = _checkoutAgent.GetRecommendedAddressList(model);

                    string htmlContent = string.Empty;
                    if (listViewModel?.AddressList?.Count > 0)
                    {
                        htmlContent = RenderRazorViewToString("../Checkout/_RecommendedAddress", listViewModel);
                    }

                    var htmlString = string.Format(WebStore_Resources.AddressHTML, model.DisplayName, model.Address1, address, model.CityName, model.StateName, model.PostalCode);
                    if (model.HasError)
                        return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);

                    return Json(new
                    {
                        html = htmlString,
                        htmlContent = htmlContent,
                        model = model
                    }
                  , JsonRequestBehavior.AllowGet);
                }

                var html = string.Format(WebStore_Resources.AddressHTML, model.Address1, model.Address2, address, model.CityName, model.StateName, model.PostalCode);
                return Json(new
                {
                    html = html,
                    model = model
                }
              , JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResult(), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult GenerateOrderNumber(int portalId)
        {
            return Json(new { orderNumber = _checkoutAgent.GenerateOrderNumber(portalId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNewAddress(int AddressId, int otherAddressId, string type, string mode = null, bool isNewAddress = false)
        {
            //Address type
            ViewBag.AddressType = type;
            ViewBag.IsNewAddress = isNewAddress;
            ViewBag.HideDisplayName = (mode == "guest") || !(HttpContext.Request.LogonUserIdentity.IsAuthenticated) ? true : false;
           
            //Get address list
            AddressViewModel model = _userAgent.GetAddressByAddressType(AddressId, type);
            return PartialView("_CreateAddress", model);
        }


        [HttpPost]
        public virtual ActionResult GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenModel)
        {
            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);

            string auth = RenderRazorViewToString("_CyberSourceGateway", paymentTokenViewModel);
            return Json(new
            {
                html = auth,
            }, JsonRequestBehavior.AllowGet);
        }

        #region Amazon Payment

        //Get AmazonPay address.
        [HttpGet]
        public virtual ActionResult GetAddressDetails(int? shippingOptionId = 0, int? shippingAddressId = 0, string shippingCode = "", int paymentSettingId = 0, string paymentCode = "", string clientId = "", string sellerId = "", bool testMode = true, string access_token = null)
        {
            ViewData["ShippingOptionId"] = shippingOptionId;
            ViewData["ShippingAddressId"] = shippingAddressId;
            ViewData["ShippingCode"] = shippingCode;
            ViewData["PaymentSettingId"] = paymentSettingId;
            ViewData["PaymentCode"] = paymentCode;
            ViewData["ClientId"] = clientId;
            ViewData["SellerId"] = sellerId;
            ViewData["TestMode"] = testMode;
            ViewData["AccessToken"] = access_token;
            return View("_AmazonPayAddress", _cartAgent.CalculateShipping(0, 0, ""));
        }

        //Submit Amazon Pay Order.
        [HttpGet]
        public virtual ActionResult SubmitAmazonOrder(string amazonOrderReferenceId = "", string paymentType = "", int shippingOptionId = 0, int paymentSettingId = 0, string paymentCode = "", string additionalInstruction = "", string captureId = "", string orderNumber = "",string inHandDate = "", string jobName = "", string shippingConstraintCode = "")
        {
            SubmitOrderViewModel submitOrderViewModel = _checkoutAgent.SetAmazonPayDetails(amazonOrderReferenceId, paymentType, shippingOptionId, paymentSettingId, paymentCode, additionalInstruction, captureId, orderNumber,inHandDate, jobName, shippingConstraintCode);
            TempData.Remove(ZnodeConstant.BrowserRefresh);
            var status = SubmitOrder(submitOrderViewModel);
            if (!User.Identity.IsAuthenticated)
            {
                _userAgent.RemoveGuestUserSession();
            }

            _cartAgent.RemoveAllCartItems();
            OrdersViewModel order = (OrdersViewModel)TempData["Order"];
            Response.Cookies["amazon_Login_state_cache"].Expires = DateTime.Now.AddDays(-1);
            TempData["OrderId"] = (order?.OmsOrderId).GetValueOrDefault();
            int omsOrderId = (order?.OmsOrderId).GetValueOrDefault();
            return HelperUtility.IsNull(status) ? RedirectToAction<CheckoutController>(x => x.Index(true)) : RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
        }

        //Get AmazonPay cart.
        public virtual ActionResult AmazonCartReview(int? shippingOptionId, int? shippingAddressId, string shippingCode, string amazonOrderReferenceId, int paymentSettingId, string total)
        {
            AddressViewModel address = _checkoutAgent.GetAmazonAddress(paymentSettingId, amazonOrderReferenceId, total);
            return PartialView("_CartReview", _cartAgent.CalculateAmazonShipping(shippingOptionId.GetValueOrDefault(), shippingAddressId.GetValueOrDefault(), shippingCode, address));
        }

        //Get Amazon Pay shipping option.
        public virtual ActionResult AmazonShippingOptions(string amazonOrderReferenceId, int paymentSettingId, string total, string accesstoken,string accountNumber, string shippingMethod)
        {
            return CheckoutActionView("AmazonShippingOption", _checkoutAgent.GetAmazonShippingOptions(amazonOrderReferenceId, paymentSettingId, total, null, accesstoken, accountNumber, shippingMethod));
        }

        //Get payment options.
        public virtual ActionResult AmazonPaymentOptions(bool isQuote = false)
        {
            ViewData["isQuote"] = isQuote;
            return CheckoutActionView("_AmazonPay", _checkoutAgent.AmazonPaymentSetting());
        }

        //Remove applied Voucher.
        [HttpGet]
        public virtual ActionResult RemoveVoucher(string voucherNumber)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveVoucher(voucherNumber);
            cartViewModel.ErrorMessage = string.IsNullOrEmpty(voucherNumber) ? WebStore_Resources.RequiredCouponCode : WebStore_Resources.ErrorCouponCode;
            string totalView = RenderRazorViewToString(TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons,
                giftCardCode = voucherNumber,
                vouchers = cartViewModel.Vouchers,
                isGiftCard = true,
                message = cartViewModel.SuccessMessage,
                encryptedTotalAmount = Encryption.EncryptPaymentToken(Convert.ToString(cartViewModel?.Total).Replace(",", "."))
            }, JsonRequestBehavior.AllowGet);
        }

        //Set mode wise result 
        protected virtual ActionResult CheckoutActionView(string viewName, object model)
        {
            if (IsEnableSinglePageCheckout)
            {
                return PartialView(viewName, model);
            }
            return View(viewName.Remove(0, 1), model);
        }

        //Set mode wise result 
        protected virtual ActionResult CheckoutActionView(string viewName)
        {
            if (IsEnableSinglePageCheckout)
            {
                return PartialView(viewName);
            }
            return View(viewName.Remove(0, 1));
        }

        #endregion

        #region protected Methods
        //Check User Is Authenticated ,If not Authenticate then Remove guest user details from session.
        protected virtual void CheckUserAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _userAgent.RemoveGuestUserSession();
            }
        }

        #endregion
        
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult GetAuthorizeNetToken(PaymentTokenViewModel paymentTokenModel)
        {
            AddressListViewModel addressListViewModel = _userAgent.GetshippingBillingAddress();
            paymentTokenModel.AddressListViewModel = addressListViewModel;
            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);

            paymentTokenViewModel.IsIframeRenderInPopup = paymentTokenModel.IsIframeRenderInPopup;
            string auth = RenderRazorViewToString("_AuthorizeNet", paymentTokenViewModel);
            return Json(new
            {
                html = auth,
                customerProfileId = paymentTokenViewModel.CustomerProfileId
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult AuthorizeIframeCommunicator()
        {     // This method calls when payment get cancel or not success. Write the code of cancelation or warning
            return View();
        }

        // To render braintree hosted fields 
        [HttpPost]
        public virtual ActionResult GetIframeViewWithToken(PaymentTokenViewModel paymentTokenModel, string partialView)
        {
            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);
            if (!string.IsNullOrEmpty(paymentTokenViewModel.PaymentGatewayToken))
            {
                paymentTokenViewModel.IsIframeRenderInPopup = paymentTokenModel.IsIframeRenderInPopup;
                string auth = RenderRazorViewToString(partialView, paymentTokenViewModel);
                return Json(new
                {
                    isSuccess = true,
                    html = auth,
                    customerProfileId = paymentTokenViewModel.CustomerProfileId
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    isSuccess = false,
                    error = WebStore_Resources.ErrorConnectingPaymentMethod
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}