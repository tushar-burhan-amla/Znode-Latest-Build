using Facebook;

using Microsoft.AspNet.Identity.Owin;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.Klaviyo.IClient;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Agents
{
    public class UserAgent : BaseAgent, IUserAgent
    {
        #region protected Variables
        protected readonly ICountryClient _countryClient;
        protected readonly IWebStoreUserClient _webStoreAccountClient;
        protected readonly IWishListClient _wishListClient;
        protected readonly IUserClient _userClient;
        protected readonly IPublishProductClient _productClient;
        protected readonly ICustomerReviewClient _customerReviewClient;
        protected readonly IOrderClient _orderClient;
        protected readonly IGiftCardClient _giftCardClient;
        protected readonly IAccountClient _accountClient;
        protected readonly IAccountQuoteClient _accountQuoteClient;
        protected readonly IOrderStateClient _orderStateClient;
        protected readonly IPortalCountryClient _portalCountryClient;
        protected readonly IProductAgent _productAgent;
        protected readonly IShippingClient _shippingClient;
        protected readonly ICartAgent _cartAgent;
        protected readonly IPaymentClient _paymentClient;
        protected readonly ICustomerClient _customerClient;
        protected readonly IRoleClient _roleClient;
        protected readonly IStateClient _stateClient;
        protected readonly IPortalProfileClient _portalProfileClient;
        protected readonly HttpContext httpContext;
        protected string _domainName;
        protected string _domainKey;
        protected const string ApiTokenKey = "ApiToken";
        public bool IsGlobalAPIAuthorization { get; set; } = Convert.ToBoolean(ConfigurationManager.AppSettings["IsGlobalAPIAuthorization"]);
        public string DomainName
        {
            get
            {
                if (!String.IsNullOrEmpty(_domainName))
                    return _domainName;

                return !IsGlobalAPIAuthorization ? HttpContext.Current.Request.Url.Authority : ConfigurationManager.AppSettings["ZnodeApiDomainName"];
            }

            set { _domainName = value; }
        }

        protected string DomainKey
        {
            get
            {
                if (!String.IsNullOrEmpty(_domainKey))
                    return _domainKey;

                return !IsGlobalAPIAuthorization ? ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.Authority] : ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }

            set { _domainKey = value; }
        }

        #endregion

        #region Constant Variables

        public const string User = "User";
        public const string CustomerDelete = "CustomerDelete";
        public const string CustomerEnableDisableAccount = "CustomerEnableDisableAccount";
        public const string JsFunction = "EditableText.prototype.DialogDelete('{0}')";
        public const string DeleteCustomersPopup = "DeleteCustomersPopup";
        public const string AccountEnable = "accountEnable";
        public const string Accountdisable = "accountdisable";

        #endregion

        #region Constructor
        public UserAgent(ICountryClient countryClient, IWebStoreUserClient webStoreAccountClient, IWishListClient wishListClient, IUserClient userClient, IPublishProductClient productClient, ICustomerReviewClient customerReviewClient, IOrderClient orderClient,
          IGiftCardClient giftCardClient, IAccountClient accountClient, IAccountQuoteClient accountQuoteClient, IOrderStateClient orderStateClient, IPortalCountryClient portalCountryClient, IShippingClient shippingClient, IPaymentClient paymentClient, ICustomerClient customerClient, IStateClient stateClient, IPortalProfileClient portalProfileClient)
        {
            _countryClient = GetClient<ICountryClient>(countryClient);
            _webStoreAccountClient = GetClient<IWebStoreUserClient>(webStoreAccountClient);
            _wishListClient = GetClient<IWishListClient>(wishListClient);
            _userClient = GetClient<IUserClient>(userClient);
            _productClient = GetClient<IPublishProductClient>(productClient);
            _customerReviewClient = GetClient<ICustomerReviewClient>(customerReviewClient);
            _orderClient = GetClient<IOrderClient>(orderClient);
            _giftCardClient = GetClient<IGiftCardClient>(giftCardClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _accountQuoteClient = GetClient<IAccountQuoteClient>(accountQuoteClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
            _portalCountryClient = GetClient<IPortalCountryClient>(portalCountryClient);
            _productAgent = new ProductAgent(GetClient<CustomerReviewClient>(), GetClient<PublishProductClient>(), GetClient<WebStoreProductClient>(), GetClient<SearchClient>(), GetClient<HighlightClient>(), GetClient<PublishCategoryClient>());
            _shippingClient = GetClient<IShippingClient>(shippingClient);
            _cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<AccountQuoteClient>(), GetClient<UserClient>());
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _customerClient = GetClient<ICustomerClient>(customerClient);
            _roleClient = GetClient<RoleClient>();
            _stateClient = GetClient<IStateClient>(stateClient);
            _portalProfileClient = GetClient<IPortalProfileClient>(portalProfileClient);
            httpContext = HttpContext.Current;
        }
        #endregion

        #region Public Methods

        #region Login
        //Authenticate the user based on User Name & Password.
        public virtual LoginViewModel Login(LoginViewModel model)
        {
            LoginViewModel loginViewModel;
            try
            {
                model.PortalId = PortalAgent.CurrentPortal?.PortalId;
                //Authenticate the user credentials.
                UserModel userModel = _userClient.Login(UserViewModelMap.ToLoginModel(model), GetExpands());
                if (HelperUtility.IsNotNull(userModel))
                {
                    //Check of Reset password Condition.
                    if (HelperUtility.IsNotNull(userModel.User) && !string.IsNullOrEmpty(userModel.User.PasswordToken))
                    {
                        loginViewModel = UserViewModelMap.ToLoginViewModel(userModel);
                        return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword, true, loginViewModel);
                    }
                    // Check user associated profiles.                
                    if (!userModel.Profiles.Any() && !userModel.IsAdminUser)
                        return ReturnErrorModel(WebStore_Resources.ProfileLoginFailedErrorMessage);

                    SetLoginUserProfile(userModel);

                    //set user detail in the cookie.
                    SetLoginUserCookie(userModel.UserId);
                    //Save the User Details in Session.
                    SaveInSession(WebStoreConstants.UserAccountKey, userModel.ToViewModel<UserViewModel>());
                    LoginViewModel loginModel = UserViewModelMap.ToLoginViewModel(userModel);

                    //To clear the session for CartCount session key.
                    if (!loginModel.HasError)
                        _cartAgent.ClearCartCountFromSession();

                    if (PortalAgent.CurrentPortal.IsKlaviyoEnable)
                    {
                        //Created task to post the data to klaviyo

                        Task.Run(() =>
                        {
                            PostDataToKlaviyo(userModel, httpContext);
                        });
                    }
                    return loginModel;
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case 2://Error Code to Reset the Password for the first time login.
                        return ReturnErrorModel(ex.ErrorMessage, true);
                    case ErrorCodes.AccountLocked:
                        return ReturnErrorModel(WebStore_Resources.ErrorAccountLocked);
                    case ErrorCodes.LoginFailed:
                        return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
                    case ErrorCodes.TwoAttemptsToAccountLocked:
                        return ReturnErrorModel(WebStore_Resources.ErrorTwoAttemptsRemain);
                    case ErrorCodes.OneAttemptToAccountLocked:
                        return ReturnErrorModel(WebStore_Resources.ErrorOneAttemptRemain);
                    case ErrorCodes.AdminApproval:
                        return ReturnErrorModel(WebStore_Resources.LoginFailAdminApproval);
                    default:
                        return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
            }
            return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
        }

        // To post the data to klaviyo
        protected virtual void PostDataToKlaviyo(UserModel userModel, HttpContext httpContext)
        {
            HttpContext.Current = httpContext;
            IKlaviyoClient _klaviyoClient = GetComponentClient<IKlaviyoClient>(GetService<IKlaviyoClient>());
            bool identifyStatus = _klaviyoClient.IdentifyKlaviyo(MapKlaviyoIdentifyModel(userModel));
        }

        protected virtual IdentifyModel MapKlaviyoIdentifyModel(UserModel userModel)
        {
            if (HelperUtility.IsNull(userModel))
                return new IdentifyModel();

            return new IdentifyModel
            {
                PhoneNumber = userModel.PhoneNumber,
                City = userModel.CityName,
                Country = userModel.CountryName,
                Zip = userModel.PostalCode,
                StoreCode = userModel.StoreCode,
                StoreName = userModel.StoreName,
                UserName = userModel.UserName,
                CompanyName = userModel.CompanyName
            };
        }

      public virtual LoginViewModel ImpersonationLogin(string token)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            int? portalId = PortalAgent.CurrentPortal?.PortalId;
            //Check token is valid or not
            ImpersonationAPIModel impersonation = _userClient.ValidateCSRToken(token);
            if (HelperUtility.IsNotNull(impersonation))
            {
                loginViewModel.HasError = !impersonation.Result;
                if (impersonation.Result)
                {
                    // Get user details
                    UserModel userModel = _userClient.GetAccountByUser(impersonation.UserName);
                    loginViewModel.Username = impersonation.UserName;
                    loginViewModel.RememberMe = false;
                    loginViewModel.IsWebStoreUser = true;
                    impersonation.ShopperName = userModel.FirstName + " " + userModel.LastName;
                    impersonation.PortalId = (PortalAgent.CurrentPortal?.PortalId).GetValueOrDefault();
                    // Use to set data in session of impersonation
                    SetImpersonationSession(impersonation);
                    SetLoginUserProfile(userModel);
                    //Save the User Details in Session.
                    SaveInSession(WebStoreConstants.UserAccountKey, userModel.ToViewModel<UserViewModel>());

                    //To clear the session for CartCount session key.
                    _cartAgent.ClearCartCountFromSession();
                    return UserViewModelMap.ToLoginViewModel(userModel);
                }
            }
            return loginViewModel;
        }

        // This method is used to set impersonation session
        protected virtual void SetImpersonationSession(ImpersonationAPIModel impersonation)
        {
            UserModel csrUserModel = _userClient.GetUserAccountData(impersonation.CRSUserId, impersonation.PortalId);
            ImpersonationModel impersonationModel = new ImpersonationModel();
            impersonationModel.Token = impersonation.Token;
            impersonationModel.IsImpersonation = impersonation.IsImpersonation;
            impersonationModel.CRSUserId = impersonation.CRSUserId;
            impersonationModel.CRSName = !Equals(csrUserModel, null) ? csrUserModel.FirstName + " " + csrUserModel.LastName : "";
            impersonationModel.WebstoreUserId = impersonation.WebstoreUserId;
            impersonationModel.ShopperName = impersonation.ShopperName;
            impersonationModel.UserName = impersonation.UserName;
            impersonationModel.PortalId = impersonation.PortalId;
            SaveInSession(WebStoreConstants.ImpersonationSessionKey, impersonationModel);
        }
        // Logout the logged in user.       
        public virtual void Logout()
        {
            //Clear address from cache.
            ClearAddressCache(GetUserId());
            RemoveCookie(WebStoreConstants.CartCookieKey);
            RemoveCookie(WebStoreConstants.UserIdCookie);
            FormsAuthentication.SignOut();
            SessionHelper.Abandon();
            SessionHelper.Clear();
            //Clear logged in user session
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie = new HttpCookie(sessionStateSection.CookieName, "");
            if (IsNotNull(cookie))
            {
                //SessionProxyHelper.RemoveAuthenticatedUserSession();
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            //To clear the session for CartCount session key.
            _cartAgent.ClearCartCountFromSession();
            //ProfileId = null;
        }

        //Logout from social login.
        public virtual string Logout(ExternalLoginInfo loginInfo)
        {
            // get the domain url.
            string redirectUrl = GetDomainUrl();
            //logout from system.
            Logout();
            SaveInSession(WebStoreConstants.SocialLoginDetails, null as ExternalLoginInfo);

            //logout from social login.
            if (!Equals(loginInfo, null) && !string.IsNullOrEmpty(loginInfo.Login.LoginProvider))
            {
                switch (loginInfo.Login.LoginProvider.ToLower())
                {
                    case "facebook":
                        string accessToken = GetAccessToken(loginInfo);
                        return $"https://www.facebook.com/logout.php?next={redirectUrl}&rd={redirectUrl}&access_token={accessToken}";
                    case "google":
                        return $"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue={redirectUrl}";
                    default:
                        break;
                }
            }
            return redirectUrl;
        }

        //Change Password for Logged in user.
        public virtual ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                //Change the Password for the user.
                _userClient.ChangePassword(UserViewModelMap.ToChangePasswordModel(model));

                return new ChangePasswordViewModel
                {
                    SuccessMessage = WebStore_Resources.SuccessPasswordChanged,
                };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return new ChangePasswordViewModel
                {
                    HasError = true,
                    ErrorMessage = string.IsNullOrEmpty(ex.ErrorMessage) ? WebStore_Resources.ErrorChangePassword : ex.ErrorMessage,
                };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (ChangePasswordViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorChangePassword);
            }
        }

        //Forgot Password, use to send the Reset Password Link to the user.
        public virtual UserViewModel ForgotPassword(UserViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                try
                {
                    //trim all white spaces from username
                    model.UserName = Regex.Replace(model.UserName, @"\s", "");
                    //Get the User Details based on username.
                    UserModel userModel = _userClient.GetAccountByUser(model.UserName);
                    if (HelperUtility.IsNotNull(userModel))
                    {
                        if (!model.HasError)
                        {
                            //Check if the Username Validates or not.
                            if (string.Equals(model.UserName, userModel.UserName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                //Set the Current Domain Url.
                                model.BaseUrl = GetDomainUrl();
                                model.Email = string.IsNullOrEmpty(userModel.Email) ? userModel.UserName : userModel.Email;

                                //Reset the Password for the user, to send the Reset Email Password Link.
                                model = ResetPassword(model);
                                return model;
                            }
                            else
                                return SetErrorProperties(model, WebStore_Resources.ValidEmailAddress);
                        }
                    }
                    else
                        return SetErrorProperties(model, WebStore_Resources.InvalidAccountInformation);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    return SetErrorProperties(model, WebStore_Resources.InvalidAccountInformation);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return SetErrorProperties(model, WebStore_Resources.InvalidAccountInformation);
                }
            }

            return model;
        }

        //Verify the Reset Password Email Link Status.
        public virtual ResetPasswordStatusTypes VerifyResetPasswordLinkStatus(ChangePasswordViewModel model)
        {
            try
            {
                if (HelperUtility.IsNotNull(model))
                {
                    //Check the Reset Password Link Status.
                    int? resetPasswordStatus = _userClient.VerifyResetPasswordLinkStatus(UserViewModelMap.ToChangePasswordModel(model));
                    switch (resetPasswordStatus)
                    {
                        case ErrorCodes.ResetPasswordContinue:
                            return ResetPasswordStatusTypes.Continue;
                        case ErrorCodes.ResetPasswordLinkExpired:
                            return ResetPasswordStatusTypes.LinkExpired;
                        case ErrorCodes.ResetPasswordNoRecord:
                            return ResetPasswordStatusTypes.NoRecord;
                        case ErrorCodes.ResetPasswordTokenMismatch:
                            return ResetPasswordStatusTypes.TokenMismatch;
                        default:
                            return ResetPasswordStatusTypes.NoRecord;
                    }
                }
                return ResetPasswordStatusTypes.NoRecord;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return ResetPasswordStatusTypes.NoRecord;
            }
        }

        //Create the WebStore Customer.
        public virtual RegisterViewModel SignUp(RegisterViewModel model)
        {
            try
            {
                model.BaseUrl = GetDomainUrl();
                var userModel = _userClient.CreateCustomerAccount(UserViewModelMap.ToSignUpModel(model));
                if (IsNotNull(userModel))
                {
                    if (model.UserVerificationTypeCode == UserVerificationTypeEnum.AdminApprovalCode)
                        throw new ZnodeException(ErrorCodes.AdminApproval, string.Empty);
                    if (model.UserVerificationTypeCode == UserVerificationTypeEnum.EmailVerificationCode)
                        throw new ZnodeException(ErrorCodes.EmailVerification, string.Empty);

                    LoginViewModel loginModel = Login(new LoginViewModel() { Username = model.UserName, Password = model.Password });
                    if (!loginModel.HasError)
                        return model;
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.ErrorUserNameAlreadyExists);
                    case ErrorCodes.AdminApproval:
                        model.ErrorCode = ErrorCodes.AdminApproval;
                        return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.AdminApprovalSuccessMessage);
                    case ErrorCodes.EmailVerification:
                        model.ErrorCode = ErrorCodes.EmailVerification;
                        return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.EmailVerificationSuccessMessage);
                    default:
                        return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.ErrorFailedToCreate);
            }
            return (RegisterViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.ErrorFailedToCreate);
        }

        // Get available profile list for user.
        public virtual List<SelectListItem> GetProfiles(List<ProfileModel> profiles)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            int? profileId = Helper.GetProfileId();
            if (profiles?.Count > 1)
            {
                listItems = (from item in profiles
                             select new SelectListItem
                             {
                                 Text = item.ProfileName,
                                 Value = item.ProfileId.ToString(),
                                 Selected = item.ProfileId == profileId
                             }).ToList();
            }
            return listItems;
        }

        //To set profile Id in session for the logged in user
        public virtual bool ChangeUserProfile(int profileId)
        {
            try
            {
                UserViewModel userModel = GetUserViewModelFromSession();
                if (HelperUtility.IsNull(userModel)) return false;
                userModel.ProfileId = profileId;
                SaveInSession(WebStoreConstants.UserAccountKey, userModel);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        public virtual string GetReturnUrlAfterLogin(LoginViewModel loginViewModel)
        {
            if (!loginViewModel.HasError)
            {
                //Added the code to navigate to the relative path provided in the global attribute
                string globalAttributeRedirectUrl = PortalAgent.CurrentPortal?.GlobalAttributes?.Attributes?.FirstOrDefault(x => x.AttributeCode == WebStoreConstants.RedirectPageOnLogin)?.AttributeValue;

                if (!string.IsNullOrEmpty(globalAttributeRedirectUrl) && !string.IsNullOrEmpty(loginViewModel.ReturnUrl) && loginViewModel.ReturnUrl.Contains("AddToWishList"))
                    return $"{loginViewModel.ReturnUrl.Remove(loginViewModel.ReturnUrl.IndexOf("="))}={HttpUtility.UrlEncode(loginViewModel.ReturnUrl.Substring(loginViewModel.ReturnUrl.IndexOf("productSKU")).Split('=')[1])}";
                else if (PortalAgent.CurrentPortal.PersistentCartEnabled && _cartAgent.GetCartCountAfterLogin(true) > 0)
                    return "~/cart";
                else if (!string.IsNullOrEmpty(globalAttributeRedirectUrl))
                    return globalAttributeRedirectUrl;
                else if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && (loginViewModel.ReturnUrl.Contains("checkout") || loginViewModel.ReturnUrl.Contains("GetReturnDetails")))
                    return $"~/Checkout/Index?{loginViewModel.IsSinglePageCheckout }";
                else if (string.IsNullOrEmpty(loginViewModel.ReturnUrl) || (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && loginViewModel.ReturnUrl.ToLower().Contains("login")))
                    return "~/";
                else if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && loginViewModel.ReturnUrl.Contains("AddToWishList"))
                    return $"{loginViewModel.ReturnUrl.Remove(loginViewModel.ReturnUrl.IndexOf("="))}={HttpUtility.UrlEncode(loginViewModel.ReturnUrl.Substring(loginViewModel.ReturnUrl.IndexOf("productSKU")).Split('=')[1])}";
            }
            return string.Empty;
        }
        #endregion

        #region Social Login

        public virtual LoginViewModel SocialLogin(ExternalLoginInfo loginInfo, bool isPersistent, string username = null)
        {
            if (HelperUtility.IsNotNull(loginInfo))
            {
                LoginViewModel loginViewModel;
                try
                {
                    loginInfo.Email = GetFacebookEmail(loginInfo);
                    UserModel userModel = _userClient.SocialLogin(new SocialLoginModel { IsPersistent = isPersistent, LoginInfo = loginInfo, PortalId = PortalAgent.CurrentPortal?.PortalId ?? 0, UserName = string.IsNullOrEmpty(username) ? GetUserName(loginInfo) : username, UserVerificationTypeCode = PortalAgent.CurrentPortal.UserVerificationTypeCode });
                    if (HelperUtility.IsNotNull(userModel))
                    {
                        //Check of Reset password Condition.
                        if (HelperUtility.IsNotNull(userModel.User) && !string.IsNullOrEmpty(userModel.User.PasswordToken))
                        {
                            loginViewModel = UserViewModelMap.ToLoginViewModel(userModel);
                            return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword, true, loginViewModel);
                        }

                        // Check user associated profiles.                
                        if (!userModel.Profiles?.Any() ?? false)
                            return ReturnErrorModel(WebStore_Resources.ProfileLoginFailedErrorMessage);

                        loginViewModel = UserViewModelMap.ToLoginViewModel(userModel);
                        //Save the User Details in Session.
                        SaveInSession(WebStoreConstants.UserAccountKey, userModel.ToViewModel<UserViewModel>());

                        //Save the social login info in session.
                        SaveInSession(WebStoreConstants.SocialLoginDetails, loginInfo);

                        //To clear the session for CartCount session key.
                        if(!loginViewModel.HasError)
                            _cartAgent.ClearCartCountFromSession();

                        return loginViewModel;
                    }
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case 2://Error Code to Reset the Password for the first time login.
                            return ReturnErrorModel(ex.ErrorMessage, true);
                        case ErrorCodes.AccountLocked:
                            return ReturnErrorModel(WebStore_Resources.ErrorAccountLocked, false, null, ex.ErrorCode);
                        case ErrorCodes.LoginFailed:
                            return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
                        case ErrorCodes.TwoAttemptsToAccountLocked:
                            return ReturnErrorModel(WebStore_Resources.ErrorTwoAttemptsRemain);
                        case ErrorCodes.OneAttemptToAccountLocked:
                            return ReturnErrorModel(WebStore_Resources.ErrorOneAttemptRemain);
                        case ErrorCodes.AdminApprovalLoginFail:
                            return ReturnErrorModel(WebStore_Resources.LoginFailAdminApproval, false, null, ex.ErrorCode);
                        case 3://In case of social login create new user.
                            return RegisterNewSocialLoginUser(loginInfo);
                        default:
                            return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
                }
            }
            return ReturnErrorModel(WebStore_Resources.InvalidUserNamePassword);
        }

        //Get the login provider details from database.
        public virtual SocialModel GetLoginProviders()
        {
            string apiUrl = $"{ZnodeWebstoreSettings.ZnodeApiRootUri}/users/getsocialloginproviders";

            using (WebClient client = new WebClient())
            {
                client.Headers.Add(GetAuthorizationHeader(DomainName, DomainKey, apiUrl));
                if (ZnodeWebstoreSettings.EnableTokenBasedAuthorization)
                {
                    client.Headers.Add($"Token:{GetToken()}");
                }
                try
                {
                    string jsonModel = client.DownloadString(apiUrl);
                    if (!string.IsNullOrEmpty(jsonModel))
                    {
                        SocialModel clientsModel = JsonConvert.DeserializeObject<SocialModel>(jsonModel);
                        return clientsModel;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                }
            }
            return null;
        }

        public string GetAuthorizationHeader(string domainName, string domainKey, string endpoint = "")
           => $"Authorization: Basic {EncodeBase64($"{domainName}|{domainKey}")}";


        //Get Token for api.
        protected virtual string GetToken()
        {
            string token = (string)HttpContext.Current.Cache[ApiTokenKey];

            if (string.IsNullOrEmpty(token))
                return GenerateAndInsertTokenIntoCache();

            return token;
        }

        //Generate token and insert token into cache.
        protected virtual string GenerateAndInsertTokenIntoCache()
        {
            string Token = string.Empty;
            string endpoint = $"{ZnodeWebstoreSettings.ZnodeApiRootUri}/token/generatetoken";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(GetAuthorizationHeader(DomainName, DomainKey, endpoint));

                string jsonModel = client.DownloadString(endpoint);

                if (!string.IsNullOrEmpty(jsonModel))
                {
                    Api.Models.Responses.StringResponse response = JsonConvert.DeserializeObject<Api.Models.Responses.StringResponse>(jsonModel);
                    Token = response.Response;
                    if (!string.IsNullOrEmpty(Token))
                        HttpContext.Current.Cache.Insert(ApiTokenKey, Token);
                }
            }
            return (string)HttpContext.Current.Cache[ApiTokenKey];
        }

        //Register user for social login.
        protected virtual UserModel RegisterSocialLoginUser(ExternalLoginInfo loginInfo)
        {
            if (IsNotNull(loginInfo))
            {
                string email = GetFacebookEmail(loginInfo);

                UserModel userModel = _userClient.CreateCustomerAccount(GetUserModelToCreate(loginInfo, email));
                if (IsNotNull(userModel))
                {
                    if (PortalAgent.CurrentPortal.UserVerificationTypeCode == UserVerificationTypeEnum.AdminApprovalCode)
                    {
                        ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginFailed, string.Empty, string.Empty, null, Admin_Resources.LogInFailed, null);
                        throw new ZnodeException(ErrorCodes.AdminApproval, WebStore_Resources.AdminApprovalSuccessMessage, HttpStatusCode.Unauthorized);
                    }
                    SocialLogin(loginInfo, false, userModel.UserName);
                    return userModel;
                }
            }
            return null;
        }

        #endregion

        #region Customer Account List
        public virtual CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = default(int?), int? recordPerPage = default(int?))
        {
            //Set the sort collection for user id desc.
            SortUserIdDesc(ref sortCollection);
            UserListModel userList = _userClient.GetCustomerAccountList(currentUserName, filters, sortCollection, pageIndex, recordPerPage);
            CustomerListViewModel customerListViewModel = new CustomerListViewModel { List = userList?.Users?.ToViewModel<CustomerViewModel>().ToList() };
            SetListPagingData(customerListViewModel, userList);
            customerListViewModel?.List?.ForEach(x => x.IsGuestUser = string.IsNullOrEmpty(x.AspNetUserId) ? true : false);

            return customerListViewModel?.List?.Count > 0 ? customerListViewModel
                : new CustomerListViewModel();
        }

        //Get account customer by user id.
        public virtual CustomerAccountViewModel GetAccountsCustomer(int userId)
        {
            //Sets the filter for UserId and IsAccountCustomer. 
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            HelperMethods.SetIsAccountCustomerFilter(filters, 1);

            //Gets the list.
            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);
            if (userList?.Users?.Count > 0)
                return userList.Users.First()?.ToViewModel<CustomerAccountViewModel>();

            return new CustomerAccountViewModel { HasError = true };
        }

        //Set customer account view model.
        public virtual void SetCustomerAccountViewModel(CustomerAccountViewModel customerAccountViewModel, int accountId)
        {
            //if null then create new instance of the same
            if (IsNull(customerAccountViewModel))
                customerAccountViewModel = new CustomerAccountViewModel();
            //Sets the properties of customerAccountViewModel
            customerAccountViewModel.AccountDepartmentList = GetAccountDepartmentList(accountId);
            customerAccountViewModel.Roles = GetAccountRoleList(customerAccountViewModel);
            customerAccountViewModel.UserApprovalList = GetApproverList(accountId, customerAccountViewModel.UserId, HttpContext.Current.User.Identity.Name);
            customerAccountViewModel.AccountId = accountId;
            AccountViewModel accountViewModel = GetAccountInformation();
            customerAccountViewModel.AccountName = accountViewModel.Name;
            customerAccountViewModel.PortalId = accountViewModel.PortalId;
        }

        //Get List of Active Countries.
        public virtual List<SelectListItem> GetAccountDepartmentList(int accountId)
        {
            List<SelectListItem> departmentSelectList = new List<SelectListItem>();
            FilterCollection filter = new FilterCollection();
            SetFiltersForAccountId(filter, accountId);
            AccountDepartmentListViewModel departments = GetAccountDepartments(filter, null, null, null);

            if (departments?.Departments?.Count > 0)
            {
                foreach (AccountDepartmentViewModel department in departments.Departments)
                    departmentSelectList.Add(new SelectListItem() { Text = department.DepartmentName, Value = department.DepartmentId.ToString() });
            }
            return departmentSelectList;
        }

        //Gets the list of b2b roles.
        public virtual List<SelectListItem> GetAccountRoleList(CustomerAccountViewModel customerAccountViewModel)
        {
            List<SelectListItem> roleList = new List<SelectListItem>();
            SortCollection sorts = new SortCollection();
            sorts.Add(AspNetRoleEnum.Name.ToString(), DynamicGridConstants.ASCKey);
            RoleListModel roleListModel = GetClient<RoleClient>().GetRoleList(new FilterCollection() { new FilterTuple(AspNetRoleEnum.TypeOfRole.ToString(), FilterOperators.Is, WebStoreConstants.B2B) }, sorts);
            if (roleListModel?.Roles?.Count > 0)
            {
                foreach (RoleModel roles in roleListModel.Roles)
                    roleList.Add(new SelectListItem() { Text = roles.Name, Value = roles.RoleId.ToString(), Selected = roles.Name == customerAccountViewModel.RoleName });
            }

            return roleList;
        }

        //Get account department list.
        public virtual AccountDepartmentListViewModel GetAccountDepartments(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            int accountId;

            AccountDepartmentListModel departmentListModel = _accountClient.GetAccountDepartments(null, filters, sorts, pageIndex, recordPerPage);
            AccountDepartmentListViewModel departmentListViewModel = new AccountDepartmentListViewModel { Departments = departmentListModel?.Departments?.ToViewModel<AccountDepartmentViewModel>().ToList() };
            SetListPagingData(departmentListViewModel, departmentListModel);

            //Get the AccountId value from the filter collection.
            int.TryParse(filters?.Find(x => x.FilterName == FilterKeys.AccountId.ToString()).FilterValue, out accountId);

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            departmentListViewModel.HasParentAccounts = accountDetails.HasParentAccounts;
            departmentListViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            departmentListViewModel.AccountId = accountDetails.AccountId;

            return departmentListViewModel?.Departments?.Count > 0 ? departmentListViewModel
               : new AccountDepartmentListViewModel() { Departments = new List<AccountDepartmentViewModel>(), HasParentAccounts = accountDetails.HasParentAccounts, AccountName = accountDetails?.CompanyAccount?.Name, AccountId = accountDetails.AccountId };
        }

        //Get the Account Details based of the Account Id.
        public virtual AccountDataViewModel GetAccountById(int accountId = 0)
        {
            AccountDataViewModel model = new AccountDataViewModel();
            if (accountId > 0)
            {
                AccountModel accountModel = _accountClient.GetAccount(accountId);
                if (IsNotNull(accountModel))
                {
                    model.CompanyAccount = accountModel.ToViewModel<AccountViewModel>();
                    model.HasParentAccounts = model.CompanyAccount?.ParentAccountId <= 0;
                    model.AccountId = accountId;
                }
            }
            return model;
        }

        //Set filters for account id.
        public virtual void SetFiltersForAccountId(FilterCollection filters, int accountId)
        {
            if (!Equals(filters, null))
            {
                //Checking For AccountId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.AccountId))
                {
                    //If AccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.AccountId);

                    //Add New AccountId Into filters
                    filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));
            }
        }

        // Get approver list based on account id.
        public virtual List<SelectListItem> GetApproverList(int accountId, int? userId, string currentUserName)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            //filters for approver list.
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString());
            filters.Add(FilterKeys.IsApprovalList, FilterOperators.NotEquals, ZnodeConstant.AccountUser);

            int loggedUserAccountId = _userClient.GetAccountByUser(currentUserName).UserId;
            //Get all available portal list.
            UserListModel approverList = _userClient.GetUserAccountList(loggedUserAccountId, filters, null, null, null);

            //Remove userId while updating the same user.
            UserModel itemToRemove = approverList?.Users?.FirstOrDefault(r => r.UserId == userId);
            if (IsNotNull(itemToRemove))
                approverList.Users.Remove(itemToRemove);

            if (approverList?.Users?.Count > 0)
            {
                listItems = (from item in approverList.Users
                             select new SelectListItem
                             {
                                 Text = item.FullName,
                                 Value = item.UserId.ToString(),
                             }).ToList();
            }
            return listItems;
        }

        //Update customer account data.
        public virtual bool UpdateCustomerAccount(CustomerAccountViewModel model, out string errorMessage)
        {
            errorMessage = WebStore_Resources.UpdateErrorMessage;
            try
            {
                return _userClient.UpdateCustomerAccount(CustomerViewModelMap.ToUserModel(model))?.UserId > 0;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.ProfileNotPresent:
                        errorMessage = WebStore_Resources.ErrorProfileNotExistsWhileUpdate;
                        break;
                    default:
                        errorMessage = WebStore_Resources.UpdateErrorMessage;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = WebStore_Resources.UpdateErrorMessage;
            }
            return false;

        }

        //userIds which are to be deleted.
        //Check Current Logged in user name - It cannot be deleted.
        public virtual bool DeleteCustomer(string userIds, string currentUserName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(userIds))
            {
                try
                {
                    //Get account id of current logged in user. 
                    int? loggedUserId = GetUserViewModelFromSession()?.UserId;
                    string[] arrayIds = userIds.Split(',');

                    //Check if array of Ids contain the Id of logged in user as it cannot be deleted.
                    if (arrayIds.Count(r => r == loggedUserId.ToString()) > 0)
                    {
                        //If selected account Id  is the only logged in user Id, then return with an error message.
                        if (Equals(userIds, loggedUserId.ToString()))
                        {
                            errorMessage = Admin_Resources.FailedDeleteLoginUser;
                            return false;
                        }
                        else
                        {
                            //Remove logged in user Id from array of Ids and then delete those.
                            arrayIds = arrayIds.Where(val => val != loggedUserId.ToString()).ToArray();
                            string accountIdsToDelete = string.Join(",", arrayIds);
                            if (_userClient.DeleteUser(new ParameterModel { Ids = accountIdsToDelete }))
                            {
                                errorMessage = Admin_Resources.SuccessDeleteMessageExceptLoginUser;
                                return true;
                            }
                            else
                            {
                                errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                                return false;
                            }
                        }
                    }
                    else
                        return _userClient.DeleteUser(new ParameterModel { Ids = userIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    errorMessage = ex.ErrorCode == ErrorCodes.AssociationDeleteError ? Admin_Resources.ErrorDeleteAccount : Admin_Resources.ErrorMessageHalfDelete;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                }
            }
            return false;
        }

        //Enable disable customer account.
        public virtual bool EnableDisableUser(string userId, bool lockUser, out string errorMessage)
        {
            errorMessage = string.Empty;
            //Logged in UserId- Remove it from string of account ids so that it does not get locked. 
            int? loggedUserId = GetUserViewModelFromSession()?.UserId;
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    string[] arrAccountIds = userId.Split(',');
                    if (arrAccountIds.Count(r => r == loggedUserId?.ToString()) > 0)
                    {
                        //If selected account Id  is the only logged in user Id, then return with an error message.
                        if (Equals(userId, loggedUserId.ToString()))
                        {
                            errorMessage = WebStore_Resources.ErrorDisableLoggedInUser;
                            return false;
                        }
                        else
                        {
                            arrAccountIds = arrAccountIds.Where(val => val != loggedUserId?.ToString()).ToArray();
                            string remainingAccountIds = string.Join(",", arrAccountIds);
                            if (_userClient.EnableDisableAccount(new ParameterModel { Ids = remainingAccountIds }, lockUser))
                            {
                                errorMessage = Admin_Resources.SuccessMsgEnableDisableExceptLoggedUser;
                                return true;
                            }
                            else
                            {
                                errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                                return false;
                            }
                        }
                    }
                    else
                        return _userClient.EnableDisableAccount(new ParameterModel { Ids = userId }, lockUser);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.LockOutEnabled:
                            errorMessage = Admin_Resources.LockOutDisabledErrorMessage;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageEnableDisable;
                }
            }
            return false;
        }

        //Reset Password of Existing User User
        public virtual bool ResetPassword(int accountId, out string errorMessage)
        {
            errorMessage = WebStore_Resources.ErrorAccessDenied;
            UserModel accountModel = _userClient.GetUserAccountData(accountId);
            try
            {
                if (IsNotNull(accountModel))
                {
                    if (Equals(accountModel.User?.Email, accountModel.Email))
                    {
                        //Set the domain url.
                        bool isB2BCustomer = IsB2BCustomer(accountModel);
                        accountModel.BaseUrl = GetDomainUrl();
                        return IsNotNull(_userClient.ForgotPassword(accountModel));
                    }
                    else
                    {
                        errorMessage = WebStore_Resources.ValidEmailAddress;
                        return false;
                    }
                }
                else
                {
                    errorMessage = WebStore_Resources.ErrorAccessDenied;
                    return false;
                }
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Warning);
                return GetErrorMessage(out errorMessage, exception);
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Error);
                errorMessage = WebStore_Resources.ErrorAccessDenied;
                return false;
            }
        }

        //Reset Password in Bulk
        public virtual bool BulkResetPassword(string accountId, out string errorMessage)
        {
            errorMessage = WebStore_Resources.ErrorResetPassword;
            if (!string.IsNullOrEmpty(accountId))
            {
                try
                {
                    return _userClient.BulkResetPassword(new ParameterModel { Ids = accountId });
                }
                catch (ZnodeException exception)
                {
                    ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Warning);
                    switch (exception.ErrorCode)
                    {
                        case ErrorCodes.ErrorResetPassword:
                            errorMessage = string.Format(WebStore_Resources.ErrorResetPassword, exception.Message);
                            break;
                        case ErrorCodes.UserNameUnavailable:
                            errorMessage = exception.ErrorMessage;
                            break;
                        case ErrorCodes.AccountLocked:
                            errorMessage = WebStore_Resources.AccountLockedErrorMessage;
                            break;
                        case ErrorCodes.EmailTemplateDoesNotExists:
                            errorMessage = WebStore_Resources.ResetPasswordTemplateNotFound;
                            return false;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        // Get department list based on account id.
        public virtual List<SelectListItem> GetAccountDepartments(int accountId)
          => GetAccountDepartmentList(accountId);

        // Get b2b permission list.
        public virtual string GetPermissionList(int accountId, int accountPermissionId)
            => AccountPermissionList(accountId, accountPermissionId);

        //Get the html for drop down of account permission list.
        public virtual string AccountPermissionList(int accountId, int? accountPermissionId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));

            AccessPermissionListModel accountPermissions = GetClient<AccessPermissionClient>().AccountPermissionList(null, filters, null, null, null);
            string permissionsOption = "<select name='AccountPermissionAccessId' id='ddlPermission'>";
            //Binds the option in select list.
            if (accountPermissions?.AccountPermissions?.Count > 0)
                foreach (AccessPermissionModel access in accountPermissions.AccountPermissions)
                {
                    string selected = access.AccountPermissionAccessId == accountPermissionId ? " selected='selected'" : " ";
                    permissionsOption += "<option data-permissioncode=" + access.PermissionCode + " value=" + access.AccountPermissionAccessId + selected + ">" + access.AccountPermissionName + "</option>";
                }

            return permissionsOption += "</select>";
        }

        //Get the grid model for binding the tools.
        public virtual GridModel GetGridModel()
            => new GridModel { FilterColumn = new FilterColumnListModel { ToolMenuList = new List<ToolMenuModel>() } };

        #endregion

        // Sign up the user for News letter.
        public virtual bool SignUpForNewsLetter(NewsLetterSignUpViewModel model, out string message)
        {
            message = string.Empty;
            try
            {
                return _userClient.SignUpForNewsLetter(new NewsLetterSignUpModel() { Email = model.Email });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                message = WebStore_Resources.EmailAlreadyExistMsg;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = WebStore_Resources.EmailAlreadyExistMsg;
            }
            return false;
        }

        //Get Address information on the basis of Address Id.
        public virtual AddressViewModel GetAddress(int? addressId)
        {
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (addressId > 0)
            {
                AddressViewModel addressViewModel = (userViewModel?.Addresses?.Count() > 0) ? userViewModel.Addresses.FirstOrDefault(x => x.AddressId == addressId) :
                                  _webStoreAccountClient.GetAddress(addressId).ToViewModel<AddressViewModel>();

                if (HelperUtility.IsNotNull(addressViewModel))
                {
                    addressViewModel.AddressCount = IsNotNull(userViewModel?.Addresses) ? userViewModel.Addresses.Count() : 0;
                    if (userViewModel.AccountId.GetValueOrDefault() == addressViewModel.AccountId || userViewModel.UserId == addressViewModel.UserId)
                        addressViewModel.Countries = GetCountries();
                }
                return addressViewModel;
            }
            return new AddressViewModel() { Countries = GetCountries(), AddressCount = IsNotNull(userViewModel?.Addresses) ? userViewModel.Addresses.Count() : 0, EmailAddress = userViewModel?.Email };
        }

        //The parameters isShippingBillingDifferent and isCreateAccountForGuestUser are not in use currently but for compatibility we have added these parameter, In future release we will remove these parameter
        // Create/Update the address of the user.       
        public virtual AddressViewModel CreateUpdateAddress(AddressViewModel addressViewModel, string addressType = null, bool isShippingBillingDifferent = false, bool isCreateAccountForGuestUser = false)
        {
            try
            {
                SetBillingShippingFlags(addressViewModel, addressType);
                UserViewModel model = GetUserViewModelFromSession();

                #region Checks to validate address.               

                Api.Models.BooleanModel booleanModel = IsValidAddress(addressViewModel);
                //Validate address if enable address validation flag is set to true.
                if (!booleanModel.IsSuccess)
                    return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, booleanModel.ErrorMessage ?? WebStore_Resources.AddressValidationError);

                //Validate Address ID with Session Address ID.
                if (HelperUtility.IsNotNull(model) && model?.Addresses?.Count > 0 && addressViewModel?.AddressId > 0)
                {
                    //Get User Address On Basis Of Address ID Passed In Query String
                    var userAddress = model.Addresses.Where(x => x.AddressId == addressViewModel.AddressId)?.FirstOrDefault();
                    if (HelperUtility.IsNull(userAddress))
                    {
                        return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, booleanModel.ErrorMessage ?? WebStore_Resources.HttpCode_401_AccessDeniedMsg);
                    }
                }

                if (isShippingBillingDifferent && isCreateAccountForGuestUser) addressViewModel.IsShippingBillingDifferent = true;

                if (HelperUtility.IsNotNull(model) && !isShippingBillingDifferent)
                    //Validate default address.
                    addressViewModel = ValidateDefaultAddress(addressViewModel, model);

                if (addressViewModel.HasError)
                    return addressViewModel;

                //if user is null then save address for anonymous user or
                // if aspnetUserId is null then save address for anonymous user 
                if (model?.AspNetUserId == null || (model?.UserId).GetValueOrDefault() < 1)
                    return SetAnonymousUserAddresses(addressViewModel, addressType);
                #endregion

                addressViewModel.UserId = model.UserId;
                addressViewModel.AccountId = model.AccountId.GetValueOrDefault();

                //Create/Update address.
                addressViewModel = CreateUpdateAddress(addressViewModel);

                //Remove address from cache and session.
                if (addressViewModel?.AddressId > 0)
                    RemoveAddressFromCacheAndSession(addressViewModel);
                return addressViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), WebStore_Resources.ErrorAddUpdateAddressNewCustomer);
                    default:
                        return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get address list of user.
        public virtual AddressListViewModel GetAddressList(bool isAddressBook = false)
        {
            return GetAddressList(0, isAddressBook);
        }


        //Get address list of user.
        public virtual AddressListViewModel GetAddressList(int userId, bool isAddressBook = false)
        {
            AddressListViewModel addressViewModel = new AddressListViewModel();
            if (userId > 0)
            {
                //Get address from cache.
                string cacheKey = $"{WebStoreConstants.UserAccountAddressList}{userId}";

                addressViewModel.UserId = userId;
                GetUserAddressList(addressViewModel, cacheKey, addressViewModel.AccountId);
            }
            else
            {
                //Get user details from session and bind account and user id to addressListViewModel.
                UserViewModel userDetails = GetUserViewModelFromSession();
                if (HelperUtility.IsNotNull(userDetails))
                {
                    addressViewModel.AccountId = userDetails.AccountId.GetValueOrDefault();
                    addressViewModel.UserId = userDetails.UserId;
                    addressViewModel.RoleName = userDetails.RoleName;
                    addressViewModel.ShippingAddress = new AddressViewModel() { AspNetUserId = userDetails.AspNetUserId };
                }
                // If AspNetUserId is null then get anonymous User Address
                if (addressViewModel.UserId == 0 || addressViewModel.ShippingAddress?.AspNetUserId == null)
                {
                    return GetAnonymousUserAddress();
                }

                //Get address from cache.
                string cacheKey = $"{WebStoreConstants.UserAccountAddressList}{addressViewModel.UserId}";

                if (isAddressBook)
                {
                    Helper.ClearCache(cacheKey);
                }

                //If data is not cached make a call else get cached data.
                if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
                {
                    GetUserAddressList(addressViewModel, cacheKey, addressViewModel.AccountId);
                }

                addressViewModel = Helper.GetFromCache<AddressListViewModel>(cacheKey);
                if (HelperUtility.IsNotNull(addressViewModel))
                {
                    addressViewModel.AddressList?.ForEach(address =>
                    {
                        address.EmailAddress = !string.IsNullOrEmpty(address.EmailAddress) ? address.EmailAddress : userDetails?.Email;
                    });
                    addressViewModel.RoleName = userDetails?.RoleName;
                }
            }
            return addressViewModel;
        }

        // Create/Update the address of the user.       
        public virtual AddressViewModel UpdateAddress(int addressId, bool isDefaultBillingAddress)
        {
            AddressViewModel addressViewModel = new AddressViewModel();

            //Get Account and UserID
            UserViewModel model = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            //Get user address.
            addressViewModel = model?.Addresses.Count > 0 ? model.Addresses.Where(x => x.AddressId == addressId)?.FirstOrDefault() :
                               _webStoreAccountClient.GetAddress(addressId).ToViewModel<AddressViewModel>();
            addressViewModel.UserId = model.UserId;

            if (isDefaultBillingAddress)
                addressViewModel.IsDefaultBilling = true;
            else
                addressViewModel.IsDefaultShipping = true;

            //Create/Update the address of the user 
            addressViewModel = _webStoreAccountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
            if (addressViewModel?.AddressId > 0)
            {
                addressViewModel.RoleName = model?.RoleName;
                //Clear address from cache.
                ClearAddressCache((model?.UserId).GetValueOrDefault());
                //Clear address from session.
                ClearSessionAddress();
            }
            return addressViewModel ?? new AddressViewModel() { Countries = GetCountries() };
        }

        // Delete the address on the basis of AddressId.   
        public virtual bool DeleteAddress(int? addressId, AddressViewModel viewModel, out string message)
        {
            bool status = false;
            message = WebStore_Resources.ErrorDeleteAddress;

            try
            {
                //Get user details from session.
                UserViewModel userDetails = GetUserViewModelFromSession();

                //If account id is greater than 0 delete account address else delete user address.
                if (userDetails?.AccountId.GetValueOrDefault() > 0)
                {
                    if (viewModel?.AccountAddressId > 0)
                        status = _accountClient.DeleteAccountAddress(new ParameterModel { Ids = string.Join(",", viewModel.AccountAddressId) });
                }
                else
                    status = _webStoreAccountClient.DeleteAddress(addressId, userDetails?.UserId);

                //if address is deleted set success message and clear cache.
                if (status)
                {
                    message = WebStore_Resources.SuccessDeleteAddress;
                    //Clear address from cache.
                    ClearAddressCache((userDetails?.UserId).GetValueOrDefault());

                    //Clear address from session.
                    ClearSessionAddress();
                }
                return status;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        message = WebStore_Resources.ErrorDeleteBillingAddress;
                        return status;
                    default:
                        message = WebStore_Resources.DeleteFailMessage;
                        return status;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = WebStore_Resources.DeleteFailMessage;
                return status;
            }
        }

        //Get order invoice details.
        public virtual OrdersListViewModel GetOrderInvoiceDetails(string orderIds)
        {
            int userId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();
            OrdersListModel ordersListModel = _orderClient.GetOrderDetailsForInvoice(new ParameterModel { Ids = orderIds.ToString() }, SetOrderInvoiceFilters(), userId > 0 ? new FilterCollection() { new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, userId.ToString()) } : null);
            return new OrdersListViewModel() { List = ordersListModel?.Orders?.ToViewModel<OrdersViewModel>().ToList() };
        }

        //Update user profile.
        public virtual UserViewModel UpdateProfile(UserViewModel model, bool webStoreUser)
        {
            UserViewModel userViewModel = GetUserViewModelFromSession();

            if (HelperUtility.IsNotNull(userViewModel))
                MapUserProfileData(model, userViewModel);
            try
            {
                _userClient.UpdateUserAccountData(userViewModel.ToModel<UserModel>(), webStoreUser);
                SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
                userViewModel.SuccessMessage = WebStore_Resources.SuccessProfileUpdated;
                userViewModel.HasError = false;
                Helper.ClearCache($"UserAccountAddressList{userViewModel.UserId}");
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (UserViewModel)GetViewModelWithErrorMessage(model, WebStore_Resources.ErrorUserNameAlreadyExists);
                    default:
                        return (UserViewModel)GetViewModelWithErrorMessage(userViewModel, WebStore_Resources.ErrorUpdateProfile);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (UserViewModel)GetViewModelWithErrorMessage(userViewModel, WebStore_Resources.ErrorUpdateProfile);
            }
            return userViewModel;
        }

        //Get orders list.
        public virtual OrdersListViewModel GetOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();
            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString());
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, GetUserViewModelFromSession()?.UserId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()));
            OrdersListModel orderList = _orderClient.GetOrderList(null, filters, sortCollection, pageIndex, recordPerPage);
            OrdersListViewModel ordersListViewModel = new OrdersListViewModel { List = orderList?.Orders?.ToViewModel<OrdersViewModel>().ToList() };
            ordersListViewModel.List?.ForEach(x => x.TotalWithCurrencyCode = (HelperMethods.FormatPriceWithCurrency((x.Total), PortalAgent.CurrentPortal.CultureCode)));
            SetListPagingData(ordersListViewModel, orderList);
            return ordersListViewModel?.List?.Count > 0 ? ordersListViewModel : new OrdersListViewModel();
        }

        //Get Order details on the basis of User's Order Number, Firstname and Lastname.
        public virtual OrdersViewModel GetOrderDetails(string orderNumber, string firstName, string lastName, string emailAddress)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeOmsOrderEnum.OrderNumber.ToString(), FilterOperators.Equals, orderNumber);
            filters.Add(ZnodeUserEnum.Email.ToString(), FilterOperators.Equals, emailAddress);
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());

            OrderModel orderModel = _orderClient.GetOrderById(0, SetExpandsForReceipt(), filters);
            if (HelperUtility.IsNotNull(orderModel) && orderModel.OmsOrderId > 0)
            {
                List<OrderLineItemModel> orderLineItemListModel = new List<OrderLineItemModel>();

                //Create new order line item model.
                CreateSingleOrderLineItem(orderModel, orderLineItemListModel);

                orderModel.OrderLineItems = orderLineItemListModel;
                if (orderModel?.OrderLineItems?.Count > 0)
                {
                    OrdersViewModel orderDetails = orderModel?.ToViewModel<OrdersViewModel>();
                    orderDetails.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                    orderDetails.CouponCode = orderDetails.CouponCode?.Replace("<br/>", ", ");
                    orderDetails?.OrderLineItems?.ForEach(x => x.UOM = orderModel?.ShoppingCartModel?.ShoppingCartItems?.Where(y => y.SKU == x.Sku).Select(y => y.UOM).FirstOrDefault());
                    RemoveCookie("OrderUserId_" + orderDetails.OrderNumber);
                    SaveInCookie("OrderUserId_"+ orderDetails.OrderNumber, Convert.ToString(orderDetails.UserId), ZnodeConstant.MinutesInAHour);
                    return orderDetails;
                }
            }

            return null;
        }

        //Create single order line item.
        public virtual void CreateSingleOrderLineItem(OrderModel orderModel, List<OrderLineItemModel> orderLineItemListModel)
        {
            List<OrderLineItemModel> childLineItems = orderModel.OrderLineItems.Where(oli => oli.ParentOmsOrderLineItemsId.HasValue).ToList();
            List<ShoppingCartItemModel> bundleLineItems = orderModel.ShoppingCartModel.ShoppingCartItems.Where(x => x.ProductType == ZnodeConstant.BundleProduct).ToList();
            foreach (ShoppingCartItemModel lineItem in bundleLineItems)
            {
                List<OrderLineItemModel> bundleLineItemsList = orderModel.OrderLineItems?.Where(x => x.Sku == lineItem?.SKU)?.ToList();
                if (IsNotNull(bundleLineItems) && bundleLineItems.Count > 0)
                    childLineItems?.AddRange(bundleLineItemsList);
            }
            foreach (OrderLineItemModel _childLineItem in childLineItems)
            {
                _childLineItem.Description = _childLineItem.Description;

                _childLineItem.PersonaliseValueList = orderModel.OrderLineItems.Where(oli => oli.OmsOrderLineItemsId == _childLineItem.ParentOmsOrderLineItemsId).FirstOrDefault()?.PersonaliseValueList;
                _childLineItem.ProductName = _childLineItem.ProductName;
                _childLineItem.Price = _childLineItem.Price;
                _childLineItem.Quantity = _childLineItem.Quantity;

                orderLineItemListModel.Add(_childLineItem);
            }

        }

        //Get dashboard data.
        public virtual UserViewModel GetDashboard()
        {
            //Get user model data from session.
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            try
            {
                if (IsNull(userViewModel))
                {
                    Logout();
                    return null;
                }

                if (IsNull(userViewModel.AddressList))
                {
                    userViewModel.IsAddressBook = true;
                }
                BindDashBoardData(userViewModel);

                SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);

                return userViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return userViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return userViewModel;
            }
        }

        public virtual AddressViewModel GetAddressByAddressType(int? addressId, string addressType = "")
        {
            AddressViewModel model = GetAddress(addressId);
            if (IsNotNull(model))
                model.States = GetStateListByCountryCode(model?.CountryName ?? model?.Countries?.FirstOrDefault().Value);

            return model;
        }

        //Get statelist on the basis of country code.
        public virtual List<SelectListItem> GetStateListByCountryCode(string countryCode)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.CountryCode, FilterOperators.Is, countryCode));
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));

            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodeStateEnum.StateName.ToString(), DynamicGridConstants.ASCKey);

            StateListModel stateListModel = _stateClient.GetStateList(filters, sorts);

            if (stateListModel?.States?.Count > 0)
                return stateListModel.States.Select(item => new SelectListItem() { Text = item?.StateName?.ToUpper(), Value = item?.StateCode }).ToList();

            return new List<SelectListItem>();
        }

        #region WishList
        //Add product to user wishlist.
        public virtual bool CreateWishList(string SKU, string AddOnProductSKUs)
        {
            UserViewModel userModel = GetUserViewModelFromSession();
            if (HelperUtility.IsNotNull(userModel))
            {
                //Checking SKU is Present in Publish Product List
                PublishProductListModel productList = _productClient.GetPublishProductList(null, GetRequiredFilters(), null, null, null, new ParameterKeyModel { Ids = SKU, ParameterKey = "SKU" });

                if (HelperUtility.IsNull(productList?.PublishProducts))
                {
                    return false;
                }
                //Check if product exist in wishlist.
                if (!CheckProductAlreadyExists(SKU, AddOnProductSKUs, userModel.UserId))
                {
                    try
                    {
                        WishListModel wishListModel = _wishListClient.AddToWishList(GetWishListModel(SKU, AddOnProductSKUs, userModel));
                        UpdateUserViewModel(wishListModel.ToViewModel<WishListViewModel>());
                        return wishListModel.UserWishListId > 0 ? true : false;
                    }
                    catch (ZnodeException ex)
                    {
                        ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                        return false;
                    }
                }
                else
                    return true;
            }
            return false;
        }

        //Get Wishlisted products for a user.
        public virtual WishListListViewModel GetWishLists()
        {
            //Getting wishlist data from user model kept in session.
            UserViewModel userModel = GetUserViewModelFromSession();
            if (HelperUtility.IsNotNull(userModel))
            {
                WishListListModel model = new WishListListModel();
                //Get skus of wishlist added products.             
                string skus = string.Join(",", userModel?.WishList?.Select(x => x.SKU));
                //Get the product data for selected skus.
                if (!string.IsNullOrEmpty(skus))
                    return GetWishListProductData(userModel, skus);
                else
                    return new WishListListViewModel();
            }
            return new WishListListViewModel();
        }

        //Delete wishlist.
        public virtual bool DeleteWishList(int wishListId)
        {
            if (_wishListClient.DeleteWishList(wishListId))
            {
                RemoveUserViewModel(new WishListViewModel() { UserWishListId = wishListId });
                return true;
            }
            return false;
        }

        //Get Current login user product review list.
        public virtual List<ProductReviewViewModel> GetProductReviewList()
        {
            //Get Login user details.
            UserViewModel currentUser = GetUserViewModelFromSession();

            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, Convert.ToString(currentUser?.UserId));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId));

            CustomerReviewListModel userReviews = _customerReviewClient.GetCustomerReviewList(Convert.ToString(PortalAgent.LocaleId), null, filters, null, null, null);

            return userReviews?.CustomerReviewList?.Count > 0 ? userReviews.CustomerReviewList.ToViewModel<ProductReviewViewModel>().ToList() : new List<ProductReviewViewModel>();
        }

        //Get list of Voucher history for a user.
        public virtual VoucherHistoryListViewModel GetVoucherHistoryList(int voucherId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            //Get Login user details.
            UserViewModel currentUser = GetUserViewModelFromSession();
            VoucherHistoryListViewModel voucherHistoryListViewModel = new VoucherHistoryListViewModel();
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodeGiftCardEnum.GiftCardId.ToString());

            filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, Convert.ToString(currentUser?.UserId));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add(ZnodeGiftCardEnum.GiftCardId.ToString(), FilterOperators.Equals, voucherId.ToString());

            GiftCardHistoryListModel giftCardHistoryList = _giftCardClient.GetGiftCardHistoryList(null, filters, sortCollection, pageIndex, recordPerPage);

            voucherHistoryListViewModel.List = giftCardHistoryList?.GiftCardHistoryList?.Count > 0 ? giftCardHistoryList.GiftCardHistoryList.ToViewModel<GiftCardHistoryViewModel>().ToList() : new List<GiftCardHistoryViewModel>();
            voucherHistoryListViewModel.voucherViewModel = IsNotNull(giftCardHistoryList?.GiftCard) ? giftCardHistoryList?.GiftCard.ToViewModel<VoucherViewModel>() : new VoucherViewModel();
            voucherHistoryListViewModel.voucherViewModel.UserId = currentUser?.UserId;
            voucherHistoryListViewModel.TotalResults = Convert.ToInt32(giftCardHistoryList?.TotalResults);
            return voucherHistoryListViewModel;
        }

        //Get list of Gift Card history for a user.
        public virtual VoucherListViewModel GetVouchers(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            //Get Login user details.
            UserViewModel currentUser = GetUserViewModelFromSession();
            VoucherListViewModel voucherListViewModel = new VoucherListViewModel();

            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.IsActive.ToString());
            filters.Add(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, Convert.ToString(currentUser.UserId));
            filters.Add(ZnodeUserEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.One);

            if (HelperUtility.IsNull(sortCollection))
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeGiftCardEnum.ExpirationDate.ToString(), DynamicGridConstants.DESCKey);
            }

            GiftCardListModel giftCardListModel = _giftCardClient.GetVouchers(null, filters, sortCollection, pageIndex, recordPerPage);

            voucherListViewModel.List = giftCardListModel?.GiftCardList?.Count > 0 ? giftCardListModel.GiftCardList.ToViewModel<VoucherViewModel>().ToList() : new List<VoucherViewModel>();
            voucherListViewModel.TotalResults = Convert.ToInt32(giftCardListModel?.TotalResults);
            return voucherListViewModel;
        }

        //Get Countries.
        public virtual List<SelectListItem> GetCountries(bool flag = true)
        => GetCountries();


        public virtual void ReordersingleLineOrderItem(int omsOrderLineItemsId)
        {
            UserViewModel currentUser = GetUserViewModelFromSession();
            bool response = _orderClient.ReorderSinglelineItemOrder(omsOrderLineItemsId, PortalAgent.CurrentPortal.PortalId, Convert.ToInt32(currentUser?.UserId));

            if (response)
            {
                _cartAgent.ClearCartCountFromSession();
                RemoveInSession(WebStoreConstants.CartModelSessionKey);
            }
        }

        public virtual void ReorderCompleteOrder(int orderId)
        {
            UserViewModel currentUser = GetUserViewModelFromSession();
            bool response = _orderClient.ReorderCompleteOrder(orderId, PortalAgent.CurrentPortal.PortalId, Convert.ToInt32(currentUser?.UserId));

            if (response)
            {
                _cartAgent.ClearCartCountFromSession();
                RemoveInSession(WebStoreConstants.CartModelSessionKey);
            }
        }


        //Get reorder items by order id.
        public virtual List<CartItemViewModel> GetReorderItems(int orderId)
        {
            List<CartItemViewModel> model = new List<CartItemViewModel>();
            OrderModel reorderItems = _orderClient.GetOrderById(orderId, SetExpandsForOrderDetails());

            var parentLineItemIds = reorderItems.OrderLineItems.Where(s => s.ParentOmsOrderLineItemsId.HasValue && s.OrderLineItemState != WebStoreConstants.Returned).Select(s => s.ParentOmsOrderLineItemsId.Value).Distinct().ToArray();
            foreach (int parentOrderLineItemId in parentLineItemIds)
            {
                reorderItems.OrderLineItems.Add(GetParentOrderLineItem(parentOrderLineItemId));
            }

            //Remove returned products from reorder cart line items.
            reorderItems.OrderLineItems = reorderItems?.OrderLineItems?.Where(x => x.OrderLineItemState.ToUpper() != ZnodeOrderStatusEnum.RETURNED.ToString())?.ToList();
            if (reorderItems?.OrderLineItems?.Count > 0)
            {
                //Take out the Group/Configurable products and their partent product SKU.  Once done remove those products from the list.
                model = AddGroupedOrConfigurableProductsInCart(ref reorderItems);

                //Iterate on each product of order line.
                foreach (OrderLineItemModel orderLineItem in reorderItems.OrderLineItems)
                    CheckProductPriceForCompleteReorder(model, reorderItems, orderLineItem);
            }

            return model.OrderBy(o => o.OmsOrderLineItemsId).ToList();
        }

        //Get order details to generate the order receipt.
        public virtual OrdersViewModel GetOrderDetails(int orderId, int portalId = 0)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());

            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            BindUserApproverData(userViewModel);
            ImpersonationModel impersonationUserViewModel = GetFromSession<ImpersonationModel>(WebStoreConstants.ImpersonationSessionKey);

            if (HelperUtility.IsNotNull(userViewModel) && orderId > 0)
            {
                //If impersonation login is true then don't add userid to filter,user id will be 0
                if(HelperUtility.IsNull(impersonationUserViewModel) || !impersonationUserViewModel.IsImpersonation)
                {
                    filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, GetUserViewModelFromSession()?.UserId.ToString()));
                }

                if(userViewModel.IsApprover)
                {
                    filters.Add(new FilterTuple(ZnodeConstant.IsApprover, FilterOperators.Equals, userViewModel?.IsApprover.ToString()));
                }

                //ToDo:Need to check the user roles on api side 
                if (!string.IsNullOrEmpty(userViewModel?.RoleName) && userViewModel.RoleName.Equals(ZnodeConstant.AdministratorRole, StringComparison.InvariantCultureIgnoreCase))
                {
                    filters.Add(new FilterTuple(ZnodeConstant.RoleName, FilterOperators.Equals, userViewModel?.RoleName.ToString()));
                }

                OrderModel orderModel = portalId > 0 ? GetOrderBasedOnPortalId(orderId, filters) : _orderClient.GetOrderById(orderId, SetExpandsForReceipt(), filters);

                if (orderModel != null && (orderModel.OmsOrderId > 0 || userViewModel.UserId == orderModel.UserId || (orderModel.IsQuoteOrder || userViewModel?.AccountId.GetValueOrDefault() > 0)))
                {
                    List<OrderLineItemModel> orderLineItemListModel = new List<OrderLineItemModel>();

                    //Create new order line item model.
                    CreateSingleOrderLineItem(orderModel, orderLineItemListModel);
                    orderModel.OrderLineItems = orderLineItemListModel;
                    orderModel.TrackingNumber = SetTrackingUrl(orderModel.TrackingNumber, orderModel.ShippingId);
                    if (orderModel?.OrderLineItems?.Count > 0)
                    {
                        OrdersViewModel orderDetails = orderModel?.ToViewModel<OrdersViewModel>();
                        orderDetails.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                        orderDetails.CouponCode = orderDetails.CouponCode?.Replace("<br/>", ", ");
                        orderDetails?.OrderLineItems?.ForEach(x => x.UOM = orderModel?.ShoppingCartModel?.ShoppingCartItems?.Where(y => y.SKU == x.Sku).Select(y => y.UOM).FirstOrDefault());
                        orderDetails?.OrderLineItems?.ForEach(x => x.TrackingNumber = SetTrackingUrl(x.TrackingNumber, orderModel.ShippingId));
                        orderDetails.IsOrderEligibleForReturn = DependencyResolver.Current.GetService<IRMAReturnAgent>()?.IsOrderEligibleForReturn(orderDetails?.OrderNumber) ?? false;
                        orderDetails.RoleName = userViewModel.RoleName;
                        orderDetails.CustomerPaymentGUID = userViewModel.CustomerPaymentGUID;
                        return orderDetails;
                    }
                }
                return null;
            }
            return new OrdersViewModel();
        }

        //Reorder single product from an order.
        public virtual CartItemViewModel GetOrderByOrderLineItemId(int orderLineItemId)
        {
            CartItemViewModel model = new CartItemViewModel();
            OrderModel reorderItems = _orderClient.GetOrderByOrderLineItemId(orderLineItemId, SetExpandsForReceipt());

            if (reorderItems?.OrderLineItems?.Count > 0)
            {
                //Iterate on each product of order line.
                foreach (OrderLineItemModel orderLineItem in reorderItems.OrderLineItems)
                {
                    //If price is not associated to the product, then don't add it to the cart.
                    OrderLineItemModel parentOrderLineItem = new OrderLineItemModel();
                    if (orderLineItem.ParentOmsOrderLineItemsId.HasValue)
                    {
                        parentOrderLineItem = GetParentOrderLineItem(orderLineItem.ParentOmsOrderLineItemsId.Value);
                        CheckProductPrice(model, reorderItems, parentOrderLineItem);
                    }
                    CheckProductPrice(model, reorderItems, orderLineItem);
                }
            }
            return model;
        }

        //Get Account Information
        public virtual AccountViewModel GetAccountInformation()
        {
            AccountViewModel accountViewModel = new AccountViewModel();

            //Get user account details.
            UserViewModel userViewModel = GetUserViewModelFromSession();
            if (HelperUtility.IsNotNull(userViewModel))
            {
                accountViewModel.AccountId = userViewModel.AccountId.GetValueOrDefault();

                //Get account details by account id.
                AccountModel accountModel = _accountClient.GetAccount(accountViewModel.AccountId);
                if (HelperUtility.IsNotNull(accountModel))
                    accountViewModel = accountModel.ToViewModel<AccountViewModel>();

                //Bind user details to AccountViewModel
                BindUserDetails(accountViewModel, userViewModel);

                //If role name is administrator bind country list to dropdown to update account info.
                if (Equals(accountViewModel.RoleName, ZnodeRoleEnum.Administrator.ToString()))
                    accountViewModel.CountryList = GetCountries();
            }
            return accountViewModel;
        }

        //Update account information.
        public virtual AddressViewModel UpdateAccountInformation(AddressViewModel addressViewModel)
        {
            try
            {
                if (HelperUtility.IsNotNull(addressViewModel))
                {
                    AddressViewModel updatedAddressModel = _accountClient.UpdateAccountAddress(addressViewModel.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                    if (HelperUtility.IsNotNull(updatedAddressModel) && updatedAddressModel.AddressId > 0)
                    {
                        int userId = (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();
                        if (userId > 0)
                            //Clear address from cache.
                            Helper.ClearCache($"{WebStoreConstants.UserAccountAddressList}{userId}");

                        return updatedAddressModel;
                    }
                }

                return new AddressViewModel() { HasError = true };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion

        //Save referral user id in session
        public virtual void SetAffiliateId(string affiliateId)
           => SaveInSession(WebStoreConstants.AffiliateIdSessionKey, affiliateId);

        // Get default billing address details.
        public virtual AddressListViewModel GetBillingAddressDetail(int billingAddressId, int shippingAddressId)
        {
            AddressListViewModel listViewModel = new ViewModels.AddressListViewModel();
            AddressListViewModel addressListViewModel = GetAddressList();
            UserViewModel model = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            AddressViewModel addressViewModel = new AddressViewModel();
            if (IsNotNull(addressListViewModel))
            {
                if (addressListViewModel.AddressList.Count > 0)
                {
                    addressViewModel = addressListViewModel?.AddressList?.Where(w => w.AddressId == billingAddressId && w.IsDefaultBilling == true)?.FirstOrDefault() ?? new AddressViewModel();

                    addressViewModel.EmailAddress = model?.Email;
                    listViewModel.BillingAddress = addressViewModel;

                    addressViewModel = new ViewModels.AddressViewModel();
                    addressViewModel = addressListViewModel?.AddressList?.Where(w => w.AddressId == shippingAddressId && w.IsDefaultShipping == true)?.FirstOrDefault() ?? new AddressViewModel();

                    addressViewModel.EmailAddress = model?.Email;
                    listViewModel.ShippingAddress = addressViewModel;
                }
                else
                {
                    if (addressListViewModel.ShippingAddress.UseSameAsShippingAddress)
                    {
                        addressViewModel.EmailAddress = addressListViewModel.ShippingAddress.EmailAddress;
                        addressViewModel = addressListViewModel.ShippingAddress;
                        listViewModel.BillingAddress = addressViewModel;

                        addressViewModel = new AddressViewModel();
                        addressViewModel.EmailAddress = addressListViewModel.ShippingAddress.EmailAddress;
                        addressViewModel = addressListViewModel.ShippingAddress;

                        listViewModel.ShippingAddress = addressViewModel;
                    }
                    else
                    {
                        addressViewModel.EmailAddress = addressListViewModel.BillingAddress.EmailAddress;
                        addressViewModel = addressListViewModel.BillingAddress;
                        listViewModel.BillingAddress = addressViewModel;
                    }

                }
                return listViewModel;
            }
            return new AddressListViewModel();
        }
        //Remove guest user details from session.
        public virtual void RemoveGuestUserSession()
        {
            // Get user from Session.
            UserViewModel model = GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
            if (HelperUtility.IsNotNull(model))
                RemoveInSession(WebStoreConstants.GuestUserKey);
            UserViewModel usermodel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (HelperUtility.IsNotNull(usermodel))
                RemoveInSession(WebStoreConstants.UserAccountKey);

            RemoveInSession(WebStoreConstants.GuestUserAddressListKey);
        }

        //Get user view model from session.
        public virtual UserViewModel GetUserViewModelFromSession() => IsUserAuthenticated() ? GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) : GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);

        //Get recommended address using USPS service
        public virtual AddressListViewModel GetRecommendedAddress(AddressViewModel addressViewModel)
        {
            if (PortalAgent.CurrentPortal.EnableAddressValidation)
            {
                AddressModel model = addressViewModel.ToModel<AddressModel>();
                model.PortalId = PortalAgent.CurrentPortal.PortalId;
                model.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;
                AddressListModel listModel = _shippingClient.RecommendedAddress(model);
                return new AddressListViewModel { AddressList = listModel.AddressList.ToViewModel<AddressViewModel>().ToList() };
            }
            return new AddressListViewModel { AddressList = new List<AddressViewModel> { addressViewModel } };
        }

        // Get states by country code.
        public virtual List<SelectListItem> GetStates(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {

                string cacheKey = string.Concat("StateList", Convert.ToString(PortalAgent.CurrentPortal.PortalId), countryCode);

                if (IsNull(HttpContext.Current.Cache[cacheKey]))
                {
                    FilterCollection _filter = new FilterCollection();
                    _filter.Add(ZnodeStateEnum.CountryCode.ToString(), FilterOperators.Is, countryCode);
                    _filter.Add(ZnodeStateEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

                    SortCollection sort = new SortCollection();
                    sort.Add(ZnodeStateEnum.StateName.ToString(), DynamicGridConstants.ASCKey);

                    StateListModel lstState = new StateListModel();
                    lstState = _stateClient.GetStateList(_filter, sort);
                    
                    if (IsNotNull(lstState.States))
                    {
                        List<SelectListItem> states = (from c in lstState?.States
                                                       select new SelectListItem
                                                       {
                                                           Text = c.StateName,
                                                           Value = c.StateCode
                                                       }).ToList();

                        // Get Profile based options and merge with All Profile options.
                        if (states?.Count > 0)
                            Helper.AddIntoCache(states, cacheKey, "StateListCacheDuration");
                    
                    } 

                }

                List<SelectListItem> stateList = Helper.GetFromCache<List<SelectListItem>>(cacheKey);

                return stateList;
            }
            return new List<SelectListItem>();
        }

        public virtual AddressListViewModel GetshippingBillingAddress() => GetAddressList();

        //Get user account information by userID.
        public virtual UserViewModel GetUserAccountData(int userId, int portalId = 0)
        {
            return _userClient.GetUserAccountData(userId, portalId)?.ToViewModel<UserViewModel>();
        }

        //to set login user profile Id
        public virtual void SetLoginUserProfile(UserModel userModel)
        {
            UserViewModel currentUser = GetUserViewModelFromSession();
            if (currentUser?.Profiles.Count > 0)
                userModel.ProfileId = Helper.GetProfileId().GetValueOrDefault();
            else
                userModel.ProfileId = userModel?.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.ProfileId ?? 0;
        }

        #region Quote History
        //Get quote history of login customer.
        public virtual AccountQuoteListViewModel GetAccountQuoteList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, bool isPendingPayment = false)
        {
            //Check if filter has QuoteOrderTotal, convert filter value from string to decimal.
            CheckFilterHasQuoteOrderTotal(filters);

            //Get user details from session.
            UserViewModel userDetails = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            int userId = Convert.ToInt32(userDetails?.UserId);
            int accountId = Convert.ToInt32(userDetails?.AccountId);
            //Set filters for UserId.
            SetUserIdFilter(filters, userId, accountId, isPendingPayment);
            AccountQuoteListModel accountQuoteListModel = _accountQuoteClient.GetAccountQuoteList(null, filters, sortCollection, pageIndex, recordPerPage);
            AccountQuoteListViewModel accountQuoteListViewModel = new AccountQuoteListViewModel { AccountQuotes = accountQuoteListModel?.AccountQuotes?.ToViewModel<AccountQuoteViewModel>()?.ToList() };
            SetListPagingData(accountQuoteListViewModel, accountQuoteListModel);

            //If quote list count is greater than 0, bind required data.
            if (accountQuoteListViewModel.AccountQuotes?.Count > 0)
            {
                //Set Quote tool menus for Administrator and Manager only.
                if (string.Equals(userDetails?.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(userDetails?.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    SetQuotesToolMenus(accountQuoteListViewModel);

                //Get Order State List.
                accountQuoteListViewModel.OrderStatusList = GetOrderStateList();
                accountQuoteListViewModel.UserId = userId;
                return accountQuoteListViewModel;
            }
            return new AccountQuoteListViewModel { AccountQuotes = new List<AccountQuoteViewModel>(), UserId = userId };
        }

        //Update the Account Quote Details.
        public virtual bool UpdateQuoteStatus(string quoteId, int status)
        {
            if (!string.IsNullOrEmpty(quoteId) && status > 0)
                return _accountQuoteClient.UpdateQuoteStatus(new QuoteStatusModel() { OmsQuoteIds = quoteId, OmsOrderStateId = status, LocaleId = PortalAgent.LocaleId })?.IsUpdated ?? false;

            return false;
        }

        //Get Quote View by omsQuoteId.
        public virtual AccountQuoteViewModel GetQuoteView(int omsQuoteId, bool IsQuoteLineItemUpdated = false)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());

            //If IsQuoteLineItemUpdated is true, add filter.
            if (IsQuoteLineItemUpdated)
                filters.Add(FilterKeys.IsQuoteLineItemUpdated, FilterOperators.Equals, ZnodeConstant.TrueValue);

            //Get account quote.
            _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            AccountQuoteModel accountQuoteModel = _accountQuoteClient.GetAccountQuote(new ExpandCollection() { ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString() }, filters);

            UserViewModel userDetails = new UserViewModel();
            if (accountQuoteModel.UserId == GetUserId())
            {
                //Get user details. 
                userDetails = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            }
            else
                userDetails = _userClient.GetUserAccountData(accountQuoteModel.UserId, new ExpandCollection { ExpandKeys.Profiles }).ToViewModel<UserViewModel>();
            if (HelperUtility.IsNotNull(accountQuoteModel))
                //Bind user details to AccountQuoteViewModel.
                return BindDataToAccountQuoteViewModel(userDetails, accountQuoteModel);
            else
                return new AccountQuoteViewModel()
                {
                    RoleName = userDetails?.RoleName,
                    PermissionCode = userDetails?.PermissionCode,
                    CartItemCount = Convert.ToString(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems.Count()) ?? GetFromCookie(WebStoreConstants.CartCookieKey),
                    LoggedInUserId = GetUserId(),
                    IsLastApprover = _accountQuoteClient.IsLastApprover(accountQuoteModel.OmsQuoteId),
                    PaymentDisplayName = accountQuoteModel.PaymentDisplayName,
                };
        }

        //Update the Account Quote Details.
        public virtual bool UpdateQuoteStatus(AccountQuoteViewModel accountQuoteViewModel)
        {
            try
            {
                return _accountQuoteClient.UpdateQuoteStatus(new QuoteStatusModel() { OmsQuoteIds = accountQuoteViewModel.OmsQuoteId.ToString(), OmsOrderStateId = accountQuoteViewModel.OmsOrderStateId, OrderStatus = accountQuoteViewModel.OrderStatus, LocaleId = PortalAgent.LocaleId, Comments = accountQuoteViewModel.Comments })?.IsUpdated ?? false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Update Quote Line Item Quantity.
        public virtual bool UpdateQuoteLineItemQuantity(CartItemViewModel cartItemViewModel)
        {
            try
            {
                AccountQuoteLineItemViewModel accountQuoteLineItemViewModel = new AccountQuoteLineItemViewModel();

                //Map CartItemViewModel to AccountQuoteLineItemModel.
                ToQuoteLineItem(accountQuoteLineItemViewModel, cartItemViewModel);
                return _accountQuoteClient.UpdateQuoteLineItemQuantity(accountQuoteLineItemViewModel?.ToModel<AccountQuoteLineItemModel>())?.OmsQuoteLineItemId > 0;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Delete Quote Line Item.
        public virtual bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId = 0)
        {
            try
            {
                return _accountQuoteClient.DeleteQuoteLineItem(omsQuoteLineItemId, omsQuoteId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Add quote line items to shopping cart.
        public virtual bool AddQuoteToCart(AccountQuoteViewModel accountQuoteViewModel)
        {
            try
            {
                //Get Quote of current user.
                _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                AccountQuoteModel accountQuoteModel = _accountQuoteClient.GetAccountQuote(new ExpandCollection() { ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString() }, new FilterCollection() { new FilterTuple(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, accountQuoteViewModel.OmsQuoteId.ToString()) });

                //If quote is not null add quote line items to shopping cart.
                if (HelperUtility.IsNotNull(accountQuoteModel))
                {
                    accountQuoteViewModel = accountQuoteModel.ToViewModel<AccountQuoteViewModel>();
                    if (accountQuoteViewModel?.ShoppingCart?.ShoppingCartItems?.Count > 0)
                    {
                        ICartAgent _cartAgent = new CartAgent(GetClient<ShoppingCartClient>(), GetClient<PublishProductClient>(), GetClient<AccountQuoteClient>(), GetClient<UserClient>());

                        //Add items into cart.
                        foreach (CartItemViewModel cart in accountQuoteViewModel.ShoppingCart.ShoppingCartItems)
                        {
                            //Add items into cart if only item is in stock or back order is allowed.
                            if (!cart.InsufficientQuantity)
                            {
                                //Bind required data from AccountQuoteViewModel to CartLineItem.
                                BindDataToCartLineItems(cart, accountQuoteViewModel);
                                _cartAgent.CreateCart(cart);
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Create quote.
        public virtual bool CreateQuote(SubmitQuoteViewModel submitQuoteViewModel, out string message)
        {
            message = string.Empty;

            //Get the cart from session.
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                                       _cartAgent.GetCartFromCookie();

            if (HelperUtility.IsNotNull(cartModel))
            {
                //Set user details.
                SetUserDetails(cartModel, submitQuoteViewModel.AdditionalNotes);
                submitQuoteViewModel.UserId = cartModel.UserId.GetValueOrDefault();

                //Set billing/shipping address.
                SetBillingShippingAddress(cartModel, submitQuoteViewModel.ShippingId);

                cartModel.OmsQuoteId = submitQuoteViewModel?.QuoteId > 0 ? submitQuoteViewModel.QuoteId : cartModel.OmsQuoteId;

                cartModel.OrderStatus = submitQuoteViewModel.OmsOrderState;
                cartModel.QuotePaymentSettingId = submitQuoteViewModel.PaymentSettingId;
                cartModel.IsPendingPayment = submitQuoteViewModel.IsPendingPayment;
                cartModel.CardType = submitQuoteViewModel.CardType;
                cartModel.CreditCardExpMonth = submitQuoteViewModel.CreditCardExpMonth;
                cartModel.CreditCardExpYear = submitQuoteViewModel.CreditCardExpYear;
                cartModel.PODocumentName = !string.IsNullOrEmpty(submitQuoteViewModel.PODocumentName) ? $"{WebStoreConstants.PODocumentPath}/{submitQuoteViewModel.PODocumentName}" : null;
                cartModel.PurchaseOrderNumber = submitQuoteViewModel.PurchaseOrderNumber;
                cartModel.CreditCardNumber = submitQuoteViewModel.CreditCardNumber;
                cartModel.CustomerServiceEmail = PortalAgent.CurrentPortal.CustomerServiceEmail;
                cartModel.PortalPaymentGroupId = submitQuoteViewModel.PortalPaymentGroupId;
                cartModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;
                cartModel.InHandDate = submitQuoteViewModel.InHandDate;
                cartModel.ShippingConstraintCode = submitQuoteViewModel.ShippingConstraintCode;
                cartModel.JobName = submitQuoteViewModel.JobName;
                cartModel.AdditionalInstructions = submitQuoteViewModel.AdditionalInstruction;
                cartModel.Shipping.ShippingCode = submitQuoteViewModel.ShippingOptionCode;
                SetUsersPaymentDetails(Convert.ToInt32(submitQuoteViewModel.PaymentSettingId), cartModel);
                cartModel.Shipping.AccountNumber= submitQuoteViewModel.AccountNumber;
                cartModel.Shipping.ShippingMethod = submitQuoteViewModel.ShippingMethod;
                //Condition for checking Available Quantity
                if (GetAvailableQuantity(cartModel))
                {
                    message = WebStore_Resources.ExceedingAvailableWithoutQuantity;
                    return false;
                }

                bool isCreditCardPayment = false;
                // Condition for "Credit Card" payment.
                if (IsNotNull(cartModel?.Payment) && Equals(cartModel.Payment.PaymentName.ToLower(), ZnodeConstant.CreditCard.ToLower()))
                {
                    isCreditCardPayment = true;
                    submitQuoteViewModel = ProcessCreditCardPayment(submitQuoteViewModel, cartModel);
                    if (submitQuoteViewModel.HasError)
                        return false;
                }

                cartModel.Token = !string.IsNullOrEmpty(submitQuoteViewModel.PaymentToken) && submitQuoteViewModel.IsFromPayPalExpress ? submitQuoteViewModel.PaymentToken : cartModel.Token;
                if (submitQuoteViewModel.IsFromAmazonPay)
                    cartModel.Token = submitQuoteViewModel.PaymentToken;
                try
                {
                    SetQuoteType(cartModel);
                    //Create Quote.
                    _accountQuoteClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                    AccountQuoteModel createdQuote = _accountQuoteClient.Create(cartModel);
                    submitQuoteViewModel.QuoteId = createdQuote.OmsQuoteId;
                    if (IsNotNull(createdQuote))
                    {
                        ClearUserDashboardPendingOrderDetailsCountFromSession(createdQuote.UserId);
                    }
                    if (HelperUtility.IsNull(createdQuote))
                    {
                        message = Admin_Resources.ErrorSubmitQuote;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(submitQuoteViewModel.OldOrderStatus))
                    {
                        _cartAgent.RemoveAllCartItems();
                        UpdateUserDetailsInSession(submitQuoteViewModel.Total);
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToCreate;
                    return false;
                }
            }
            ClearAddressCache(Convert.ToInt32(cartModel.UserId));
            return true;
        }

        //Get the list of user approvers.
        public virtual UserApproverListViewModel GetUserApproverList(int omsQuoteId, bool showAllApprovers)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));
            filters.Add(new FilterTuple(ZnodeOmsTemplateEnum.UserId.ToString(), FilterOperators.Equals, GetUserViewModelFromSession()?.UserId.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.ShowAllApprovers.ToString().ToLower(), FilterOperators.Equals, showAllApprovers.ToString()));

            UserApproverListModel listModel = _accountQuoteClient.GetUserApproverList(null, filters, null, null, null);
            return new UserApproverListViewModel { UserApprover = listModel?.UserApprovers?.ToViewModel<UserApproverViewModel>()?.ToList() };
        }

        //Get the address details.
        public virtual AddressViewModel GetAddressDetail(AddressViewModel addressViewModel)
        {
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (IsNotNull(addressViewModel) && IsNotNull(userViewModel))
            {
                addressViewModel.AddressCount = IsNotNull(userViewModel?.Addresses) ? userViewModel.Addresses.Count() : 0;
                addressViewModel.Countries = GetCountries();
            }
            else
                addressViewModel = GetAddress(addressViewModel.AddressId);

            return addressViewModel;
        }

        // Convert Quote to Order.
        public virtual OrdersViewModel ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel)
        {
            try
            {
                if (IsNotNull(accountQuoteViewModel))
                {
                    accountQuoteViewModel.CurrencyCode = DefaultSettingHelper.DefaultCurrency;
                    accountQuoteViewModel.CultureCode = DefaultSettingHelper.DefaultCulture;

                    OrdersViewModel ordersViewModel = _accountQuoteClient.ConvertToOrder(accountQuoteViewModel.ToModel<AccountQuoteModel>())?.ToViewModel<OrdersViewModel>();
                    WebstoreHelper helper = new WebStore.WebstoreHelper();
                    RemoveCookie(WebStoreConstants.UserOrderReceiptOrderId);
                    CookieHelper.SetCookie(WebStoreConstants.UserOrderReceiptOrderId, Convert.ToString(ordersViewModel.OmsOrderId), 60);
                    return ordersViewModel;

                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //To remove the cart count key from session.
        public virtual void ClearUserDashboardPendingOrderDetailsCountFromSession(int userId)
        {
            RemoveInSession(ZnodeConstant.PendingQuotesKey + userId);
        }
        #endregion

        #region Template
        //Check whether the payment name already exists.
        public virtual bool IsTemplateNameExist(string templateName, int omsTemplateId = 0)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                templateName = templateName.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsTemplateEnum.TemplateName.ToString(), FilterOperators.Contains, templateName));
                filters.Add(new FilterTuple(ZnodeOmsTemplateEnum.UserId.ToString(), FilterOperators.Equals, GetUserViewModelFromSession()?.UserId.ToString()));

                //Get the payment list based on the payment name filter.
                AccountTemplateListModel paymentList = _accountQuoteClient.GetTemplateList(null, filters, null, null, null);
                if (paymentList?.AccountTemplates?.Count > 0)
                {
                    if (omsTemplateId > 0)
                        //Set the status in case the payment is open in edit mode.
                        paymentList.AccountTemplates.RemoveAll(x => x.OmsTemplateId == omsTemplateId);

                    return paymentList.AccountTemplates.FindIndex(x => string.Equals(x.TemplateName, templateName, StringComparison.CurrentCultureIgnoreCase)) != -1;
                }
            }
            return false;
        }

        public virtual bool ValidateUserBudget(out string message)
        {
            message = string.Empty;
            bool status = true;

            //Get the cart from session.
            ShoppingCartModel cartModel = GetCloneFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                                       _cartAgent.GetCartFromCookie();
            UserViewModel userViewModel;
            //Get user details from session.
            //CartModel.UserId will be 0 while placing order with guest.
            if (cartModel.UserId < 1 || cartModel.UserId == GetUserId())
            {
                //Get user details. 
                userViewModel = GetCloneFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            }
            else
                userViewModel = _userClient.GetUserAccountData(cartModel.UserId ?? 0, new ExpandCollection { ExpandKeys.Profiles }).ToViewModel<UserViewModel>();

            //Code to check the per order and annual limit.
            bool enablePerOrderlimit = Convert.ToBoolean(PortalAgent.CurrentPortal?.GlobalAttributes?.Attributes?.FirstOrDefault(x => x.AttributeCode == WebStoreConstants.EnableUserOrderLimit)?.AttributeValue);
            bool enableUserOrderAnnualLimit = Convert.ToBoolean(PortalAgent.CurrentPortal?.GlobalAttributes?.Attributes?.FirstOrDefault(x => x.AttributeCode == WebStoreConstants.EnableUserOrderAnnualLimit)?.AttributeValue);
            string cultureCode = PortalAgent.CurrentPortal.CultureCode;

            if (!ValidateUserPerOrderBudget(userViewModel, cartModel.Total, enablePerOrderlimit))
            {
                message = $"{WebStore_Resources.PerOrderLimitFailed}{HelperMethods.FormatPriceWithCurrency(userViewModel.PerOrderLimit, cultureCode)}.";
                status = false;
            }

            if (!ValidateUserAnnualBudget(userViewModel, cartModel.Total, enableUserOrderAnnualLimit))
            {
                message = $"{WebStore_Resources.AnnualOrderLimitFailed} {HelperMethods.FormatPriceWithCurrency(userViewModel.AnnualOrderLimit, cultureCode)}.";
                status = false;
            }
            return status;
        }
        #endregion

        //Get Account Side Menus 
        public virtual UserViewModel GetAccountMenus()
        {
            //Get user model data from session.
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            try
            {
                if (IsNull(userViewModel))
                {
                    Logout();
                    return null;
                }

                GetUserDashbaordPendingOrderDetailsCount(userViewModel);

                SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);

                return userViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                return userViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                return userViewModel;
            }
        }

        //Get Account Orders
        public virtual OrdersListViewModel GetAccountUserOrderList(int accountId, int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            string updatePageType = WebStoreConstants.ZnodeWebStoreOrder;
            ZnodeLogging.LogMessage("updatePageType:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new { updatePageType = updatePageType });
            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            if (userId > 0)
                SetUserIdFilter(filters, userId);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });

            OrdersListModel orderList = _userClient.GetAccountUserOrderList(accountId, null, filters, sortCollection, pageIndex, recordPerPage);
            if (orderList?.Orders?.Count() > 0)
            {
                foreach (var item in orderList.Orders)
                {
                    //assign orderamount to total
                    item.UpdatePageType = updatePageType;
                    item.AccountId = accountId;
                    item.UserId = userId > 0 ? userId : item.UserId;
                    item.OrderTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(item.OrderAmount, item.CultureCode);
                }
                return BindDataToOrderListViewModel(orderList, accountId, userId);
            }

            return new OrdersListViewModel() { AccountName = GetAccountById(accountId)?.CompanyAccount?.Name, HasParentAccounts = (orderList?.HasParentAccounts).GetValueOrDefault() };
        }
        #endregion

        #region protected Method
        //Clear address from cache.
        protected virtual void ClearAddressCache(int userId) => Helper.ClearCache($"{WebStoreConstants.UserAccountAddressList}{userId}");

        //Get User address list and add into cache.
        protected virtual void GetUserAddressList(AddressListViewModel addressViewModel, string cacheKey, int accountId)
        {
            FilterCollection filters = new FilterCollection();
            if (accountId > 0)
            {
                //Get Address List of logged in account user.
                filters.Add(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString());
                addressViewModel.AddressList = _accountClient.GetAddressList(new ExpandCollection() { ZnodeAccountAddressEnum.ZnodeAddress.ToString() }, filters, null, null, null)?.AddressList?.ToViewModel<AddressViewModel>()?.ToList();
            }
            else
            {
                //Get Address List of logged in user.
                filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, addressViewModel.UserId.ToString()));
                addressViewModel.AddressList = _webStoreAccountClient.GetUserAddressList(new ExpandCollection() { ZnodeAccountAddressEnum.ZnodeAddress.ToString(), ZnodeUserAddressEnum.ZnodeUser.ToString() }, filters)?.ToViewModel<AddressViewModel>().ToList();
            }
            addressViewModel.BillingAddress = addressViewModel.AddressList?.FirstOrDefault(x => x.IsDefaultBilling);
            addressViewModel.ShippingAddress = addressViewModel.AddressList?.FirstOrDefault(x => x.IsDefaultShipping);

            if (HelperUtility.IsNotNull(addressViewModel.AddressList))
            {
                Helper.AddIntoCache(addressViewModel, cacheKey, "CurrentPortalCacheDuration");

                //Save created/updated address in session.
                SaveAddressInSession(addressViewModel);
            }
        }

        //Bind user details to AccountQuoteViewModel.
        protected virtual AccountQuoteViewModel BindDataToAccountQuoteViewModel(UserViewModel userDetails, AccountQuoteModel accountQuoteModel)
        {
            AccountQuoteViewModel accountQuoteViewModel = accountQuoteModel?.ToViewModel<AccountQuoteViewModel>();
            accountQuoteViewModel.CurrencyCode = DefaultSettingHelper.DefaultCurrency;
            accountQuoteViewModel.CultureCode = DefaultSettingHelper.DefaultCulture;
            accountQuoteViewModel.RoleName = userDetails?.RoleName;
            accountQuoteViewModel.PermissionCode = userDetails?.PermissionCode;
            accountQuoteViewModel.CartItemCount = Convert.ToString(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.ShoppingCartItems.Count()) ??
                      GetFromCookie(WebStoreConstants.CartCookieKey);
            PortalViewModel portal = PortalAgent.CurrentPortal;
            accountQuoteViewModel.CurrencyCode = portal?.CurrencyCode;

            foreach (AccountQuoteLineItemViewModel quoteLineItem in accountQuoteViewModel.AccountQuoteLineItemList)
            {
                quoteLineItem.PersonaliseValuesDetail = accountQuoteModel.ShoppingCart?.ShoppingCartItems?.FirstOrDefault(x => x.OmsQuoteLineItemId == quoteLineItem?.OmsQuoteLineItemId)?.PersonaliseValuesDetail;                
            }               

            if (accountQuoteModel.ShoppingCart?.ShoppingCartItems?.Count() > 0)
            {
                //If inventory is out of stock set error message.
                if (accountQuoteModel.ShoppingCart.ShoppingCartItems.Any(x => x.InsufficientQuantity))
                    accountQuoteViewModel.OutOfStockMessage = portal?.OutOfStockMessage;
            }
            accountQuoteViewModel.LoggedInUserId = GetUserId();
            accountQuoteViewModel.IsLastApprover = _accountQuoteClient.IsLastApprover(accountQuoteModel.OmsQuoteId);
            accountQuoteModel.ShoppingCart.Shipping.ShippingName = accountQuoteModel.ShippingName;
            return accountQuoteViewModel;
        }

        //Set user details.
        protected virtual void SetUserDetails(ShoppingCartModel model, string additionalNotes)
        {
            model.UserDetails = new UserModel();
            model.UserDetails = _userClient.GetUserAccountData(model.UserId.GetValueOrDefault(), new ExpandCollection { ExpandKeys.Profiles });
            if (HelperUtility.IsNotNull(model.UserDetails))
            {
                model.UserDetails.UserId = model.UserId.GetValueOrDefault();
                model.UserDetails.ProfileId = (model.UserDetails.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId).GetValueOrDefault();
                model.ProfileId = model.UserDetails.ProfileId;
                model.AdditionalInstructions = model.AdditionalInstructions;
                model.AdditionalNotes = additionalNotes;
                model.QuotesAccountId = model.UserDetails?.AccountId.GetValueOrDefault();
            }
        }

        //Set billing / shipping address.
        protected virtual void SetBillingShippingAddress(ShoppingCartModel cartModel, int shippingId)
        {
            if (HelperUtility.IsNull(cartModel.BillingAddress))
                cartModel.BillingAddress = new AddressModel() { AddressId = (cartModel?.ShippingAddress?.AddressId).GetValueOrDefault() };

            cartModel.ShippingAddressId = (cartModel.ShippingAddress?.AddressId).GetValueOrDefault() > 0 ? cartModel.ShippingAddress.AddressId : cartModel.ShippingAddressId;
            cartModel.BillingAddressId = (cartModel.BillingAddress?.AddressId).GetValueOrDefault() > 0 ? cartModel.BillingAddress.AddressId : cartModel.BillingAddressId;
            cartModel.ShippingId = shippingId > 0 ? shippingId : cartModel.ShippingId;
        }

        //Check if filter has QuoteOrderTotal, convert filter value from string to decimal.
        protected virtual void CheckFilterHasQuoteOrderTotal(FilterCollection filters)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                FilterTuple newfilter = filters.Find(x => x.FilterName == FilterKeys.QuoteOrderTotal);
                if (HelperUtility.IsNotNull(newfilter))
                {
                    filters.RemoveAll(x => x.FilterName == FilterKeys.QuoteOrderTotal);
                    string quoteOrderTotal = Regex.Split(newfilter.FilterValue, WebStoreConstants.DecimalRegex).Where(c => c != "." && c.Trim() != "").FirstOrDefault();
                    if (!string.IsNullOrEmpty(quoteOrderTotal))
                        filters.Add(newfilter.FilterName, newfilter.FilterOperator, quoteOrderTotal);
                }
            }
        }

        //Bind required data from AccountQuoteViewModel to CartLineItem.
        protected virtual void BindDataToCartLineItems(CartItemViewModel cart, AccountQuoteViewModel accountQuoteViewModel)
        {
            cart.OmsQuoteId = accountQuoteViewModel.OmsQuoteId;
            cart.OrderStatus = accountQuoteViewModel.OrderStatus;
            cart.ShippingId = accountQuoteViewModel.ShippingId;
            cart.ShippingAddressId = accountQuoteViewModel.ShippingAddressId;
            cart.BillingAddressId = accountQuoteViewModel.BillingAddressId;
            cart.SelectedAccountUserId = accountQuoteViewModel.UserId;
        }

        //Get List of Active Countries.
        //Set Expands For Order Details.
        protected virtual ExpandCollection SetExpandsForOrderDetails()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Store);
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString());
            expands.Add(ExpandKeys.ZnodeShipping);
            expands.Add(ExpandKeys.IsFromReOrder);
            return expands;
        }

        //Set Expands For Order Receipt.
        public virtual ExpandCollection SetExpandsForReceipt()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString());
            expands.Add(ExpandKeys.Store);
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentType.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodePaymentSetting.ToString());
            expands.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderState.ToString());
            expands.Add(ExpandKeys.ZnodeShipping);
            expands.Add(ExpandKeys.IsFromOrderReceipt);
            expands.Add(ExpandKeys.PortalTrackingPixel);
            expands.Add(ExpandKeys.IsWebStoreOrderReceipt);
            return expands;
        }
        //Bind Sku and Quantity by OrderLineItemRelationType.(Addons, bundle, configurable and group products).
        protected virtual void BindSkuQuantityByOrderLineRelationType(List<CartItemViewModel> model, OrderModel reorderItems, OrderLineItemModel item, string skus)
        {
            switch (item.OrderLineItemRelationshipTypeId)
            {
                //Add on products.
                case (int)ZnodeCartItemRelationshipTypeEnum.AddOns:
                    model.Where(x => x.SKU == reorderItems.OrderLineItems.Where(y => y.OmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.FirstOrDefault()?.Sku)?
                         .Where(x => x.AddOnProductSKUs == null)
                         .ToList()
                         .ForEach(x => x.AddOnProductSKUs = skus);
                    break;

                //Bundle products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Bundles:
                    model.Where(x => x.SKU == reorderItems.OrderLineItems.Where(y => y.OmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.FirstOrDefault()?.Sku)?.ToList().ForEach(x => x.BundleProductSKUs = skus);
                    break;

                //Configurable products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Configurable:
                    model.Where(x => x.SKU == reorderItems.OrderLineItems.Where(y => y.OmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.FirstOrDefault()?.Sku)?.ToList().ForEach(x => x.ConfigurableProductSKUs = skus);
                    break;

                //Group products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Group:
                    string groupedProductQuantities = string.Join("_", reorderItems.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.Select(y => y.Quantity));
                    model.Where(x => x.SKU == reorderItems.OrderLineItems.Where(y => y.OmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.FirstOrDefault()?.Sku)?.ToList().ForEach(x => x.GroupProductsQuantity = groupedProductQuantities);
                    model.Where(x => x.SKU == reorderItems.OrderLineItems.Where(y => y.OmsOrderLineItemsId == item.ParentOmsOrderLineItemsId)?.FirstOrDefault()?.Sku)?.ToList().ForEach(x => x.GroupProductSKUs = skus);
                    break;

            }
        }

        //Get countries to bind in drop down.
        protected virtual List<SelectListItem> GetCountries()
        {

            string cacheKey = string.Concat("CountriesList", Convert.ToString(PortalAgent.CurrentPortal.PortalId));

            if (IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                FilterCollection _filter = new FilterCollection();
                _filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId));
                // Sort countries on the basis of display order assigned to them.
                SortCollection sort = new SortCollection();
                sort.Add(ZnodeCountryEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
                sort.Add(ZnodeCountryEnum.IsDefault.ToString(), DynamicGridConstants.DESCKey);
                List<SelectListItem> countriesList = (from c in _portalCountryClient.GetAssociatedCountryList(null, _filter, sort, null, null)?.Countries
                                                      select new SelectListItem
                                                      {
                                                          Text = c.CountryName,
                                                          Value = c.CountryCode
                                                      }).ToList();

                // Get Profile based options and merge with All Profile options.
                if (countriesList?.Count > 0)
                    Helper.AddIntoCache(countriesList, cacheKey, "CountryListCacheDuration");

            }

            List<SelectListItem> countries = Helper.GetFromCache<List<SelectListItem>>(cacheKey);
            countries?.ForEach(item => { if(item.Selected) item.Selected = false; });

            return countries;
        }

        //Binds the addresses available for user in select list
        protected virtual List<SelectListItem> GetAddressesInSelectList(List<AddressViewModel> addresses)
        {
            List<SelectListItem> userAddresses = new List<SelectListItem>();
            if (addresses?.Count > 0)
            {
                userAddresses = (from item in addresses
                                 select new SelectListItem
                                 {
                                     Text = item.DisplayName,
                                     Value = item.AddressId.ToString(),
                                     Disabled = addresses.Count == 1
                                 }).ToList();
            }
            return userAddresses;
        }

        //Get product wish list model.
        protected virtual WishListModel GetWishListModel(string SKU, string AddOnProductSKUs, UserViewModel accountModel)
        {
            WishListModel model = new WishListModel();
            model.UserId = accountModel.UserId;
            model.SKU = SKU;
            model.WishListAddedDate = DateTime.Today;
            model.AddOnProductSKUs = AddOnProductSKUs;
            model.PortalId = PortalAgent.CurrentPortal.PortalId;
            return model;
        }

        //Check if product exist in wish list.
        protected virtual bool CheckProductAlreadyExists(string SKU, string AddOnProductSKUs, int userId)
        {
            UserViewModel userViewModel = GetUserViewModelFromSession();
            if (userViewModel?.WishList?.Count > 0)
                return userViewModel.WishList.Any(x => x.SKU == SKU && x.AddOnProductSKUs == AddOnProductSKUs);
            return false;
        }

        //Set the Error properties values for Login View Model.
        protected virtual LoginViewModel ReturnErrorModel(string errorMessage, bool hasResetPassword = false, LoginViewModel model = null, int? errorCode = null)
        {
            if (IsNull(model))
                model = new LoginViewModel();

            //Set Model Properties.
            model.HasError = true;
            model.IsResetPassword = hasResetPassword;
            model.ErrorMessage = string.IsNullOrEmpty(errorMessage) ? WebStore_Resources.InvalidUserNamePassword : errorMessage;
            model.ErrorCode = errorCode;
            return model;
        }

        //Update the Model with error properties.Return the updated model in UserViewModelFormat.
        protected virtual UserViewModel SetErrorProperties(UserViewModel model, string errorMessage)
        {
            model.HasError = true;
            model.ErrorMessage = errorMessage;
            return model;
        }

        //Method gets the Domain Base Url.
        protected virtual string GetDomainUrl()
            => (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)))
            ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;

        //Reset the Password for the user.
        protected virtual UserViewModel ResetPassword(UserViewModel model)
        {
            try
            {
                _userClient.ForgotPassword(UserViewModelMap.ToUserModel(model));
                model.SuccessMessage = WebStore_Resources.SuccessResetPassword;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                SetErrorProperties(model, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return model;
        }

        // Returns the Expands needed for the user agent.       
        protected virtual ExpandCollection GetExpands()
        {
            return new ExpandCollection()
                {
                    ExpandKeys.Addresses,
                    ExpandKeys.Profiles,
                    ExpandKeys.WishLists,
                    ExpandKeys.Orders,
                    ExpandKeys.OrderLineItems,
                    ExpandKeys.GiftCardHistory,
                };
        }

        //Get product expands.
        protected virtual ExpandCollection GetProductExpands()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Promotions);
            expands.Add(ExpandKeys.Pricing);
            expands.Add(ExpandKeys.Inventory);
            expands.Add(ExpandKeys.SEO);
            return expands;
        }

        //Update UserViewModel in session accordingly.
        protected virtual void UpdateUserViewModel(object model)
        {
            UserViewModel userModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            if (HelperUtility.IsNull(userModel)) return;

            WishListViewModel viewModel = model as WishListViewModel;
            if (userModel.WishList.Any(x => x.UserWishListId == viewModel.UserWishListId))
                userModel.WishList.Remove(userModel.WishList.First(x => x.UserWishListId == viewModel.UserWishListId));
            //ProductAgent productAgent = new ProductAgent();
            //viewModel.Product = _productClient.GetPublishProductBySKU(viewModel.SKU, PortalAgent.CurrentPortal.PortalId, PortalAgent.LocaleId, GetProductExpands())?.ToViewModel<ProductViewModel>();

            userModel.WishList.Add(viewModel);

            SaveInSession(WebStoreConstants.UserAccountKey, userModel);
        }

        //Remove data from UserViewModel accordingly.
        protected virtual void RemoveUserViewModel(object model)
        {
            UserViewModel accountModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            WishListViewModel viewModel = model as WishListViewModel;
            if (accountModel.WishList.Any(x => x.UserWishListId == viewModel.UserWishListId))
                accountModel.WishList.Remove(
                    accountModel.WishList.First(x => x.UserWishListId == viewModel.UserWishListId));

            SaveInSession(WebStoreConstants.UserAccountKey, accountModel);
        }

        //Get wishlisted product data.
        protected virtual WishListListViewModel GetWishListProductData(UserViewModel userModel, string skus)
        {
            _productClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
            ExpandCollection expands = GetProductExpands();
            expands.Add(ExpandKeys.WishlistAddOns);
            PublishProductListModel productList = _productClient.GetPublishProductList(expands, GetRequiredFilters(), null, null, null, new ParameterKeyModel { Ids = skus, ParameterKey = "SKU" });
            WishListListViewModel wishListViewModel = new WishListListViewModel { WishList = new List<WishListViewModel>() };
            wishListViewModel.WishList = userModel?.WishList;
            wishListViewModel.WishList.ForEach(item =>
            {
                var _product = productList?.PublishProducts?.FirstOrDefault(y => y.SKU == item.SKU);
                item.Product = _product?.ToViewModel<ProductViewModel>();

                if (HelperUtility.IsNotNull(item.Product))
                {
                    Helper.SetProductCartParameter(item.Product, item.AddOnProductSKUs);

                    string minQuantity = item.Product.Attributes?.Value(ZnodeConstant.MinimumQuantity);

                    decimal quantity = Convert.ToDecimal(string.IsNullOrEmpty(minQuantity) ? "0" : minQuantity);
                    _productAgent.CheckInventory(item.Product, quantity);

                    if (!Equals(item.Product.ProductType, ZnodeConstant.GroupedProduct) || !Equals(item.Product.ProductType, ZnodeConstant.ConfigurableProduct))
                    {
                        string addonSKu = item.AddOnProductSKUs ?? string.Empty;
                        GetProductFinalPrice(item.Product, item.Product.AddOns, 1, addonSKu);
                    }
                }
            });

            return wishListViewModel;
        }

        //Map user profile data.
        protected virtual void MapUserProfileData(UserViewModel model, UserViewModel userViewModel)
        {
            userViewModel.Email = model.Email;
            userViewModel.EmailOptIn = model.EmailOptIn;
            userViewModel.FirstName = model.FirstName;
            userViewModel.LastName = model.LastName;
            userViewModel.PhoneNumber = model.PhoneNumber;
            userViewModel.UserName = model.UserName;
            userViewModel.ExternalId = model.ExternalId;
            userViewModel.Custom1 = model.Custom1;
            userViewModel.Custom2 = model.Custom2;
            userViewModel.Custom3 = model.Custom3;
            userViewModel.Custom4 = model.Custom4;
            userViewModel.Custom5 = model.Custom5;
            userViewModel.IsVerified = true;
            userViewModel.SMSOptIn = model.SMSOptIn;
        }

        //Bind address data for dashboard.
        protected virtual void BindAddressData(UserViewModel userViewModel)
        {
            AddressListViewModel addresses = GetAddressList(userViewModel.IsAddressBook);
            userViewModel.AddressList = GetAddressesInSelectList(addresses?.AddressList);
            //If address not available in address book then there will not be any default address for user.
            userViewModel.ShippingAddress = addresses?.AddressList?.FirstOrDefault(x => x.IsDefaultShipping && x.DontAddUpdateAddress == false);
            userViewModel.BillingAddress = addresses?.AddressList?.FirstOrDefault(x => x.IsDefaultBilling && x.DontAddUpdateAddress == false);
        }

        //Bind product review data for dashboard.
        protected virtual void BindProductReviewData(UserViewModel userViewModel) => userViewModel.ReviewCount = GetProductReviewList().Count;


        //Bind wishlist data for dashboard.
        protected virtual void BindWishListData(UserViewModel userViewModel)
        {
            if (userViewModel?.WishList?.Count > 0)
                userViewModel.WishList = GetWishListProductData(userViewModel, string.Join(",", userViewModel.WishList.Select(x => x.SKU).ToList())).WishList;
        }

        //Bind Sku Quantity for Single Product.
        protected virtual void BindSkuQuantityForSingleProduct(CartItemViewModel model, OrderModel reorderItems, OrderLineItemModel orderLineItem, string skus)
        {
            switch (orderLineItem.OrderLineItemRelationshipTypeId)
            {
                //Add on products.
                case (int)ZnodeCartItemRelationshipTypeEnum.AddOns:
                    model.AddOnProductSKUs = skus;
                    break;

                //Bundle products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Bundles:
                    model.BundleProductSKUs = skus;
                    break;

                //Configurable products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Configurable:

                    //Bind configurable product quantity.
                    string configurableProductQuantities = string.Join("_", reorderItems.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == orderLineItem.ParentOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == orderLineItem.OrderLineItemRelationshipTypeId)?.Select(y => y.Quantity));
                    model.Quantity = Convert.ToDecimal(configurableProductQuantities);

                    model.ConfigurableProductSKUs = skus;
                    break;

                //Group products.
                case (int)ZnodeCartItemRelationshipTypeEnum.Group:
                    string groupedProductQuantities = string.Join("_", reorderItems.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == orderLineItem.ParentOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == orderLineItem.OrderLineItemRelationshipTypeId)?.Select(y => y.Quantity));
                    model.GroupProductsQuantity = groupedProductQuantities;
                    model.GroupProductSKUs = skus;
                    break;
            }
        }

        //Set guest user address and save it in session.
        protected virtual AddressViewModel SetAnonymousUserAddresses(AddressViewModel addressViewModel, string addressType)
        {
            //get cart model from session
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            //if address type is shipping then save shipping address in cart session 
            if (Equals(addressType, WebStoreConstants.ShippingAddressType))
            {
                cartModel.ShippingAddress = addressViewModel.ToModel<AddressModel>();
                cartModel.ShippingAddress.IsDefaultShipping = true;
                cartModel.BillingEmail = addressViewModel.EmailAddress;
                cartModel.ShippingAddress.StateCode = addressViewModel.StateName;

                //If "UseSameAsShippingAddress" is true then set billing address as same as shipping address.
                if (addressViewModel.UseSameAsShippingAddress)
                {
                    cartModel.ShippingAddress.IsDefaultBilling = true;
                    cartModel.BillingAddress = addressViewModel.ToModel<AddressModel>();
                    cartModel.BillingAddress.IsDefaultBilling = false;
                    cartModel.BillingAddress.StateCode = addressViewModel.StateName;
                }
                else
                    cartModel.ShippingAddress.IsDefaultBilling = false;
            }
            //if address type is billing then save billing address in cart session 
            else if (Equals(addressType, WebStoreConstants.BillingAddressType) || addressViewModel.IsDefaultBilling)
            {
                cartModel.BillingAddress = addressViewModel.ToModel<AddressModel>();
                cartModel.BillingAddress.IsDefaultBilling = true;
                cartModel.ShippingAddress.IsDefaultBilling = false;
                cartModel.BillingAddress.StateCode = addressViewModel.StateName;

                addressViewModel.EmailAddress = cartModel?.ShippingAddress?.EmailAddress;
                cartModel.BillingAddress.EmailAddress = cartModel?.ShippingAddress?.EmailAddress;
            }

            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, cartModel);

            //Map AddressListViewModel with address and save in session.
            MapAddressListViewModel(addressViewModel, addressType);

            return addressViewModel;
        }

        protected virtual void MapAddressListViewModel(AddressViewModel addressViewModel, string addressType)
        {
            AddressListViewModel addressListViewModel = GetFromSession<AddressListViewModel>(WebStoreConstants.GuestUserAddressListKey) ?? new AddressListViewModel();
            addressViewModel.StateCode = addressViewModel.StateName;

            if (addressViewModel.UseSameAsShippingAddress)
            {
                addressListViewModel.ShippingAddress = addressViewModel;
                addressListViewModel.BillingAddress = addressViewModel;
            }
            else
            {
                addressListViewModel.ShippingAddress.IsBilling = false;
                addressListViewModel.BillingAddress.IsShipping = false;

                switch (addressType)
                {
                    case "shipping":
                        {
                            addressListViewModel.ShippingAddress = addressViewModel;
                            addressListViewModel.ShippingAddress.UseSameAsShippingAddress = false;
                            addressListViewModel.BillingAddress.UseSameAsShippingAddress = false;
                            addressListViewModel.BillingAddress = addressListViewModel.BillingAddress;
                        }
                        break;
                    case "billing":
                        {
                            addressListViewModel.BillingAddress = addressViewModel;
                            addressListViewModel.BillingAddress.UseSameAsShippingAddress = false;
                            addressListViewModel.ShippingAddress.UseSameAsShippingAddress = false;
                            addressListViewModel.ShippingAddress = addressListViewModel.ShippingAddress;
                        }
                        break;
                    default:
                        break;
                }
            }

            SaveInSession<AddressListViewModel>(WebStoreConstants.GuestUserAddressListKey, addressListViewModel);
        }

        //Get guest user address from cart session.
        public virtual AddressListViewModel GetAnonymousUserAddress()
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            AddressListViewModel addressViewModel = new AddressListViewModel();

            if (HelperUtility.IsNotNull(cartModel))
            {
                addressViewModel.AddressList = new List<AddressViewModel>();
                //Get shipping address if not null.
                if (HelperUtility.IsNotNull(cartModel.ShippingAddress))
                {
                    addressViewModel.ShippingAddress = cartModel.ShippingAddress?.ToViewModel<AddressViewModel>();
                    if (!string.IsNullOrEmpty(cartModel.BillingEmail))
                    {
                        addressViewModel.ShippingAddress.EmailAddress = cartModel.ShippingAddress.EmailAddress;
                    }
                    //initializing again because of its get false on transaction fails
                    if (addressViewModel.ShippingAddress.IsBilling && addressViewModel.ShippingAddress.IsShipping)
                    {
                        addressViewModel.ShippingAddress.IsBothBillingShipping = true;
                    }
                }
                //Get billing address if not null.
                if (HelperUtility.IsNotNull(cartModel.BillingAddress))
                {
                    addressViewModel.BillingAddress = cartModel.BillingAddress?.ToViewModel<AddressViewModel>();
                    addressViewModel.BillingAddress.EmailAddress = cartModel.BillingAddress.EmailAddress;

                    //initializing again because of its get false on transaction fails
                    if (addressViewModel.BillingAddress.IsBilling && addressViewModel.BillingAddress.IsShipping)
                    {
                        addressViewModel.BillingAddress.IsBothBillingShipping = true;
                    }
                }

            }

            addressViewModel = GetFromSession<AddressListViewModel>(WebStoreConstants.GuestUserAddressListKey) == null ?
                            addressViewModel : GetFromSession<AddressListViewModel>(WebStoreConstants.GuestUserAddressListKey);
            return addressViewModel;
        }

        //Bind user details to AccountViewModel.
        protected virtual void BindUserDetails(AccountViewModel accountViewModel, UserViewModel userViewModel)
        {
            accountViewModel.RoleName = userViewModel.RoleName;
            accountViewModel.PhoneNumber = accountViewModel?.Address?.PhoneNumber;
            accountViewModel.UserExternalId = userViewModel.ExternalId;
        }

        //Set filter for User Id.
        protected virtual void SetUserIdFilter(FilterCollection filters, int userId, int accountId, bool isPendingPayment)
        {

            //If UserId or WebStoreQuote Already present in filters Remove It.
            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodeConstant.WebStoreQuotes);

            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.WebStoreQuotes, FilterOperators.Equals, ZnodeConstant.TrueValue));

            //Checking For AccountId already Exists in Filters Or Not 
            if (filters.Exists(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString()))
                //If AccountId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString());

            if (accountId > 0)
                //Add New AccountId Into filters
                filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));

            filters.Add(new FilterTuple("PendingPayment", FilterOperators.Equals, Convert.ToString(isPendingPayment)));
            //filters.Add(new FilterTuple("IsParentPendingOrder", FilterOperators.Equals, ZnodeConstant.TrueValue));
        }

        //Set the Tool Menus for Sub Account List Grid View.
        protected virtual void SetQuotesToolMenus(AccountQuoteListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                List<SelectListItem> statusList = GetOrderStateList();
                if (statusList?.Count > 0)
                {
                    model.GridModel = new GridModel();
                    model.GridModel.FilterColumn = new FilterColumnListModel();
                    model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                    //Binds the function.
                    foreach (SelectListItem item in statusList)
                        model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = $"{Admin_Resources.LabelUpdateStatus} to {item.Text}", JSFunctionName = $"User.prototype.UpdateQuoteStatus(this,{item.Value})", ControllerName = "User", ActionName = "UpdateQuoteStatus" });
                }
            }
        }

        //Get Order State List.
        protected virtual List<SelectListItem> GetOrderStateList()
          => ToOrderStateSelectListItemList(_orderStateClient.GetOrderStates(null, new FilterCollection() { new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString(), FilterOperators.Equals, "true") }, null, null, null)?.OrderStates);

        //Convert Enumerable list of Order state model to select lit items.
        protected virtual List<SelectListItem> ToOrderStateSelectListItemList(IEnumerable<OrderStateModel> orderStateList)
        {
            List<SelectListItem> orderStateItems = new List<SelectListItem>();
            if (orderStateList?.Count() > 0)
            {
                orderStateItems = (from item in orderStateList
                                   orderby item.OrderStateName ascending
                                   select new SelectListItem
                                   {
                                       Text = item.OrderStateName?.Replace("_", " "),
                                       Value = item.OrderStateId.ToString()
                                   })?.ToList();

                orderStateItems = orderStateItems.Where(x => !string.Equals(x.Text, "Draft", StringComparison.CurrentCultureIgnoreCase) & !string.Equals(x.Text, "Ordered", StringComparison.CurrentCultureIgnoreCase))?.ToList();

                //Remove Ordered status from list.
                orderStateItems.RemoveAll(x => string.Equals(x.Text, "Ordered", StringComparison.CurrentCultureIgnoreCase));
            }
            return orderStateItems;
        }

        //Set Filters Order Invoice.
        protected virtual ExpandCollection SetOrderInvoiceFilters()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.OrderLineItems);
            expands.Add(ExpandKeys.OrderShipment);
            expands.Add(ExpandKeys.PaymentType);
            expands.Add(ExpandKeys.UserDetails);
            expands.Add(ExpandKeys.OmsPaymentState);
            expands.Add(ExpandKeys.ShippingType);
            return expands;
        }

        //Map CartItemViewModel to AccountQuoteLineItemModel.
        protected virtual void ToQuoteLineItem(AccountQuoteLineItemViewModel accountQuoteLineItemViewModel, CartItemViewModel cartItemViewModel)
        {
            accountQuoteLineItemViewModel.OmsQuoteId = cartItemViewModel.OmsQuoteId;
            accountQuoteLineItemViewModel.OmsQuoteLineItemId = cartItemViewModel.OmsQuoteLineItemId;
            accountQuoteLineItemViewModel.ParentOmsQuoteLineItemId = cartItemViewModel.ParentOmsQuoteLineItemId;
            accountQuoteLineItemViewModel.OrderLineItemRelationshipTypeId = cartItemViewModel.OrderLineItemRelationshipTypeId;
            accountQuoteLineItemViewModel.CustomText = cartItemViewModel.CustomText;
            accountQuoteLineItemViewModel.CartAddOnDetails = cartItemViewModel.CartAddOnDetails;
            accountQuoteLineItemViewModel.SKU = cartItemViewModel.SKU;
            accountQuoteLineItemViewModel.Quantity = cartItemViewModel.Quantity;
            accountQuoteLineItemViewModel.QuoteOrderTotal = cartItemViewModel.QuoteOrderTotal;
            accountQuoteLineItemViewModel.Sequence = cartItemViewModel.Sequence;
        }

        //Validate default address.
        protected virtual AddressViewModel ValidateDefaultAddress(AddressViewModel addressViewModel, UserViewModel model)
        {
            var allAddress = Helper.GetFromCache<AddressListViewModel>($"UserAccountAddressList{model?.UserId}");

            if (allAddress?.AddressList?.Count > 0)
            {
                var defaultBillingAddresses = allAddress.AddressList.Where(x => x.AddressId != addressViewModel.AddressId && x.IsDefaultBilling).ToList();
                var defaultShippingAddresses = allAddress.AddressList.Where(x => x.AddressId != addressViewModel.AddressId && x.IsDefaultShipping).ToList();

                if (!addressViewModel.IsDefaultBilling && !addressViewModel.IsDefaultShipping)
                {
                    if (!defaultBillingAddresses.Any() && !defaultShippingAddresses.Any())
                        return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, WebStore_Resources.ErrorDefaultBillingShippingAddress);
                }

                addressViewModel.HasError = false;
                addressViewModel.ErrorMessage = string.Empty;
                return addressViewModel;
            }
            else if (addressViewModel.IsDefaultBilling && addressViewModel.IsDefaultShipping)
                return addressViewModel;

            else if ((!addressViewModel.IsDefaultBilling && addressViewModel.IsDefaultShipping)
                    || (addressViewModel.IsDefaultBilling && !addressViewModel.IsDefaultShipping))
            {
                //Allow either shipping or billing address to be the default address for allowing separatet billing/shipping.
                addressViewModel.HasError = false;
                addressViewModel.ErrorMessage = string.Empty;
                return addressViewModel;
            }

            else if (!addressViewModel.IsDefaultBilling && !addressViewModel.IsDefaultShipping)
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, WebStore_Resources.ErrorDefaultBillingShippingAddress);

            return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, WebStore_Resources.ErrorAddAddress);
        }

        //Check whether address is valid or not.
        protected virtual Api.Models.BooleanModel IsValidAddress(AddressViewModel addressViewModel)
        {
            if (PortalAgent.CurrentPortal.EnableAddressValidation)
            {
                AddressModel addressModel = addressViewModel.ToModel<AddressModel>();
                addressModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;
                return _shippingClient.IsShippingAddressValid(addressModel);
            }


            return new Api.Models.BooleanModel { IsSuccess = true };
        }

        //Get the access token for facebook user.
        protected virtual string GetAccessToken(ExternalLoginInfo loginInfo)
            => loginInfo?.ExternalIdentity?.Claims?.FirstOrDefault(x => x.Type == ZnodeConstant.FacebookAccessToken)?.Value;

        //Clear address from session.
        protected virtual void ClearSessionAddress()
        {
            //Clear address from UserAccountKey session.
            ClearUserAccountKeySessionAddress();

            //Clear address from CartModelSessionKey session.
            ClearCartModelSessionKeyAddress();
        }

        //Clear address from UserAccountKey session.
        protected virtual void ClearUserAccountKeySessionAddress()
        {
            UserViewModel user = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (HelperUtility.IsNotNull(user))
            {
                user.Addresses = new List<AddressViewModel>();
                user.ShippingAddress = new AddressViewModel();
                user.BillingAddress = new AddressViewModel();
                SaveInSession(WebStoreConstants.UserAccountKey, user);
            }
        }

        //Clear address from CartModelSessionKey session.
        protected virtual void ClearCartModelSessionKeyAddress()
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            if (HelperUtility.IsNotNull(cartModel))
            {
                cartModel.BillingAddress = new AddressModel();
                cartModel.ShippingAddress = new AddressModel();
                SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            }
        }

        //Save created/updated address in session.
        protected virtual void SaveAddressInSession(AddressListViewModel addressViewModel)
        {
            if (addressViewModel?.AddressList?.Count > 0)
            {
                SaveAddressInUserAccountKeySession(addressViewModel);
                SaveAddressInCartModelSessionKeySession(addressViewModel);
            }
        }

        //Save created/updated address in UserAccountKey session.
        protected virtual void SaveAddressInUserAccountKeySession(AddressListViewModel addressViewModel)
        {
            UserViewModel user = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (HelperUtility.IsNotNull(user))
            {
                user.Addresses = addressViewModel.AddressList;
                SaveInSession(WebStoreConstants.UserAccountKey, user);
            }
        }

        //Save created/updated address in CartModelSessionKey session.
        protected virtual void SaveAddressInCartModelSessionKeySession(AddressListViewModel addressViewModel)
        {
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            if (HelperUtility.IsNotNull(cartModel))
            {
                cartModel.BillingAddress = addressViewModel.AddressList?.Where(x => x.IsDefaultBilling)?.FirstOrDefault()?.ToModel<AddressModel>();
                cartModel.ShippingAddress = addressViewModel.AddressList?.Where(x => x.IsDefaultShipping)?.FirstOrDefault()?.ToModel<AddressModel>();
                SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            }
        }

        //Get final price of product.
        protected virtual void GetProductFinalPrice(ProductViewModel viewModel, List<AddOnViewModel> addOns, decimal minQuantity, string addOnIds)
        {
            viewModel.UnitPrice = viewModel.SalesPrice > 0 ? viewModel.SalesPrice : viewModel.RetailPrice;
            //Apply tier price if any.
            if (viewModel.TierPriceList?.Count > 0 && viewModel.TierPriceList.Where(x => minQuantity >= x.MinQuantity)?.Count() > 0)
                viewModel.ProductPrice = viewModel.TierPriceList.FirstOrDefault(x => minQuantity >= x.MinQuantity && minQuantity < x.MaxQuantity)?.Price * minQuantity;
            else
                viewModel.ProductPrice = (minQuantity > 0 && HelperUtility.IsNotNull(viewModel.SalesPrice)) ? viewModel.SalesPrice * minQuantity : viewModel.PromotionalPrice > 0 ? viewModel.PromotionalPrice : viewModel.RetailPrice * minQuantity;

            if (addOns?.Count > 0)
            {
                decimal? addonPrice = 0.00M;

                if (!string.IsNullOrEmpty(addOnIds))
                {
                    foreach (string addOn in addOnIds.Split(','))
                    {
                        AddOnValuesViewModel addOnValue = addOns.SelectMany(
                                   y => y.AddOnValues.Where(x => x.SKU == addOn))?.FirstOrDefault();
                        if (HelperUtility.IsNotNull(addOnValue))
                            addonPrice = addonPrice + (HelperUtility.IsNotNull(addOnValue.SalesPrice) ? addOnValue.SalesPrice : addOnValue.RetailPrice) * minQuantity;

                    }
                }
                viewModel.ProductPrice = addonPrice > 0 ? viewModel.ProductPrice + addonPrice : viewModel.ProductPrice;
            }
            if (HelperUtility.IsNull(viewModel.ProductPrice))
            {
                viewModel.ShowAddToCart = false;
                viewModel.InventoryMessage = Convert.ToBoolean(viewModel?.Attributes?.Value(ZnodeConstant.CallForPricing)) ? string.Empty : WebStore_Resources.ErrorPriceNotAssociate;
            }
        }

        //Check if price is associated to the product.
        protected virtual void CheckProductPrice(CartItemViewModel model, OrderModel reorderItems, OrderLineItemModel orderLineItem)
        {
            //Check if product has price associated to it.
            ProductViewModel productViewModel = _productAgent.GetProductPriceAndInventory(orderLineItem.Sku, orderLineItem.Quantity.ToString(), string.Empty);
            if (Equals(productViewModel.ProductType, ZnodeConstant.SimpleProduct) || Equals(productViewModel.ProductType, ZnodeConstant.BundleProduct))
            {
                if (IsNotNull(productViewModel.ProductPrice) || orderLineItem.Price > 0)
                    CreateOrderLineItem(model, reorderItems, orderLineItem);
            }
            else
                CreateOrderLineItem(model, reorderItems, orderLineItem);
        }
        //Get parent order line item from supplied line item id.
        protected virtual OrderLineItemModel GetParentOrderLineItem(int parentOmsOrderLineItemId)
        {
            OrderModel orderItems = _orderClient.GetOrderByOrderLineItemId(parentOmsOrderLineItemId, SetExpandsForReceipt());
            return orderItems?.OrderLineItems?.FirstOrDefault();
        }
        //Create OrderLineItem.
        protected virtual void CreateOrderLineItem(CartItemViewModel model, OrderModel reorderItems, OrderLineItemModel orderLineItem)
        {
            if (orderLineItem.ParentOmsOrderLineItemsId > 0 && orderLineItem.OrderLineItemRelationshipTypeId > 0 && orderLineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple)
            {
                string skus = string.Join(",", reorderItems.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == orderLineItem.ParentOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == orderLineItem.OrderLineItemRelationshipTypeId)?.Select(y => y.Sku));
                BindSkuQuantityForSingleProduct(model, reorderItems, orderLineItem, skus);
            }
            //If product is a simple product.
            else
            {
                model.Quantity = orderLineItem.Quantity;
                model.SKU = orderLineItem.Sku;
                model.PersonaliseValuesList = (HelperUtility.IsNotNull(model.PersonaliseValuesList) && model.PersonaliseValuesList.Count > 0) ? model.PersonaliseValuesList : orderLineItem.PersonaliseValueList;
                model.PersonaliseValuesDetail = (HelperUtility.IsNotNull(model.PersonaliseValuesDetail) && model.PersonaliseValuesDetail.Count > 0) ? model.PersonaliseValuesDetail : orderLineItem.PersonaliseValuesDetail;

            }
        }

        //Check price for products in case of complete order reorder.
        protected virtual void CheckProductPriceForCompleteReorder(List<CartItemViewModel> model, OrderModel reorderItems, OrderLineItemModel orderLineItem)
        {
            //Check if product has price associated to it.
            ProductViewModel productViewModel = _productAgent.GetProductPriceAndInventory(orderLineItem.Sku, orderLineItem.Quantity.ToString(), string.Empty);
            if (Equals(productViewModel.ProductType, ZnodeConstant.SimpleProduct) || Equals(productViewModel.ProductType, ZnodeConstant.BundleProduct))
            {
                if (HelperUtility.IsNotNull(productViewModel.ProductPrice) || orderLineItem.Price > 0)
                    CreateMultipleOrderLineItems(model, reorderItems, orderLineItem);
            }
            else
                CreateMultipleOrderLineItems(model, reorderItems, orderLineItem);
        }

        //Create cart line item in case of complete order reorder.
        protected virtual void CreateMultipleOrderLineItems(List<CartItemViewModel> model, OrderModel reorderItems, OrderLineItemModel orderLineItem)
        {
            if (orderLineItem.ParentOmsOrderLineItemsId > 0 && orderLineItem.OrderLineItemRelationshipTypeId > 0 && orderLineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple)
            {
                string skus = string.Join(",", reorderItems.OrderLineItems.Where(x => x.ParentOmsOrderLineItemsId == orderLineItem.ParentOmsOrderLineItemsId)?.Select(y => y.Sku));
                BindSkuQuantityByOrderLineRelationType(model, reorderItems, orderLineItem, skus);
            }
            //If product is a simple product. If a product is simple product then only the product can be an addon.
            else if (orderLineItem.AutoAddonSku != orderLineItem.Sku)
                model.Add(new CartItemViewModel { PersonaliseValuesList = orderLineItem.PersonaliseValueList, PersonaliseValuesDetail = orderLineItem.PersonaliseValuesDetail, Quantity = orderLineItem.Quantity, SKU = orderLineItem.Sku, AutoAddonSKUs = orderLineItem.AutoAddonSku, OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId });
        }

        //Get the login name for social login user.
        protected virtual string GetUserName(ExternalLoginInfo loginInfo)
        {
            if (HelperUtility.IsNotNull(loginInfo))
                return DefaultSettingHelper.AllowGlobalLevelUserCreation ? $"{loginInfo.Login.ProviderKey}@{loginInfo.Login.LoginProvider}.com" : $"{loginInfo.Login.ProviderKey}_{PortalAgent.CurrentPortal?.PortalId}@{loginInfo.Login.LoginProvider}.com";
            return string.Empty;
        }

        //Create update address according to address id.
        protected virtual AddressViewModel CreateUpdateAddress(AddressViewModel addressViewModel)
        {
            SetIsBillingShippingDifferentFlag(addressViewModel);

            //Address Creation.
            if (addressViewModel.AddressId == 0)
            {
                addressViewModel = _webStoreAccountClient.CreateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                if (!addressViewModel.HasError)
                    addressViewModel.SuccessMessage = WebStore_Resources.SuccessAddressAdded;
            }
            //Address Update.
            else
            {
                addressViewModel = _webStoreAccountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                if (!addressViewModel.HasError)
                    addressViewModel.SuccessMessage = WebStore_Resources.SuccessAddressUpdated;
            }

            return addressViewModel;
        }

        //Remove address from cache and session.
        protected virtual void RemoveAddressFromCacheAndSession(AddressViewModel addressViewModel)
        {
            //Clear address from cache.
            ClearAddressCache(addressViewModel.UserId);

            //Clear address from session.
            ClearSessionAddress();
        }

        //Bind dashboard data.
        protected virtual void BindDashBoardData(UserViewModel userViewModel)
        {
            BindWishListData(userViewModel);
            BindProductReviewData(userViewModel);
            BindAddressData(userViewModel);
            BindUserProfileDropdownData(userViewModel);
            BindVouchersData(userViewModel);
            //Orders. - To show top 3 orders only on dashboard.
            userViewModel.OrderList = GetOrderList(Filters, null, 1, 3).List;
            userViewModel.ReturnList = GetReturnList();
            BindCreditCardData(userViewModel);
            BindReferralCommissionData(userViewModel);
            BindUserApproverData(userViewModel);
        }

        //Bind user approvers data.
        protected virtual void BindUserApproverData(UserViewModel userViewModel)
        {
            ApproverDetailsModel approverDetailsModel = SessionProxyHelper.GetApproverDetails(userViewModel.UserId);
            if (IsNotNull(approverDetailsModel))
            {
                userViewModel.IsApprover = approverDetailsModel.IsApprover;
                userViewModel.HasApprovers = approverDetailsModel.HasApprovers;
            }
        }

        //Get pending payments and pending orders count for showing account menus
        protected virtual void GetUserDashbaordPendingOrderDetailsCount(UserViewModel userViewModel)
        {
            UserDashboardPendingOrdersModel pendingOrdersModel = SessionProxyHelper.GetUserDashboardPendingOrderDetailsCount(userViewModel);
            if (IsNotNull(pendingOrdersModel))
            {
                userViewModel.PendingPaymentCount = pendingOrdersModel.PendingPaymentsCount;
                userViewModel.PendingOrdersCount = pendingOrdersModel.PendingOrdersCount;
            }
        }

        //Bind credit card data.
        protected virtual void BindCreditCardData(UserViewModel userViewModel)
        {
            //Added For Getting Active payment card ID
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId)));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(userViewModel.UserId)));
            //Get Payment option list.
            PaymentSettingListModel paymentOptionListModel = _paymentClient.GetPaymentSettings(null, filters, null, null, null);
            userViewModel.PaymentSettingId = paymentOptionListModel?.PaymentSettings?.Where(x => x.PaymentTypeName == ZnodeConstant.CreditCard)?.Select(x => x.PaymentSettingId)?.FirstOrDefault();
        }

        //Bind referral commission data of a user.
        protected virtual void BindReferralCommissionData(UserViewModel userViewModel)
        {
            //Get referral commission data of a user.
            ReferralCommissionModel referralCommissionModel = _customerClient.GetCustomerAffiliate(userViewModel.UserId, null);
            userViewModel.ReferralCommissionData = referralCommissionModel?.ToViewModel<ReferralCommissionViewModel>();
        }

        //Bind user profile dropdown for dashboard.
        protected virtual void BindUserProfileDropdownData(UserViewModel userViewModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId)));

            ProfileListModel list = new ProfileListModel();
            if (userViewModel.IsAdminUser)
            {
                PortalProfileListModel portalProfilelist = _portalProfileClient.GetPortalProfiles(null, filters, null, null, null);
                list.Profiles = MapPortalProfileToProfileModel(portalProfilelist?.PortalProfiles);
            }
            else
            {
                filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(userViewModel.UserId)));
                SortCollection sort = new SortCollection();
                sort.Add(ZnodeProfileEnum.ProfileId.ToString(), DynamicGridConstants.ASCKey);
                //Get profile list.
                list = _customerClient.GetCustomerPortalProfilelist(null, filters, sort, null, null);
            }
            userViewModel.ProfileList = GetProfiles(list.Profiles);
        }

        protected virtual List<ProfileModel> MapPortalProfileToProfileModel(List<PortalProfileModel> portalProfileList)
        {
            List<ProfileModel> list = new List<ProfileModel>();
            if (portalProfileList?.Count > 0)
                foreach (var item in portalProfileList)
                    list.Add(
                        new ProfileModel
                        {
                            ProfileId = item.ProfileId,
                            Name = item.ProfileName,
                            ProfileName = item.ProfileName,
                        });

            return list;
        }

        //Gets the user model for creating user.
        protected virtual UserModel GetUserModelToCreate(ExternalLoginInfo loginInfo, string email)
        {
            UserModel registerUser = UserViewModelMap.ToSignUpModel(new RegisterViewModel() { UserName = GetUserName(loginInfo), IsWebStoreUser = true, Email = email });
            registerUser.User.Email = string.IsNullOrEmpty(email) ? GetUserName(loginInfo) : email;
            registerUser.ExternalLoginInfo = loginInfo;
            registerUser.IsSocialLogin = true;
            return registerUser;
        }

        //Get email for email.
        protected virtual string GetFacebookEmail(ExternalLoginInfo loginInfo)
        {
            string email = loginInfo.Email;
            //if the provider is facebook then request for email.
            if (string.Equals(loginInfo.Login.LoginProvider, ZnodeConstant.Facebook, StringComparison.InvariantCultureIgnoreCase))
            {
                FacebookClient client = new FacebookClient(GetAccessToken(loginInfo));
                dynamic userDetails = client.Get("me?fields=id,name,email");
                email = (HelperUtility.IsNotNull(userDetails) && !string.IsNullOrEmpty(userDetails.email)) ? userDetails.email : string.Empty;
            }

            return email;
        }

        //Add group products in the cart.
        protected virtual List<CartItemViewModel> AddGroupedOrConfigurableProductsInCart(ref OrderModel reorderItems)
        {
            List<CartItemViewModel> model = new List<CartItemViewModel>();
            List<OrderLineItemModel> orderLineItemModel = new List<OrderLineItemModel>();
            List<OrderLineItemModel> configurableProducts = new List<OrderLineItemModel>();
            List<OrderLineItemModel> addOns = new List<OrderLineItemModel>();
            foreach (OrderLineItemModel lineItem in reorderItems.OrderLineItems)
                CheckProductType(orderLineItemModel, configurableProducts, addOns, lineItem);

            //check if the Group products are in the list or not
            if (orderLineItemModel.Count > 0)
                CreateGroupOrderLineItem(reorderItems, model, orderLineItemModel, addOns);

            //Check if configurable products are in the list or not.
            if (configurableProducts?.Count > 0)
                CreateConfigurableOrderLineItem(reorderItems, model, configurableProducts, addOns);

            return model;
        }

        //Check product type.
        protected virtual void CheckProductType(List<OrderLineItemModel> groupedProducts, List<OrderLineItemModel> configurableProducts, List<OrderLineItemModel> addOns, OrderLineItemModel lineItem)
        {
            //Check if the product is configurable product or not.
            if (lineItem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Configurable))
                configurableProducts.Add(lineItem);

            //Check if the product is grouped product or not.
            if (lineItem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Group))
                groupedProducts.Add(lineItem);

            //Check add ons.
            if (lineItem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.AddOns))
                addOns.Add(lineItem);
        }

        //Create Configurable product's OrderLineItem.
        protected virtual void CreateConfigurableOrderLineItem(OrderModel reorderItems, List<CartItemViewModel> model, List<OrderLineItemModel> configurableProducts, List<OrderLineItemModel> addOns)
        {
            //Take out the parent ids from configurable product list
            string configurableProductParentOrderLineItemId = string.Join(",", configurableProducts.Select(x => x.ParentOmsOrderLineItemsId).ToList());
            List<OrderLineItemModel> configurableParentProducts = GetParentLineItemsforGroupedProduct(reorderItems, configurableProductParentOrderLineItemId);

            if (configurableParentProducts?.Count > 0)
            {
                //Iterate through the configurable products using parent products and remove those from re-order list.
                foreach (OrderLineItemModel configurableLineItem in configurableProducts)
                {
                    foreach (OrderLineItemModel parentLineItem in configurableParentProducts)
                    {
                        if (parentLineItem.OmsOrderLineItemsId.Equals(configurableLineItem.ParentOmsOrderLineItemsId))
                        {
                            List<OrderLineItemModel> addOnList = addOns.Where(x => x.ParentOmsOrderLineItemsId == parentLineItem.OmsOrderLineItemsId).Select(x => x).ToList();
                            string associateAddOns = string.Join(",", addOnList.Select(x => x.Sku));
                            model.Add(new CartItemViewModel { SKU = parentLineItem.Sku, Quantity = configurableLineItem.Quantity, ConfigurableProductSKUs = configurableLineItem.Sku, AddOnProductSKUs = associateAddOns, PersonaliseValuesList = parentLineItem.PersonaliseValueList, PersonaliseValuesDetail = parentLineItem.PersonaliseValuesDetail, OmsOrderLineItemsId = configurableLineItem.OmsOrderLineItemsId });
                            reorderItems.OrderLineItems.Remove(parentLineItem);
                            foreach (OrderLineItemModel addOn in addOnList)
                                reorderItems.OrderLineItems.Remove(addOn);
                            break;
                        }
                    }
                    reorderItems.OrderLineItems.Remove(configurableLineItem);
                }
            }
        }

        //Create Group product's OrderLineItem.
        protected virtual void CreateGroupOrderLineItem(OrderModel reorderItems, List<CartItemViewModel> model, List<OrderLineItemModel> groupedProducts, List<OrderLineItemModel> addOns)
        {
            //Take out the parent ids from group product list
            string groupProductParentOrderLineItemId = string.Join(",", groupedProducts.Select(x => x.ParentOmsOrderLineItemsId).ToList());
            List<OrderLineItemModel> groupedParentProducts = GetParentLineItemsforGroupedProduct(reorderItems, groupProductParentOrderLineItemId);

            if (groupedParentProducts?.Count > 0)
            {
                //Iterate through the group products using parent products and remove those from re-order list.
                foreach (OrderLineItemModel groupLineItem in groupedProducts)
                {
                    foreach (OrderLineItemModel parentLineItem in groupedParentProducts)
                    {
                        if (parentLineItem.OmsOrderLineItemsId.Equals(groupLineItem.ParentOmsOrderLineItemsId))
                        {
                            List<OrderLineItemModel> addOnList = addOns.Where(x => x.ParentOmsOrderLineItemsId == parentLineItem.OmsOrderLineItemsId).Select(x => x).ToList();
                            string associateAddOns = string.Join(",", addOnList.Select(x => x.Sku));
                            model.Add(new CartItemViewModel { SKU = parentLineItem.Sku, GroupProductsQuantity = groupLineItem.Quantity.ToString(), GroupProductSKUs = groupLineItem.Sku, AddOnProductSKUs = associateAddOns, AutoAddonSKUs = parentLineItem.AutoAddonSku });
                            reorderItems.OrderLineItems.Remove(parentLineItem);
                            foreach (OrderLineItemModel addOn in addOnList)
                                reorderItems.OrderLineItems.Remove(addOn);
                            break;
                        }
                    }
                    reorderItems.OrderLineItems.Remove(groupLineItem);
                }
            }
        }

        //Get the list of the parent products.
        protected virtual List<OrderLineItemModel> GetParentLineItemsforGroupedProduct(OrderModel reorderItems, string groupProductParentOrderLineItemId)
            => reorderItems.OrderLineItems.Where(x => groupProductParentOrderLineItemId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(x.OmsOrderLineItemsId.ToString())).ToList();

        //Get order details based on portalId.
        public virtual OrderModel GetOrderBasedOnPortalId(int orderId, FilterCollection filters)
        {
            return _orderClient.GetOrderById(orderId, SetExpandsForReceipt(), filters);
        }

        // Pay Invoice.
        public virtual OrdersViewModel PayInvoice(PayInvoiceViewModel payInvoiceModel)
        {
            try
            {
                if (IsNotNull(payInvoiceModel) && payInvoiceModel.OmsOrderId > 0)
                {
                    OrdersViewModel ordersViewModel = _userClient.PayInvoice(payInvoiceModel.ToModel<PayInvoiceModel>())?.ToViewModel<OrdersViewModel>();
                    UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                    if (string.IsNullOrEmpty(userViewModel.CustomerPaymentGUID))
                    {
                        userViewModel.CustomerPaymentGUID = payInvoiceModel?.PaymentDetails?.CustomerGuid;
                        SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
                    }
                    return ordersViewModel;

                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), Admin_Resources.ConvertQuoteToOrderErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), Admin_Resources.ProcessingFailedError);
            }
        }

        //Get TrackingUrl by ShippingId.
        protected virtual string GetTrackingUrlByShippingId(int shippingId)
            => _shippingClient.GetShipping(shippingId)?.TrackingUrl;

        //Set tracking url.
        protected virtual string SetTrackingUrl(string trackingNo, int shippingId)
        {
            string trackingUrl = GetTrackingUrlByShippingId(shippingId);
            return string.IsNullOrEmpty(trackingUrl) ? trackingNo : "<a target=_blank href=" + GetTrackingUrlByShippingId(shippingId) + trackingNo + ">" + trackingNo + "</a>";
        }

        //Set the sort collection for user id desc.
        protected virtual void SortUserIdDesc(ref SortCollection sortCollection)
        {
            if (IsNull(sortCollection) || sortCollection.Count == 0)
            {
                sortCollection = new SortCollection();
                sortCollection.Add(ZnodeUserEnum.UserId.ToString(), DynamicGridConstants.DESCKey);
            }
        }

        //Check whether the current login user has Customer user and B2B role or not.
        protected virtual bool IsB2BCustomer(UserModel accountModel)
        {
            //Get list of Roles.
            List<string> lstRole = GetB2BRoles();
            return IsNotNull(accountModel.RoleName) ? lstRole.Any(s => accountModel.RoleName.Contains(s)) : false;
        }

        //Get B2B Roles.
        protected virtual List<string> GetB2BRoles()
        {
            //Gets the filtered list of roles which are contains b2b roles and user role.
            List<RoleModel> rolesList = _roleClient.GetRoleList(null, null, null, null)?.Roles?
                     .Where(x => Equals(x.TypeOfRole?.ToLower(), ZnodeRoleEnum.B2B.ToString().ToLower()) || Equals(x.Name?.ToLower(), ZnodeRoleEnum.User.ToString().ToLower()) && x.IsActive == true).ToList();
            if (rolesList?.Count > 0)
                return rolesList.OrderBy(x => x.Name).Select(x => x.Name).ToList();
            return new List<string>();
        }

        //Get the error message on the basis of error codes.
        protected virtual bool GetErrorMessage(out string errorMessage, ZnodeException exception)
        {
            switch (exception.ErrorCode)
            {
                case ErrorCodes.ErrorSendResetPasswordLink:
                    errorMessage = Admin_Resources.ErrorSendResetPasswordLink;
                    return false;

                case ErrorCodes.UserNameUnavailable:
                    errorMessage = exception.ErrorMessage;
                    return false;

                case ErrorCodes.AccountLocked:
                    errorMessage = Admin_Resources.AccountLockedErrorMessage;
                    return false;
                case ErrorCodes.EmailTemplateDoesNotExists:
                    errorMessage = Admin_Resources.ResetPasswordTemplateNotFound;
                    return false;
                default:
                    errorMessage = Admin_Resources.ErrorAccessDenied;
                    return false;
            }
        }

        //Set the Tool Menus for Customer Account List Grid View.
        protected virtual void SetCustomerAccountToolMenuList(CustomerListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = WebStore_Resources.Activate,
                    JSFunctionName = string.Format(JsFunction, AccountEnable),
                    ControllerName = User,
                    ActionName = CustomerEnableDisableAccount
                });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = WebStore_Resources.DeActivate,
                    JSFunctionName = string.Format(JsFunction, Accountdisable),
                    ControllerName = User,
                    ActionName = CustomerEnableDisableAccount
                });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = WebStore_Resources.ResetPasswordButtonText,
                    JSFunctionName = string.Format(JsFunction, "accountresetpassword"),
                    ControllerName = User,
                    ActionName = "BulkResetPassword"
                });
            }
        }

        //Set is IsBilling, IsShipping as per data in addressViewModel and addressType
        protected virtual void SetBillingShippingFlags(AddressViewModel addressViewModel, string addressType)
        {
            if (addressViewModel.IsBothBillingShipping)
            {
                //Set default values
                if (addressType == WebStoreConstants.ShippingAddressType)
                    addressViewModel.IsDefaultBilling = addressViewModel.IsDefaultShipping;
                else if (addressType == WebStoreConstants.BillingAddressType)
                    addressViewModel.IsDefaultShipping = addressViewModel.IsDefaultBilling;
                addressViewModel.IsShipping = true;
                addressViewModel.IsBilling = true;
                addressViewModel.IsShippingBillingDifferent = false;
            }
            else
            {
                //In Edit mode                
                addressViewModel.IsShippingBillingDifferent = true;
                if (addressViewModel.IsAddressBook && addressViewModel.IsDefaultShipping && addressViewModel.IsDefaultBilling)
                {
                    addressViewModel.IsShippingBillingDifferent = false;
                }
                if (HelperUtility.IsNotNull(addressType))
                {
                    //Set address type and is default values
                    addressViewModel.IsShipping = Equals(addressType, WebStoreConstants.ShippingAddressType);
                    addressViewModel.IsBilling = Equals(addressType, WebStoreConstants.BillingAddressType);
                }
            }
        }

        //Set is IsShippingBillingDifferent as per data in addressViewModel
        protected virtual void SetIsBillingShippingDifferentFlag(AddressViewModel addressViewModel)
        {
            if (addressViewModel.IsBothBillingShipping)
            {
                addressViewModel.IsBilling = true;
                addressViewModel.IsShipping = true;
                addressViewModel.IsShippingBillingDifferent = false;
            }
            else
            {
                addressViewModel.IsShippingBillingDifferent = true;
                if (addressViewModel.IsAddressBook && addressViewModel.IsDefaultShipping && addressViewModel.IsDefaultBilling)
                {
                    addressViewModel.IsShippingBillingDifferent = false;
                }
            }

        }

        // Process credit card payment.
        protected virtual SubmitQuoteViewModel ProcessCreditCardPayment(SubmitQuoteViewModel submitQuoteViewModel, ShoppingCartModel cartModel)
        {
            SetUsersPaymentDetails(Convert.ToInt32(submitQuoteViewModel.PaymentSettingId), cartModel);
            submitQuoteViewModel.PaymentType = cartModel?.Payment?.PaymentName;

            GatewayResponseModel gatewayResponse = GetPaymentResponse(cartModel, submitQuoteViewModel);

            if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
            {
                return (SubmitQuoteViewModel)GetViewModelWithErrorMessage(new SubmitQuoteViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment);
            }

            //Map payment token
            cartModel.Token = gatewayResponse.Token;
            cartModel.IsGatewayPreAuthorize = gatewayResponse.IsGatewayPreAuthorize;

            return submitQuoteViewModel;
        }

        protected virtual void SetUsersPaymentDetails(int paymentSettingId, ShoppingCartModel model)
        {
            PaymentSettingModel paymentSetting = GetPaymentSettingFromCache(paymentSettingId, PortalAgent.CurrentPortal.PortalId);
            string paymentName = string.Empty;
            if (IsNotNull(paymentSetting))
                paymentName = paymentSetting.PaymentTypeName;

            model.Payment = PaymentViewModelMap.ToPaymentModel(model, paymentSetting, paymentName);
        }

        private PaymentSettingModel GetPaymentSettingFromCache(int paymentSettingId, int portalId, bool isPaymentApplication = false)
        {
            string cacheKey = (WebStoreConstants.PaymentSettingCacheKey + "_" + Convert.ToString(paymentSettingId) + "_" + Convert.ToString(portalId) + "_" + Convert.ToString(isPaymentApplication)).ToLower();
            PaymentSettingModel paymentSettingModel = null;
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                paymentSettingModel = _paymentClient.GetPaymentSetting(paymentSettingId, isPaymentApplication, new ExpandCollection { ZnodePaymentSettingEnum.ZnodePaymentType.ToString() }, portalId);
                Helper.AddIntoCache(paymentSettingModel, cacheKey, "CurrentPortalCacheDuration");
            }
            else
            {
                paymentSettingModel = Helper.GetFromCache<PaymentSettingModel>(cacheKey);
            }
            return paymentSettingModel;
        }
        // Get payment response.
        protected virtual GatewayResponseModel GetPaymentResponse(ShoppingCartModel cartModel, SubmitQuoteViewModel submitQuoteViewModel)
        {
            // Map shopping Cart model and submit Payment view model to Submit payment model 
            SubmitPaymentModel model = PaymentViewModelMap.ToModel(cartModel, submitQuoteViewModel);

            // Map Customer Payment Guid for Save Credit Card 
            if (!string.IsNullOrEmpty(submitQuoteViewModel.CustomerGuid) && string.IsNullOrEmpty(cartModel.UserDetails.CustomerPaymentGUID))
            {
                UserModel userModel = _userClient.GetUserAccountData(submitQuoteViewModel.UserId);
                userModel.CustomerPaymentGUID = submitQuoteViewModel.CustomerGuid;
                _userClient.UpdateCustomerAccount(userModel);

                UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                if (string.IsNullOrEmpty(userViewModel.CustomerPaymentGUID))
                {
                    userViewModel.CustomerPaymentGUID = submitQuoteViewModel.CustomerGuid;
                    SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
                }
            }

            model.Total = GetOrderTotal();

            return _paymentClient.PayNow(model);
        }
        //GetOrderTotal
        protected virtual string GetOrderTotal()
        {
            decimal? total = 0;
            //Get shopping Cart from Session or cookie
            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                           _cartAgent.GetCartFromCookie();

            if (HelperUtility.IsNotNull(shoppingCart))
                total = shoppingCart.Total;

            string strTotal = Convert.ToString(total).Replace(",", ".");
            return strTotal;
        }

        //Update the Session as per the new order total
        protected virtual void UpdateUserDetailsInSession(decimal? total)
        {
            UserViewModel user = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            if (IsNotNull(user) && IsNotNull(total))
            {
                decimal updatedBalance = user.AnnualBalanceOrderAmount - total.GetValueOrDefault();
                user.AnnualBalanceOrderAmount = updatedBalance <= 0 ? 0 : updatedBalance;
            }
            SaveInSession(WebStoreConstants.UserAccountKey, user);
        }

        //Check if user satisfy the per order limit
        protected virtual bool ValidateUserPerOrderBudget(UserViewModel user, decimal? total, bool enablePerOrderlimit)
        {
            bool isValidated = true;
            if (IsNotNull(user) && total > 0)
            {
                if (enablePerOrderlimit && user.PerOrderLimit > 0 && user.PerOrderLimit <= total)
                {
                    isValidated = false;
                }
            }
            return isValidated;
        }

        //Check if user satisfy the Annual budget
        protected virtual bool ValidateUserAnnualBudget(UserViewModel user, decimal? total, bool enableUserOrderAnnualLimit)
        {
            bool isValidated = true;
            if (IsNotNull(user) && total > 0)
            {
                if (enableUserOrderAnnualLimit && user.AnnualOrderLimit > 0 && (user.AnnualBalanceOrderAmount - total) <= 0)
                {
                    isValidated = false;
                }
            }
            return isValidated;
        }

        //To set quote type code
        protected virtual void SetQuoteType(ShoppingCartModel cartModel)
        {
            if (HelperUtility.IsNotNull(cartModel.Payment))
                cartModel.QuoteTypeCode = (cartModel?.Payment?.PaymentSetting?.IsOABRequired ?? false) ? ZnodeConstant.OAB : ZnodeConstant.Pending_Approval;
            else
                cartModel.QuoteTypeCode = ZnodeConstant.Quote;
        }
        //Check if user satisfy the Annual budget
        protected virtual LoginViewModel RegisterNewSocialLoginUser(ExternalLoginInfo loginInfo)
        {
            try
            {
                return UserViewModelMap.ToLoginViewModel(RegisterSocialLoginUser(loginInfo));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                return ReturnErrorModel(WebStore_Resources.AdminApprovalSuccessMessage, false, null, ex.ErrorCode);
            }
        }

        //Check if user is authenticated or not and user session is not null.
        protected virtual bool IsUserAuthenticated() => IsNotNull(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)) && HttpContext.Current.User.Identity.IsAuthenticated ? true : false;

        //Get return list for dashboard.
        protected virtual List<RMAReturnViewModel> GetReturnList() =>
             DependencyResolver.Current.GetService<IRMAReturnAgent>()?.GetReturnList(Filters, null, 1, WebStoreConstants.DashboardReturnListSize)?.ReturnList;

        //Bind voucher history data for dashboard.
        protected virtual void BindVouchersData(UserViewModel userViewModel)
        {
            List<VoucherViewModel> vouchers = GetVouchers().List;
            if (IsNotNull(vouchers))
            {
                userViewModel.VoucherCount = vouchers.Count;
                userViewModel.VoucherRemainingBalance = Helper.FormatPriceWithCurrency(vouchers.Sum(x => x.VoucherBalanceAmount), PortalAgent.CurrentPortal.CultureCode);
            }
        }

        //If Quantity is Less than MinQuantity and Greater Than MaxQuantity then return true otherwise false.
        protected virtual bool GetAvailableQuantity(ShoppingCartModel cartModel)
        {
            if (cartModel.ShoppingCartItems.Count > 0)
            {
                foreach (ShoppingCartItemModel ShoppingCartItemModel in cartModel.ShoppingCartItems)
                {
                    if (!(ShoppingCartItemModel.Quantity >= ShoppingCartItemModel.MinQuantity && ShoppingCartItemModel.Quantity <= ShoppingCartItemModel.MaxQuantity))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        //Set filter for User Id.
        private static void SetUserIdFilter(FilterCollection filters, int userId)
        {
            //Checking For UserId already Exists in Filters Or Not 
            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            return;
        }

        private OrdersListViewModel BindDataToOrderListViewModel(OrdersListModel orderList, int accountId, int userId)
        {
            OrdersListViewModel ordersListViewModel = new OrdersListViewModel() { List = orderList.Orders?.ToViewModel<OrdersViewModel>()?.ToList() };

            //Get Parent Account Details, to bind the details.
            ordersListViewModel.AccountName = GetAccountById(accountId)?.CompanyAccount?.Name;
            ordersListViewModel.AccountId = accountId;
            ordersListViewModel.UserId = userId;
            ordersListViewModel.HasParentAccounts = orderList.HasParentAccounts;
            SetListPagingData(ordersListViewModel, orderList);
            return ordersListViewModel;
        }
        #endregion
    }
}