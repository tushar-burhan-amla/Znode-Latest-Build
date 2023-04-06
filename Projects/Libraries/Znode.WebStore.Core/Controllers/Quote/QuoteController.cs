using System;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Controllers
{
    public class QuoteController : BaseController
    {
        #region Private Read-only members
        private readonly IQuoteAgent _quoteAgent;
        private readonly ICartAgent _cartAgent;
        private readonly ICheckoutAgent _checkoutAgent;
        private const string singlePageCheckoutView = "~/Views/Themes/{0}/Views/Checkout/SinglePage.cshtml";
        private readonly string QuoteRequestHistoryView = "QuoteRequestHistory";
        private readonly string CheckoutReceipt = "CheckoutReceipt";
        private const string AmazonPayDetailsView = "~/Views/Themes/{0}/Views/Quote/_AmazonPay.cshtml";
        #endregion

        #region Public Constructor
        public QuoteController(IQuoteAgent quoteAgent,ICartAgent cartAgent, ICheckoutAgent checkoutAgent)
        {
            _quoteAgent = quoteAgent;
            _cartAgent = cartAgent;
            _checkoutAgent = checkoutAgent;
        }
        #endregion
        // GET: Checkout
        public virtual ActionResult Index()
        {
            if (!Equals(Request.QueryString["ShippingId"], null))
            {
                _cartAgent.AddEstimatedShippingIdToCartViewModel(int.Parse(Convert.ToString(Request.QueryString["ShippingId"])));
            }

            if (User.Identity.IsAuthenticated)
            {
                if (_cartAgent.GetCartCount() < 1)
                { return RedirectToAction<HomeController>(x => x.Index()); }

                CheckoutViewModel model = _checkoutAgent.GetUserDetails();
                model.IsQuoteRequest = true;
                model.InHandDate = DateTime.Now.AddDays(WebStoreConstants.DefaultInHandDays);

                return View(string.Format(singlePageCheckoutView, PortalAgent.CurrentPortal.Theme), model);
            }
            return RedirectToAction("Login", "User", new { returnUrl = "~/quote" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubmitQuote(QuoteCreateViewModel quoteCreateViewModel)
        {
            QuoteCreateViewModel quote = _quoteAgent.Create(quoteCreateViewModel);

            if (HelperUtility.IsNotNull(quote) && !quote.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.QuoteCreateSuccessMessage));
                return RedirectToAction<QuoteController>(x => x.GetQuoteReceipt(quote.OmsQuoteId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(!string.IsNullOrEmpty(quote?.ErrorMessage) ? quote?.ErrorMessage : WebStore_Resources.QuoteCreateErrorMessage));
            return RedirectToAction<QuoteController>(x => x.Index());
        }

        //Quote receipt after successful quote creation.
        public virtual ActionResult GetQuoteReceipt(int quoteId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (quoteId > 0)
                {
                    QuoteResponseViewModel quoteResponseViewModel = _quoteAgent.GetQuoteReceipt(quoteId);
                    if (HelperUtility.IsNotNull(quoteResponseViewModel) && !quoteResponseViewModel.HasError)
                    {
                        return HelperUtility.IsNotNull(quoteResponseViewModel) ? View("_QuoteReceipt", quoteResponseViewModel) : RedirectToAction<HomeController>(x => x.Index());
                    }
                    return RedirectToAction<ErrorPageController>(x => x.PageNotFound());
                }
            }
            return RedirectToAction<QuoteController>(x => x.Index());
        }

        //Quote receipt after successful quote creation.
        public virtual ActionResult GetQuote(int omsQuoteId)
        {
            if (GlobalAttributeHelper.IsQuoteRequestEnabled() == false)
            {
                return Redirect("/404");
            }
            if (User.Identity.IsAuthenticated)
            {
                if (omsQuoteId > 0)
                {
                    QuoteResponseViewModel quoteResponseViewModel = _quoteAgent.GetQuote(omsQuoteId);
                    if (HelperUtility.IsNotNull(quoteResponseViewModel) && !quoteResponseViewModel.HasError)
                    {
                        return HelperUtility.IsNotNull(quoteResponseViewModel) ? View("QuoteDetails", quoteResponseViewModel) : RedirectToAction<HomeController>(x => x.Index());
                    }
                    return RedirectToAction<ErrorPageController>(x => x.PageNotFound());
                }
            }
            return RedirectToAction<ErrorPageController>(x => x.PageNotFound());
        }

        //Convert quote to an order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderViewModel) 
        {
            if (HelperUtility.IsNotNull(convertToOrderViewModel))
            {
                OrdersViewModel order = _quoteAgent.ConvertQuoteToOrder(convertToOrderViewModel);

                //Below code is used for "PayPal Express" to redirect payment website.
                if (!string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.PayPalReturnUrl) && !string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.PayPalCancelUrl) && IsPaypalExpressPayment(convertToOrderViewModel?.PaymentDetails?.PaymentType))
                {
                        TempData["Error"] = order.HasError;
                        TempData["ErrorMessage"] = order.ErrorMessage;
                        return Json(new { responseText = order.PayPalExpressResponseText , orderNumber = order.OrderNumber });
                }
                //Below code is used, after payment success from "PayPal Express" return to "return url" of "PayPal Express" i.e. "SubmitPaypalRequest".
                if (convertToOrderViewModel.PaymentDetails.IsFromPayPalExpress)
                {
                    TempData["Order"] = order;
                    return Json(new { success = true });
                }
                // Below code is used for "Amazon Pay" to redirect payment website.
                if (!string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.AmazonPayReturnUrl) && !string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.AmazonPayCancelUrl) && IsAmazonPayPayment(convertToOrderViewModel?.PaymentDetails?.PaymentType))
                {
                    return Json(new { responseText = Convert.ToString(order.PaymentStatus), responseToken = order.TrackingNumber, orderNumber = order.OrderNumber });
                }
                // Below code is used, after payment success from "Amazon Pay" return to "return url" of "AmazonPay" i.e. "SubmitAmazonPayOrder".
                if (convertToOrderViewModel.PaymentDetails.IsFromAmazonPay)
                {
                    TempData["Order"] = order;
                    return Json(new { success = true });
                }

                if (order.OmsOrderId > 0)
                {
                    // Below code is used, for after successfully payment from "Credit Card" return receipt.
                    if (IsCreditCardPayment(convertToOrderViewModel?.PaymentDetails.PaymentType) || IsACHPayment(convertToOrderViewModel?.PaymentDetails.PaymentType))
                    {
                        return Json(new { receiptHTML = RenderRazorViewToString(CheckoutReceipt, order), omsOrderId = order.OmsOrderId });
                    }

                    return RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
                }

                // Return error message, if payment through "Credit Card" raises any error.
                if (IsCreditCardPayment(convertToOrderViewModel?.PaymentDetails.PaymentType))
                {
                    return Json(new { error = order.ErrorMessage });
                }

                // Return error message, if payment through "AmazonPay" raises any error.
                if (!string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.AmazonPayReturnUrl) && !string.IsNullOrEmpty(convertToOrderViewModel?.PaymentDetails?.AmazonPayCancelUrl))
                {
                    return Json(new { error = order.ErrorMessage, responseText = order.PaymentStatus });
                }

                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ConvertQuoteToOrderErrorMessage));
            }
            return RedirectToAction<QuoteController>(x => x.GetQuote(convertToOrderViewModel.OmsQuoteId));
        }

        //Check payment type is ACH payment method
        public virtual bool IsACHPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType, ZnodeConstant.ACH, StringComparison.InvariantCultureIgnoreCase);
            }
            else return false;
        }

        //Submit Amazon Pay Order.
        [HttpGet]
        public virtual ActionResult AmazonRequest(int quoteId, string amazonOrderReferenceId = "", string paymentType = "", int paymentSettingId = 0, string paymentCode = "", string captureId = "", string orderNumber= "")
        {
            ConvertQuoteToOrderViewModel submitOrderViewModel = _quoteAgent.SetAmazonPayDetails(amazonOrderReferenceId, paymentType, paymentSettingId, paymentCode, quoteId, captureId, orderNumber);
            
            var status = ConvertQuoteToOrder(submitOrderViewModel);

            OrdersViewModel order = (OrdersViewModel)TempData["Order"];
            Response.Cookies["amazon_Login_state_cache"].Expires = DateTime.Now.AddDays(-1);
            TempData["OrderId"] = (order?.OmsOrderId).GetValueOrDefault();
            int omsOrderId = (order?.OmsOrderId).GetValueOrDefault();
            return HelperUtility.IsNull(status) ? RedirectToAction<CheckoutController>(x => x.Index(true)) : RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
        }

        [HttpGet]
        public virtual ActionResult PaypalRequest(int paymentSettingId, string token, string paymentCode, int quoteId)
        {
            if (string.IsNullOrEmpty(token))
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorUnablePlaceOrder));
                return RedirectToAction<CheckoutController>(x => x.Index(true));
            }

            ConvertQuoteToOrderViewModel convertToOrderViewModel = _quoteAgent.SetPayPalToken(token, paymentSettingId, paymentCode, quoteId);

            var status = ConvertQuoteToOrder(convertToOrderViewModel);
            //Added PayPalToken and Identity in tempdata to be used in further action method.

            bool hasError = HelperUtility.IsNotNull(TempData["Error"]) ? Convert.ToBoolean(TempData["Error"]) : false;
            string ErrorMessage = HelperUtility.IsNotNull(TempData["ErrorMessage"]) ? Convert.ToString(TempData["ErrorMessage"]) : null;
            OrdersViewModel order = (OrdersViewModel)TempData["Order"];
            if (!hasError)
            {
                TempData["OrderId"] = (order?.OmsOrderId).GetValueOrDefault();
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                SetNotificationMessage(GetErrorNotificationMessage(ErrorMessage));
            }

            int omsOrderId = (order?.OmsOrderId).GetValueOrDefault();
            return hasError ? RedirectToAction<CheckoutController>(x => x.Index(true)) : RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
        }

        //Get quote history 
        public virtual ActionResult QuoteHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (GlobalAttributeHelper.IsQuoteRequestEnabled() == false)
            {
                return Redirect("/404");
            }
            if (User.Identity.IsAuthenticated)
            {
                //Get Quote list.
                QuoteListViewModel quoteListViewModel = _quoteAgent.GetQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

                //Get the grid model.
                quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.Quotes, WebStoreConstants.ZnodeWebStoreQuoteRequests, string.Empty, null, true, true, null);
                quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

                //Returns the Account Quote list.
                return ActionView(QuoteRequestHistoryView, quoteListViewModel);
            }
            return RedirectToAction("Login", "User", new { returnUrl = "~/quote/quotehistory" });
        }

        //Get AmazonPay address.
        [HttpGet]
        public virtual ActionResult GetAmazonPaymentDetails(int? shippingOptionId = 0, int? shippingAddressId = 0, string shippingCode = "", int paymentSettingId = 0, string paymentCode = "", string clientId = "", string sellerId = "", bool testMode = true, string access_token = null, int quoteId = 0)
        {
            ViewData["PaymentSettingId"] = paymentSettingId;
            ViewData["PaymentCode"] = paymentCode;
            ViewData["ClientId"] = clientId;
            ViewData["SellerId"] = sellerId;
            ViewData["TestMode"] = testMode;
            ViewData["AccessToken"] = access_token;
            ViewData["QuoteId"] = quoteId;
            return View("_AmazonPayAddress", _quoteAgent.GetQuote(quoteId));
        }

        //Get Amazon Payment Details
        [HttpGet]
        public virtual ActionResult AmazonPaymentOptions(bool isQuotes = false)
        {
            PaymentSettingViewModel paymentDetailsViewModel = _checkoutAgent.AmazonPaymentSetting(isQuotes);
            string amazonDetailHtml = RenderRazorViewToString(string.Format(AmazonPayDetailsView, PortalAgent.CurrentPortal.Theme), paymentDetailsViewModel);
            return Json(new
            {
                html = amazonDetailHtml,
            },  JsonRequestBehavior.AllowGet);
        }

        #region private method

        //Check payment type is Credit Card payment method
        public virtual bool IsCreditCardPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType, ZnodeConstant.CreditCard, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
        }

        //Check payment type is Paypal Express
        protected virtual bool IsPaypalExpressPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType.Replace("_", ""), ZnodeConstant.PayPalExpress, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
        }

        //Check payment type is Amazon Pay
        protected virtual bool IsAmazonPayPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType, ZnodeConstant.AmazonPay, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
        }
        #endregion
    }
}
