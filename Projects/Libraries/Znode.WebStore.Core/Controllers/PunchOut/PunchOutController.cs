using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using Znode.WebStore.Core.Extensions;
using Znode.Engine.Exceptions;

namespace Znode.Engine.WebStore.Controllers
{
    //To Do: This is Phase-1 implementation. Phase 2 will be implemented in the next release.
    public class PunchOutController : BaseController
    {
        #region Private Readonly members       
        private readonly ITradeCentricAgent _tradeCentricAgent;
        private readonly IUserAgent _userAgent;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IProductAgent _productAgent;
        private readonly ICartAgent _cartAgent;
        #endregion

        #region Public Constructor
        public PunchOutController(ITradeCentricAgent tradeCentricAgent, IUserAgent userAgent, IAuthenticationHelper authenticationHelper, IProductAgent productAgent, ICartAgent cartAgent)
        {
            _tradeCentricAgent = tradeCentricAgent;
            _userAgent = userAgent;
            _authenticationHelper = authenticationHelper;
            _productAgent = productAgent;
            _cartAgent = cartAgent;
        }
        #endregion

        [HttpPost]
        [TradeCentricAuthorize]
        public virtual ActionResult CreateSession([System.Web.Http.FromBody] TradeCentricSessionRequestViewModel punchOutSessionRequestViewModel)
        {
            string launchUrl = string.Empty;
            if (IsNotNull(punchOutSessionRequestViewModel))
            {
                launchUrl = _tradeCentricAgent.GetUserStoreSessionUrl(punchOutSessionRequestViewModel);
                if (string.IsNullOrEmpty(launchUrl))
                    return View("ErrorPage");
            }
            ZnodeLogging.LogMessage("Launch URL: ", WebStoreConstants.TradeCentricComponent, TraceLevel.Info, launchUrl);
            return Json(new { start_url = launchUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult InitializeSession(string token, string operation, string selectedItem = "")
        {
            ZnodeLogging.LogMessage("Token to validate: ", WebStoreConstants.TradeCentricComponent, TraceLevel.Warning, token);
            LoginViewModel loginViewModel = _tradeCentricAgent.ValidateLogin(token);
            if (!loginViewModel.HasError)
            {
                _authenticationHelper.SetAuthCookie(loginViewModel.Username, loginViewModel.RememberMe);
                _tradeCentricAgent.LoadUserCart(operation);
                loginViewModel.IsFromSocialMedia = false;
                _cartAgent.ClearCartCountFromSession();

                //Redirection to the login url.
                _authenticationHelper.RedirectFromLoginPage(loginViewModel.Username, true);

                string url = _productAgent.GetProductUrl(selectedItem, Url);
                if (string.IsNullOrEmpty(url))
                    url = GetReturnUrlAfterLogin(loginViewModel, string.Empty);

                if (!string.IsNullOrEmpty(url))
                    return Redirect(url);
            }
            else
            {
                ZnodeLogging.LogMessage("LoginFailed ", WebStoreConstants.TradeCentricComponent, TraceLevel.Warning, loginViewModel);
            }
            TempData[WebStoreConstants.Notifications] = GenerateNotificationMessages(WebStore_Resources.InvalidTradeCentricToken, NotificationType.error);
            return RedirectToAction<UserController>(x => x.Login("/"));
        }

        [HttpGet]
        public virtual ActionResult TransferCart()
        {
            CartViewModel model = _cartAgent.GetCart(false, true);
            string redirectURL = _tradeCentricAgent.TransferCart(model);
            if (!string.IsNullOrEmpty(redirectURL))
            {
                return Redirect(redirectURL);
            }
            TempData[WebStoreConstants.Notifications] = GenerateNotificationMessages(WebStore_Resources.TradeCentricTransferCartErrorMessage, NotificationType.error);
            return RedirectToAction<HomeController>(o => o.Index());
        }

        [NonAction]
        protected string GetReturnUrlAfterLogin(LoginViewModel loginViewModel, string returnUrl, bool isFromSocialMedia = false)
        {
            loginViewModel.ReturnUrl = returnUrl;
            return _userAgent.GetReturnUrlAfterLogin(loginViewModel);
        }

        //Place order for TradeCentric user.
        [HttpPost]
        [TradeCentricAuthorize]
        public virtual ActionResult PlaceOrder(TradeCentricOrderViewModel tradeCentricOrderViewModel)
        {
            if (IsNotNull(tradeCentricOrderViewModel))
            {
                OrdersViewModel ordersViewModel = _tradeCentricAgent.SubmitOrder(tradeCentricOrderViewModel);
                if (ordersViewModel.HasError)
                {
                    return Json(new { response = ErrorCodes.ErrorUnAuthorized, message = ordersViewModel.ErrorMessage });
                }
                return Json(new { response = ErrorCodes.Success, order_id = ordersViewModel.OmsOrderId, order_number = ordersViewModel.OrderNumber, message = WebStore_Resources.SuccessOrderCreated });
            }
            return Json(new { response = ErrorCodes.NullModel, message = WebStore_Resources.TradeCentricUserModelNotNull });
        }
    }
}

