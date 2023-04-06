using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.WebStore.Core.Agents;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Engine.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Znode.WebStore.Core.Extensions;

namespace Znode.Engine.WebStore.Controllers
{
    public partial class UserController : BaseController
    {
        #region Private Read-only members
        private readonly IUserAgent _userAgent;
        private readonly IPaymentAgent _paymentAgent;
        private readonly IImportAgent _importAgent;
        private readonly IAuthenticationHelper _authenticationHelper;
        private LoginViewModel model = null;
        private ApplicationSignInManager _signInManager;
        private readonly ICartAgent _cartAgent;
        private readonly IFormBuilderAgent _formBuilderAgent;
        private readonly IPowerBIAgent _powerBIAgent;
        private readonly IRMAReturnAgent _rmaReturnAgent;
        private readonly ISaveForLaterAgent _saveForLater;
        private readonly string _QuickOrderPadView = "_QuickOrderPadView";
        private readonly string _MultipleQuickOrdersView = "_MultipleQuickOrders";
        private readonly string createEditTemplateView = "CreateEditTemplate";
        private readonly string SavedCreditCards = "SavedCreditCards";
        private readonly string CreditCardDetail = "_CreditCardDetails";
        private readonly string _UpdatedTemplateName = "UpdatedTemplateName";
        private readonly string ImportShippingAddressView = "ImportShippingAddress";
        private readonly string ImportUserView = "ImportUser";
        private readonly string ModelStatePortalIds = "PortalIds";
        private readonly string ImportLogsView = "ImportLogs";
        private readonly string UserImportLogsView = "_UserImportLogs";
        private readonly string CheckoutReceipt = "CheckoutReceipt";
        private readonly string WebStoreCustomerQuoteApprovalHistoryGridType = "WebStoreCustomerQuoteApprovalHistory";
        #endregion

        #region Public Constructor        
        public UserController(IUserAgent userAgent, ICartAgent cartAgent, IAuthenticationHelper authenticationHelper, IPaymentAgent paymentAgent, IImportAgent importAgent, IFormBuilderAgent formBuilderAgent, IPowerBIAgent powerBIAgent)
        {
            _userAgent = userAgent;
            _authenticationHelper = authenticationHelper;
            _cartAgent = cartAgent;
            _paymentAgent = paymentAgent;
            _importAgent = importAgent;
            _formBuilderAgent = formBuilderAgent;
            _powerBIAgent = powerBIAgent;
            _rmaReturnAgent = DependencyResolver.Current.GetService<IRMAReturnAgent>();
            _saveForLater = DependencyResolver.Current.GetService<ISaveForLaterAgent>();
        }
        #endregion

        #region Public Methods

        #region Login
        //Login.
        [AllowAnonymous]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult Login(string returnUrl)
        {
            //Check whether the Auth Cookie & Session data is available for the logged in user. If valid data available then redirect to Index view.
            if (User.Identity.IsAuthenticated && HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)))
                return RedirectToAction(ZnodeWebstoreSettings.HomeAction, ZnodeWebstoreSettings.HomeController);
            else if (User.Identity.IsAuthenticated)
                _userAgent.Logout();

            //Get user name from cookies.
            GetLoginRememberMeCookie();

            System.Web.HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            if (Request.IsAjaxRequest())
                return PartialView("_Login", model);

