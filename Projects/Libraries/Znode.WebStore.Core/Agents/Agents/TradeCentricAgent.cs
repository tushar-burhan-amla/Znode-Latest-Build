using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.WebStore.Agents
{
    public class TradeCentricAgent : BaseAgent, ITradeCentricAgent
    {
        #region Private Variables        
        private readonly IUserClient _userClient;
        private readonly ITradeCentricClient _tradeCentricClient;
        private readonly IShoppingCartClient _shoppingCartClient;
        private readonly IPaymentAgent _paymentAgent;
        private readonly ICheckoutAgent _checkoutAgent;
        private readonly ICartAgent _cartAgent;
        private readonly IUserAgent _userAgent;
        #endregion

        #region Constructor
        public TradeCentricAgent(IUserClient userClient, ITradeCentricClient tradeCentricClient, IShoppingCartClient shoppingCartClient, IPaymentAgent paymentAgent, ICheckoutAgent checkoutAgent, ICartAgent cartAgent, IUserAgent userAgent)
        {
            _userClient = GetClient<IUserClient>(userClient);
            _tradeCentricClient = GetClient<ITradeCentricClient>(tradeCentricClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _paymentAgent = paymentAgent;
            _checkoutAgent = checkoutAgent;
            _cartAgent = cartAgent;
            _userAgent =userAgent;
        }
        #endregion

        #region Public Methods
        //Gets the webstore url on portal id.
        public virtual string GetUserStoreSessionUrl(TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel)
        {
            try
            {
                string webStoreUrl = GetDomainUrl();
                ZnodeLogging.LogMessage("GetUserStoreSessionUrl method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                int portalId = PortalAgent.CurrentPortal?.PortalId ?? 0;

                //Get userId to update cart.
                int userId = GetUserId(tradeCentricSessionRequestViewModel);

                if (userId > 0)
                {
                    return GenerateLaunchUrl(portalId, userId, tradeCentricSessionRequestViewModel, webStoreUrl);
                }
                else
                {
                    ZnodeLogging.LogMessage("Unable to generate launch url.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.URLRedirectCreationFailed, WebStore_Resources.HttpCode_404_PageNotFoundMsg);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                return null;
            }
        }

        //Generate launch url.
        protected virtual string GenerateLaunchUrl(int portalId, int userId, TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel, string webStoreUrl)
        {
            ZnodeLogging.LogMessage("GenerateLaunchUrl method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            if (IsNotNull(tradeCentricSessionRequestViewModel))
            {
                //Generate encrypted token on the basis of CSRId, Webstore userId, webstore username.
                string launchToken = GenerateToken(portalId, userId, tradeCentricSessionRequestViewModel.Contact?.Email);

                //Get webstore url for the particular portal.
                if (string.IsNullOrEmpty(webStoreUrl) || string.IsNullOrEmpty(tradeCentricSessionRequestViewModel?.Contact?.Email) || portalId == 0)
                    return null;

                string launchUrl = $"{webStoreUrl}/PunchOut/InitializeSession?token={launchToken}&Operation={tradeCentricSessionRequestViewModel?.Operation}&selectedItem={tradeCentricSessionRequestViewModel?.Selected_item}";
                ZnodeLogging.LogMessage("Launch Url:", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose, new { launch_Url = launchUrl });
                return launchUrl;
            }
            ZnodeLogging.LogMessage("Model cannot be Null.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return null;
        }

        //Save tradecentric user data.
        protected virtual void SaveTradecentricUserDetails(TradeCentricUserModel tradeCentricUserModel, TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel)
        {
            ZnodeLogging.LogMessage("SaveTradecentricUserDetails method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose);

            TradeCentricUserModel tradeCentricUser = new TradeCentricUserModel()
            {
                TradeCentricUserId = IsNotNull(tradeCentricUserModel) ? tradeCentricUserModel.TradeCentricUserId : 0,
                OrganizationId = tradeCentricSessionRequestViewModel?.Custom?.Organization_Id ?? string.Empty,
                OrganizationName = tradeCentricSessionRequestViewModel?.Custom?.Organization_Name ?? string.Empty,
                ReturnUrl = tradeCentricSessionRequestViewModel?.Return_Url,
                UserId = tradeCentricUserModel.UserId
            };
            SaveTradeCentricUser(tradeCentricUser);
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
        }

        //Get tradecentric user details.
        protected virtual TradeCentricUserModel GetTradeCentricUser(TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel, int userId)
        {
            ZnodeLogging.LogMessage("ManageTradeCentricUser method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose);
            if (tradeCentricSessionRequestViewModel?.Items?.Count > 0)
            {
                string supplierAuxId = tradeCentricSessionRequestViewModel?.Items.FirstOrDefault().Supplier_aux_id;
                ShoppingCartModel shoppingCartModel = GetCartFromCookie(userId, supplierAuxId);
                RemoveCartItem(tradeCentricSessionRequestViewModel?.Items, shoppingCartModel);
                UpdateCartItem(tradeCentricSessionRequestViewModel?.Items, shoppingCartModel);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return _tradeCentricClient.GetTradeCentricUser(userId);
        }

        //Create customer account for tradecentric user.
        protected virtual int CreateCustomerAccount(TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel)
        {
            ZnodeLogging.LogMessage("CreateCustomerAccount method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose);
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.FirstName = tradeCentricSessionRequestViewModel?.Contact?.Name;
            registerViewModel.UserName = tradeCentricSessionRequestViewModel?.Contact?.Email;
            registerViewModel.IsWebStoreUser = true;
            registerViewModel.IsTradeCentricUser = true;
            UserModel userModel = _userClient.CreateCustomerAccount(UserViewModelMap.ToSignUpModel(registerViewModel));
            if (IsNull(userModel))
                throw new ZnodeException(ErrorCodes.NullModel, WebStore_Resources.NoDataFound);

            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return userModel.UserId;
        }

        //Get userId for tradecentric user.
        protected virtual int GetUserId(TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel)
        {
            ZnodeLogging.LogMessage("GetUserId method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            TradeCentricUserModel tradeCentricUserModel = new TradeCentricUserModel();

            //Check if user details exists.
            bool isUsernameExists = CheckIfUserNameExist(tradeCentricSessionRequestViewModel?.Contact?.Email);
            if (isUsernameExists)
            {
                UserModel userDetails = _userClient.GetAccountByUser(tradeCentricSessionRequestViewModel?.Contact?.Email);
                //Get user details from trade centric table if exists otherwise return null.
                tradeCentricUserModel = GetTradeCentricUser(tradeCentricSessionRequestViewModel, userDetails.UserId);
                tradeCentricUserModel.UserId = userDetails.UserId;
            }
            else
            {
                tradeCentricUserModel.UserId = CreateCustomerAccount(tradeCentricSessionRequestViewModel);
            }
            SaveTradecentricUserDetails(tradeCentricUserModel, tradeCentricSessionRequestViewModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return tradeCentricUserModel.UserId;
        }

        //Check if UserName is already exist.
        public virtual bool CheckIfUserNameExist(string username)
        {
            bool status = false;
            try
            {
                ZnodeLogging.LogMessage("CheckIfUserNameExist method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                username = username?.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.Username, FilterOperators.Is, username));

                ZnodeLogging.LogMessage("Parameters:", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose, new { filters = filters });
                status = _userClient.CheckIsUserNameAnExistingShopper(username);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Warning);

                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return true;
                    case ErrorCodes.CustomerAccountError:
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error);
                return false;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return status;
        }

        //Validate login from token value.
        public virtual LoginViewModel ValidateLogin(string token)
        {
            ZnodeLogging.LogMessage("ValidateLogin method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            LoginViewModel loginViewModel = new LoginViewModel();
            //Check token is valid or not.
            ImpersonationAPIModel impersonation = _userClient.ValidateCSRToken(token);
            if (IsNotNull(impersonation))
            {
                loginViewModel.HasError = !impersonation.Result;
                if (impersonation.Result)
                {
                    //Get user details.
                    UserModel userModel = _userClient.GetAccountByUser(impersonation.UserName);
                    loginViewModel.Username = impersonation.UserName;
                    loginViewModel.RememberMe = false;
                    loginViewModel.IsWebStoreUser = true;
                    SetLoginUserProfile(userModel);
                    if (userModel?.UserId > 0)
                    {
                        TradeCentricUserModel tradeCentricUser = _tradeCentricClient.GetTradeCentricUser(userModel.UserId);
                        if (IsNotNull(tradeCentricUser))
                        {
                            //Save tradecentric information in session.
                            SaveInSession(WebStoreConstants.TradeCentricSessionKey, tradeCentricUser);
                        }

                    }
                    //Save the user details in session.
                    SaveInSession(WebStoreConstants.UserAccountKey, userModel.ToViewModel<UserViewModel>());
                    ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                    return UserViewModelMap.ToLoginViewModel(userModel);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return loginViewModel;
        }

        // Transfer cart.
        public virtual string TransferCart(CartViewModel cartViewModel)
        {
            ZnodeLogging.LogMessage("TransferCart method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            string redirectUrl = string.Empty;
            if (cartViewModel?.ShoppingCartItems?.Count > 0)
            {
                try
                {
                    TradeCentricUserModel tradeCentricUser = MapModel(cartViewModel);
                    redirectUrl = _tradeCentricClient.TransferCart(tradeCentricUser);
                    ZnodeLogging.LogMessage("Redirect Url:", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose, new { Redirect_Url = redirectUrl });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return redirectUrl;
        }

        //Save tradecentric user details.
        public virtual bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel)
        {
            ZnodeLogging.LogMessage("SaveTradeCentricUser method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            bool flag = false;
            if (IsNotNull(tradeCentricUserModel))
                flag = _tradeCentricClient.SaveTradeCentricUser(tradeCentricUserModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return flag;
        }

        //Submit order for tradecentric user.
        public virtual OrdersViewModel SubmitOrder(TradeCentricOrderViewModel tradeCentricOrderViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("SubmitOrder method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                if (IsNull(tradeCentricOrderViewModel))
                {
                    ZnodeLogging.LogMessage(WebStore_Resources.TradeCentricUserModelNotNull, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.NoDataFound);
                }

                //Check if user details exist.
                if (!CheckIfUserNameExist(tradeCentricOrderViewModel.Details?.Contact?.Email))
                {
                    ZnodeLogging.LogMessage("User not found.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorTradeCentricUserNotExist);
                }

                //Get user Details by username.
                UserModel userDetails = _userClient.GetAccountByUser(tradeCentricOrderViewModel.Details?.Contact?.Email);
                if (IsNull(userDetails))
                {
                    ZnodeLogging.LogMessage("User not found.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.NoDataFound);
                }

                //Get shopping cart by cookie mapping id.
                ShoppingCartModel cartModel = GetCartFromCookie(userDetails.UserId, tradeCentricOrderViewModel?.Items?.FirstOrDefault()?.Supplier_aux_id);
                if (IsNull(cartModel))
                {
                    ZnodeLogging.LogMessage(WebStore_Resources.ErrorNoItemsShoppingCart, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorNoItemsShoppingCart);
                }

                //Remove cart items that are not available in tradecentric shopping cart.
                //Update cart items in shopping cart items.
                if (RemoveCartItem(tradeCentricOrderViewModel?.Items, cartModel) | UpdateCartItem(tradeCentricOrderViewModel?.Items, cartModel))
                    cartModel = GetCartFromCookie(userDetails.UserId, tradeCentricOrderViewModel?.Items?.FirstOrDefault()?.Supplier_aux_id);

                //Map required details to shopping cart model.
                cartModel = MapShoppingCartModel(cartModel, userDetails, tradeCentricOrderViewModel);

                //Proceed to place order.
                OrdersViewModel ordersViewModel = PlaceOrder(cartModel);

                //After placing order, delete the cart.
                if (IsNotNull(ordersViewModel) && !ordersViewModel.HasError)
                    DeleteCart(cartModel);

                //Save details in session.
                SaveInSession(WebStoreConstants.TradeCentricSessionKey, cartModel);

                ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                return ordersViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SubmitOrder method is " + ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                return new OrdersViewModel() { HasError = true, ErrorMessage = ex.Message };
            }
        }

        //Map details to shopping cart model.
        public virtual ShoppingCartModel MapShoppingCartModel(ShoppingCartModel cartModel, UserModel userDetails, TradeCentricOrderViewModel tradeCentricOrderViewModel)
        {
            //Map userdetails to ShoppingCartModel
            cartModel.UserDetails = userDetails;
            cartModel.UserId = userDetails?.UserId;

            //Generate order number.
            cartModel.OrderNumber = _checkoutAgent.GenerateOrderNumber(cartModel.PortalId);

            //Get address list by userId.
            List<AddressModel> userAddressList = GetAddressListByUserId(cartModel?.UserId);
            //Get billing and shipping addresses.
            cartModel.BillingAddress = GetBillingAddress(userAddressList, cartModel, tradeCentricOrderViewModel);
            cartModel.ShippingAddress = GetShippingAddress(userAddressList, cartModel, tradeCentricOrderViewModel);

            //Get payment details.
            cartModel.Payment = new PaymentModel();
            cartModel.Payment.PaymentSetting = GetPaymentDetails(cartModel);
            cartModel.Payment.PaymentDisplayName = cartModel.Payment?.PaymentSetting?.PaymentDisplayName;
            if (string.IsNullOrEmpty(cartModel.Payment.PaymentDisplayName))
            {
                ZnodeLogging.LogMessage(WebStore_Resources.ErrorConnectingPaymentMethod, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorPaymentNotAssociated);
            }

            //Get the shipping details associated with the store.
            //Get shipping id where shipping type is "Custom" and shipping code is 'FreeShipping'.
            cartModel.Shipping = new OrderShippingModel();
            int? shippingId = GetClient<ShippingClient>().GetAssociatedShippingList(new ExpandCollection() { ZnodeProfileShippingEnum.ZnodeShipping.ToString() }, new FilterCollection() { new FilterTuple(ZnodeConstant.PortalId, FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()) }, null, null, null)?.ShippingList?.FirstOrDefault(x => x.ShippingTypeName.Equals("Custom", StringComparison.InvariantCultureIgnoreCase) && x.ShippingCode.Equals("FreeShipping", StringComparison.InvariantCultureIgnoreCase))?.ShippingId;
            if (IsNull(shippingId))
            {
                ZnodeLogging.LogMessage("Shipping method not found.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NullModel, WebStore_Resources.ErrorShippingNotAssociated);
            }

            //Map shippingId to cartModel.
            cartModel.Shipping.ShippingId = (int)shippingId;
            cartModel.ShippingId = (int)shippingId;

            //Bind PO details.
            cartModel.PurchaseOrderNumber = tradeCentricOrderViewModel?.Header?.Po_order_id;

            //This is used to skip pre-submit order validation, since all the validations have already been validated at the checkout page.
            cartModel.SkipPreprocessing = true;

            return cartModel;
        }

        //Delete Cart.
        public virtual void DeleteCart(ShoppingCartModel cartModel)
        {
            if (_shoppingCartClient.RemoveAllCartItem(new CartParameterModel { UserId = cartModel.UserId.GetValueOrDefault(), CookieMappingId = cartModel.CookieMappingId, PortalId = cartModel.PortalId }))
                ZnodeLogging.LogMessage("Cart deleted successfully.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            else
                ZnodeLogging.LogMessage("Unable to delete the cart.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
        }

        //Load tradecentric Cart items in edit session
        public virtual void LoadUserCart(string operation)
        {
            ZnodeLogging.LogMessage("LoadUserCart method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            TradeCentricUserModel tradeCentricUser = GetFromSession<TradeCentricUserModel>(WebStoreConstants.TradeCentricSessionKey);
            if (IsNotNull(tradeCentricUser))
            {
                if (operation.Contains("create"))
                {
                    RemoveCookie(WebStoreConstants.CartCookieKey);
                    _shoppingCartClient.RemoveAllCartItem(new CartParameterModel { UserId = tradeCentricUser.UserId, CookieMappingId = GetCookieMappingId(), PortalId = PortalAgent.CurrentPortal.PortalId });
                    SaveInSession(WebStoreConstants.CartModelSessionKey, new ShoppingCartModel());
                    _cartAgent.ClearCartCountFromSession();
                }
                else
                {
                    ShoppingCartModel cart = _cartAgent.GetCartFromCookie();

                    if (cart?.ShoppingCartItems?.Count > 0)
                    {
                        ZnodeLogging.LogMessage("TradeCentric Cart Count:", ZnodeConstant.TradeCentricComponent.ToString(), TraceLevel.Verbose, new { CartCount = cart?.ShoppingCartItems?.Count });
                        SaveInSession(WebStoreConstants.CartModelSessionKey, cart);
                    }
                }
                ZnodeLogging.LogMessage("LoadPunchOutUserCart method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            }
        }

        #endregion

        #region Protected Methods
        // Map tradecentric model.
        protected virtual TradeCentricUserModel MapModel(CartViewModel cartViewModel)
        {
            TradeCentricUserModel tradeCentricUser = new TradeCentricUserModel();
            tradeCentricUser = _tradeCentricClient.GetTradeCentricUser(cartViewModel.UserId);
            tradeCentricUser.CartModel = new TradeCentricCartModel();
            string cookieValue = GetCookieMappingId();
            tradeCentricUser.CookieMappingKey = cookieValue;
            tradeCentricUser.CartModel.Total = cartViewModel?.Total ?? 0;
            tradeCentricUser.UserId = cartViewModel.UserId;
            cartViewModel?.ShoppingCartItems?.ForEach(shoppingCartItem =>
            {
                TradeCentricCartItemModel tradeCentricCartItemModel = new TradeCentricCartItemModel();
                if (shoppingCartItem.GroupProducts?.Count == 0)
                {
                    tradeCentricCartItemModel = MapCartItemModel(tradeCentricCartItemModel, shoppingCartItem);
                    tradeCentricUser.CartModel.Items.Add(tradeCentricCartItemModel);
                }
                else
                {
                    //Get group product.
                    GetGroupProducts(shoppingCartItem, tradeCentricUser?.CartModel);
                }
            });
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return tradeCentricUser;
        }

        // Map user cart.
        protected virtual TradeCentricCartItemModel MapCartItemModel(TradeCentricCartItemModel tradeCentricCartItemModel, CartItemViewModel cartItemViewModel)
        {
            ZnodeLogging.LogMessage("MapPunchOutCartItemModel method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            if (!string.IsNullOrEmpty(cartItemViewModel?.ConfigurableProductSKUs))
                tradeCentricCartItemModel.Supplier_id = cartItemViewModel?.ConfigurableProductSKUs;
            else if (cartItemViewModel?.GroupProducts?.Count > 0)
                tradeCentricCartItemModel.Supplier_id = cartItemViewModel?.GroupProducts?.FirstOrDefault()?.Sku;
            else
                tradeCentricCartItemModel.Supplier_id = cartItemViewModel.SKU;

            tradeCentricCartItemModel.Options = new TradeCentricAdditionalOptionsModel();
            tradeCentricCartItemModel.Options.ParentSupplierId = string.IsNullOrEmpty(tradeCentricCartItemModel?.Options?.ParentSupplierId) ? cartItemViewModel?.SKU : tradeCentricCartItemModel?.Options?.ParentSupplierId;
            tradeCentricCartItemModel.Supplier_aux_id = GetCookieMappingId();
            tradeCentricCartItemModel.Description = cartItemViewModel?.ProductName;
            tradeCentricCartItemModel.Uom = "EA";
            tradeCentricCartItemModel.Classification = cartItemViewModel?.ProductCode;
            tradeCentricCartItemModel.Unitprice = cartItemViewModel.UnitPrice;
            tradeCentricCartItemModel.Quantity = cartItemViewModel.Quantity;
            tradeCentricCartItemModel.CurrencyCode = cartItemViewModel?.CurrencyCode;
            return tradeCentricCartItemModel;
        }

        // Get group products.
        protected virtual void GetGroupProducts(CartItemViewModel cartItemViewModel, TradeCentricCartModel tradeCentricCart)
        {
            ZnodeLogging.LogMessage("GetGroupProducts method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            AssociatedProductModel groupProduct = cartItemViewModel.GroupProducts?.FirstOrDefault();

            CartItemViewModel groupItemViewModel = new CartItemViewModel();
            groupItemViewModel.SKU = groupProduct?.Sku;
            groupItemViewModel.ProductCode = cartItemViewModel?.ProductCode;
            groupItemViewModel.ProductName = groupProduct?.ProductName;
            groupItemViewModel.UOM = cartItemViewModel?.UOM;
            groupItemViewModel.UnitPrice = Convert.ToDecimal(groupProduct.UnitPrice);
            groupItemViewModel.Quantity = groupProduct.Quantity;
            groupItemViewModel.CurrencyCode = cartItemViewModel?.CurrencyCode;

            TradeCentricCartItemModel tradeCentricCartItemModel = new TradeCentricCartItemModel();
            tradeCentricCartItemModel.Options = new TradeCentricAdditionalOptionsModel();
            tradeCentricCartItemModel.Options.ParentSupplierId = cartItemViewModel?.SKU;
            MapCartItemModel(tradeCentricCartItemModel, groupItemViewModel);
            tradeCentricCart.Items.Add(tradeCentricCartItemModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);

        }

        // Get cookie mapping ID.
        protected virtual string GetCookieMappingId()
        {
            ZnodeLogging.LogMessage("GetCookieMappingId method execution started.", WebStoreConstants.TradeCentricComponent, TraceLevel.Info);
            string cookieMappingId = GetFromCookie(WebStoreConstants.CartCookieKey);

            if (string.IsNullOrEmpty(cookieMappingId))
            {
                ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
                if (IsNotNull(cartModel))
                {
                    cookieMappingId = cartModel.CookieMappingId;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return cookieMappingId;
        }

        //To set login user profile id.
        protected virtual void SetLoginUserProfile(UserModel userModel)
        {
            ZnodeLogging.LogMessage("SetLoginUserProfile method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            UserViewModel currentUser = GetUserViewModelFromSession();
            if (currentUser?.Profiles.Count > 0)
                userModel.ProfileId = Helper.GetProfileId().GetValueOrDefault();
            else
                userModel.ProfileId = userModel?.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.ProfileId ?? 0;
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
        }

        //Get user view model from session.
        protected virtual UserViewModel GetUserViewModelFromSession() => IsUserAuthenticated() ? GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) : GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);

        //Check if user is authenticated or not and user session is not null.
        protected virtual bool IsUserAuthenticated() => IsNotNull(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)) && HttpContext.Current.User.Identity.IsAuthenticated;


        //Method gets the domain base url.
        protected virtual string GetDomainUrl()
            => (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)))
            ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;

        //Generate encrypted token on the basis of userid and username.
        protected virtual string GenerateToken(int portalId, int userId, string userName)
        {
            ZnodeLogging.LogMessage("GenerateToken method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            string encryptedToken = ZnodeTokenHelper.GenerateCSRToken(portalId, userId, userName);
            //Browser's URL decoding converts "+" sign into space character.
            //This code will replace + sign with space.
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return encryptedToken.Replace("+", "%2B");
        }

        protected virtual List<AddressModel> GetAddressListByUserId(int? userId)
        {
            if (IsNotNull(userId) && userId > 0)
            {
                FilterCollection billingFilters = new FilterCollection();
                billingFilters.Add(new FilterTuple(ZnodeDepartmentUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

                return GetClient<CustomerClient>().GetAddressList(expands, billingFilters, null, null, null)?.AddressList;
            }
            ZnodeLogging.LogMessage("UserId cannot be null.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
            throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.HttpCode_404_PageNotFoundMsg);
        }

        //Get billing address for tradeCentric user.
        protected virtual AddressModel GetBillingAddress(List<AddressModel> userAddressList, ShoppingCartModel cartModel, TradeCentricOrderViewModel tradeCentricOrderViewModel)
        {
            ZnodeLogging.LogMessage("GetBillingAddress execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            List<AddressModel> billingAddressList = userAddressList?.Where(x => x.ExternalId == tradeCentricOrderViewModel?.Details?.Bill_to?.Address_id && x.IsBilling == true)?.ToList();

            //Insert billing address in ZnodeAddress table.
            if (!IsNotNull(billingAddressList) || billingAddressList?.Count == 0)
            {
                cartModel.BillingAddress = new AddressModel();
                cartModel.BillingAddress.IsBilling = true;
                cartModel.BillingAddress.DisplayName = tradeCentricOrderViewModel?.Details?.Bill_to?.Address_name;
                cartModel.BillingAddress.CityName = tradeCentricOrderViewModel?.Details?.Bill_to?.City;
                cartModel.BillingAddress.StateName = tradeCentricOrderViewModel?.Details?.Bill_to?.State;
                cartModel.BillingAddress.PostalCode = tradeCentricOrderViewModel?.Details?.Bill_to?.Postalcode;
                cartModel.BillingAddress.CountryName = tradeCentricOrderViewModel?.Details?.Bill_to?.Country_code;
                cartModel.BillingAddress.CountryCode = tradeCentricOrderViewModel?.Details?.Bill_to?.Country_code;
                cartModel.BillingAddress.EmailAddress = tradeCentricOrderViewModel?.Details?.Bill_to?.Email;
                cartModel.BillingAddress.PhoneNumber = tradeCentricOrderViewModel?.Details?.Bill_to?.Telephone;
                cartModel.BillingAddress.FirstName = tradeCentricOrderViewModel?.Details?.Contact?.Name?.Split(' ')[0];
                cartModel.BillingAddress.LastName = tradeCentricOrderViewModel?.Details?.Contact?.Name?.Split(' ')[1];
                cartModel.BillingAddress.UserId = cartModel.UserDetails.UserId;
                cartModel.BillingAddress.ExternalId = tradeCentricOrderViewModel?.Details?.Bill_to?.Address_id;
                cartModel.BillingAddress.Address1 = tradeCentricOrderViewModel?.Details?.Bill_to?.Street;

                //Insert and get trade centric user billing address.
                return GetClient<WebStoreUserClient>().CreateAccountAddress(cartModel?.BillingAddress);
            }
            ZnodeLogging.LogMessage("Execution done.", WebStoreConstants.TradeCentricComponent, TraceLevel.Info);
            return billingAddressList?.FirstOrDefault();
        }

        //Get shipping address for tradeCentric user.
        protected virtual AddressModel GetShippingAddress(List<AddressModel> userAddressList, ShoppingCartModel cartModel, TradeCentricOrderViewModel tradeCentricOrderViewModel)
        {
            ZnodeLogging.LogMessage("GetShippingAddress method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            List<AddressModel> shippingAddressList = userAddressList?.Where(x => x.ExternalId == tradeCentricOrderViewModel?.Details?.Ship_to?.Address_id && x.IsShipping == true)?.ToList();

            //Insert shipping address in ZnodeAddress table.
            if (!IsNotNull(shippingAddressList) || shippingAddressList?.Count == 0)
            {
                cartModel.ShippingAddress = new AddressModel();
                cartModel.ShippingAddress.IsShipping = true;
                cartModel.ShippingAddress.DisplayName = tradeCentricOrderViewModel?.Details?.Ship_to?.Address_name;
                cartModel.ShippingAddress.CityName = tradeCentricOrderViewModel?.Details?.Ship_to?.City;
                cartModel.ShippingAddress.StateName = tradeCentricOrderViewModel?.Details?.Ship_to?.State;
                cartModel.ShippingAddress.PostalCode = tradeCentricOrderViewModel?.Details?.Ship_to?.Postalcode;
                cartModel.ShippingAddress.CountryName = tradeCentricOrderViewModel?.Details?.Ship_to?.Country_code;
                cartModel.ShippingAddress.CountryCode = tradeCentricOrderViewModel?.Details?.Ship_to?.Country_code;
                cartModel.ShippingAddress.EmailAddress = tradeCentricOrderViewModel?.Details?.Ship_to?.Email;
                cartModel.ShippingAddress.PhoneNumber = tradeCentricOrderViewModel?.Details?.Ship_to?.Telephone;
                cartModel.ShippingAddress.FirstName = tradeCentricOrderViewModel?.Details?.Contact?.Name?.Split(' ')[0];
                cartModel.ShippingAddress.LastName = tradeCentricOrderViewModel?.Details?.Contact?.Name?.Split(' ')[1];
                cartModel.ShippingAddress.UserId = cartModel.UserDetails.UserId;
                cartModel.ShippingAddress.ExternalId = tradeCentricOrderViewModel?.Details?.Ship_to?.Address_id;
                cartModel.ShippingAddress.Address1 = tradeCentricOrderViewModel?.Details?.Ship_to?.Street;

                //Insert and get trade centric user shipping address.
                return GetClient<WebStoreUserClient>().CreateAccountAddress(cartModel?.ShippingAddress);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return shippingAddressList?.FirstOrDefault();
        }

        //Get payment Details for type "purchase order" only.
        protected virtual PaymentSettingModel GetPaymentDetails(ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("GetPaymentDetails method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            UserPaymentSettingModel userPaymentSettingModel = new UserPaymentSettingModel();
            userPaymentSettingModel.UserId = (int)cartModel.UserId;
            userPaymentSettingModel.PortalId = cartModel.PortalId;
            List<PaymentSettingModel> paymentSetting = _paymentAgent.GetPaymentSettingByUserDetailsFromCache(userPaymentSettingModel, false, false);
            if (IsNull(paymentSetting) || paymentSetting.Count == 0)
            {
                ZnodeLogging.LogMessage("Payment details not found.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NullModel, WebStore_Resources.NoPaymentAssociated);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return paymentSetting?.FirstOrDefault(x => x.PaymentTypeName.Equals(WebStoreConstants.PurchaseOrder, StringComparison.InvariantCultureIgnoreCase));
        }

        //Get shopping cart by cookie mapping id.
        protected virtual ShoppingCartModel GetCartFromCookie(int userId, string supplierAuxId)
        {
            ZnodeLogging.LogMessage("GetCartFromCookie method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            ShoppingCartModel shoppingCartModel = new ShoppingCartModel();
            try
            {
                shoppingCartModel = _shoppingCartClient.GetShoppingCart(new CartParameterModel
                {
                    CookieMappingId = supplierAuxId,
                    LocaleId = PortalAgent.LocaleId,
                    PortalId = PortalAgent.CurrentPortal.PortalId,
                    PublishedCatalogId = GetCatalogId().GetValueOrDefault(),
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
            }
            ZnodeLogging.LogMessage("Execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return shoppingCartModel;
        }

        //Place order for tradecentric user.
        protected virtual OrdersViewModel PlaceOrder(ShoppingCartModel cartModel)
        {
            try
            {
                if (IsNotNull(cartModel))
                {
                    ZnodeLogging.LogMessage("PlaceOrder method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                    //If transactiondate is null then set current date as transaction date.
                    if (cartModel?.TransactionDate == null)
                        cartModel.TransactionDate = HelperUtility.GetDateWithTime();
                    //As Complete data is already available in shopping cart model,calculate call is avoided.
                    cartModel.SkipCalculations = true;
                    //Set the default InHandDate to 30 days from now.
                    cartModel.InHandDate = Convert.ToDateTime(DateTime.Now.Date.AddDays(WebStoreConstants.DefaultInHandDays).ToShortDateString());
                    //Place the order.
                    GetClient<OrderClient>().SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());

                    ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                    return GetClient<OrderClient>().CreateOrder(cartModel)?.ToViewModel<OrdersViewModel>();
                }
                ZnodeLogging.LogMessage("Shopping cart cannot be null.", ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);

                //Set error message according to ErrorCode.
                switch (exception.ErrorCode)
                {
                    case ErrorCodes.ProcessingFailed:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
                    case ErrorCodes.ErrorSendResetPasswordLink:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel() { }, WebStore_Resources.ErrorOrderEmailNotSend);
                    case ErrorCodes.OutOfStockException:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel() { }, WebStore_Resources.OutOfStockException);
                    case ErrorCodes.AllowedTerritories:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel() { }, WebStore_Resources.AllowedTerritoriesError);
                    default:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Remove cart item from shopping cart.
        protected virtual bool RemoveCartItem(List<TradeCentricCartItemViewModel> tradeCentricCartItemViewModelList, ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("RemoveCartItem method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            List<string> skus = tradeCentricCartItemViewModelList?.Select(x => x.Supplier_id)?.ToList();
            List<string> removedSkus = new List<string>();
            string sku = string.Empty;
            bool isCartItemRemoved = false;
            if (IsNotNull(shoppingCartModel))
                removedSkus = (shoppingCartModel?.ShoppingCartItems?.Where(x => x.GroupProducts.Count == 0 || x.ConfigurableProductSKUs == string.Empty)?.Select(x => x.SKU))?.ToList()?.Except(skus)?.ToList();

            foreach (ShoppingCartItemModel shoppingCartItemModel in shoppingCartModel.ShoppingCartItems)
            {
                sku = shoppingCartItemModel?.SKU;
                if (shoppingCartItemModel?.GroupProducts?.Count > 0)
                    sku = shoppingCartItemModel?.GroupProducts?.FirstOrDefault()?.Sku;
                else if (!string.IsNullOrEmpty(shoppingCartItemModel?.ConfigurableProductSKUs))
                    sku = shoppingCartItemModel?.ConfigurableProductSKUs;

                if (RemoveItem(tradeCentricCartItemViewModelList, shoppingCartItemModel, sku))
                    isCartItemRemoved = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return isCartItemRemoved;
        }

        //Remove cart item line from shopping cart.
        protected virtual bool RemoveItem(List<TradeCentricCartItemViewModel> tradeCentricCartItemViewModelList, ShoppingCartItemModel shoppingCartItemModel, string sku)
        {
            ZnodeLogging.LogMessage("RemoveItem method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            if (!tradeCentricCartItemViewModelList.Any(x => x.Supplier_id == sku))
                return _shoppingCartClient.RemoveCartLineItem(Convert.ToInt32(shoppingCartItemModel?.OmsSavedcartLineItemId));
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return false;
        }

        //Update quantity of cart item.
        protected virtual bool UpdateCartItem(List<TradeCentricCartItemViewModel> tradeCentricCartItemViewModelList, ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("UpdateCartItem method execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            bool isUpdateRequired = false;

            //Update quantity and update the cart.
            foreach (ShoppingCartItemModel cartItem in shoppingCartModel.ShoppingCartItems)
            {
                decimal cartQuantity = cartItem.Quantity;
                string sku = cartItem?.SKU;
                bool isGroupProduct = false;
                if (cartItem?.GroupProducts?.Count > 0)
                {
                    sku = cartItem?.GroupProducts?.FirstOrDefault()?.Sku;
                    cartQuantity = cartItem.GroupProducts.FirstOrDefault().Quantity;
                    isGroupProduct = true;
                }
                else if (!string.IsNullOrEmpty(cartItem?.ConfigurableProductSKUs))
                    sku = cartItem?.ConfigurableProductSKUs;

                decimal tradeCentricQuantity = Convert.ToDecimal(tradeCentricCartItemViewModelList?.FirstOrDefault(x => x.Supplier_id == sku)?.Quantity);
                if (!Equals(cartQuantity, tradeCentricQuantity))
                {
                    if (isGroupProduct)
                        cartItem.GroupProducts.FirstOrDefault(x => x.Sku == sku).Quantity = tradeCentricQuantity;
                    else
                        cartItem.Quantity = tradeCentricQuantity;

                    isUpdateRequired = true;
                }
            }
            if (isUpdateRequired)
                _shoppingCartClient.CreateCart(shoppingCartModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return isUpdateRequired;
        }
        #endregion
    }
}