            return View(model);
        }

        //Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaAuthorization(AttributeCode = "IsCaptchaRequiredForLogin")]
        public virtual ActionResult Login(LoginViewModel model, string returnUrl, bool isSinglePageCheckout = false)
        {
            if (ModelState.IsValid)
            {
                model.IsWebStoreUser = true;
                //Authenticate the User Credentials.
                LoginViewModel loginViewModel = _userAgent.Login(model);

                if (!loginViewModel.HasError)
                {
                    SetDataAfterLogin(model);
                    loginViewModel.IsFromSocialMedia = false;

                    //Redirection to the Login Url.
                    _authenticationHelper.RedirectFromLoginPage(model.Username, true);

                    string url = GetReturnUrlAfterLogin(loginViewModel, returnUrl);
                    if (string.IsNullOrEmpty(url))
                        url = returnUrl;

                    if (Request.IsAjaxRequest() && !string.IsNullOrEmpty(url))
                        return RedirectToAction<UserController>(x => x.ReturnRedirectUrl(url));


                    if (!string.IsNullOrEmpty(url))
                        return Redirect(url);

                }
                if (Request.IsAjaxRequest())
                {
                    if (HelperUtility.IsNotNull(loginViewModel) && loginViewModel.IsResetPassword)
                    {
                        TempData[WebStoreConstants.UserName] = model.Username;
                        TempData[WebStoreConstants.CheckoutReturnUrl] = WebStoreConstants.CheckoutReturnUrl;
                        return Json(new { status = true, error = "", isResetPassword = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, error = loginViewModel.ErrorMessage, captchaHtml = GlobalAttributeHelper.IsCaptchaRequiredForLogin() ? RenderRazorViewToString("_Captcha", new object()) : string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (HelperUtility.IsNotNull(loginViewModel) && loginViewModel.IsResetPassword)
                    {
                        TempData[WebStoreConstants.UserName] = model.Username;
                        return RedirectToAction<UserController>(x => x.ResetWebstorePassword());
                    }
                    else
                        SetNotificationMessage(GetErrorNotificationMessage(loginViewModel.ErrorMessage));
                }
            }
            else if (Request.IsAjaxRequest() && GlobalAttributeHelper.IsCaptchaRequiredForLogin())
            {
                model.ErrorMessage = WebStore_Resources.ErrorCaptchaCode;
                return PartialView("_Login", model);
            }

            return View(model);
        }


        [HttpGet]
        public virtual JsonResult ReturnRedirectUrl(string redirectUrl)
        {
            if (!string.IsNullOrEmpty(redirectUrl))
                redirectUrl = redirectUrl.Replace("~", "");
            return Json(new { status = true, link = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ImpersonationLogin(string token)
        {

            LoginViewModel loginViewModel = _userAgent.ImpersonationLogin(token);
            if (!loginViewModel.HasError)
            {
                //adding cart details for impersonation.
                SetDataAfterImpersonationLogin(loginViewModel);
                loginViewModel.IsFromSocialMedia = false;

                //Redirection to the Login Url.
                _authenticationHelper.RedirectFromLoginPage(loginViewModel.Username, true);

                string Url = GetReturnUrlAfterLogin(loginViewModel, loginViewModel.ReturnUrl);
                if (!string.IsNullOrEmpty(Url))
                    return Redirect(Url);

                if (Request.IsAjaxRequest())
                    return Json(new { status = true, error = "" }, JsonRequestBehavior.AllowGet);
            }
            TempData[WebStoreConstants.Notifications] = GenerateNotificationMessages(WebStore_Resources.InvalidCSRToken, NotificationType.error);
            return RedirectToAction<UserController>(x => x.Login("/"));
        }
        //Login Status.
        [AllowAnonymous]
        public virtual PartialViewResult LoginStatus()
        {
            ViewBag.FirstName = _userAgent.GetUserViewModelFromSession()?.FirstName;
            return PartialView("_LoginPartial");
        }

        #endregion

        #region Log Out
        //Logs off the user from the site.
        [AllowAnonymous]
        public virtual ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                ExternalLoginInfo loginInfo = SessionHelper.GetDataFromSession<ExternalLoginInfo>(WebStoreConstants.SocialLoginDetails);

                if (HelperUtility.IsNotNull(loginInfo))
                    return Redirect(_userAgent.Logout(loginInfo));
                _userAgent.Logout();
            }          
            if (Request.IsAjaxRequest())
                return Json(null, JsonRequestBehavior.AllowGet);
            return RedirectToAction<UserController>(x => x.Login("/"));
        }
        #endregion

        #region Change Password
        //Change Password Page.
        [Authorize]
        public virtual ActionResult ChangePassword() => View();

        //Change Password.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = HelperUtility.EncodeBase64(User.Identity.Name);

                //Change the Password for the user.
                model = _userAgent.ChangePassword(model);
                SetNotificationMessage(model.HasError
                    ? GetErrorNotificationMessage(model.ErrorMessage)
                    : GetSuccessNotificationMessage(model.SuccessMessage));

                if (!model.HasError)
                    return RedirectToAction<UserController>(x => x.Dashboard());
            }
            return View(model);
        }
        #endregion

        #region Forgot Password
        //Forgot Password.
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction<HomeController>(x => x.Index());
            if (Request.IsAjaxRequest())
                return PartialView("_ForgotPassword");

            return View();
        }

        //Forgot Password.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ForgotPassword(UserViewModel model)
        {
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                model = _userAgent.ForgotPassword(model);

                if (!model.HasError)
                {
                    if (Request.IsAjaxRequest())
                        return PartialView("_Login", new LoginViewModel() { SuccessMessage = model.SuccessMessage });

                    SetNotificationMessage(GenerateNotificationMessages(model.SuccessMessage, NotificationType.info));
                    return RedirectToAction<UserController>(x => x.Login(string.Empty));
                }
                else if (!Request.IsAjaxRequest())
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            if (Request.IsAjaxRequest())
                return PartialView("_ForgotPassword", model);

            return View(model);
        }
        #endregion

        #region Reset Password
        //Reset Password.
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ResetPassword(string passwordToken, string userName)
        {
            ChangePasswordViewModel resetPassword = new ChangePasswordViewModel();
            resetPassword.UserName = userName;
            resetPassword.PasswordToken = passwordToken;
            ResetPasswordStatusTypes enumStatus;

            //Set ResetPasword flag, use to hide Old Password field in View.
            resetPassword.IsResetPassword = true;

            enumStatus = _userAgent.VerifyResetPasswordLinkStatus(resetPassword);
            switch (enumStatus)
            {
                case ResetPasswordStatusTypes.Continue:
                    return View(resetPassword);
                case ResetPasswordStatusTypes.LinkExpired:
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ResetPasswordLinkExpired));
                    break;
                case ResetPasswordStatusTypes.TokenMismatch:
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ResetPasswordLinkExpired));
                    break;
                case ResetPasswordStatusTypes.NoRecord:
                default:

                    return RedirectToAction<UserController>(x => x.ForgotPassword(null));
            }
            return RedirectToAction<UserController>(x => x.ForgotPassword(null));
        }

        //Reset Password.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ResetPassword(ChangePasswordViewModel model)
        {
            model.IsResetPassword = true;
            ModelState.Remove("OldPassword");
            if (ModelState.IsValid)
            {
                //Change the User Password based on the Reset Password Email Token.
                ChangePasswordViewModel changepasswordmodel = _userAgent.ChangePassword(model);
                if (!changepasswordmodel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.ResetPasswordSuccess));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(changepasswordmodel.ErrorMessage));
            }
            return RedirectToAction<UserController>(x=>x.Login(string.Empty));
        }
        #endregion

        #region Social Login
        //
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //Get value of ApplicationSignInManager which is used for the application.
        public virtual ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
            // Request a redirect to the external login provider
            => new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "User", new
            {
                ReturnUrl = returnUrl
            }));

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public virtual ActionResult ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = AuthenticationManager.GetExternalLoginInfo();

            var loginViewModel = _userAgent.SocialLogin(loginInfo, false);

            if (!loginViewModel.HasError)
            {
                SetDataAfterLogin(loginViewModel);
                loginViewModel.IsFromSocialMedia = true;

                string Url = GetReturnUrlAfterLogin(loginViewModel, returnUrl, true);
                //Encode the query string parameter for special characters.
                if (!string.IsNullOrEmpty(Url))
                    return Redirect(Url);

                //Redirection to the Login Url.
                _authenticationHelper.RedirectFromLoginPage(model.Username, true);

                if (Request.IsAjaxRequest())
                    return Json(new { status = true, error = "" }, JsonRequestBehavior.AllowGet);
            }
            if (Request.IsAjaxRequest())
            {
                if (HelperUtility.IsNotNull(loginViewModel) && loginViewModel.IsResetPassword)
                {
                    TempData[WebStoreConstants.UserName] = model.Username;
                    TempData[WebStoreConstants.CheckoutReturnUrl] = WebStoreConstants.CheckoutReturnUrl;
                    return Json(new { status = true, error = "", isResetPassword = true }, JsonRequestBehavior.AllowGet);
                }
                else return Json(new { status = false, error = loginViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (HelperUtility.IsNotNull(loginViewModel) && loginViewModel.IsResetPassword)
                {
                    TempData[WebStoreConstants.UserName] = model.Username;
                    return RedirectToAction<UserController>(x => x.ResetWebstorePassword());
                }
                else if (loginViewModel.ErrorCode == ErrorCodes.AdminApproval)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(loginViewModel.ErrorMessage));
                    return RedirectToAction<UserController>(x => x.Login(string.Empty));
                }
                else if (loginViewModel.ErrorCode == ErrorCodes.AdminApprovalLoginFail)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(loginViewModel.ErrorMessage));
                    return RedirectToAction<UserController>(x => x.Login(string.Empty));
                }
                else if (loginViewModel.ErrorCode == ErrorCodes.AccountLocked)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(loginViewModel.ErrorMessage));
                    return RedirectToAction<UserController>(x => x.Login(string.Empty));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(loginViewModel.ErrorMessage));
            }
            return RedirectToAction<HomeController>(x => x.Index());
        }

        private void SetDataAfterLogin(LoginViewModel loginViewModel)
        {
            //Set the Authentication Cookie.           
            _authenticationHelper.SetAuthCookie(loginViewModel.Username, loginViewModel.RememberMe);

            //Remember me.
            if (loginViewModel.RememberMe)
                SaveLoginRememberMeCookie(loginViewModel.Username);

            ShoppingCartModel cart = SessionHelper.GetDataFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            TempData["CartCount"] = cart?.ShoppingCartItems?.Count;

            _cartAgent.MergeGuestUserCart();
        }

        //adding user cart details for impersonation
        private void SetDataAfterImpersonationLogin(LoginViewModel loginViewModel)
        {
            //Set the Authentication Cookie.           
            _authenticationHelper.SetAuthCookie(loginViewModel.Username, loginViewModel.RememberMe);

            //Remember me.
            if (loginViewModel.RememberMe)
                SaveLoginRememberMeCookie(loginViewModel.Username);

            ShoppingCartModel cart = SessionHelper.GetDataFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            TempData["CartCount"] = cart?.ShoppingCartItems?.Count;

            //Merge user cart detail for impersonation
            _cartAgent.MergeUserCartAfterImpersonationLogin();
        }

        //Redirect logic after the user logged in using the social media
        protected virtual string GetReturnUrlAfterLogin(LoginViewModel loginViewModel, string returnUrl, bool isFromSocialMedia = false)
        {
            loginViewModel.ReturnUrl = returnUrl;
            return _userAgent.GetReturnUrlAfterLogin(loginViewModel);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //Redirect to local.
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        //Used for social login.
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider
            {
                get; set;
            }
            public string RedirectUri
            {
                get; set;
            }
            public string UserId
            {
                get; set;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion

        #region Reset webstore Password       
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult ResetWebstorePassword()
        {
            if (HelperUtility.IsNull(TempData[WebStoreConstants.UserName]))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            ViewBag.CheckoutLogin = TempData[WebStoreConstants.CheckoutReturnUrl];
            if (Request.IsAjaxRequest())
                return PartialView("_ResetWebstorePassword", new ChangePasswordViewModel() { UserName = HelperUtility.EncodeBase64(Convert.ToString(TempData[WebStoreConstants.UserName])) });

            return View(new ChangePasswordViewModel() { UserName = HelperUtility.EncodeBase64(Convert.ToString(TempData[WebStoreConstants.UserName])) });
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult ResetWebstorePassword(ChangePasswordViewModel model, string returnUrl, bool isSinglePageCheckout = false)
        {
            if (ModelState.IsValid)
            {
                ChangePasswordViewModel resetPasswordModel = _userAgent.ChangePassword(model);
                if (!resetPasswordModel.HasError)
                {
                    LoginViewModel loginModel = new LoginViewModel() { Username = HelperUtility.DecodeBase64( model.UserName), Password = model.NewPassword, };
                    LoginViewModel accountViewModel = _userAgent.Login(loginModel);

                    if (!accountViewModel.HasError)
                    {
                        SessionHelper.SaveDataInSession<string>(WebStoreConstants.SuccessMessage, resetPasswordModel.SuccessMessage);
                        _authenticationHelper.SetAuthCookie(loginModel.Username, true);
                        SetNotificationMessage(GetSuccessNotificationMessage(resetPasswordModel.SuccessMessage));
                        if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains(WebStoreConstants.CheckoutReturnUrl))
                            return RedirectToAction<CheckoutController>(x => x.Index(isSinglePageCheckout));
                        return RedirectToAction<UserController>(x => x.Dashboard());
                    }
                    else
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(accountViewModel.ErrorMessage));
                        if (Request.IsAjaxRequest())
                            return PartialView(resetPasswordModel);
                        return View(resetPasswordModel);
                    }
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(resetPasswordModel.ErrorMessage));
                    if (Request.IsAjaxRequest())
                        return PartialView(resetPasswordModel);
                    return View(resetPasswordModel);
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }
        #endregion

        //Signup.
        [AllowAnonymous]
        [HttpGet]
        public virtual ActionResult Signup()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction<HomeController>(x => x.Index());

            return ActionView(Request.IsAjaxRequest() ? "_SinglePageRegister" : "Register", new RegisterViewModel { UserVerificationTypeCode = PortalAgent.CurrentPortal.UserVerificationTypeCode});
        }

        //Signup.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public virtual ActionResult Signup(RegisterViewModel model, string returnUrl, bool isSinglePageCheckout = false)
        {
            if (model.UserVerificationTypeCode == UserVerificationTypeEnum.EmailVerificationCode)
            {
                ModelState.Remove("Password");
                ModelState.Remove("ReTypePassword");
            }
            if (ModelState.IsValid)
            {
                model.IsWebStoreUser = true;
                model = _userAgent.SignUp(model);
                CreateAccountAddressForGuestUser(isSinglePageCheckout);
                if (!model.HasError)
                {
                    _authenticationHelper.SetAuthCookie(model.UserName, true);
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        _authenticationHelper.SetAuthCookie(model.UserName, true);

                        //Merge existing shopping cart to newly created account shopping cart while checkout.
                        _cartAgent.MergeGuestUserCart();

                        if (isSinglePageCheckout)
                            return Json(new
                            {
                                hasError = false,
                            }, JsonRequestBehavior.AllowGet);

                        return RedirectToAction<UserController>(x => x.Dashboard());
                    }
                    //Merge existing shopping cart to newly created account shopping cart after sign out.
                    _cartAgent.MergeGuestUserCart();
                    _authenticationHelper.RedirectFromLoginPage(model.UserName, true);

                    return new EmptyResult();
                }
            }

            if (isSinglePageCheckout)
            {
                return Json(new
                {
                    hasError = true,
                    message = model.ErrorMessage,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (model.ErrorCode == ErrorCodes.AdminApproval || model.ErrorCode == ErrorCodes.EmailVerification)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(model.ErrorMessage));
                    return RedirectToAction<UserController>(x => x.Login(string.Empty));
                }
                else
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return View("Register", model);
            }
        }

        //Dashboard.
        [Authorize]
        public virtual ActionResult Dashboard()
        {
            FilterCollectionDataModel fcModel = new FilterCollectionDataModel();
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            UserViewModel model = _userAgent.GetDashboard();
            fcModel.Filters = new FilterCollection();
            model.AllPendingOrdersList = _userAgent.GetAccountQuoteList(fcModel.Filters, null, null, null)?.AccountQuotes;

            model.PendingOrdersList = model.AllPendingOrdersList.Where(e => e.UserId == model.UserId).ToList();
            model.PendingOrdersCount = GetPendingQuoteHistoryCount(fcModel);
            model.PendingPaymentCount = GetPendingPaymentHistoryCount(fcModel);

            //Get Quote List
            IQuoteAgent _quoteAgent = ZnodeDependencyResolver.GetService<IQuoteAgent>();
            model.QuoteList = _quoteAgent.GetQuoteList();

            if (HelperUtility.IsNull(model))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            if (model.AccountId > 0 && HelperUtility.IsNull(model.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View("Dashboard", model);
        }

        [Authorize]
        public virtual ActionResult GetAccountMenus()
        {
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<HomeController>(x => x.Index());
            UserViewModel model = _userAgent.GetAccountMenus();
            if (HelperUtility.IsNull(model))
                return RedirectToAction<HomeController>(x => x.Index());
            SessionHelper.SaveDataInSession(WebStoreConstants.UserAccountKey, model);
            return PartialView("_AccountMenu", model);
        }

        #region B2B
        #region Account Information
        //Account Information.
        [Authorize]
        public virtual ActionResult AccountInformation()
        {
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View(_userAgent.GetAccountInformation());
        }

        //Account Information.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult AccountInformation(AccountViewModel model)
        {
            ModelState.Remove(WebStoreConstants.EmailAddress);
            if (ModelState.IsValid)
            {
                //If Account Information is updated set success message.
                if (!_userAgent.UpdateAccountInformation(model?.Address).HasError)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<UserController>(x => x.AccountInformation());
        }

        [Authorize]
        public virtual ActionResult ImportShippingLogs([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            ImportProcessLogsListViewModel importProcessLogs = _importAgent.ImportShippingLogs(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importProcessLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importProcessLogs?.ProcessLogs, WebStoreEnum.ZnodeShippingAddressImportProcessLog.ToString(), string.Empty, null, true, true, importProcessLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importProcessLogs.GridModel.TotalRecordCount = importProcessLogs.TotalResults;
            return ActionView(ImportLogsView, importProcessLogs);
        }

        [Authorize]
        public virtual ActionResult ImportShippingAddress()
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());
            else
                return View(new ImportViewModel());
        }

        [Authorize]
        [HttpPost]
        public virtual ActionResult DownloadShippingAddressTemplate()
           => View(ImportShippingAddressView, _importAgent.DownloadShippingAddressTemplate(Response));

        [Authorize]
        [HttpPost]
        public virtual ActionResult ImportShippingAddress(HttpPostedFileBase importData)
        {
            ImportViewModel importViewModel = _importAgent.ImportShippingAddress(importData);
            if (importViewModel.HasError)
                SetNotificationMessage(GetErrorNotificationMessage(importViewModel.ErrorMessage));
            else
                SetNotificationMessage(GetSuccessNotificationMessage(importViewModel.SuccessMessage));

            return RedirectToAction<UserController>(x => x.ImportShippingLogs(null));
        }

        [Authorize]
        public virtual ActionResult ImportUser()
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());
            else
                return View(new ImportViewModel());
        }

        [Authorize]
        [HttpPost]
        public virtual ActionResult DownloadUserTemplate()
           => View(ImportUserView, _importAgent.DownloadUserTemplate(Response));

        [Authorize]
        [HttpPost]
        public virtual ActionResult ImportUser(HttpPostedFileBase importData)
        {
            ImportViewModel importViewModel = _importAgent.ImportUsers(importData);
            if (importViewModel.HasError)
                SetNotificationMessage(GetErrorNotificationMessage(importViewModel.ErrorMessage));
            else
                SetNotificationMessage(GetSuccessNotificationMessage(importViewModel.SuccessMessage));

            return RedirectToAction<UserController>(x => x.ImportUserLogs(null));
        }

        //Get user import logs details.
        [Authorize]
        public virtual ActionResult ImportUserLogs([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            ImportProcessLogsListViewModel importProcessLogs = _importAgent.ImportUserLogs(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importProcessLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importProcessLogs?.ProcessLogs, WebStoreEnum.ZnodeUserImportProcessLog.ToString(), string.Empty, null, true, true, importProcessLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importProcessLogs.GridModel.TotalRecordCount = importProcessLogs.TotalResults;
            return ActionView(UserImportLogsView, importProcessLogs);
        }
        #endregion

        #region Quote History
        [Authorize]
        //Get quote history of login B2B user.
        public virtual ActionResult QuoteHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            model.Filters.Add(new FilterTuple(ZnodeConstant.IsParentPendingOrder.ToLower().ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

            //Get Account Quote list.
            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.AccountQuotes, "WebStoreCustomerQuoteHistory", string.Empty, null, true, true, null);
            quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

            //Returns the Account Quote list.
            return ActionView(quoteListViewModel);
        }

        [Authorize]
        //Get the pending orders of users for which the current user is an approver.
        public virtual ActionResult QuoteApprovalHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            //model.Filters.Add(new FilterTuple(ZnodeConstant.IsParentPendingOrder.ToLower().ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            quoteListViewModel.IsPendingApprovalHistory = true;

            quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.AccountQuotes, "WebStoreCustomerQuoteApprovalHistory", string.Empty, null, true, true, null);
            quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

            return ActionView(WebStoreConstants.QuoteHistory, quoteListViewModel);
        }

        //Update multiple quote status.
        public virtual JsonResult UpdateQuoteStatus(string quoteId, int status)
        {
            //Check for null or empty.
            if (!string.IsNullOrEmpty(quoteId) && status > 0)
            {
                bool isUpdated = _userAgent.UpdateQuoteStatus(quoteId, status);
                return Json(new { status = isUpdated, message = isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorDraftOrderedStatus }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //View Quote Details by QuoteId
        [Authorize]
        public virtual ActionResult QuoteView(int omsQuoteId, string orderStatus = null, bool IsQuoteLineItemUpdated = false)
        {
            AccountQuoteViewModel accountQuoteModel = _userAgent.GetQuoteView(omsQuoteId, IsQuoteLineItemUpdated);
            if(omsQuoteId > 0)
            {
                if(accountQuoteModel.ShoppingCart.ShoppingCartItems.Count < 1)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageForDisableProduct));
                    return RedirectToAction<UserController>(x => x.QuoteHistory(null));
                }
                else
                {
                    //return View();
                    return ActionView("QuoteView", accountQuoteModel);
                }
            }
            else
            {
                return RedirectToAction<UserController>(x => x.QuoteHistory(null));
            }
         }
        [Authorize]
        public virtual ActionResult QuoteApprovalView(int omsQuoteId, string orderStatus = null, bool IsQuoteLineItemUpdated = false)
        {
            if (omsQuoteId > 0)
            {
                return View("QuoteView", _userAgent.GetQuoteView(omsQuoteId, IsQuoteLineItemUpdated));
            }
            return RedirectToAction<UserController>(x => x.QuoteHistory(null));
        }

        //View Pending Payment Quote Details by QuoteId
        [Authorize]
        public virtual ActionResult PendingPaymentQuoteView(int omsQuoteId, string orderStatus = null, bool IsQuoteLineItemUpdated = false)
            => omsQuoteId > 0 ? View(_userAgent.GetQuoteView(omsQuoteId, IsQuoteLineItemUpdated)) : RedirectToAction<UserController>(x => x.QuoteHistory(null));

        //Convert quote to order.
        public virtual ActionResult ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel)
        {
            OrdersViewModel order = _userAgent.ConvertToOrder(accountQuoteViewModel);
            return RedirectToAction<UserController>(x => x.QuoteApprovalHistory(null));
        }

        //Convert quote to order.
        public virtual ActionResult ConvertToOrderCallbackQuoteList(AccountQuoteViewModel accountQuoteViewModel)
        {
            OrdersViewModel order = _userAgent.ConvertToOrder(accountQuoteViewModel);
            return RedirectToAction<UserController>(x => x.QuoteApprovalHistory(null));
        }

        //Update quote.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateQuote(AccountQuoteViewModel accountQuoteViewModel)
        {
            SetNotificationMessage(_userAgent.UpdateQuoteStatus(accountQuoteViewModel) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            return RedirectToAction<UserController>(x => x.QuoteApprovalHistory(null));
        }

        //Update quote line item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateQuoteLineItemQuantity(CartItemViewModel cartItemViewModel)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_userAgent.UpdateQuoteLineItemQuantity(cartItemViewModel) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
               : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            }
            return RedirectToAction<UserController>(x => x.QuoteView(cartItemViewModel.OmsQuoteId, null, true));
        }

        //Delete quote line item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId, int quoteLineItemCount = 0, string orderStatus = null, string roleName = null)
        {
            if (omsQuoteLineItemId > 0)
            {
                bool isDeleted = false;
                string message = Admin_Resources.DeleteErrorMessage;
                int quoteId = (string.Equals(orderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase) && quoteLineItemCount == 1) ? omsQuoteId : 0;

                //If trying to delete last quote line item having rejected order status,  quote line item will not get deleted.
                if ((string.Equals(roleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(roleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(orderStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)) && !string.Equals(orderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase) && quoteLineItemCount == 1)
                    message = WebStore_Resources.UnableToDeleteErrMessageForRejectedQuote;
                else
                    isDeleted = _userAgent.DeleteQuoteLineItem(omsQuoteLineItemId, quoteId);

                //If orderStatus is draft json, json response is returned.
                if (string.Equals(orderStatus, ZnodeOrderStatusEnum.DRAFT.ToString()) && quoteLineItemCount == 1)
                    return Json(new
                    {
                        status = isDeleted,
                        message = isDeleted ? WebStore_Resources.SuccessDraftDeleted : Admin_Resources.DeleteErrorMessage,
                    }, JsonRequestBehavior.AllowGet);
                else
                    SetNotificationMessage(isDeleted ? GetSuccessNotificationMessage(Admin_Resources.DeleteMessage)
                              : GetErrorNotificationMessage(message));
            }
            return RedirectToAction<UserController>(x => x.QuoteView(omsQuoteId, null, true));
        }

        //Deletes all items from shopping cart and add quote line items to shopping cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ProcessQuote(AccountQuoteViewModel accountQuoteViewModel)
        {
            //Deletes all items from shopping cart
            _cartAgent.RemoveAllCartItems();

            //Add quote line items to shopping cart.
            return _userAgent.AddQuoteToCart(accountQuoteViewModel) ? RedirectToAction<CheckoutController>(x => x.Index(true)) :
              RedirectToAction<UserController>(x => x.QuoteView(accountQuoteViewModel.OmsQuoteId, null, false));
        }

        //Create new quote
        [HttpPost]
        public virtual ActionResult CreateQuote(SubmitQuoteViewModel submitQuoteViewModel)
        {
            AccountQuoteViewModel accountQuoteViewModel = SessionHelper.GetDataFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)
                ?.ToViewModel<AccountQuoteViewModel>();

            if (accountQuoteViewModel?.OmsQuoteId > 0)
            {
                submitQuoteViewModel.AdditionalNotes = "Order is " + (submitQuoteViewModel?.OmsOrderState == "APPROVER MISSING" ? "APPROVED" : submitQuoteViewModel?.OmsOrderState) + $" by {User?.Identity?.Name}";
            }
            string message = Admin_Resources.ErrorSubmitQuote;
            int oldQuoteId = submitQuoteViewModel.QuoteId;

            //Submit quote for approval.
            bool isQuoteCreated = _userAgent.CreateQuote(submitQuoteViewModel, out message);

            if (oldQuoteId > 0 && !string.Equals(submitQuoteViewModel.OldOrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && submitQuoteViewModel.OmsOrderState == ZnodeOrderStatusEnum.DRAFT.ToString())
                message = isQuoteCreated ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;
            else if (string.Equals(submitQuoteViewModel.OldOrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && submitQuoteViewModel.OmsOrderState == ZnodeOrderStatusEnum.DRAFT.ToString() && oldQuoteId > 0)
                message = isQuoteCreated ? WebStore_Resources.DraftUpdateMessage : Admin_Resources.UpdateErrorMessage;
            else if (string.Equals(submitQuoteViewModel.OmsOrderState, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase)
               && submitQuoteViewModel.OmsOrderState == ZnodeOrderStatusEnum.DRAFT.ToString() && oldQuoteId <= 0)
                message = isQuoteCreated ? WebStore_Resources.DraftCreatedMessage : Admin_Resources.ErrorFailedToCreate;
            else
                message = isQuoteCreated ? WebStore_Resources.SuccessPlacedOrderForApproval : message;

            SetNotificationMessage(isQuoteCreated ? GetSuccessNotificationMessage(message) : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            if (accountQuoteViewModel?.OmsQuoteId > 0)
            {
                accountQuoteViewModel.OrderStatus = submitQuoteViewModel.OmsOrderState;

                SetNotificationMessage(_userAgent.UpdateQuoteStatus(accountQuoteViewModel) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                    : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            }

            return Json(new
            {
                status = isQuoteCreated,
                message = message,
                omsQuoteId = submitQuoteViewModel.QuoteId
            }, JsonRequestBehavior.AllowGet);
        }

        //Get user approver list.
        public ActionResult GetUserApproverList(int omsQuoteId)
        {
            UserApproverListViewModel listModel = _userAgent.GetUserApproverList(omsQuoteId, false);
            listModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
            return View("../User/_UserApproverList", listModel);
        }

        [Authorize]
        //Get quote history of login B2B user.
        public virtual ActionResult UserQuoteHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            //Get Account Quote list.
            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.AccountQuotes, WebStoreConstants.WebStoreB2BCustomerQuoteHistory, string.Empty, null, true, true, null);
            quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

            //Returns the Account Quote list.
            return ActionView("QuoteHistory", quoteListViewModel);
        }
        #endregion

        #region Template
        //Add the template.
        [HttpPost]
        [Authorize]
        public virtual ActionResult AddToTemplate(TemplateCartItemViewModel cartItem)
        {
            TemplateViewModel cartViewModel = _cartAgent.AddToTemplate(cartItem);

            if (cartViewModel.HasError)
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorProductAreadyExistsInTemplate));

            return View("_cartTemplateRow", cartViewModel);
        }

        //Create Template.
        [Authorize]
        public virtual ActionResult CreateTemplate()
        {
            TemplateViewModel templateViewModel = _cartAgent.GetTemplateCartModelSession();
            bool isQuickOrder = HelperUtility.IsNull(templateViewModel) ? false : templateViewModel.IsQuickOrderPad;
            if (!isQuickOrder)
            {
                _cartAgent.SetTemplateCartModelSessionToNull();
                //save the value of AutoIndex in session in order to get the correct index value while adding new template rows.
                SessionHelper.SaveDataInSession<int>("AutoIndex", Convert.ToInt32(WebStoreConstants.DefaultQuickOrderPadRows));
            }
            SetUpdatedTemplateName(templateViewModel);
            return View(createEditTemplateView, HelperUtility.IsNull(templateViewModel) ? new TemplateViewModel() : templateViewModel);
        }

        //Create template.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateTemplate(TemplateViewModel cartItem)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_cartAgent.CreateTemplate(cartItem) ? GetSuccessNotificationMessage(WebStore_Resources.SuccessTemplateSaved)
                : GetErrorNotificationMessage(WebStore_Resources.ErrorTemplateSaved));

                return RedirectToAction<UserController>(x => x.TemplateList(null));
            }
            return View(createEditTemplateView, HelperUtility.IsNull(cartItem) ? new TemplateViewModel() : cartItem);
        }

        //Get template list.
        [Authorize]
        public virtual ActionResult TemplateList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            TemplateListViewModel templateList = _cartAgent.GetTemplateList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            _cartAgent.SetTemplateCartModelSessionToNull();
            templateList.GridModel = FilterHelpers.GetDynamicGridModel(model, templateList?.List, WebStoreConstants.ZnodeOmsTemplate.ToString(), string.Empty, null, true, true, templateList?.GridModel?.FilterColumn?.ToolMenuList);
            templateList.GridModel.TotalRecordCount = templateList.TotalResults;
            return ActionView("TemplateList", templateList);
        }

        //Delete template on the basis of omsTemplateId.
        [Authorize]
        public virtual ActionResult DeleteTemplate(string omsTemplateId)
        {
            if (!string.IsNullOrEmpty(omsTemplateId))
            {
                bool isDeleted = _cartAgent.DeleteTemplate(omsTemplateId);

                return Json(new { status = isDeleted, message = isDeleted ? WebStore_Resources.DeleteMessage : WebStore_Resources.DeleteFailMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = WebStore_Resources.DeleteFailMessage }, JsonRequestBehavior.AllowGet);
        }

        //Edit Template.
        [Authorize]
        public virtual ActionResult EditTemplate(int omsTemplateId , bool isClearAll = true,string productId ="", string errorMessageInTemplateToCart = "" )
        {
         TemplateViewModel templateViewModel = _cartAgent.GetTemplate(omsTemplateId, isClearAll);

            SetUpdatedTemplateName(templateViewModel);

            if (HelperUtility.IsNull(templateViewModel))
                return Redirect("/404");

            if (templateViewModel.HasError && templateViewModel.ErrorMessage == WebStore_Resources.HttpCode_401_AccessDeniedMsg)
                return Redirect("/404");
            if (HelperUtility.IsNotNull(templateViewModel?.TemplateCartItems))
            {
                foreach (var item in templateViewModel.TemplateCartItems)
                {
                    if (item.GroupProducts?.Count > 0)
                    {
                        foreach (var groupProduct in item.GroupProducts)
                        {
                            templateViewModel.IsAddToCartButtonDisable = templateViewModel.IsAddToCartButtonDisable ? templateViewModel.IsAddToCartButtonDisable : (bool)GetCartItems(groupProduct.ProductId, Convert.ToInt32(groupProduct.Quantity)).GetDynamicProperty("Data").GetProperty("status");

                            if (groupProduct.ProductId.ToString() == productId)
                                item.ErrorMessage = errorMessageInTemplateToCart;
                        }
                    }
                    else
                    {
                        templateViewModel.IsAddToCartButtonDisable = templateViewModel.IsAddToCartButtonDisable? templateViewModel.IsAddToCartButtonDisable : (bool)GetCartItems(Convert.ToInt32(item.ProductId), Convert.ToInt32(item.Quantity)).GetDynamicProperty("Data").GetProperty("status");
                    }

                    if (item.ProductId == productId)
                        item.ErrorMessage = errorMessageInTemplateToCart;
                }
            }
            return View(createEditTemplateView, HelperUtility.IsNull(templateViewModel) ? new TemplateViewModel() : templateViewModel);
        }

        //Edit Template.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditTemplate(TemplateViewModel cartItem)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_cartAgent.CreateTemplate(cartItem) ? GetSuccessNotificationMessage(WebStore_Resources.SuccessTemplateSaved)
                : GetErrorNotificationMessage(WebStore_Resources.ErrorTemplateSaved));

                if (cartItem.HasError && cartItem.ErrorMessage == WebStore_Resources.HttpCode_401_AccessDeniedMsg)
                    return Redirect("/404");

                return RedirectToAction<UserController>(x => x.TemplateList(null));
            }
            return View(createEditTemplateView, HelperUtility.IsNull(cartItem) ? new TemplateViewModel() : cartItem);
        }

        //Edit Template.
        [Authorize]
        public virtual ActionResult ViewTemplate(int omsTemplateId, bool isClearAll)
        {
            TemplateViewModel templateViewModel = _cartAgent.GetTemplateCartModelSession();

            if (HelperUtility.IsNull(templateViewModel) ? false : templateViewModel.IsQuickOrderPad)
                _cartAgent.SetTemplateCartModelSessionToNull();

            templateViewModel = _cartAgent.GetTemplate(omsTemplateId, isClearAll);

            if (HelperUtility.IsNull(templateViewModel))
                return Redirect("/404");

            return View(HelperUtility.IsNull(templateViewModel) ? new TemplateViewModel() : templateViewModel);
        }

        public virtual ActionResult IsTemplateItemsModified(int omsTemplateId) => Json(new
        {
            status = _cartAgent.IsTemplateItemsModified(omsTemplateId)
        }, JsonRequestBehavior.AllowGet);

        //Remove single cart item from template.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveTemplateCartItem(string guid, int omsTemplateId)
        {
            SetNotificationMessage(_cartAgent.RemoveTemplateCartItem(guid) ? GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage)
            : GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));

           return omsTemplateId > 0 ? RedirectToAction<UserController>(x => x.EditTemplate(omsTemplateId,false,"","")) : RedirectToAction<UserController>(x => x.CreateTemplate());
        }

        //Remove all cart item from template.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveAllTemplateCartItem(int omsTemplateId, bool isClearAll = true)
        {
            SetNotificationMessage(_saveForLater.RemoveAllTemplateCartItems(omsTemplateId, Convert.ToBoolean(ZnodeConstant.True)) ? GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage)
              : GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));
                 SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage));

            return omsTemplateId >= 0 ? RedirectToAction<UserController>(x => x.EditTemplate(omsTemplateId,true,"","")) : RedirectToAction<UserController>(x => x.CreateTemplate());
        }

        //Get the view of Quick Order pad template.
        public virtual ActionResult QuickOrderPadTemplate(string templateName = "")
        {
            TempData.Add(_UpdatedTemplateName, templateName);
            SessionHelper.SaveDataInSession<int>("AutoIndex", Convert.ToInt32(WebStoreConstants.DefaultQuickOrderPadRows));
            return View(_QuickOrderPadView);
        }

        //Generate new row for quick order pad.
        public virtual ActionResult QuickOrder()
        {
            int index = Convert.ToInt32(SessionHelper.GetDataFromSession<int>("AutoIndex")) + 1;
            SessionHelper.SaveDataInSession<int>("AutoIndex", index);
            return PartialView(_MultipleQuickOrdersView);
        }

        //Add multiple products to cart template.
        [HttpPost]
        public virtual ActionResult AddMultipleProductsToCartTemplate(List<TemplateCartItemViewModel> cartItems)
        {
            string errorMessage = _cartAgent.AddMultipleProductsToCartTemplate(cartItems);
            return Json(new
            {
                isSuccess = string.IsNullOrEmpty(errorMessage),
                message = string.IsNullOrEmpty(errorMessage) ? WebStore_Resources.SuccessTemplateSaved : errorMessage,
                cartCount = _cartAgent.GetCartCount(),
                omsTemplateId = _cartAgent.GetTemplateCartModelSession().OmsTemplateId,
            }, JsonRequestBehavior.AllowGet);
        }

        //Update quantity of cart item.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateTemplateCartQuantity(string guid, decimal quantity, int productId = 0, int omsTemplateId = 0)
        {
            _cartAgent.UpdateTemplateItemQuantity(guid, quantity, productId);
            return (omsTemplateId > 0) ? RedirectToAction<UserController>(x => x.EditTemplate(omsTemplateId,false,"",""))
                : RedirectToAction<UserController>(x => x.CreateTemplate());
        }

        //Add template to cart.
        [Authorize]
        public virtual ActionResult AddTemplateToCart(int omsTemplateId)
        {
         string errorMessage = _cartAgent.AddTemplateToCart(omsTemplateId);
            if (!string.IsNullOrEmpty(errorMessage)){
                string[] productId = errorMessage.Split('/');
                errorMessage = productId[0];
                return RedirectToAction<UserController>(x => x.EditTemplate(omsTemplateId, false,productId[1], errorMessage));
            }
            else
                return RedirectToAction<CartController>(x => x.Index());
        }

        public virtual ActionResult GetCartItems(int productId, int selectedQty)
        {
            ShoppingCartModel cart = _cartAgent.GetCartItems();

            foreach (ShoppingCartItemModel item in cart.ShoppingCartItems)
            {
                if (item.Product.PublishProductId == productId)
                {
                    if (item.Quantity + selectedQty > item.MaxQuantity)
                    {
                        return Json(new
                        {
                            status = true
                        }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            return Json(new
            {
                status = false
            }, JsonRequestBehavior.AllowGet);
        }

        //Check whether template name exists.
        [HttpPost]
        public virtual JsonResult IsTemplateNameExist(string templateName, int omsTemplateId = 0)
            => Json(!_userAgent.IsTemplateNameExist(templateName, omsTemplateId), JsonRequestBehavior.AllowGet);
        #endregion
        #endregion

        //Wishlist.
        [Authorize]
        public virtual ActionResult Wishlist(int? wishid)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            if (wishid.HasValue)
            {
                bool deleteWishList = _userAgent.DeleteWishList(wishid.GetValueOrDefault(0));

                return Json(new
                {
                    success = deleteWishList,
                    message = deleteWishList ? WebStore_Resources.SuccessProductDeleteWishlist : WebStore_Resources.ErrorProductDeleteWishlist,
                    data = new
                    {
                        style = deleteWishList ? "success" : "error",
                        total = _userAgent.GetUserViewModelFromSession().WishList.Count
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            WishListListViewModel wishListListViewModel = _userAgent.GetWishLists();
            return View("Wishlist", wishListListViewModel);
        }

        //ToDo
        [Authorize]
        public virtual ActionResult History([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            OrdersListViewModel list = _userAgent.GetOrderList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //get distinct order list
            List<OrdersViewModel> historyList = list.List.GroupBy(x => x.OmsOrderId).Select(y => y.FirstOrDefault()).ToList();

            list.GridModel = FilterHelpers.GetDynamicGridModel(model, historyList, WebStoreConstants.ZnodeWebStoreOrder, string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("OrderList", list);
        }

        //Pay Invoice
        [HttpPost]
        public virtual ActionResult PayInvoice(PayInvoiceViewModel payInvoiceViewModel)
        {
            if (HelperUtility.IsNotNull(payInvoiceViewModel))
            {
                OrdersViewModel order = _userAgent.PayInvoice(payInvoiceViewModel);                              
           
                if (order.OmsOrderId > 0)
                {
                    // Below code is used, for after successfully payment from "Credit Card" return receipt.
                    if (IsCreditCardPayment(payInvoiceViewModel?.PaymentDetails.PaymentType) || IsACHPayment(payInvoiceViewModel?.PaymentDetails.PaymentType))
                    {
                        return Json(new { receiptHTML = RenderRazorViewToString(CheckoutReceipt, order), omsOrderId = order.OmsOrderId });
                    }
                    
                    return RedirectToAction<CheckoutController>(x => x.OrderCheckoutReceipt());
                }

                // Return error message, if payment through "Credit Card" raises any error.
                if (IsCreditCardPayment(payInvoiceViewModel?.PaymentDetails.PaymentType) || IsACHPayment(payInvoiceViewModel?.PaymentDetails.PaymentType))
                {
                    return Json(new { error = order.ErrorMessage });
                }


                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ConvertQuoteToOrderErrorMessage));
            }
            //hardcoded
            return null;

        }

        //Check payment type is Credit Card payment method
        public virtual bool IsCreditCardPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType, ZnodeConstant.CreditCard, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
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

        //Order Receipt.
        [Authorize]
        public virtual ActionResult OrderReceipt(int omsOrderId, int portalId = 0)
        {
            OrdersViewModel ordersViewModel = _userAgent.GetOrderDetails(omsOrderId, portalId);
            //return value of isEnableCreateReturn true if session userid and ordersViewModel userid is equal else return false
            ViewBag.isEnableCreateReturn = HelperUtility.Equals(_userAgent.GetUserViewModelFromSession()?.UserId, ordersViewModel.UserId) ? true : false;
            ordersViewModel.IsOrderEligibleForReturnReceipt = ordersViewModel?.PaymentHistoryList?.PaymentHistoryList?.Count > 0 ? false : true;
            return View("OrderReceipt", ordersViewModel);
        }
        //Order Receipt.
        [Authorize]
        public virtual ActionResult OrderReceiptForOfflinePayment(int omsOrderId, int portalId = 0)
        {
            SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.OrderReceiptThankYouText));

            OrdersViewModel ordersViewModel =_userAgent.GetOrderDetails(omsOrderId, portalId);
            return View("OrderReceipt", ordersViewModel);
        }

        [HttpGet]
        public virtual ActionResult GetOrderDetails() => ActionView("_UserOrderInformation", new UserOrderViewModel());

        //Get Order details on the basis of User's Order Number, Firstname and Lastname.
        [HttpPost]
        public virtual ActionResult GetOrderDetails(string orderNumber, string firstName, string lastName, string emailAddress)
            => ActionView("OrderDetails", _userAgent.GetOrderDetails(orderNumber, firstName, lastName, emailAddress));

        #region Reorder

        // Reorder complete order.
        [Authorize]
        public virtual ActionResult ReorderProducts(int omsOrderId)
        {
            List<CartItemViewModel> cartItemList = new List<CartItemViewModel>();
            _userAgent.ReorderCompleteOrder(omsOrderId);
            return RedirectToAction<CartController>(x => x.Index());
        }

        // Reorder single item of order.
        [Authorize]
        public virtual ActionResult ReorderOrderLineItem(int id)
        {
            _userAgent.ReordersingleLineOrderItem(id);
            return RedirectToAction<CartController>(x => x.Index());
        }

        //Re order Products List.
        //TODO        
        [Authorize]
        public virtual ActionResult ReorderProductsList(int id, int orderLineItemId, bool isOrder)
        => RedirectToAction("Index", "Cart");

        #endregion

        //Address Book.
        [Authorize]
        [HttpGet]
        public virtual ActionResult AddressBook()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View("AddressBook", _userAgent.GetAddressList(true));
        }

        //Address Book.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult AddressBook(int id, bool isDefaultBillingAddress)
        {
            AddressViewModel model = _userAgent.UpdateAddress(id, isDefaultBillingAddress);
            if (!model.HasError && model.IsDefaultBilling)
                SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.ChangedPrimaryBilling));
            else if (!model.HasError && model.IsDefaultShipping)
                SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.ChangedPrimaryShipping));
            else
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorChangedAddress));
            return RedirectToAction<UserController>(x => x.AddressBook());
        }

        [HttpPost]
        public virtual ActionResult GetRecommendedAddress(AddressViewModel addressViewModel)
        {
            AddressListViewModel listViewModel = _userAgent.GetRecommendedAddress(addressViewModel);
            string htmlContent = string.Empty;
            if (listViewModel?.AddressList?.Count > 0)
            {
                listViewModel.AddressList.ForEach(x => x.AddressType = addressViewModel.AddressType);
                htmlContent = RenderRazorViewToString("../User/_RecommendedAddress", listViewModel);
            }

            return Json(new
            {
                html = htmlContent,
            }, JsonRequestBehavior.AllowGet);
        }

        //Review history.
        [Authorize]
        [HttpGet]
        public virtual ActionResult Reviews()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View("ReviewHistory", _userAgent.GetProductReviewList());
        }

        //Edit Profile.
        [Authorize]
        [HttpGet]
        public virtual ActionResult EditProfile()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));
            UserViewModel userViewModel =  _userAgent.GetUserViewModelFromSession();
            userViewModel.IsSmsProviderEnabled = PortalAgent.CurrentPortal.IsSmsProviderEnabled;
            return View("EditProfile", userViewModel);
        }

        //Edit Profile.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditProfile(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = _userAgent.UpdateProfile(model, true);

                if (!model.HasError)
                    SetNotificationMessage(GetSuccessNotificationMessage(model.SuccessMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            if (model.UserName == HttpContext.User.Identity.Name)
                return RedirectToAction("EditProfile");
            else
            {
                ExternalLoginInfo loginInfo = SessionHelper.GetDataFromSession<ExternalLoginInfo>(WebStoreConstants.SocialLoginDetails);

                if (HelperUtility.IsNotNull(loginInfo))
                    return Redirect(_userAgent.Logout(loginInfo));
                _userAgent.Logout();

                return RedirectToAction<UserController>(x => x.Login("/"));
            }
        }


        //Get Saved Credit Card Details.
        [Authorize]
        [HttpGet]
        public virtual ActionResult GetSavedCreditCards()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            return View(SavedCreditCards);
        }

        //Delete saved card details
        public virtual ActionResult DeleteCardDetails(string paymentGUID)
        {
            if (_paymentAgent.DeleteSavedCreditCardDetail(paymentGUID))
                SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));
            return RedirectToAction<UserController>(x => x.GetSavedCreditCards());
        }

        //Fetch saved credit card 
        public virtual ActionResult CreditCardDetails()
       => View(CreditCardDetail, _paymentAgent.GetPaymentCreditCardDetails(_userAgent.GetUserViewModelFromSession()?.CustomerPaymentGUID));

        [Authorize]
        [HttpGet]
        public virtual ActionResult Address(int? id)
        => View("EditAddress", _userAgent.GetAddress(id));

        //Create and update address.
        [HttpPost]
        public virtual ActionResult Address(AddressViewModel addressViewModel)
        {
            ModelState.Remove("EmailAddress");
            if (ModelState.IsValid)
            {
                addressViewModel = _userAgent.CreateUpdateAddress(addressViewModel);
                if (!addressViewModel.HasError)
                    SetNotificationMessage(GetSuccessNotificationMessage(addressViewModel.SuccessMessage));
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(addressViewModel.ErrorMessage));
                }
            }
            else
                return View("EditAddress", _userAgent.GetAddressDetail(addressViewModel));

            return RedirectToAction<UserController>(x => x.AddressBook());
        }

        //Delete address on the basis of addressId.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteAddress(int? addressId)
        {
            string message = string.Empty;
            AddressViewModel addressViewModel = _userAgent.GetAddress(addressId);
            if (addressViewModel.IsDefaultShipping && addressViewModel.IsDefaultBilling)
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorDeleteShippingAddressAndDeleteBillingAddress));
                return RedirectToAction<UserController>(x => x.AddressBook());
            }
            if (addressViewModel.IsDefaultShipping)
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorDeleteShippingAddress));
                return RedirectToAction<UserController>(x => x.AddressBook());
            }
            if (addressViewModel.IsDefaultBilling)
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorDeleteBillingAddress));
                return RedirectToAction<UserController>(x => x.AddressBook());
            }
            if (_userAgent.DeleteAddress(addressId, addressViewModel, out message))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(message));
                return RedirectToAction<UserController>(x => x.AddressBook());
            }
            SetNotificationMessage(GetErrorNotificationMessage(message));
            return RedirectToAction<UserController>(x => x.Address(addressId));
        }

        //Delete current address.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteCurrentAddress(AddressViewModel addressViewModel)
        {
            string message = string.Empty;
            ModelState.Remove("EmailAddress");
            if (ModelState.IsValid)
            {
                if (addressViewModel.IsDefaultShipping)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorDeleteShippingAddress));
                    return RedirectToAction<UserController>(x => x.Address(addressViewModel.AddressId));
                }
                if (addressViewModel.IsDefaultBilling)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorDeleteBillingAddress));
                    return RedirectToAction<UserController>(x => x.Address(addressViewModel.AddressId));
                }
                if (!addressViewModel.HasError)
                {
                    if (_userAgent.DeleteAddress(addressViewModel.AddressId, addressViewModel, out message))
                    {
                        SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.SuccessDeleteAddress));
                        return RedirectToAction<UserController>(x => x.AddressBook());
                    }
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(message));
            return RedirectToAction<UserController>(x => x.Address(addressViewModel.AddressId));
        }

        //Affiliate Information.
        [Authorize]
        public virtual ActionResult AffiliateInformation()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));
            return View(_userAgent.GetUserViewModelFromSession());
        }

        #region Download Invoice
        //Get order invoice detail in PDF format.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DownloadPDF(string orderIds)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(orderIds.Trim()))
            {
                OrdersListViewModel orderInvoiceModel = _userAgent.GetOrderInvoiceDetails(orderIds);

                if (orderInvoiceModel?.List?.Count <= 0)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.ErrorGenerateInvoice));
                    return RedirectToAction<UserController>(x => x.History(null));
                }

                //Generate order invoice html
                var htmlContent = RenderRazorViewToString("_OrderInvoice", orderInvoiceModel);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                try
                {
                    var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                    using (MemoryStream Stream = new MemoryStream(pdfBytes))
                    {
                        // set HTTP response headers
                        HttpContext.Response.Clear();
                        HttpContext.Response.AddHeader("Content-Type", "application/pdf");
                        HttpContext.Response.AddHeader("Cache-Control", "max-age=0");
                        HttpContext.Response.AddHeader("Accept-Ranges", "none");

                        HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=Invoice_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf");

                        // send the generated PDF
                        Stream.WriteTo(Response.OutputStream);
                        Stream.Close();
                        HttpContext.Response.Flush();
                        HttpContext.Response.End();
                        status = true;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = ex.Message;
                    throw;
                }
            }
            return Json(new { Success = status });
        }
        #endregion

        #region Customers

        // This method will fetch the list of all the customer account details.
        [Authorize]
        public virtual ActionResult CustomersList(int accountId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string currentUserName = HttpContext.User.Identity.Name;

            //Set filters for Account id and IsAccountCustomer
            HelperMethods.SetAccountIdFilters(model.Filters, accountId);
            HelperMethods.SetIsAccountCustomerFilter(model.Filters, 1);

            //Get the list of customers            
            CustomerListViewModel customerViewModel = _userAgent.GetCustomerAccountList(currentUserName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model      
            customerViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, WebStoreConstants.ZnodeAccountUser, string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            customerViewModel.GridModel.TotalRecordCount = customerViewModel.TotalResults;
            customerViewModel.AccountId = accountId;
            //Returns the customer list view
            return ActionView("_CustomerList", customerViewModel);
        }


        // Edit Customer User.
        [HttpGet]
        public virtual ActionResult CustomerEdit(int userId, int accountId)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            if (userId > 0)
            {
                CustomerAccountViewModel customerAccountDetails = _userAgent.GetAccountsCustomer(userId);
                _userAgent.SetCustomerAccountViewModel(customerAccountDetails, accountId);
                return ActionView("_AddCustomerAsidePanel", customerAccountDetails);
            }
            return RedirectToAction<UserController>(x => x.CustomersList(accountId, null));
        }

        // This method update the users account details.
        [HttpPost]
        public virtual ActionResult CustomerEdit(CustomerAccountViewModel model)
        {
            ModelState.Remove(ModelStatePortalIds);
            if (ModelState.IsValid)
            {
                string errorMessage = string.Empty;
                SetNotificationMessage(_userAgent.UpdateCustomerAccount(model, out errorMessage) ? GetSuccessNotificationMessage(WebStore_Resources.UpdateMessage)
                    : GetErrorNotificationMessage(errorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.UpdateErrorMessage));
            return RedirectToAction<UserController>(x => x.CustomerEdit(Convert.ToInt32(model.UserId), Convert.ToInt32(model.AccountId)));
        }

        // Delete customer account.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult CustomerDelete(string userId)
        {
            string message = WebStore_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(userId))
            {
                string currentUserName = HttpContext.User.Identity.Name;
                bool status = _userAgent.DeleteCustomer(userId, currentUserName, out message);
                return Json(new { status = status, message = status ? WebStore_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = WebStore_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        // This method will enable or disable the customer account.
        public virtual ActionResult CustomerEnableDisableAccount(int accountId, string userId, bool isLock, bool isRedirect = true)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string message = WebStore_Resources.DisableMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(userId))
                status = _userAgent.EnableDisableUser(userId, !isLock, out message);

            if (!isRedirect)
                return Json(new { status = status, message = (status && isLock) ? WebStore_Resources.EnableMessage : (status && !isLock) ? WebStore_Resources.DisableMessage : message }, JsonRequestBehavior.AllowGet);
            else
            {
                if (status && isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.EnableMessage));
                else if (status && !isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.DisableMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
                return RedirectToAction<UserController>(x => x.CustomersList(accountId, null));
            }
        }

        //Get Entity Attribute Details based on EntityId & Entity Type. 
        [HttpGet]
        public ActionResult GetEntityAttributeDetails(int entityId, string entityType)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            return ActionView(WebStoreConstants.GlobalAttributeEntityView, _formBuilderAgent.GetEntityAttributeDetails(entityId, entityType));
        }

        //Save Entity Attribute Details based on EntityId & Entity Type. 
        [HttpPost]
        public virtual ActionResult SaveEntityDetails([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string errorMessage = string.Empty;

            EntityAttributeViewModel entityAttributeViewModel = _formBuilderAgent.SaveEntityAttributeDetails(model, out errorMessage);
            SetNotificationMessage(!entityAttributeViewModel.HasError ? GetSuccessNotificationMessage(WebStore_Resources.UpdateMessage) : GetErrorNotificationMessage(errorMessage));

            return RedirectToAction<UserController>(x => x.GetEntityAttributeDetails(entityAttributeViewModel.EntityValueId, entityAttributeViewModel.EntityType));
        }


        /// This method will reset the password for the single user admin.
        [HttpGet]
        public virtual ActionResult SingleResetPassword(int userId)
        {
            if (userId > 0)
            {
                string errorMessage = string.Empty;
                bool status = _userAgent.ResetPassword(userId, out errorMessage);
                return Json(new { status = status, message = status ? WebStore_Resources.SuccessResetPassword : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = WebStore_Resources.ErrorAccessDenied }, JsonRequestBehavior.AllowGet);
        }

        // Reset the password for the user in bulk.       
        public virtual JsonResult BulkResetPassword(int accountId, string userId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(userId))
                if (!_userAgent.BulkResetPassword(userId, out message))
                    return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);

            return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        // Gets b2b permission list.
        [HttpGet]
        public virtual JsonResult GetPermissionList(int accountId, int accountPermissionId = 0)
            => Json(_userAgent.GetPermissionList(accountId, accountPermissionId), JsonRequestBehavior.AllowGet);


        // Gets user approver list.
        [HttpGet]
        public virtual JsonResult GetApproverList(int accountId, int? userId)
            => Json(_userAgent.GetApproverList(accountId, userId, HttpContext.User.Identity.Name), JsonRequestBehavior.AllowGet);

        // Gets b2b account role list.
        [HttpGet]
        public virtual JsonResult GetAccountDepartments(int accountId)
            => Json(_userAgent.GetAccountDepartments(accountId), JsonRequestBehavior.AllowGet);

        //ToDo
        [Authorize]
        public virtual ActionResult UserApprovalList()
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            UserApproverListViewModel userapprovalViewModel = _userAgent.GetUserApproverList(0, true);
            userapprovalViewModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
            return ActionView("ApprovalUserList", userapprovalViewModel);
        }

        // Get Payment Quote History
        public virtual ActionResult PaymentQuoteHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            model.Filters.Add(new FilterTuple("IsParentPendingOrder", FilterOperators.Equals, ZnodeConstant.TrueValue));
            //Get Account Quote list.
            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);

            //Get the grid model.
            quoteListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, quoteListViewModel?.AccountQuotes, "WebStoreCustomerPendingPaymentQuoteHistory", string.Empty, null, true, true, null);
            quoteListViewModel.GridModel.TotalRecordCount = quoteListViewModel.TotalResults;

            //Returns the Account Quote list.
            return ActionView(quoteListViewModel);
        }
        #endregion

        //ECert
        [Authorize]
        public virtual ActionResult eCertificateBalance() => View("eCertificateBalance");

        public virtual ActionResult ValidateUserBudget()
        {
            string message = string.Empty;
            bool status = _userAgent.ValidateUserBudget(out message);
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        // Get states by country code.
        public virtual ActionResult GetStates(string countryCode) => Json(new
        {
            states = _userAgent.GetStates(countryCode)
        }, JsonRequestBehavior.AllowGet);

        //Update multiple quote status.
        public virtual JsonResult ChangeUserProfile(int profileId)
        {
            if (profileId > 0)
            {
                bool isUpdated = _userAgent.ChangeUserProfile(profileId);
                return Json(new { status = isUpdated, message = isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Sets the primary billing or shipping address for the user on drop-down change from AccountDashboard page.
        public virtual ActionResult SetPrimaryBillingShippingAddress(int addressId, string addressType)
        {
            AddressViewModel model = _userAgent.UpdateAddress(addressId, addressType == WebStoreConstants.BillingAddressType);
            string htmlContent = RenderRazorViewToString("../User/_DisplayAddress", model);

            return Json(new
            {
                html = htmlContent,
                status = !model.HasError,
                message = !model.HasError ? (model.IsDefaultBilling ? WebStore_Resources.ChangedPrimaryBilling : WebStore_Resources.ChangedPrimaryShipping) : WebStore_Resources.ErrorChangedAddress
            }, JsonRequestBehavior.AllowGet);
        }

        #region Power BI

        //Method to get PowerBI reports
        [Authorize]
        [HttpGet]
        public virtual async Task<ActionResult> PowerBIReport()
        {
            return View("PowerBIReport", await _powerBIAgent.GetPowerBIReportsData(_powerBIAgent.GetPowerBISettings()));
        }

        #endregion Power BI

        #region Voucher
        //Get voucher history.
        [Authorize]
        public virtual ActionResult VoucherHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int voucherId)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            VoucherHistoryListViewModel list = _userAgent.GetVoucherHistoryList(voucherId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list?.List, WebStoreConstants.ZnodeWebStoreVoucherHistory, string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("GiftCardHistory", list);
        }

        //Get voucher list.
        [Authorize]
        public virtual ActionResult Vouchers([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            VoucherListViewModel list = _userAgent.GetVouchers(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list?.List, WebStoreConstants.ZnodeWebStoreVoucher, string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            list.GridModel.TotalRecordCount = list.TotalResults;
            return ActionView("VoucherList", list);

        }
        #endregion Voucher



        #region Guest Return

        [HttpGet]
        public virtual ActionResult GetOrderDetailsForReturn(string orderNumber)
        {
            RMAReturnOrderDetailViewModel rmaReturnOrderDetailViewModel = _rmaReturnAgent.GetOrderDetailsForReturn(orderNumber);
            return ActionView("SelectReturnItems", rmaReturnOrderDetailViewModel);
        }

        //Perform calculations for an order return line item.
        [HttpPost]
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

        // Validate guest user return.
        [HttpGet]
        public virtual ActionResult ValidateGuestUserReturn(string orderNumber)
        {
            UserViewModel userViewModel = _rmaReturnAgent.ValidateGuestUserReturn(orderNumber);

            return Json(new
            {
                hasError = userViewModel.HasError,
                errorMessage = userViewModel.ErrorMessage,
                isGuestUser = userViewModel.IsGuestUser,
            }, JsonRequestBehavior.AllowGet);
        }

        //Submit order return
        [HttpPost]
        public virtual ActionResult SubmitOrderReturn(RMAReturnViewModel returnViewModel)
        {
            RMAReturnViewModel rmaReturnViewModel = _rmaReturnAgent.SubmitOrderReturn(returnViewModel);

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

        //Check if the order is eligible for return
        [HttpGet]
        public virtual ActionResult CheckOrderEligibilityForReturn(string orderNumber)
        {
            bool isOrderEligibleForReturn = _rmaReturnAgent.IsOrderEligibleForReturn(orderNumber);
            return Json(new
            {
                isEligible = isOrderEligibleForReturn,
            }, JsonRequestBehavior.AllowGet);
        }

        //Print Return Receipt
        [HttpGet]
        public virtual ActionResult PrintReturnReceipt(string returnNumber, bool isReturnDetailsReceipt = true)
        {
            return ActionView("../RMAReturn/PrintReturnReceipt", _rmaReturnAgent.GetReturnDetails(returnNumber, isReturnDetailsReceipt));
        }
        #endregion

        //Account Users.
        [Authorize]
        public virtual ActionResult AccountUsers(int accountId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string currentUserName = HttpContext.User.Identity.Name;

            //Set filters for Account id and IsAccountCustomer
            HelperMethods.SetAccountIdFilters(model.Filters, accountId);
            HelperMethods.SetIsAccountCustomerFilter(model.Filters, 1);

            //Get the list of customers            
            CustomerListViewModel customerViewModel = _userAgent.GetCustomerAccountList(currentUserName, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model      
            customerViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, customerViewModel.List, WebStoreConstants.ZnodeAccountUserForWebstore, string.Empty, null, true, true, customerViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            customerViewModel.GridModel.TotalRecordCount = customerViewModel.TotalResults;
            customerViewModel.AccountId = accountId;
            //Returns the customer list view
            return ActionView("AccountUsers", customerViewModel);
        }


        // This method will enable or disable the customer account.
        public virtual ActionResult EnableDisableAccountUsers(int accountId, string userId, bool isLock, bool isRedirect = true)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string message = WebStore_Resources.DisableMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(userId))
                status = _userAgent.EnableDisableUser(userId, !isLock, out message);

            if (!isRedirect)
                return Json(new { status = status, message = (status && isLock) ? WebStore_Resources.EnableMessage : (status && !isLock) ? WebStore_Resources.DisableMessage : message }, JsonRequestBehavior.AllowGet);
            else
            {
                if (status && isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.EnableMessage));
                else if (status && !isLock)
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.DisableMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
                return RedirectToAction<UserController>(x => x.AccountUsers(accountId, null));
            }
        }

        /// This method will reset the password for the single user admin.
        public virtual ActionResult AccountResetPassword(int userId)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            string message = string.Empty;
            _userAgent.ResetPassword(userId, out message);

            SetNotificationMessage(GetErrorNotificationMessage(message));
            return RedirectToAction<UserController>(x => x.AccountUsers(userId, null));
        }

        //Account Orders.
        [Authorize]
        public virtual ActionResult AccountOrders([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId = 0, int accountId = 0)
        {
            if (!_importAgent.IsAdminUser())
                return RedirectToAction<HomeController>(c => c.Index());

            //Get the list of customers            
            OrdersListViewModel orders = _userAgent.GetAccountUserOrderList(accountId, userId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model      
            orders.GridModel = FilterHelpers.GetDynamicGridModel(model, orders.List, WebStoreConstants.ZnodeAccountOrderForWebstore, string.Empty, null, true, true, orders?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            orders.GridModel.TotalRecordCount = orders.TotalResults;
            orders.AccountId = accountId;
            orders.UserId = userId;
            //Returns the customer list view
            return ActionView("AccountOrders", orders);

        }
        #endregion
        #region Private Methods
        //Remember me function for logged in user on remember me selection.
        // This function is used to save user name in cookies.
        protected void SaveLoginRememberMeCookie(string userId)
        {
            //Check if the browser support cookies 
            if ((HttpContext.Request.Browser.Cookies))
            {
                CookieHelper.SetCookie("loginCookie", userId, (Convert.ToDouble(ConfigurationManager.AppSettings["CookieExpiresValue"]) * ZnodeConstant.MinutesInADay), true);
            }
        }

        //Get the User Details from the Login Cookie.
        public void GetLoginRememberMeCookie()
        {
            if ((HttpContext.Request.Browser.Cookies))
            {
                if (HelperUtility.IsNotNull(HttpContext.Request.Cookies[WebStoreConstants.LoginCookieNameValue]))
                {
                    if (CookieHelper.IsCookieExists(WebStoreConstants.LoginCookieNameValue))
                    {
                        string loginName = HttpUtility.HtmlEncode(CookieHelper.GetCookieValue<string>(WebStoreConstants.LoginCookieNameValue));
                        model = new LoginViewModel();
                        model.Username = loginName;
                        model.RememberMe = true;
                    }
                }
            }
        }

        //To set the template name.
        [NonAction]
        private void SetUpdatedTemplateName(TemplateViewModel templateViewModel)
        {
            string updatedTemplateName = !string.IsNullOrEmpty(templateViewModel?.TemplateName) ? templateViewModel.TemplateName : (string)TempData[_UpdatedTemplateName];
            if (!HelperUtility.IsNull(templateViewModel) && !string.IsNullOrEmpty(updatedTemplateName))
                templateViewModel.TemplateName = updatedTemplateName;
        }

        private void CreateAccountAddressForGuestUser(bool isSinglePageCheckout)
        {
            if (isSinglePageCheckout)
            {
                AddressListViewModel guestUserAddressList = _userAgent.GetAnonymousUserAddress();
                if (guestUserAddressList != null)
                {
                    //only billing address is entered.
                    if ((guestUserAddressList.BillingAddress.IsDefaultBilling == true && guestUserAddressList.BillingAddress.IsDefaultShipping == false) && (guestUserAddressList.ShippingAddress.IsDefaultShipping == false && guestUserAddressList.ShippingAddress.IsDefaultBilling == false))
                        _userAgent.CreateUpdateAddress(guestUserAddressList.BillingAddress, WebStoreConstants.BillingAddressType, true, true);
                    //only shipping address is entered.
                    else if ((guestUserAddressList.ShippingAddress.IsDefaultShipping == true && guestUserAddressList?.ShippingAddress.IsDefaultBilling == false) && (guestUserAddressList.BillingAddress.IsDefaultBilling == false && guestUserAddressList.BillingAddress.IsDefaultShipping == false))
                        _userAgent.CreateUpdateAddress(guestUserAddressList.ShippingAddress, WebStoreConstants.ShippingAddressType, true, true);
                    //shipping address same as billing address
                    else if ((guestUserAddressList?.ShippingAddress.IsDefaultShipping == true && guestUserAddressList.ShippingAddress.IsDefaultBilling == true) || (guestUserAddressList.BillingAddress.IsDefaultShipping == true && guestUserAddressList.BillingAddress.IsDefaultBilling == true))
                        _userAgent.CreateUpdateAddress(guestUserAddressList.ShippingAddress, WebStoreConstants.ShippingAddressType, false, true);
                    //shipping address and billing address are different.
                    else
                    {
                        _userAgent.CreateUpdateAddress(guestUserAddressList.ShippingAddress, WebStoreConstants.ShippingAddressType, true, true);
                        _userAgent.CreateUpdateAddress(guestUserAddressList.BillingAddress, WebStoreConstants.BillingAddressType, true, true);
                    }
                }
            }
        }

        //Get the Pending Order Count
        private int GetPendingQuoteHistoryCount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            model.Filters = new FilterCollection();
            model.Filters.Add(new FilterTuple(ZnodeConstant.IsParentPendingOrder.ToLower().ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));

            //Get Account Quote list.
            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            return quoteListViewModel?.AccountQuotes?.Count() > 0 ? quoteListViewModel.AccountQuotes.Count() : 0;
        }

        //Get Pending Payment Count
        private int GetPendingPaymentHistoryCount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            model.Filters = new FilterCollection();
            //Get Account Quote list.
            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);

            return quoteListViewModel?.AccountQuotes?.Count() > 0 ? quoteListViewModel.AccountQuotes.Count() : 0;
        }

        private int GetPendingApprovalHistoryCount([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            model.Filters = new FilterCollection();
            model.Filters.Add(new FilterTuple(ZnodeConstant.IsParentPendingOrder.ToLower().ToString(), FilterOperators.Equals, ZnodeConstant.FalseValue));

            AccountQuoteListViewModel quoteListViewModel = _userAgent.GetAccountQuoteList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            return quoteListViewModel?.AccountQuotes?.Count() > 0 ? quoteListViewModel.AccountQuotes.Count() : 0;
        }

        #endregion
    }
}
