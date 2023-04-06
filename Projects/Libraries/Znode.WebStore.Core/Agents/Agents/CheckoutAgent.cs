using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
using Znode.Libraries.Klaviyo.Helper;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Agents
{
    public class CheckoutAgent : BaseAgent, ICheckoutAgent
    {
        #region protected Variables
        protected readonly IShippingClient _shippingsClient;
        protected readonly IPaymentClient _paymentClient;
        protected readonly IPortalProfileClient _profileClient;
        protected readonly ICustomerClient _customerClient;
        protected readonly IUserClient _userClient;
        protected readonly IOrderClient _orderClient;
        protected readonly ICartAgent _cartAgent;
        protected readonly IUserAgent _userAgent;
        protected readonly IPaymentAgent _paymentAgent;
        protected readonly IAccountClient _accountClient;
        protected readonly IWebStoreUserClient _webStoreAccountClient;
        protected readonly IPortalClient _portalClient;
        protected readonly IShoppingCartClient _shoppingCartClient;
        protected readonly IAddressAgent _addressAgent;
        #endregion

        public CheckoutAgent(IShippingClient shippingsClient, IPaymentClient paymentClient, IPortalProfileClient profileClient, ICustomerClient customerClient, IUserClient userClient, IOrderClient orderClient, IAccountClient accountClient, IWebStoreUserClient webStoreAccountClient, IPortalClient portalClient, IShoppingCartClient shoppingCartClient, IAddressClient addressClient)
        {
            _shippingsClient = GetClient<IShippingClient>(shippingsClient);
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _profileClient = GetClient<IPortalProfileClient>(profileClient);
            _customerClient = GetClient<ICustomerClient>(customerClient);
            _userClient = GetClient<IUserClient>(userClient);
            _orderClient = GetClient<IOrderClient>(orderClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _webStoreAccountClient = GetClient<IWebStoreUserClient>(webStoreAccountClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
            _userAgent = GetService<IUserAgent>();
            _cartAgent = GetService<ICartAgent>();
            _paymentAgent = GetService<IPaymentAgent>();
            _addressAgent = GetService<IAddressAgent>();
        }

        #region Public Methods
        //Bind shipping option list.
        public virtual ShippingOptionListViewModel GetShippingOptions(string shippingTypeName = null, bool isQuote = false)
        {
            List<ShippingOptionViewModel> shippingOptions;
            bool isB2BUser = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.AccountId > 0;

            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                             _cartAgent.GetCartFromCookie();

            int omsQuoteId = (cartModel?.OmsQuoteId).GetValueOrDefault();
            List<ShippingOptionModel> shippingOptionList = new List<ShippingOptionModel>();
            try
            {
                if (IsNotNull(cartModel) && !string.IsNullOrEmpty(shippingTypeName))
                    SetShippingTypeNameToModel(shippingTypeName, cartModel);

                //Get address associated to the cart, If it is not available then get address from user address book.
                AddressListViewModel addressList = GetCartAddressList(cartModel);
                if (addressList?.ShippingAddress == null || addressList?.ShippingAddress?.AddressId == 0)
                    addressList = _userAgent.GetAddressList();

                if (!IsValidShippingAddress(addressList))
                    return new ShippingOptionListViewModel() { IsB2BUser = isB2BUser, OmsQuoteId = omsQuoteId };

                cartModel.BillingAddress = addressList?.BillingAddress?.ToModel<AddressModel>();
                cartModel.Payment = new PaymentModel { ShippingAddress = addressList?.ShippingAddress?.ToModel<AddressModel>() };
                cartModel.IsCalculatePromotionAndCoupon = isQuote ? false : true;

                shippingOptions = GetShippingListAndRates(addressList?.ShippingAddress?.PostalCode, cartModel)?.ShippingOptions;
                shippingOptionList = shippingOptions.ToModel<ShippingOptionModel>().ToList();

                if (cartModel?.Shipping?.ShippingId > 0)
                {
                    shippingOptions?.Where(x => x.ShippingId == cartModel?.Shipping.ShippingId)?.Select(y => { y.IsSelected = true; return y; })?.ToList();
                }

                //if not any shipping method was selected then Shipping values set to 0
                if (IsNotNull(shippingOptions))
                {
                    if (shippingOptions.All(x => x.IsSelected == false))
                    {
                        cartModel.ShippingCost = cartModel.ShippingHandlingCharges = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "GetShippingOptions", TraceLevel.Error);
                shippingOptions = new List<ShippingOptionViewModel>();
            }

            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            shoppingCart.ShippingOptions = shippingOptionList;

            if (shoppingCart?.ShoppingCartItems?.Count > 0)
            {
                shippingOptions.Where(x => x.ShippingId == shoppingCart.ShippingId)?.Select(y => { y.IsSelected = true; shoppingCart.Shipping.ShippingId = y.ShippingId; return y; }).FirstOrDefault();
                if (shoppingCart.ShoppingCartItems.Any(x => x.Quantity > 500 && shippingOptions?.Count() == 0))
                {
                    SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, shoppingCart);
                    return new ShippingOptionListViewModel() { ShippingOptions = shippingOptions, IsB2BUser = isB2BUser, OmsQuoteId = omsQuoteId, ErrorMessage = Admin_Resources.ErrorShippingExceeded, HasError = true };
                }
            }
            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, shoppingCart);
            return new ShippingOptionListViewModel() { ShippingOptions = shippingOptions, IsB2BUser = isB2BUser, OmsQuoteId = omsQuoteId };
        }



        //If shippingTypeNamehas value assign it to model.
        protected virtual void SetShippingTypeNameToModel(string shippingTypeName, ShoppingCartModel cartModel)
        {
            if (IsNull(cartModel?.Shipping))
            {
                cartModel.Shipping = new OrderShippingModel();
            }

            cartModel.Shipping.ShippingTypeName = shippingTypeName;
        }

        //Get shipping options and its rates.
        protected virtual ShippingOptionListViewModel GetShippingListAndRates(string postalCode, ShoppingCartModel cartModel)
        {
            string zipCode = postalCode;

            cartModel.PublishStateId = DefaultSettingHelper.GetCurrentOrDefaultAppType(PortalAgent.CurrentPortal.PublishState);
            ShippingOptionListViewModel listViewModel = new ShippingOptionListViewModel { ShippingOptions = _shoppingCartClient.GetShippingEstimates(zipCode, cartModel)?.ShippingList?.ToViewModel<ShippingOptionViewModel>()?.ToList() };
            string cultureCode = PortalAgent.CurrentPortal.CultureCode;

            listViewModel?.ShippingOptions?.ToList().ForEach(x => x.FormattedShippingRate = HelperMethods.FormatPriceWithCurrency(x.ShippingRate, cultureCode));
            listViewModel?.ShippingOptions?.ToList().ForEach(x => x.ShippingRate = (x.ShippingRate - x.HandlingCharge));

            listViewModel?.ShippingOptions?.ToList().ForEach(x => x.FormattedShippingRateWithoutDiscount = (x?.ShippingRateWithoutDiscount > 0) ? HelperMethods.FormatPriceWithCurrency(x?.ShippingRateWithoutDiscount, cultureCode) : string.Empty);
            return listViewModel?.ShippingOptions?.Count > 0 ? listViewModel : new ShippingOptionListViewModel() { ShippingOptions = new List<ShippingOptionViewModel>() };
        }

        public virtual List<BaseDropDownOptions> PaymentOptions(bool isUsedForOfflinePayment = false, bool isQuotes = false)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId)));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(GetUserUserIdFromSession())));
            filters.Add(new FilterTuple(FilterKeys.ProfileId, FilterOperators.Equals, Convert.ToString(Helper.GetProfileId().GetValueOrDefault())));
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, FilterOperators.Equals, "true"));

            List<PaymentSettingModel> paymentSettingList;

            if (!isUsedForOfflinePayment)
            {
                paymentSettingList = _paymentAgent.GetPaymentSettingByUserDetailsFromCache(_paymentAgent.GetUserPaymentSettingDetails(), isUsedForOfflinePayment, isQuotes);
                return IsNotNull(paymentSettingList) ? GetPaymentOptions(paymentSettingList) : new List<BaseDropDownOptions>();
            }

            filters.Add(new FilterTuple(WebStoreConstants.FilterforOfflinePayment, FilterOperators.Equals, isUsedForOfflinePayment.ToString()));

            SortCollection sort = new SortCollection();
            sort.Add(FilterKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            if (DefaultSettingHelper.IsDataSeparationAllowedforAppType())
            {
                if (PortalAgent.CurrentPortal.PublishState == ZnodePublishStatesEnum.PRODUCTION)
                    filters.Add(new FilterTuple(FilterKeys.PublishState, FilterOperators.In, Convert.ToString(PortalAgent.CurrentPortal.PublishState)));
            }

            paymentSettingList = _paymentAgent.GetPaymentSettingListFromCache(PortalAgent.CurrentPortal.PortalId, Helper.GetProfileId().GetValueOrDefault(), filters, sort, isUsedForOfflinePayment);

            return IsNotNull(paymentSettingList) ? GetPaymentOptions(paymentSettingList) : new List<BaseDropDownOptions>();
        }

        //Update address details.
        public virtual AddressViewModel UpdateSearchAddress(AddressViewModel viewModel)
        {
            if (IsNotNull(viewModel))
            {
                viewModel.Address3 = viewModel.Address2;
                viewModel.Address2 = viewModel.Address1;
                viewModel.Address1 = viewModel.DisplayName;

                AddressViewModel addressViewModel = _customerClient.UpdateSearchAddress(viewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();

                //Update cart session.
                if (IsNotNull(addressViewModel))
                {
                    UpdateChangedAddressWithCart(addressViewModel, addressViewModel);
                    return addressViewModel;
                }
            }
            return new AddressViewModel();
        }
        
        public virtual OrdersViewModel SubmitOrder(SubmitOrderViewModel submitOrderViewModel)
        {
            try
            {
                if (IsNull(submitOrderViewModel))
                {
                    ZnodeLogging.LogMessage("The submit order model is null", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                    return new OrdersViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
                }

                //Get deep copy of the cart to remove the reference type dependency of session object or by cookie.
                ShoppingCartModel cartModel = GetCloneFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                              _cartAgent.GetCartFromCookie();

                if (IsNull(cartModel))
                {
                    ZnodeLogging.LogMessage("The session shopping cart model is null", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                    return new OrdersViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
                }

                //Condition for checking Available Quantity
                if (GetAvailableQuantity(cartModel))
                {
                    return new OrdersViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ExceedingAvailableWithoutQuantity };
                }

                UserViewModel userViewModel = GetCloneFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                //Set IsQuoteOrder true if quote id is greater than zero or user permission access is does not require approver.
                if (IsNotNull(userViewModel))
                {
                    userViewModel.CreatedDate = string.Empty;
                    SetIsQuoteOrder(cartModel, userViewModel);
                    string message = string.Empty;
                    if (!_userAgent.ValidateUserBudget(out message))
                    {
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), message);
                    }
                    UpdateUserDetailsInSession(cartModel, userViewModel);
                }

                RemoveInvalidDiscountCode(cartModel);

                UserViewModel user = (IsNotNull(userViewModel) && userViewModel.UserId > 0) ? userViewModel : GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);

                //Get the payment details.
                GetPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel);

                //Check if billing address of cart model is not null then create it billing address of guest user.
                if (IsNotNull(cartModel.BillingAddress) && !Convert.ToBoolean(cartModel.ShippingAddress.IsDefaultBilling))
                {
                    if (cartModel.Payment?.PaymentSetting?.IsBillingAddressOptional == true)
                    {
                        cartModel.BillingAddress = cartModel.ShippingAddress;
                        cartModel.BillingAddress.AddressId = 0;
                        cartModel.BillingAddress.IsBilling = true;
                        cartModel.BillingAddress.IsDefaultBilling = true;
                    }
                }

                cartModel.OrderNumber = !string.IsNullOrEmpty(submitOrderViewModel.OrderNumber) ? submitOrderViewModel.OrderNumber
                                        : GenerateOrderNumber(cartModel.PortalId);

                if (IsNull(user) || user?.UserId < 1)
                {
                    if (IsAmazonPayEnable(submitOrderViewModel))
                    {
                        SetUsersPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel, true);
                        SetAmazonAddress(submitOrderViewModel, cartModel);
                        user = CreateAnonymousUserAccount(cartModel.BillingAddress, cartModel.ShippingAddress?.EmailAddress);
                        UserViewModel oldSession = GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
                        if (!Equals(oldSession, null))
                        {
                            oldSession.GuestUserId = oldSession.UserId;
                            if (IsNotNull(userViewModel))
                                userViewModel.UserId = oldSession.UserId;
                            SaveInSession(WebStoreConstants.GuestUserKey, oldSession);
                        }
                    }
                    else
                    {
                        user = CreateAnonymousUserAccount(cartModel.BillingAddress, cartModel.ShippingAddress?.EmailAddress);
                    }
                }

                //Get the list of all addresses associated to current logged in user.
                List<AddressModel> userAddresses = GetUserAddressList();

                if (IsNull(userAddresses) || userAddresses.Count < 1)
                {
                    if (IsAmazonPayEnable(submitOrderViewModel) && !Equals(cartModel.ShippingAddress, null) && (string.IsNullOrEmpty(cartModel.ShippingAddress.Address1) || string.IsNullOrEmpty(cartModel.ShippingAddress.FirstName)))
                    {
                        SetUsersPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel, true);
                        SetAmazonAddress(submitOrderViewModel, cartModel);
                        cartModel.ShippingAddress.IsDefaultBilling = true;
                        cartModel.ShippingAddress.IsDefaultShipping = true;
                    }
                    if (!string.IsNullOrEmpty(cartModel?.Payment?.PaymentName) && Equals(cartModel.Payment.PaymentName.Replace("_", "").ToLower(), ZnodeConstant.PayPalExpress.ToLower()))
                    {
                        if (string.IsNullOrEmpty(submitOrderViewModel.PayPalToken))
                        {
                            userAddresses = GetAnonymousUserAddresses(cartModel, submitOrderViewModel);
                        }
                    }
                    else if (IsNotNull(submitOrderViewModel?.PaymentType) && Equals(submitOrderViewModel?.PaymentType.ToLower(), ZnodeConstant.AmazonPay.ToLower()))
                    {
                        SetAmazonAddress(submitOrderViewModel, cartModel);
                        SetUserDetails(user, cartModel);
                        cartModel.ShippingAddress.IsDefaultBilling = true;
                        cartModel.ShippingAddress.IsDefaultShipping = true;
                        userAddresses = GetAnonymousUserAddresses(cartModel, submitOrderViewModel);
                    }
                    else
                    {
                        userAddresses = GetAnonymousUserAddresses(cartModel, submitOrderViewModel);
                    }
                }
                submitOrderViewModel.UserId = user.UserId;

                //Send shipping address in cart for validation, 
                //if it is not available then only send shipping address from user address list for validation in USPS.
                Api.Models.BooleanModel booleanModel;
                if (IsAmazonPayEnable(submitOrderViewModel) == true || IsAddressValidationRequiredForOrder() == false)
                {
                    booleanModel = new Api.Models.BooleanModel { IsSuccess = true };
                    SetPublishStateIdInAddressModel((IsNull(cartModel?.ShippingAddress) || cartModel?.ShippingAddress?.AddressId == 0) ? userAddresses?.Where(x => x.AddressId == submitOrderViewModel.ShippingAddressId)?.FirstOrDefault() : cartModel?.ShippingAddress);
                }
                else { booleanModel = IsValidAddressForCheckout((IsNull(cartModel?.ShippingAddress) || cartModel?.ShippingAddress?.AddressId == 0) ? userAddresses?.Where(x => x.AddressId == submitOrderViewModel.ShippingAddressId)?.FirstOrDefault() : cartModel?.ShippingAddress); }
                //Check whether address is valid or not.
                if ((!booleanModel.IsSuccess) &&
                    !(bool)PortalAgent.CurrentPortal.PortalFeatureValues.Where(x => x.Key.Contains(StoreFeature.Require_Validated_Address.ToString()))?.FirstOrDefault().Value)
                {
                    return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), booleanModel.ErrorMessage ?? WebStore_Resources.AddressValidationFailed);
                }

                //Set shoppingcart details like shipping. payment setting, etc.
                SetShoppingCartDetails(submitOrderViewModel, userAddresses, cartModel);

                // Perform the calculation in case the session order total & order total from page does not match
                // In case of multi tab scenario where at the time of order place, the cart update from other tab
                cartModel = EnsureShoppingCartCalculations(cartModel, submitOrderViewModel);

                bool isCreditCardPayment = false;
                // Condition for "Credit Card" payment.
                if (IsNotNull(cartModel?.Payment) && Equals(cartModel.Payment.PaymentName.ToLower(), ZnodeConstant.CreditCard.ToLower()))
                {
                    isCreditCardPayment = true;
                    OrdersViewModel orderViewModel = ProcessCreditCardPayment(submitOrderViewModel, cartModel);
                    if (orderViewModel.HasError)
                    {
                        return orderViewModel;
                    }
                }
                else if (IsNotNull(cartModel?.Payment) && Equals(cartModel.Payment.PaymentName.ToLower(), ZnodeConstant.ACH.ToLower()))
                {
                    OrdersViewModel orderViewModel = ProcessCreditCardPayment(submitOrderViewModel, cartModel);
                    if (orderViewModel.HasError)
                    {
                        return orderViewModel;
                    }
                }
                // Condition for "PayPal Express".                   
                else if (!string.IsNullOrEmpty(cartModel?.Payment?.PaymentName) && Equals(cartModel.Payment.PaymentName.Replace("_", "").ToLower(), ZnodeConstant.PayPalExpress.ToLower()))
                {
                    submitOrderViewModel.PortalName = PortalAgent.CurrentPortal?.Name;
                    ZnodeLogging.LogMessage($"Paypal Token - {submitOrderViewModel.PayPalToken}");
                    OrdersViewModel order = new OrdersViewModel();
                    if (string.IsNullOrEmpty(submitOrderViewModel.PayPalToken))
                    {
                        return PayPalExpressPaymentProcess(submitOrderViewModel, cartModel, userAddresses);
                    }
                    else
                    {
                        order = PayPalExpressPaymentProcess(submitOrderViewModel, cartModel, userAddresses);
                    }

                    if (!string.IsNullOrEmpty(order?.PayPalExpressResponseToken))
                    {
                        cartModel.Token = order.PayPalExpressResponseToken;
                        cartModel.Payment.PaymentStatusId = Convert.ToInt16(Enum.Parse(typeof(Znode.Engine.Api.Models.Enum.ZnodePaymentStatus), order.PaymentStatus));
                    }
                    else
                    {
                        return order;
                    }
                }
                //Amazon payment.
                else if (IsNotNull(submitOrderViewModel?.PaymentType) && Equals(submitOrderViewModel?.PaymentType.ToLower(), ZnodeConstant.AmazonPay.ToLower()) && !string.IsNullOrEmpty(submitOrderViewModel.AmazonPayReturnUrl) && !string.IsNullOrEmpty(submitOrderViewModel.AmazonPayCancelUrl))
                {
                    return AmazonPaymentProcess(submitOrderViewModel, cartModel, userAddresses);
                }

                if (submitOrderViewModel.IsFromAmazonPay)
                {
                    cartModel.Token = cartModel.Token;
                }

                if (!string.IsNullOrEmpty(submitOrderViewModel.PayPalToken) && submitOrderViewModel.IsFromPayPalExpress)
                {
                    submitOrderViewModel.CardType = "PayPal";
                    submitOrderViewModel.TransactionId = cartModel.Token;
                }

                if (submitOrderViewModel.IsFromAmazonPay)
                {
                    cartModel.Token = submitOrderViewModel.PaymentToken;
                    submitOrderViewModel.CardType = "Amazon";
                    submitOrderViewModel.TransactionId = submitOrderViewModel.PaymentToken;
                }

                //Card Type
                cartModel.CardType = submitOrderViewModel.CardType;
                cartModel.CcCardExpiration = submitOrderViewModel.CcExpiration;
                cartModel.TransactionId = submitOrderViewModel.TransactionId;
                cartModel.Payment.PaymentSetting.GatewayCode = submitOrderViewModel.GatewayCode;
                if (IsNotNull(PortalAgent.CurrentPortal.PublishState))
                {
                    cartModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;
                }

                cartModel.OmsOrderStatusId = PortalAgent.CurrentPortal.DefaultOrderStateID;
                cartModel.IsOrderFromWebstore = true;

                //This is used to skip pre-submit order validation, since all the validations have already been validated at the checkout page.
                cartModel.SkipPreprocessing = true;

                OrdersViewModel _ordersViewModel = PlaceOrder(cartModel);

                //Update the new balance values against the user.
                if (_ordersViewModel.OmsOrderId > 0)
                {
                    RemoveCookie(WebStoreConstants.UserOrderReceiptOrderId);
                    SaveInCookie(WebStoreConstants.UserOrderReceiptOrderId, Convert.ToString(_ordersViewModel.OmsOrderId), ZnodeConstant.MinutesInAHour);
                    UpdateUserDetailsInSession(_ordersViewModel.Total);
                }

                //Get address from cache.
                string cacheKey = $"{WebStoreConstants.UserAccountAddressList}{cartModel.UserId}";
                Helper.ClearCache(cacheKey);

                if (isCreditCardPayment && !cartModel.IsGatewayPreAuthorize && _ordersViewModel.OmsOrderId > 0 && !string.IsNullOrEmpty(cartModel.Token))
                {
                    CapturePayment(_ordersViewModel.OmsOrderId, cartModel.Token, _ordersViewModel);
                }

                if (string.Equals(cartModel.Payment.PaymentName.Replace("_", ""), ZnodeConstant.PayPalExpress, StringComparison.InvariantCultureIgnoreCase) && !cartModel.Payment.IsPreAuthorize && _ordersViewModel.OmsOrderId > 0 && !string.IsNullOrEmpty(cartModel.Token))
                {
                    CapturePayment(_ordersViewModel.OmsOrderId, cartModel.Token, _ordersViewModel);
                }
                else if (isCreditCardPayment)
                    _orderClient.CreateOrderHistory(new OrderHistoryModel() { OmsOrderDetailsId = _ordersViewModel.OmsOrderDetailsId, Message = WebStore_Resources.TextTransactionPreAuthorized, TransactionId = _ordersViewModel.TransactionId, CreatedBy = _ordersViewModel.CreatedBy, ModifiedBy = _ordersViewModel.ModifiedBy });

                if (IsNotNull(submitOrderViewModel?.PaymentType) && Equals(submitOrderViewModel?.PaymentType.ToLower(), ZnodeConstant.AmazonPay.ToLower()) && !cartModel.IsGatewayPreAuthorize && _ordersViewModel.OmsOrderId > 0 && !string.IsNullOrEmpty(cartModel.Token))
                {
                    CapturePayment(_ordersViewModel.OmsOrderId, submitOrderViewModel.PaymentToken, _ordersViewModel);
                }

                if (PortalAgent.CurrentPortal.IsKlaviyoEnable)
                {
                    // Created the task to post the data to klaviyo
                    HttpContext httpContext = HttpContext.Current;

                    Task.Run(() =>
                    {
                        PostDataToKlaviyo(_ordersViewModel, httpContext);
                    });
                }
                if (string.Equals(submitOrderViewModel.GatewayCode, ZnodeConstant.CyberSource, StringComparison.CurrentCultureIgnoreCase))
                {
                    RemoveInSession(WebStoreConstants.UserAccountKey);
                }
                return _ordersViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SubmitOrder method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new OrdersViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ErrorFailedToCreate };
            }
        }
        // To post the data to klaviyo
        protected virtual void PostDataToKlaviyo(OrdersViewModel _ordersViewModel, HttpContext httpContext)
        {
            HttpContext.Current = httpContext;
            IKlaviyoClient _klaviyoClient = GetComponentClient<IKlaviyoClient>(GetService<IKlaviyoClient>());
            KlaviyoProductDetailModel klaviyoProductDetailModel = MapOrderModelToKlaviyoModel(_ordersViewModel);
            bool isTrackKlaviyo = _klaviyoClient.TrackKlaviyo(klaviyoProductDetailModel);
        }

        // Map the order model to klaviyo model
        protected virtual KlaviyoProductDetailModel MapOrderModelToKlaviyoModel(OrdersViewModel _ordersViewModel)
        {
            KlaviyoProductDetailModel klaviyoProductDetailModel = new KlaviyoProductDetailModel();
            klaviyoProductDetailModel.OrderLineItems = new List<OrderLineItemDetailsModel>();
            klaviyoProductDetailModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            klaviyoProductDetailModel.FirstName = userViewModel?.FirstName;
            klaviyoProductDetailModel.LastName = userViewModel?.LastName;
            klaviyoProductDetailModel.Email = _ordersViewModel?.EmailAddress;
            klaviyoProductDetailModel.StoreName = PortalAgent.CurrentPortal.Name;
            klaviyoProductDetailModel.PropertyType = (int)KlaviyoEventType.CheckOutSuccessEvent;
            foreach (OrderLineItemViewModel orderLineItem in _ordersViewModel.OrderLineItems)
            {
                klaviyoProductDetailModel.OrderLineItems.Add(new OrderLineItemDetailsModel { ProductName = orderLineItem.ProductName, SKU = orderLineItem.Sku, Quantity = orderLineItem.Quantity, UnitPrice = orderLineItem.Price, Image = orderLineItem.Image });
            }
            return klaviyoProductDetailModel;
        }
        //To set publish state Id in address model.
        protected virtual void SetPublishStateIdInAddressModel(AddressModel addressModel)
        {
            if (addressModel != null && IsNotNull(PortalAgent.CurrentPortal.PublishState))
                addressModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;
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
            if (IsNotNull(user) && total > 0 && user.AnnualBalanceOrderAmount > 0)
            {
                if (enableUserOrderAnnualLimit && user.AnnualOrderLimit > 0 && (user.AnnualBalanceOrderAmount - total) <= 0)
                {
                    isValidated = false;
                }
            }
            return isValidated;
        }
        public virtual bool IsAmazonPayEnable(SubmitOrderViewModel submitOrderViewModel)
        {
            return (IsNotNull(submitOrderViewModel?.PaymentType) && Equals(submitOrderViewModel?.PaymentType.ToLower(), ZnodeConstant.AmazonPay.ToLower()) && !submitOrderViewModel.IsFromAmazonPay);
        }

        //Set billing shipping from default address
        public virtual AddressListViewModel GetBillingShippingAddress(string type = "", int addressId = 0, int otherAddressId = 0, int userId = 0, bool isCartAddress = false, bool IsFromEdit = false)
        {
            //passing userid to handle condition when userid is bounded if transaction fails
            return GetBillingShippingAddress(userId, false, type, addressId, otherAddressId, IsFromEdit);

        }


        //Set billing shipping from default address
        public virtual AddressListViewModel GetBillingShippingAddress(int userId, bool isCartAddress, string type = "", int addressId = 0, int otherAddressId = 0, bool IsFromEdit = false, bool isQuoteRequest = false, bool isCalculateCart = true)
        {
            //get countries 
            List<SelectListItem> countries = _userAgent.GetCountries();
            AddressListViewModel addressList = new AddressListViewModel();
            UserViewModel userDetails = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            string roleName = userDetails?.RoleName;

                //Validate User ID.
                if (HelperUtility.IsNotNull(userDetails) && userDetails?.UserId > 0 && userId > 0)
                {
                    if (!HelperUtility.IsValidIdInQueryString(userDetails.UserId, userId))
                    {
                        return (AddressListViewModel)GetViewModelWithErrorMessage(new AddressListViewModel(), WebStore_Resources.HttpCode_401_AccessDeniedMsg);
                    }
                }

                //Get Address List of logged in user
                addressList = _userAgent.GetAddressList(userId, false);

            if (IsNotNull(addressList))
            {
                if (isCartAddress)
                {
                    SetAddressDetailsFromSession(addressList, type);
                }

                //Selected addressId
                if (IsFromEdit)
                    addressList.SelectedAddressId = addressId;

                if (addressList.SelectedAddressId > 0 && Equals(type, WebStoreConstants.BillingAddressType))
                {
                    addressList.BillingAddress = addressList.AddressList?.FirstOrDefault(x => x.AddressId == addressList.SelectedAddressId);
                    if (IsNotNull(addressList.BillingAddress))
                    {
                        addressList.BillingAddress.AddressType = WebStoreConstants.BillingAddressType;
                        addressList.BillingAddress.IsBilling = true;
                    }
                }
                else if (addressList.SelectedAddressId > 0 && Equals(type, WebStoreConstants.ShippingAddressType))
                {
                    addressList.ShippingAddress = addressList.AddressList?.FirstOrDefault(x => x.AddressId == addressList.SelectedAddressId);
                    if (IsNotNull(addressList.ShippingAddress))
                    {
                        addressList.ShippingAddress.IsShipping = true;
                        addressList.ShippingAddress.AddressType = WebStoreConstants.ShippingAddressType;
                        addressList.ShippingAddress.EmailAddress = !string.IsNullOrEmpty(addressList.ShippingAddress.EmailAddress) ? addressList.ShippingAddress.EmailAddress : userDetails?.Email;
                    }
                }

                //Set Billing address 
                if (IsNull(addressList.BillingAddress))
                {
                    addressList.BillingAddress = addressList.AddressList?.FirstOrDefault(x => x.IsDefaultBilling);
                    if (IsNotNull(addressList.BillingAddress))
                    {
                        addressList.BillingAddress.AddressType = WebStoreConstants.BillingAddressType;
                        addressList.BillingAddress.IsBilling = true;
                    }
                }

                //Set Shipping address
                if (IsNull(addressList.ShippingAddress))
                {
                    addressList.ShippingAddress = addressList.AddressList?.FirstOrDefault(x => x.IsDefaultShipping);
                    if (IsNotNull(addressList.ShippingAddress))
                    {
                        addressList.ShippingAddress.IsShipping = true;
                        addressList.ShippingAddress.AddressType = WebStoreConstants.ShippingAddressType;
                        addressList.ShippingAddress.EmailAddress = !string.IsNullOrEmpty(addressList.ShippingAddress.EmailAddress) ? addressList.ShippingAddress.EmailAddress : userDetails?.Email;
                    }
                }
                //Add countries for shipping
                if (IsNotNull(addressList.ShippingAddress))
                {
                    addressList.ShippingAddress.AddressType = WebStoreConstants.ShippingAddressType;
                    addressList.ShippingAddress.Countries = countries;
                    if (!isCartAddress)
                    {
                        addressList.ShippingAddress.EmailAddress = string.IsNullOrEmpty(userDetails?.Email) ? addressList.ShippingAddress.EmailAddress : userDetails?.Email;
                    }
                }
                else
                {
                    addressList.ShippingAddress = new AddressViewModel() { Countries = countries, AddressType = WebStoreConstants.ShippingAddressType };
                }

                //Add countries for billing
                if (IsNotNull(addressList.BillingAddress))
                {
                    addressList.BillingAddress.AddressType = WebStoreConstants.BillingAddressType;
                    addressList.BillingAddress.Countries = countries;
                }
                else
                {
                    addressList.BillingAddress = new AddressViewModel() { Countries = countries, AddressType = WebStoreConstants.BillingAddressType };
                }

                addressList.RoleName = roleName;
                if (!IsFromEdit)
                {
                    bool isCalculatePromotionAndCoupon = isQuoteRequest ? false : true;
                    UpdateChangedAddressWithCart(addressList.BillingAddress, addressList.ShippingAddress, isCalculatePromotionAndCoupon, isCalculateCart);
                }
                return addressList;
            }
            return new AddressListViewModel() { BillingAddress = new AddressViewModel { Countries = countries, RoleName = roleName, AddressType = WebStoreConstants.BillingAddressType }, ShippingAddress = new AddressViewModel { Countries = countries, RoleName = roleName, EmailAddress = userDetails?.Email, AddressType = WebStoreConstants.ShippingAddressType } };
        }

        // This function is used to get the address details from the session.
        protected virtual void SetAddressDetailsFromSession(AddressListViewModel addressList, string addressTypeName = "")
        {
            //Get Shopping cart from session or cookie.
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

            if (IsNotNull(cartModel?.BillingAddress))
            {
                addressList.BillingAddress = cartModel.BillingAddress?.ToViewModel<AddressViewModel>();
                if (!IsValidAddress(cartModel?.BillingAddress))
                {
                    addressList.BillingAddress = null;
                }
            }
            if (IsNotNull(cartModel?.ShippingAddress) && IsNotNull(cartModel?.ShippingAddress.PostalCode))
            {
                addressList.ShippingAddress = cartModel.ShippingAddress?.ToViewModel<AddressViewModel>();
                if (!IsValidAddress(cartModel?.ShippingAddress))
                {
                    addressList.ShippingAddress = null;
                }
            }
            if (addressTypeName.Equals(WebStoreConstants.ShippingAddressType, StringComparison.InvariantCultureIgnoreCase) && IsNotNull(addressList?.ShippingAddress))
            {
                addressList.SelectedAddressId = addressList.ShippingAddress.AddressId;
            }
            if (addressTypeName.Equals(WebStoreConstants.BillingAddressType, StringComparison.InvariantCultureIgnoreCase) && IsNotNull(addressList?.BillingAddress))
            {
                addressList.SelectedAddressId = addressList.BillingAddress.AddressId;
            }
        }

        // This method is used to check the address having valid properties like postal code & display name.
        // Because some time properties are black in the address.
        protected virtual bool IsValidAddress(AddressModel addressModel)
        {
            if ((string.IsNullOrEmpty(addressModel?.DisplayName) && addressModel?.UserId > 0) || string.IsNullOrEmpty(addressModel?.Address1) || string.IsNullOrEmpty(addressModel?.PostalCode))
            {
                return false;
            }
            return true;
        }

        protected virtual void UpdateChangedAddressWithCart(AddressViewModel billingAddress, AddressViewModel shippingAddress, bool isCalculatePromotionAndCoupon = true, bool isCalculateCart = true)
        {
            ShoppingCartModel _cart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ?? _cartAgent.GetCartFromCookie();
            ShoppingCartModel oldShoppingCartModel = _cart;

            if (IsNotNull(_cart))
            {
                _cart.IsCalculateVoucher = true;
                _cart.IsCalculatePromotionAndCoupon = isCalculatePromotionAndCoupon;
                _cart.ShippingAddress = shippingAddress?.ToModel<AddressModel>();
                _cart.BillingAddress = billingAddress?.ToModel<AddressModel>();

                ZnodeLogging.LogMessage($"Calculating the taxes for the address {_cart.ShippingAddress?.Address1} {_cart.ShippingAddress?.CityName} . ", "Address", TraceLevel.Info);
                //Calculate the taxes for the address. 
                if (isCalculateCart == true)
                {
                    ShoppingCartModel calculatedCart = _shoppingCartClient.Calculate(_cart);
                    _cart.SubTotal = calculatedCart.SubTotal;
                    _cart.Total = calculatedCart.Total;
                    _cart.CustomTaxCost = calculatedCart.CustomTaxCost;
                    _cart.TaxCost = calculatedCart.TaxCost;
                    _cart.SalesTax = calculatedCart.SalesTax;
                    _cart.Vouchers = calculatedCart.Vouchers;
                }

                _cartAgent.MapQuantityOnHandAndSeoName(oldShoppingCartModel, _cart);

                SaveInSession(WebStoreConstants.CartModelSessionKey, _cart);
            }
        }

        // Map data which is comes from paypal return url.
        public virtual SubmitOrderViewModel SetPayPalToken(string token, int shippingAddressId, int billingAddressId, int shippingOptionId, int paymentSettingId, string additionalInstruction, string paymentCode, string orderNumber, string inHandDate = "", string jobName = "", string shippingConstraintCode = "")
        {
            SubmitOrderViewModel submitOrderViewModel = new SubmitOrderViewModel();
            submitOrderViewModel.AdditionalInstruction = additionalInstruction;
            submitOrderViewModel.ShippingAddressId = shippingAddressId;
            submitOrderViewModel.BillingAddressId = billingAddressId;
            submitOrderViewModel.ShippingOptionId = shippingOptionId;
            submitOrderViewModel.PaymentSettingId = paymentSettingId;
            submitOrderViewModel.IsFromPayPalExpress = true;
            submitOrderViewModel.PayPalToken = token;
            submitOrderViewModel.PaymentToken = token;
            submitOrderViewModel.PaymentCode = paymentCode;
            submitOrderViewModel.OrderNumber = orderNumber;

            if (!string.IsNullOrEmpty(inHandDate))
            {
                DateTime date;
                DateTime.TryParse(inHandDate, out date);
                submitOrderViewModel.InHandDate = date;
            }

            submitOrderViewModel.JobName = jobName;
            submitOrderViewModel.ShippingConstraintCode = shippingConstraintCode;

            return submitOrderViewModel;
        }

        public virtual void SetBillingShippingAddress(int addressId, int otherAddressId, int userId, string type)
        {
            AddressListViewModel addressList = GetAddressListForUser(addressId, otherAddressId);
            if (addressList?.AddressList?.Count > 0)
            {
                AddressViewModel selectedAddress = addressList.AddressList.FirstOrDefault(x => x.AddressId == addressId) ?? new AddressViewModel();
                AddressViewModel otherAddress = addressList.AddressList.FirstOrDefault(x => x.AddressId == otherAddressId) ?? new AddressViewModel();

                if (Equals(type, WebStoreConstants.BillingAddressType))
                {
                    addressList.BillingAddress = selectedAddress;
                    addressList.ShippingAddress = (IsNotNull(selectedAddress) && selectedAddress.IsShipping) ? selectedAddress : otherAddress ?? addressList.ShippingAddress;
                }
                else if (Equals(type, WebStoreConstants.ShippingAddressType))
                {
                    addressList.ShippingAddress = selectedAddress;
                    addressList.BillingAddress = (IsNotNull(selectedAddress) && selectedAddress.IsBilling) ? selectedAddress : otherAddress ?? addressList.BillingAddress;
                }
            }

            if (userId > 0)
            {
                Helper.ClearCache($"UserAccountAddressList{userId}");
                Helper.AddIntoCache(addressList, $"UserAccountAddressList{userId}", "CurrentPortalCacheDuration");
            }
            else
            {
                Helper.ClearCache("UserAccountAddressList");
                Helper.AddIntoCache(addressList, "UserAccountAddressList", "CurrentPortalCacheDuration");
            }

        }

        //Create guest user account.
        public virtual UserViewModel CreateAnonymousUserAccount(AddressModel address, string emailAddress)
        {
            UserViewModel user = _userClient.CreateCustomerAccount(new UserModel { FirstName = address?.FirstName, LastName = address?.LastName, Email = emailAddress, IsGuestUser = true, PortalId = PortalAgent.CurrentPortal.PortalId, ProfileId = PortalAgent.CurrentPortal.ProfileId })?.ToViewModel<UserViewModel>();
            user.Email = emailAddress;
            SaveInSession(WebStoreConstants.GuestUserKey, user);
            return user;
        }

        //Get addresses of guest users.
        public virtual List<AddressModel> GetAnonymousUserAddresses(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel)
        {
            List<AddressModel> addressList = new List<AddressModel>();
            if (IsNotNull(cartModel))
            {
                int userId = (GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey)?.UserId).GetValueOrDefault();
                if (userId < 1)
                {
                    userId = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId ?? 0;
                }

                //Check if shipping address of cart model is not null then create it shipping address of guest user.
                if (IsNotNull(cartModel.ShippingAddress))
                {
                    AddressModel address = CreateGuestUserShippingAddress(userId, cartModel);
                    if (IsNotNull(address))
                    {
                        addressList.Add(address);
                        submitOrderViewModel.ShippingAddressId = address.AddressId;
                        submitOrderViewModel.BillingAddressId = address.AddressId;
                        cartModel.ShippingAddress.AddressId = address.AddressId;
                    }
                }
                //For amazon pay skipping billing address for anonymous user.
                if (Equals(cartModel.BillingAddress, cartModel.ShippingAddress) && Equals(cartModel.Payment.PaymentName, ZnodeConstant.Amazon_Pay))
                {
                    return addressList;
                }
                if (IsNotNull(cartModel.BillingAddress) && !cartModel.BillingAddress.IsShippingBillingDifferent)
                {
                    cartModel.BillingAddress.AddressId = cartModel.ShippingAddress.AddressId;
                    cartModel.BillingAddress.UserId = cartModel.ShippingAddress.UserId;
                    cartModel.BillingAddress.IsGuest = cartModel.ShippingAddress.IsGuest;
                }
                //Check if billing address of cart model is not null then create it billing address of guest user.
                if (IsNotNull(cartModel.BillingAddress) && cartModel.BillingAddress.IsShippingBillingDifferent == true)
                {
                    cartModel.BillingAddress.UserId = userId;

                    //Create guest users addresses.
                    AddressModel address = CreateAnonymousUserAddress(cartModel.BillingAddress);
                    if (IsNotNull(address))
                    {
                        addressList.Add(address);
                        submitOrderViewModel.BillingAddressId = address.AddressId;
                        cartModel.BillingAddress.AddressId = address.AddressId;
                    }
                }
            }
            return addressList;
        }
        //Get customer details required for checkout page.
        public virtual CheckoutViewModel GetUserDetails(int userId = 0)
        {
            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
            checkoutViewModel.UserId = userId;
            //Get customer required details from shopping cart.
            GetUserCartDetails(checkoutViewModel);

            //Get customer information from session.
            GetUserInfo(checkoutViewModel);

            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            checkoutViewModel.ShippingId = Convert.ToInt32(cartModel?.ShippingId);
            checkoutViewModel.IsPendingOrderRequest = checkoutViewModel.ShowPlaceOrderButton == false ? true : false;

            cartModel.IsPendingOrderRequest = checkoutViewModel.IsPendingOrderRequest;
            SaveInSession(WebStoreConstants.CartModelSessionKey, cartModel);
            return checkoutViewModel;
        }

        public virtual OrdersViewModel GetOrderViewModel(int omsOrderId)
        {
            OrderModel orderModel = _orderClient.GetOrderReceiptByOrderId(omsOrderId);

            if (orderModel?.OmsOrderId > 0)
            {
                List<OrderLineItemModel> orderLineItemListModel = new List<OrderLineItemModel>();

                //Create new order line item model.
                CreateSingleOrderLineItem(orderModel, orderLineItemListModel);

                orderModel.OrderLineItems = orderLineItemListModel;
            }
            OrdersViewModel viewModel = orderModel?.ToViewModel<OrdersViewModel>();
            int userId = orderModel.IsQuoteOrder ? orderModel.UserId : GetUserUserIdFromSession();

            if (IsNotNull(viewModel))
            {
                UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? new UserViewModel();
                if (userId <= 0 && userViewModel?.GuestUserId <= 0)
                {
                    userViewModel.GuestUserId = viewModel.UserId;
                    SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
                }

                if ((userViewModel.GuestUserId == viewModel.UserId || viewModel.UserId == userId) && viewModel.OrderLineItems?.Count() > 0)
                {
                    //Order Receipt
                    string trackingUrl = GetTrackingUrlByShippingId(orderModel.ShippingId);
                    viewModel.TrackingNumber = SetTrackingUrl(orderModel.TrackingNumber, trackingUrl);
                    viewModel.CurrencyCode = PortalAgent.CurrentPortal?.CurrencyCode;
                    viewModel.CultureCode = PortalAgent.CurrentPortal?.CultureCode;
                    viewModel.CouponCode = viewModel.CouponCode?.Replace("<br/>", ", ");
                    viewModel?.OrderLineItems?.ForEach(item =>
                    {
                        item.UOM = orderModel?.ShoppingCartModel?.ShoppingCartItems?.FirstOrDefault(x => x.SKU == item.Sku)?.UOM;
                        item.TrackingNumber = SetTrackingUrl(item.TrackingNumber, trackingUrl);
                    });

                    int count = 0;
                    StringBuilder cjURL = new StringBuilder();
                    //Append line item sku, quantity and amount to url.
                    foreach (OrderLineItemViewModel orderDetail in viewModel.OrderLineItems)
                    {
                        count++;
                        cjURL.Append($"&ITEM{count}={orderDetail.Sku}");
                        cjURL.Append($"&AMT{count}={orderDetail.Price}");
                        cjURL.Append($"&QTY{count}={orderDetail.Quantity}");
                    }
                    viewModel.OrderLineItemQueryString = cjURL.ToString();
                    return viewModel;
                }
            }
            return null;
        }


        // Get filter.
        public virtual FilterCollection GetFilter(AddressListViewModel addressList)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationCountryCode, FilterOperators.Equals, addressList?.ShippingAddress?.CountryName));
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationStateCode, FilterOperators.Equals, addressList?.ShippingAddress?.StateName));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId)));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(GetUserUserIdFromSession())));
            return filters;
        }

        /// <summary>
        /// Call PayPal payment finalize method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Order view Model</returns>
        public OrdersViewModel DoPayPalExpressPaymentProcess(SubmitPaymentModel submitPaymentModel)
        {
            GatewayResponseModel gatewayResponse = null;
            try
            {
                gatewayResponse = _paymentAgent.FinalizePayPalProcess(submitPaymentModel);

                if (!string.IsNullOrEmpty(gatewayResponse.TransactionId))
                    //Update transaction Id Order details.
                    _orderClient.UpdateOrderTransactionId(Convert.ToInt32(submitPaymentModel.OrderId), gatewayResponse.TransactionId);

                if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
                {
                    _cartAgent.ClearCartCountFromSession();
                    RemoveInSession(WebStoreConstants.CartModelSessionKey);
                    return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment);
                }
                return new OrdersViewModel() { PayPalExpressResponseText = gatewayResponse.ResponseText, PayPalExpressResponseToken = gatewayResponse.PaymentToken };
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in DoPayPalExpressPaymentProcess method for OrderId " + submitPaymentModel?.OrderId + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment);
            }
        }

        //Create single order line item.
        public virtual void CreateSingleOrderLineItem(OrderModel orderModel, List<OrderLineItemModel> orderLineItemListModel)
        {
            List<OrderLineItemModel> childLineItems = orderModel.OrderLineItems?.Where(oli => oli.ParentOmsOrderLineItemsId.HasValue)?.ToList();
            List<OrderLineItemModel> bundleLineItems = orderModel.OrderLineItems?.Where(x => x.ProductType == ZnodeConstant.BundleProduct)?.ToList();
            if (bundleLineItems != null)
            {
                foreach (OrderLineItemModel lineItem in bundleLineItems)
                {
                    OrderLineItemModel bundleLineItem = orderModel.OrderLineItems?.FirstOrDefault(x => x.Sku == lineItem?.Sku);
                    if (IsNotNull(bundleLineItem))
                        childLineItems?.Add(bundleLineItem);
                }
            }
            if (childLineItems != null)
            {
                foreach (OrderLineItemModel _childLineItem in childLineItems)
                {
                    _childLineItem.Description = _childLineItem.Description;

                    _childLineItem.PersonaliseValueList = orderModel.OrderLineItems?.FirstOrDefault(oli => oli.OmsOrderLineItemsId == _childLineItem.ParentOmsOrderLineItemsId)?.PersonaliseValueList;
                    _childLineItem.ProductName = _childLineItem.ProductName;
                    _childLineItem.Price = _childLineItem.Price;
                    _childLineItem.Quantity = _childLineItem.Quantity;

                    orderLineItemListModel.Add(_childLineItem);
                }
            }
        }

        //Get address details on the basis of address id.
        public virtual AddressViewModel GetAddressById(int? addressId, string addressType = "", bool isCalculateCart = true)
        {
            try
            {
                if (addressId > 0)
                {
                    //Get user details from session.
                    UserViewModel userDetails = GetUserViewModelFromSession();

                    ShoppingCartModel _cart = SessionHelper.GetDataFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
                    AddressViewModel CartBillingAddressAvailable = _cart?.BillingAddress?.ToViewModel<AddressViewModel>();
                    AddressViewModel CartShippingAddressAvailable = _cart?.ShippingAddress?.ToViewModel<AddressViewModel>();

                    AddressViewModel addressViewModel = _webStoreAccountClient.GetAddress(addressId)?.ToViewModel<AddressViewModel>();
                    
                    addressViewModel.StateCode = addressViewModel.StateCode ?? addressViewModel.StateName;

                //Checking ShippingAddressSuggestion Flag Value for user and store
                bool enableUserShippingAddressSuggestion = Convert.ToBoolean(userDetails?.UserGlobalAttributes?.FirstOrDefault(o => o.AttributeCode == "EnableUserShippingAddressSuggestion")?.AttributeValue);
                bool enableStoreShippingAddressSuggestion = Convert.ToBoolean(PortalAgent.CurrentPortal.GlobalAttributes?.Attributes?.FirstOrDefault(o => o.AttributeCode == "EnableStoreShippingAddressSuggestion")?.AttributeValue);

                
                
                if (enableUserShippingAddressSuggestion || enableStoreShippingAddressSuggestion)
                {
                    //If enableUserShippingAddressSuggestion or enableStoreShippingAddressSuggestion flag is ON and user tries to fetch store addresses then need to add firstname and lastname for only store addresses as the fields are mandatory in validation.                   
                    if (string.IsNullOrEmpty(addressViewModel.FirstName) && string.IsNullOrEmpty(addressViewModel.LastName))
                    {
                            // Binding the emailaddress username from it's first/last name for only the users doesn't have firstname/lastname added in profile.
                            //Hence this is only added to proceed with data from email field it will not get saved in DB as user's address.
                            string userName = userDetails?.Email?.Substring(0, userDetails.Email.IndexOf("@"));
                        addressViewModel.FirstName = string.IsNullOrEmpty(userDetails.FirstName) ? userName : userDetails.FirstName;
                        addressViewModel.LastName = string.IsNullOrEmpty(userDetails.LastName) ? userName : userDetails.LastName;
                    }
                }
                else
                {
                    if (HelperUtility.IsNotNull(userDetails) && userDetails?.UserId > 0 && addressViewModel?.CreatedBy > 0)
                    {
                        //Validate UserID.
                        if (!HelperUtility.IsValidIdInQueryString(addressViewModel.CreatedBy, userDetails.UserId))
                        {
                            return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), WebStore_Resources.HttpCode_401_AccessDeniedMsg);
                        }
                    }
                }

                

                //Filter by address type
                switch (addressType)
                {
                    case WebStoreConstants.ShippingAddressType:
                        {
                            UpdateChangedAddressWithCart(CartBillingAddressAvailable, addressViewModel, isCalculateCart);
                            break;
                        }
                    case WebStoreConstants.BillingAddressType:
                        {
                            UpdateChangedAddressWithCart(addressViewModel, CartShippingAddressAvailable, isCalculateCart);
                            break;
                        }
                }

                    return addressViewModel;
                }
            }
            catch (ZnodeException exception)
            {
                if (HelperUtility.Equals(exception.ErrorCode, ErrorCodes.InvalidData))
                    return (AddressViewModel)GetViewModelWithErrorMessage(new AddressViewModel(), WebStore_Resources.ErrorBadRequest);

            }
            return new AddressViewModel();
        }

        //Set first/last name of user in the cart.
        public virtual AddressViewModel SetAddressRecipientNameInCart(string firstName, string lastName, string addressType = "")
        {
            AddressViewModel returnModel = new AddressViewModel();
            ShoppingCartModel _cart = SessionHelper.GetDataFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);
            AddressViewModel CartBillingAddressAvailable = _cart?.BillingAddress?.ToViewModel<AddressViewModel>();
            AddressViewModel CartShippingAddressAvailable = _cart?.ShippingAddress?.ToViewModel<AddressViewModel>();


            //Filter by address type
            switch (addressType)
            {
                case WebStoreConstants.ShippingAddressType:
                    {
                        if (IsNotNull(CartShippingAddressAvailable))
                        {
                            CartShippingAddressAvailable.FirstName = firstName;
                            CartShippingAddressAvailable.LastName = lastName;
                        }
                        returnModel = CartShippingAddressAvailable;
                        break;
                    }
                case WebStoreConstants.BillingAddressType:
                    {
                        if (IsNotNull(CartBillingAddressAvailable))
                        {
                            CartBillingAddressAvailable.FirstName = firstName;
                            CartBillingAddressAvailable.LastName = lastName;
                        }
                        returnModel = CartBillingAddressAvailable;
                        break;
                    }
            }
            return returnModel;

        }

        //Get valid recommended address list.
        public virtual AddressListViewModel GetRecommendedAddressList(AddressViewModel model)
        {
            if (IsNotNull(model))
            {

                //Get valid recommended addresses.
                return _userAgent.GetRecommendedAddress(model);
            }

            return new AddressListViewModel();
        }

        //Get list of search locations.
        public virtual List<AutoComplete> GetSearchLocation(string searchTerm, int portalId, string addressType)
        {
            if (string.IsNullOrEmpty(searchTerm) || (searchTerm.Length < 3))
            {
                return new List<AutoComplete>();
            }

            AddressListModel listModel = _customerClient.GetSearchLocation(portalId, searchTerm);

            AppendUserAddress(searchTerm, listModel, addressType);

            AddressListViewModel addressListViewModel = listModel?.ToViewModel<AddressListViewModel>();
            List<AutoComplete> _autoComplete = new List<AutoComplete>();
            if ((addressListViewModel?.AddressList?.Count > 0))
            {
                addressListViewModel.AddressList.ForEach(item =>
                {
                    AutoComplete _item = new AutoComplete();
                    string address = CheckAndAppendAlternateAddress(item);
                    _item.Name = string.Format(WebStore_Resources.AutoCompleteLabelForSearchForLocation,
                                               item.Address1,
                                               item.Address2,
                                               address,
                                               item.CityName,
                                               item.StateName,
                                               item.PostalCode,
                                               IsNotNull(item.DisplayName) ? item.DisplayName + "<br>" : "");

                    _item.Id = item.AddressId;

                    if (!AlreadyExist(_autoComplete, _item))
                    {
                        _autoComplete.Add(_item);
                    }
                });

            }
            else
            {
                AutoComplete _item = new AutoComplete();
                _item.Name = WebStore_Resources.TextNoAddressMatched;
                _item.Id = 0;
                _autoComplete.Add(_item);
            }
            return _autoComplete;
        }

        //Check whether alternate address exist, if exist append separator.
        public virtual string CheckAndAppendAlternateAddress(AddressViewModel model)
        {
            return !string.IsNullOrEmpty(model.Address3) ? string.Format("| " + model.Address3) : model.Address3;
        }

        public virtual int GetOrderIdFromCookie()
        {
            int OrderId = 0;
            Int32.TryParse(CookieHelper.GetCookieValue<string>(WebStoreConstants.UserOrderReceiptOrderId), out OrderId);
            return OrderId;
        }

        //To generate unique order number on basis of current date.
        public virtual string GenerateOrderNumber(int portalId)
        {
            try
            {
                string portalName = PortalAgent.CurrentPortal.Name;
                string orderNumber = string.Empty;

                if (!string.IsNullOrEmpty(portalName))
                {
                    orderNumber = portalName.Trim().Length > 2 ? portalName.Substring(0, 2) : portalName.Substring(0, 1);
                }

                string randomSuffix = GetRandomCharacters();

                DateTime date = DateTime.Now;
                // we have removed '-fff' from the date string as order number field length not exceeds the limit.
                // This change in made for the ticket ZPD-13806
                String strDate = date.ToString("yyMMdd-HHmmss");
                orderNumber += $"-{strDate}-{randomSuffix}";

                return orderNumber.ToUpper();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GenerateOrderNumber method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Set billing and shipping address
        public virtual AddressViewModel SetAddressByAddressType(string type, int addressId, AddressListViewModel addressListViewModel)
        {
            if (addressId == 0 || (addressListViewModel.AddressList == null || addressListViewModel.AddressList.Count == 0))
            {
                if (type.Equals(WebStoreConstants.ShippingAddressType, StringComparison.InvariantCultureIgnoreCase))
                {
                    return addressListViewModel.ShippingAddress;
                }
                else
                {
                    return addressListViewModel.BillingAddress;
                }
            }
            else
            {
                return addressListViewModel.AddressList.FirstOrDefault(x => x.AddressId == addressId);
            }
        }

        #endregion

        #region AmazonPay
        // Process amazon payment.
        public virtual AddressViewModel GetAmazonAddress(int PaymentSettingId, string amazonOrderReferenceId, string total, string accessToken = null)
        {
            try
            {
                PaymentSettingModel paymentSettingModel = _paymentAgent.GetPaymentSetting(PaymentSettingId)?.ToModel<PaymentSettingModel>();
                SubmitPaymentModel paymentModel = PaymentViewModelMap.ToAmazonPaySubmitPayModel(paymentSettingModel, amazonOrderReferenceId, total, accessToken);
                return PaymentViewModelMap.ToAddressViewModel(_paymentAgent.GetAmazonPayAddressDetails(paymentModel));
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetAmazonAddress method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Map data which is comes from amazon return url.
        public virtual SubmitOrderViewModel SetAmazonPayDetails(string amazonOrderReferenceId, string paymentType, int shippingOptionId, int paymentSettingId, string paymentCode, string additionalInstruction, string captureId, string orderNumber, string inHandDate = "", string jobName = "", string shippingConstraintCode = "")
        {
            SubmitOrderViewModel submitOrderViewModel = new SubmitOrderViewModel();
            submitOrderViewModel.AdditionalInstruction = additionalInstruction;
            submitOrderViewModel.AmazonOrderReferenceId = amazonOrderReferenceId;
            submitOrderViewModel.PaymentType = paymentType;
            submitOrderViewModel.ShippingOptionId = shippingOptionId;
            submitOrderViewModel.PaymentSettingId = paymentSettingId;
            submitOrderViewModel.PaymentCode = paymentCode;
            submitOrderViewModel.PaymentToken = captureId;
            submitOrderViewModel.IsFromAmazonPay = true;
            submitOrderViewModel.OrderNumber = orderNumber;

            if (!string.IsNullOrEmpty(inHandDate))
            {
                DateTime date;
                DateTime.TryParse(inHandDate, out date);
                submitOrderViewModel.InHandDate = date;
            }

            submitOrderViewModel.JobName = jobName;
            submitOrderViewModel.ShippingConstraintCode = shippingConstraintCode;
            return submitOrderViewModel;
        }

        //AmazonPay payment process.
        protected virtual OrdersViewModel AmazonPaymentProcess(SubmitOrderViewModel submitOrderViewModel, ShoppingCartModel cartModel, List<AddressModel> userAddresses)
        {
            try
            {
                ////Added Payment type as Amazon Pay via as Card Type.
                submitOrderViewModel.CardType = "Amazon";

                SubmitPaymentModel model = PaymentViewModelMap.ToModel(cartModel, submitOrderViewModel);
                if (Equals(cartModel?.ShippingAddress?.AddressId, 0))
                    return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorProcessPayment);

                GatewayResponseModel gatewayResponse = _paymentAgent.ProcessPayNow(model);
                if (gatewayResponse?.HasError ?? true)
                {
                    _cartAgent.ClearCartCountFromSession();
                    RemoveInSession(WebStoreConstants.CartModelSessionKey);
                    return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment);
                }
                //submitOrderViewModel.AmazonPayReturnUrl = 
                return new OrdersViewModel() { TrackingNumber = gatewayResponse.Token, PaymentStatus = string.IsNullOrWhiteSpace(gatewayResponse.Token) ? "False" : "True" };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in AmazonPaymentProcess method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get amazon payment option.
        public virtual PaymentSettingViewModel AmazonPaymentSetting(bool isQuotes = false)
        {
            List<PaymentSettingModel> paymentSettingList = _paymentAgent.GetPaymentSettingByUserDetailsFromCache(_paymentAgent.GetUserPaymentSettingDetails(), false, isQuotes);

            PaymentSettingModel paymentSettingModel = paymentSettingList?.Where(x => string.Equals(x.PaymentTypeName, ZnodeConstant.Amazon_Pay, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return IsNotNull(paymentSettingModel) ? _paymentAgent.GetPaymentSetting(paymentSettingModel.PaymentSettingId) : null;
        }

        //Get Amazon shipping Option.
        public virtual ShippingOptionListViewModel GetAmazonShippingOptions(string amazonOrderReferenceId, int paymentSettingId, string total, string shippingTypeName = null, string accessToken = null, string accountNumber = null, string shippingMethod = null)
        {
            AddressListViewModel addressList = new AddressListViewModel() { ShippingAddress = new AddressViewModel() };
            addressList.ShippingAddress = GetAmazonAddress(paymentSettingId, amazonOrderReferenceId, total, accessToken);
            bool isB2BUser = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.AccountId > 0;

            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                         _cartAgent.GetCartFromCookie();

            if (IsNotNull(cartModel) && !string.IsNullOrEmpty(shippingTypeName))
            {
                SetShippingTypeNameToModel(shippingTypeName, cartModel);
            }

            addressList = GetCartAddressList(cartModel);
            if (addressList?.ShippingAddress == null || addressList?.ShippingAddress?.AddressId == 0)
                addressList = _userAgent.GetAddressList();

            int omsQuoteId = (cartModel?.OmsQuoteId).GetValueOrDefault();

            if (!IsValidShippingAddress(addressList))
            {
                return new ShippingOptionListViewModel() { IsB2BUser = isB2BUser, OmsQuoteId = omsQuoteId };
            }

            if (string.IsNullOrEmpty(cartModel?.ShippingAddress?.PostalCode))
            {
                cartModel.ShippingAddress = addressList.ShippingAddress?.ToModel<AddressModel>();
            }

            List<ShippingOptionViewModel> ShippingOptions = GetShippingListAndRates(addressList?.ShippingAddress?.PostalCode, cartModel)?.ShippingOptions;
            if (IsNotNull(cartModel?.ShippingId))
                ShippingOptions?.Where(x => x.ShippingId == cartModel.ShippingId)?.FirstOrDefault(y => y.IsSelected = true);

            return new ShippingOptionListViewModel() { ShippingOptions = ShippingOptions, IsB2BUser = isB2BUser, OmsQuoteId = omsQuoteId, AccountNumber = accountNumber, ShippingMethod = shippingMethod };
        }

        //Get payment api header
        public virtual AjaxHeadersModel GetPaymentAPIHeader()
        {
            string cacheKey = WebStoreConstants.PaymentApiHeaderCacheKey + "_" + ZnodeAdminSettings.PaymentApplicationUrl.Replace("/", "").Replace(":", "");
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                var _header = GetClient<MediaManagerClient>();
                AjaxHeadersModel ajaxHeadersModel = new AjaxHeadersModel { Authorization = _header.GetAuthorizationHeader(string.Empty, string.Empty, ZnodeAdminSettings.PaymentApplicationUrl) };
                Helper.AddIntoCache(ajaxHeadersModel, cacheKey, "CurrentPortalCacheDuration");
            }
            return Helper.GetFromCache<AjaxHeadersModel>(cacheKey);
        }

        //Get application header
        public virtual AjaxHeadersModel GetAppHeader()
        {
            MediaManagerClient _header = GetClient<MediaManagerClient>();
            return new AjaxHeadersModel { Authorization = _header.GetAuthorizationHeader(_header.DomainName, _header.DomainKey), DomainName = _header.DomainName, Token = _header.Token };
        }

        #endregion

        #region protected Methods
        protected virtual List<BaseDropDownOptions> GetPaymentOptions(List<PaymentSettingModel> options)
        {
            return (from n in options.OrderBy(x => x.DisplayOrder)
                    select new BaseDropDownOptions
                    {
                        Id = n.PaymentCode,
                        Text = n.PaymentDisplayName,
                        Value = n.PaymentSettingId.ToString(),
                        Type = n.PaymentTypeName,
                        Status = n.IsApprovalRequired,
                        CustomStatus = n.IsOABRequired,
                        PortalPaymentGroupId = n.PortalPaymentGroupId ?? 0,
                        IsSelected = Equals(GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey)?.QuotePaymentSettingId, n.PaymentSettingId)
                    }).ToList();
        }

        //Set IsQuoteOrder true if quote id is greater than zero or user permission access is does not require approver.
        protected virtual void SetIsQuoteOrder(ShoppingCartModel cartModel, UserViewModel userViewModel)
        {
            if (cartModel.OmsQuoteId > 0 && (string.Equals(cartModel.OrderStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(userViewModel.PermissionCode, ZnodePermissionCodeEnum.DNRA.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(userViewModel.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(userViewModel.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            {
                cartModel.IsQuoteOrder = true;
            }
        }

        //Get Shipping Id.
        protected virtual int GetShippingId(ShoppingCartModel cartModel)
        {
            return cartModel.Shipping?.ShippingId < 1 ? cartModel.ShippingId : Convert.ToInt32(cartModel?.Shipping?.ShippingId);
        }

        protected virtual string GetProfileId()
        {
            UserViewModel Usermodel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

            //Get profileIds of logged in user. In case of guest user get ProfileId from Current portal object.
            string _profileId = Usermodel?.Profiles?.Count > 0 ?
                string.Join(",", Usermodel?.Profiles?.Select(i => i.ProfileId.ToString()).ToArray()) :
                PortalAgent.CurrentPortal.ProfileId > 0 ? PortalAgent.CurrentPortal.ProfileId.ToString() : string.Empty;

            if (string.IsNullOrEmpty(_profileId))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalProfileEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()));
                filters.Add(new FilterTuple(ZnodePortalProfileEnum.IsDefaultAnonymousProfile.ToString(), FilterOperators.Equals, "1"));
                PortalProfileListModel profileList = _profileClient.GetPortalProfiles(null, filters, null, null, null);
                _profileId = profileList?.PortalProfiles?.Count > 0 ? string.Join(",", profileList?.PortalProfiles?.Select(i => i.ProfileId.ToString()).ToArray()) : string.Empty;
            }
            return _profileId;
        }

        protected virtual List<ShippingOptionViewModel> ToViewModel(List<ShippingModel> model, int shippingOptionId)
        {
            if (IsNotNull(model))
            {

                return (from n in model.OrderBy(x => x.DisplayOrder)
                        select new ShippingOptionViewModel
                        {
                            ShippingId = n.ShippingId,
                            ProfileId = n.ProfileId,
                            ShippingCode = n.ShippingCode,
                            DestinationCountryCode = n.DestinationCountryCode,
                            Description = n.Description,
                            IsActive = n.IsActive,
                            IsSelected = Equals(n.ShippingId, shippingOptionId),
                            StateCode = n.StateCode
                        }).ToList();
            }
            return new List<ShippingOptionViewModel>();
        }

        //Get all address list of customer.
        protected virtual List<AddressModel> GetUserAddressList()
        {
            return _userAgent.GetAddressList()?.AddressList?.ToModel<AddressModel>()?.ToList();
        }

        //Check whether shipping address is valid or not.
        protected virtual Api.Models.BooleanModel IsValidAddressForCheckout(AddressModel addressModel)
        {
            if ((bool)PortalAgent.CurrentPortal.PortalFeatureValues.Where(x => x.Key.Contains(StoreFeature.Address_Validation.ToString()))?.FirstOrDefault().Value)
            {
                if (addressModel != null) addressModel.PublishStateId = (byte)PortalAgent.CurrentPortal.PublishState;

                //Do not allow the customer to go to next page if valid shipping address required is enabled.
                return _shippingsClient.IsShippingAddressValid(addressModel);
            }

            return new Api.Models.BooleanModel { IsSuccess = true };
        }

        //This method will be used to determine whether to check address validation or not at the time of place order.
        //Currently, it will return false but its behavior can be overridden to enable address validation check for place order.
        protected virtual bool IsAddressValidationRequiredForOrder()
                => false;

        //Get user details.
        protected virtual UserModel SetUserDetails(int userId)
        {
            try
            {
                //Get current user details from session.
                UserModel user = HelperUtility.CreateDeepCloneObject(_userAgent.GetUserViewModelFromSession())?.ToModel<UserModel>();

                //If session data is null get user details by userId.
                if (IsNull(user))
                {
                    user = _userClient.GetUserAccountData(userId);
                }

                //Get current user profile.
                string profileId = GetProfileId();

                if (user?.ProfileId <= 0)
                {
                    if (!string.IsNullOrEmpty(profileId))
                    {
                        if (profileId.Contains(','))
                            user.ProfileId = string.IsNullOrEmpty(profileId) ? 0 : Convert.ToInt32(profileId.Split(',')[0]);
                        else
                            user.ProfileId = string.IsNullOrEmpty(profileId) ? 0 : Convert.ToInt32(profileId);
                    }
                    else
                        user.ProfileId = 0;
                }

                return user;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetUserDetails method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Payment details.
        protected virtual void GetPaymentDetails(int paymentSettingId, ShoppingCartModel model)
        {
            SetUsersPaymentDetails(paymentSettingId, model, true);
        }

        //Bind the shipping and billing address of user to cart model.
        protected virtual void SetShippingBillingAddress(int shippingAddressId, int billingAddressId, List<AddressModel> addressList, ShoppingCartModel cartModel)
        {
            try
            {
                if (addressList?.Count > 0 && IsNotNull(cartModel))
                {
                    //If address id is not available in checkout model then use address of user.
                    cartModel.ShippingAddress = (cartModel?.ShippingAddress?.AddressId > 0 && IsNotNull(cartModel.ShippingAddress)) ? cartModel.ShippingAddress : addressList.FirstOrDefault(x => x.AddressId == shippingAddressId);
                    cartModel.BillingAddress = (cartModel?.BillingAddress?.AddressId > 0 && IsNotNull(cartModel.BillingAddress)) ? cartModel.BillingAddress : addressList.FirstOrDefault(x => x.AddressId == billingAddressId);
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetShippingBillingAddress method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Set amazon address.
        protected virtual void SetAmazonAddress(SubmitOrderViewModel submitOrderViewModel, ShoppingCartModel cartModel)
        {

            SubmitPaymentModel model = _paymentAgent.GetAmazonPayAddressDetails(new SubmitPaymentModel
            {
                AmazonOrderReferenceId = submitOrderViewModel.AmazonOrderReferenceId,
                PaymentCode = submitOrderViewModel.PaymentCode,
                PaymentSettingId = cartModel.Payment.PaymentSetting.PaymentSettingId,
                GatewayLoginPassword = cartModel.Payment.PaymentSetting.GatewayPassword,
                GatewayLoginName = cartModel.Payment.PaymentSetting.GatewayUsername,
                GatewayTransactionKey = cartModel.Payment.PaymentSetting.TransactionKey,
                Total = Convert.ToString(submitOrderViewModel.Total),
                OrderId = cartModel.OrderNumber
            });
            string[] names = model?.BillingName?.Split(' ');
            AddressModel addressModel = new AddressModel
            {
                Address1 = model.BillingStreetAddress1,
                Address2 = model.BillingStreetAddress2,
                CityName = model.BillingCity,
                StateCode = model.BillingStateCode,
                CountryName = model.BillingCountryCode,
                StateName = model.BillingStateCode,
                PostalCode = model.BillingPostalCode,
                FirstName = names?.Length > 0 ? names[0] : null,
                LastName = names?.Length > 1 ? names[1] : null,
                PhoneNumber = model.BillingPhoneNumber,
                DisplayName = "Amazon",
                DontAddUpdateAddress = false,
                EmailAddress = model.BillingEmailId,
                AddressId = cartModel?.BillingAddress?.AddressId > 0 ? cartModel.BillingAddress.AddressId : 0,

            };

            cartModel.ShippingAddress = addressModel;
            cartModel.BillingAddress = addressModel;
        }

        //Bind all details of shopping  cart model.
        protected virtual void SetShoppingCartDetails(SubmitOrderViewModel submitOrderViewModel, List<AddressModel> addressList, ShoppingCartModel cartModel)
        {
            try
            {
                if (IsNotNull(cartModel))
                {
                    cartModel.OrderDate = DateTime.Now;
                    cartModel.UserDetails = SetUserDetails(submitOrderViewModel.UserId);

                    if (submitOrderViewModel.IsFromAmazonPay)
                    {
                        SetAmazonData(submitOrderViewModel, addressList, cartModel);
                    }

                    //Get shipping and billing address of current user.
                    SetShippingBillingAddress(submitOrderViewModel.ShippingAddressId, submitOrderViewModel.BillingAddressId, addressList, cartModel);


                    cartModel.UserDetails.Email = string.IsNullOrEmpty(cartModel.UserDetails?.Email) ? cartModel.BillingEmail : cartModel.UserDetails?.Email;
                    cartModel.UserDetails.UserId = cartModel.OmsQuoteId > 0 ? cartModel.SelectedAccountUserId : submitOrderViewModel.UserId;

                    //Get the shipping details.
                    SetShippingDetails(submitOrderViewModel.ShippingOptionId, cartModel, submitOrderViewModel.AccountNumber, submitOrderViewModel.ShippingMethod);

                    //Get the payment details.
                    if (IsNull(cartModel.Payment?.PaymentName))
                    {
                        GetPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel);
                    }

                    if (submitOrderViewModel.IsFromAmazonPay)
                    {
                        PaymentSettingModel _paymentModel = _paymentAgent.GetPaymentSettingByPaymentCodeFromCache(submitOrderViewModel.PaymentCode);
                        cartModel.Payment.IsPreAuthorize = cartModel.IsGatewayPreAuthorize = _paymentModel.PreAuthorize;
                    }
                    string affiliateId = GetFromSession<string>(WebStoreConstants.AffiliateIdSessionKey);
                    cartModel.UserDetails.ReferralUserId = string.IsNullOrEmpty(affiliateId) ? (int?)null : Convert.ToInt32(affiliateId);

                    cartModel.PurchaseOrderNumber = submitOrderViewModel.PurchaseOrderNumber;
                    cartModel.PODocumentName = !string.IsNullOrEmpty(submitOrderViewModel.PODocumentName) ? $"{WebStoreConstants.PODocumentPath}/{submitOrderViewModel.PODocumentName}" : null;

                    cartModel.AdditionalInstructions = submitOrderViewModel.AdditionalInstruction;
                    cartModel.CreditCardNumber = submitOrderViewModel.CreditCardNumber;
                    cartModel.CardType = submitOrderViewModel.CardType;
                    cartModel.CreditCardExpMonth = submitOrderViewModel.CreditCardExpMonth;
                    cartModel.CreditCardExpYear = submitOrderViewModel.CreditCardExpYear;
                    cartModel.InHandDate = submitOrderViewModel.InHandDate;
                    cartModel.JobName = submitOrderViewModel.JobName;
                    cartModel.ShippingConstraintCode = submitOrderViewModel.ShippingConstraintCode;
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetShoppingCartDetails method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Bind amazonpay data.
        protected virtual void SetAmazonData(SubmitOrderViewModel submitOrderViewModel, List<AddressModel> addressList, ShoppingCartModel cartModel)
        {
            try
            {
                UserViewModel user = GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey);
                SetUsersPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel, true);
                cartModel.IsGatewayPreAuthorize = cartModel.Payment.IsPreAuthorize;
                if (user?.GuestUserId > 0)
                {
                    submitOrderViewModel.ShippingAddressId = addressList?.FirstOrDefault()?.AddressId ?? 0;
                    submitOrderViewModel.BillingAddressId = addressList?.FirstOrDefault()?.AddressId ?? 0;
                }
                else

                {
                    SetAmazonAddress(submitOrderViewModel, cartModel);
                    AddressModel addressDetails = SetAddressForAmazon(addressList, cartModel);
                    int addressId = addressDetails?.AddressId > 0 ? addressDetails.AddressId : addressList?.FirstOrDefault(x => x.AddressId == cartModel.ShippingAddress.AddressId)?.AddressId ?? 0;
                    submitOrderViewModel.ShippingAddressId = addressId;
                    submitOrderViewModel.BillingAddressId = addressId;

                    if (submitOrderViewModel.ShippingAddressId == 0)
                    {
                        AddressViewModel addressModel = _userAgent.CreateUpdateAddress(new AddressViewModel
                        {
                            Address1 = cartModel.ShippingAddress.Address1,
                            Address2 = cartModel.ShippingAddress.Address2,
                            CityName = cartModel.ShippingAddress.CityName,
                            StateName = cartModel.ShippingAddress.StateName,
                            CountryName = cartModel.ShippingAddress.CountryName,
                            PostalCode = cartModel.ShippingAddress.PostalCode,
                            FirstName = cartModel.ShippingAddress.FirstName,
                            LastName = cartModel.ShippingAddress.LastName,
                            PhoneNumber = cartModel.ShippingAddress.PhoneNumber,
                            DisplayName = "Amazon Address",
                            UseSameAsShippingAddress = true,
                            EmailAddress = cartModel.ShippingAddress.EmailAddress,
                            DontAddUpdateAddress = false
                        });
                        submitOrderViewModel.ShippingAddressId = addressModel.AddressId;
                        submitOrderViewModel.BillingAddressId = addressModel.AddressId;
                        addressList.Add(addressModel.ToModel<AddressModel>());
                    }
                    else
                    {
                        cartModel.ShippingAddress.AddressId = addressId;
                        cartModel.BillingAddress.AddressId = addressId;
                    }

                }

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetAmazonData method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        // this method used for matching and selecting address from address list.
        protected virtual AddressModel SetAddressForAmazon(List<AddressModel> addressList, ShoppingCartModel cartModel)
        {
            AddressModel addressDetails = new AddressModel();
            if (IsNotNull(addressList))
            {
                foreach (AddressModel address in addressList)
                {
                    if (Equals(address.FirstName, cartModel.ShippingAddress.FirstName) && Equals(address.LastName, cartModel.ShippingAddress.LastName) &&
                        Equals(address.Address1, cartModel.ShippingAddress.Address1) && Equals(address.Address2, cartModel.ShippingAddress.Address2) &&
                        Equals(address.CityName, cartModel.ShippingAddress.CityName) &&
                        Equals(address.PostalCode, cartModel.ShippingAddress.PostalCode)
                        && Equals(address.PhoneNumber, cartModel.ShippingAddress.PhoneNumber))
                    {
                        return address;
                    }
                }
            }
            return addressDetails;
        }
        //Get shipping details  by shippingId.
        protected virtual void SetShippingDetails(int shippingId, ShoppingCartModel cartModel, string accountNumber, string shippingMethod)
        {
            try
            {
                if (IsNull(cartModel?.Shipping))
                {
                    cartModel.Shipping = new OrderShippingModel();
                }

                cartModel.Shipping.AccountNumber = accountNumber;
                cartModel.Shipping.ShippingMethod = shippingMethod;
                cartModel.Shipping.ShippingId = cartModel.FreeShipping ? cartModel.Shipping.ShippingId : shippingId;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetShippingDetails method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Place new order.
        protected virtual OrdersViewModel PlaceOrder(ShoppingCartModel cartModel)
        {
            try
            {
                if (IsNotNull(cartModel))
                {
                    //If transactiondate is null then set current date as transaction date.
                    if (cartModel?.TransactionDate == null)
                        cartModel.TransactionDate = HelperUtility.GetDateWithTime();
                    //As Complete data is already available in shopping cart model,calculate call is avoided.
                    cartModel.SkipCalculations = SkipOrderCalculations();
                    //Place the order.
                    _orderClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                    return _orderClient.CreateOrder(cartModel)?.ToViewModel<OrdersViewModel>();
                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Error);
                //to delete the upload zip file which is temporarily saved.
                if (!string.IsNullOrEmpty(cartModel.PODocumentName))
                {
                    DeletePurchaseOrderFile(cartModel.PODocumentName);
                }

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
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Create guest users addresses.
        protected virtual AddressModel CreateAnonymousUserAddress(AddressModel address)
        {
            return _webStoreAccountClient.CreateAccountAddress(address);
        }

        protected virtual void SetUsersPaymentDetails(int paymentSettingId, ShoppingCartModel model, bool isRequiredExpand = false)
        {
            try
            {
                PaymentSettingModel paymentSetting = _paymentAgent.GetPaymentSetting(paymentSettingId, PortalAgent.CurrentPortal.PortalId)?.ToModel<PaymentSettingModel>();

                string paymentName = string.Empty;
                if (IsNotNull(paymentSetting))
                {
                    paymentName = paymentSetting.PaymentTypeName;
                }

                model.Payment = PaymentViewModelMap.ToPaymentModel(model, paymentSetting, paymentName);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetUsersPaymentDetails method for OrderNumber " + model?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

        }

        // Get payment response.
        public virtual GatewayResponseModel GetPaymentResponse(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel)
        {
            try
            {
                // Map shopping Cart model and submit Payment view model to Submit payment model 
                SubmitPaymentModel model = PaymentViewModelMap.ToModel(cartModel, submitOrderViewModel);

                // Map Customer Payment Guid for Save Credit Card 
                if (!string.IsNullOrEmpty(submitOrderViewModel.CustomerGuid) && string.IsNullOrEmpty(cartModel.UserDetails.CustomerPaymentGUID))
                {
                    UserModel userModel = _userClient.GetUserAccountData(submitOrderViewModel.UserId);
                    userModel.CustomerPaymentGUID = submitOrderViewModel.CustomerGuid;
                    _userClient.UpdateCustomerAccount(userModel);

                    UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);

                    if (string.IsNullOrEmpty(userViewModel.CustomerPaymentGUID))
                    {
                        userViewModel.CustomerPaymentGUID = submitOrderViewModel.CustomerGuid;
                        SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
                    }
                }

                model.Total = _paymentAgent.FormatOrderTotal(cartModel.Total);

                return _paymentAgent.ProcessPayNow(model);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetPaymentResponse method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Check if paypal express return url shipping and billing address id is "0".
        protected virtual string CheckQueryStringForAddressId(string payPalReturnUrl, List<AddressModel> userAddresses)
        {
            bool isChange = false;
            if (!string.IsNullOrEmpty(payPalReturnUrl))
            {
                Uri uri = new Uri(payPalReturnUrl);
                NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
                UserViewModel guestViewModel = GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey) ?? new ViewModels.UserViewModel();
                AddressListViewModel addressList = _userAgent.GetAddressList();
                if (Equals(queryString.Get("ShippingAddressId"), "0"))
                {
                    string shippingAddressId = string.Empty;
                    if (guestViewModel?.UserId > 1)
                    {
                        shippingAddressId = Convert.ToString(userAddresses?.Where(w => w.IsDefaultShipping == true).Select(s => s.AddressId).FirstOrDefault());
                    }
                    else
                    {
                        shippingAddressId = Convert.ToString(addressList?.AddressList?.Where(w => w.IsDefaultShipping == true).Select(s => s.AddressId).FirstOrDefault());
                    }

                    queryString.Set("ShippingAddressId", shippingAddressId);
                    isChange = true;

                }
                if (Equals(queryString.Get("BillingAddressId"), "0"))
                {
                    string billingAddressId = string.Empty;
                    if (guestViewModel?.UserId > 1)
                    {
                        billingAddressId = Convert.ToString(userAddresses?.Where(w => w.IsDefaultBilling == true).Select(s => s.AddressId).FirstOrDefault());
                    }
                    else
                    {
                        billingAddressId = Convert.ToString(addressList?.AddressList?.Where(w => w.IsDefaultBilling == true).Select(s => s.AddressId).FirstOrDefault());
                    }

                    queryString.Set("BillingAddressId", billingAddressId);
                    isChange = true;
                }
                if (isChange)
                {
                    string[] url = payPalReturnUrl.Split('?');
                    payPalReturnUrl = url[0] + "?" + queryString;
                }
            }
            return payPalReturnUrl;
        }

        //Remove Invalid coupon code/giftcard.
        protected virtual void RemoveInvalidDiscountCode(ShoppingCartModel cartModel)
        {
            //Remove invalid coupon code.
            if (cartModel.Coupons?.Count > 0)
            {
                cartModel.Coupons.RemoveAll(x => !x.CouponApplied);
            }
        }

        //to get filtered shipping option by zipcode
        protected virtual List<ShippingModel> GetShippingByZipCode(string zipcode, List<ShippingModel> shippinglist)
        {
            if (shippinglist?.Count > 0)
            {
                List<ShippingModel> filteredShippingList = new List<ShippingModel>();
                //to check each shipping option have zipcode entered by user
                foreach (ShippingModel shipping in shippinglist)
                {
                    //if shipping option zipcode is null or "*" then allow for all zipcode entered by user 
                    if (string.IsNullOrEmpty(shipping.ZipCode) || shipping.ZipCode.Trim() == "*")
                    {
                        filteredShippingList.Add(shipping);
                    }
                    else
                    {
                        //if shipping option zipcode contains "," then it will have more than one zipcode allows
                        if (shipping.ZipCode.Contains(","))
                        {
                            string[] allZipCodesAssignToShipping = shipping.ZipCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            //to check each zipcode that entered against shipping  option comma separate
                            foreach (string shippingZipCode in allZipCodesAssignToShipping)
                            {
                                //to check zipcode for each shipping 
                                if (IsValidShippingZipCode(zipcode, shippingZipCode, shipping, filteredShippingList))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            IsValidShippingZipCode(zipcode, shipping.ZipCode, shipping, filteredShippingList);
                        }
                    }
                }
                return filteredShippingList;
            }
            return shippinglist;
        }

        //to check zipcode is valid for shipping option
        protected virtual bool IsValidShippingZipCode(string userZipcode, string shippingOptionZipcode, ShippingModel shipping, List<ShippingModel> filteredShippingList)
        {
            bool result = false;
            //add shipping Option zipcode having "*"
            if (shippingOptionZipcode.Contains("*"))
            {
                string shippingZipCode = shippingOptionZipcode.Replace("*", string.Empty).Trim();
                //shipping Option Zipcode start with the user zipcode then allow to add
                if (userZipcode.Trim().StartsWith(shippingZipCode))
                {
                    filteredShippingList.Add(shipping);
                    result = true;
                }
            }
            //add if shipping Option Zipcode is same as user zipcode then allow to add
            else if (string.Equals(shippingOptionZipcode.Trim(), userZipcode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                filteredShippingList.Add(shipping);
                result = true;
            }
            return result;
        }

        //to check user shippingaddress is valid
        protected virtual bool IsValidShippingAddress(AddressListViewModel addressList)
        {
            bool isValid = true;
            string shippingCountryCode = addressList?.ShippingAddress?.CountryName ?? string.Empty;
            string shippingstateCode = addressList?.ShippingAddress?.StateName ?? string.Empty;
            string shippingZipCode = addressList?.ShippingAddress?.PostalCode ?? string.Empty;

            //if user shipping CountryCode, state and zipcode is null then no shipping option will available for that user address
            if (string.IsNullOrEmpty(shippingCountryCode) || string.IsNullOrEmpty(shippingstateCode) || string.IsNullOrEmpty(shippingZipCode))
            {
                isValid = false;
            }
            return isValid;
        }

        //Get customer required details from shopping cart.
        protected virtual void GetUserCartDetails(CheckoutViewModel checkoutViewModel)
        {
            try
            {
                ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ?? _cartAgent.GetCartFromCookie();
                if (IsNotNull(cartModel))
                {
                    checkoutViewModel.OrderStatus = cartModel.OrderStatus;
                    checkoutViewModel.QuoteId = cartModel.OmsQuoteId;
                    checkoutViewModel.SubTotal = cartModel.SubTotal.GetValueOrDefault();
                    checkoutViewModel.Total = cartModel.Total.GetValueOrDefault();
                    checkoutViewModel.IsLevelApprovedOrRejected = cartModel.IsLevelApprovedOrRejected;

                    IWebstoreHelper _webstoreHelper = GetService<IWebstoreHelper>();
                    checkoutViewModel.InHandDate = _webstoreHelper.GetInHandDate();
                    checkoutViewModel.ShippingConstraints = _webstoreHelper.GetEnumMembersNameAndDescription(ShippingConstraintsEnum.ShipComplete);

                    if (checkoutViewModel.QuoteId > 0)
                    {
                        IAccountQuoteClient _accountQuoteClient = GetClient<AccountQuoteClient>();
                        checkoutViewModel.IsLastApprover = _accountQuoteClient.IsLastApprover(cartModel.OmsQuoteId);
                    }
                    else
                        checkoutViewModel.IsLastApprover = cartModel?.IsLastApprover ?? false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetUserCartDetails method for is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Check whether approval routing is required for the current user quote.
        protected virtual void IsRequireApprovalRouting(CheckoutViewModel checkoutViewModel)
        {
            //Needed approvers associated to user irrespective of quote to show it on Account dashboard.
            UserApproverListViewModel model = _userAgent.GetUserApproverList(0, true);
            if (model?.UserApprover?.Count > 0)
            {
                checkoutViewModel.ApproverCount = true;
                int firstLevelOrder = model.UserApprover.Min(x => x.ApproverOrder);
                decimal? firstLevelBudgetStart = model.UserApprover.Where(x => x.ApproverOrder == firstLevelOrder)?.Select(x => x.FromBudgetAmount)?.FirstOrDefault();
                if (IsNotNull(firstLevelBudgetStart) && checkoutViewModel.Total > firstLevelBudgetStart)
                    checkoutViewModel.IsRequireApprovalRouting = true;
            }
        }

        //Get customer information from session.
        protected virtual void GetUserInfo(CheckoutViewModel checkoutViewModel)
        {
            UserViewModel userViewModel = (checkoutViewModel.UserId == 0) ? _userAgent.GetUserViewModelFromSession() : _userAgent.GetUserAccountData(checkoutViewModel.UserId);
            if (IsNotNull(userViewModel))
            {
                checkoutViewModel.UserId = userViewModel.UserId;
                checkoutViewModel.RoleName = userViewModel.RoleName;
                checkoutViewModel.PermissionCode = userViewModel.PermissionCode;
                checkoutViewModel.BudgetAmount = userViewModel.BudgetAmount.GetValueOrDefault();
                if (PortalAgent.CurrentPortal.EnableApprovalManagement)
                {
                    checkoutViewModel.EnableApprovalRouting = PortalAgent.CurrentPortal.EnableApprovalManagement;
                    PortalApprovalModel portalApprovalModel = _portalClient.GetPortalApproverDetailsById(PortalAgent.CurrentPortal.PortalId);
                    if (portalApprovalModel.OrderLimit == 0 || checkoutViewModel.SubTotal >= portalApprovalModel.OrderLimit)
                        checkoutViewModel.ShowPlaceOrderButton = false;
                    else
                        checkoutViewModel.ShowPlaceOrderButton = true;

                    checkoutViewModel.OrderLimit = portalApprovalModel?.OrderLimit ?? 0;
                    if (portalApprovalModel?.PortalApprovalTypeName == ZnodePortalApprovalsLevelEnum.User.ToString())
                        IsRequireApprovalRouting(checkoutViewModel);
                    checkoutViewModel.ApprovalType = portalApprovalModel.PortalApprovalTypeName;
                }
                else
                    checkoutViewModel.ShowPlaceOrderButton = true;
            }
            //For guest user place order button should be always visible.
            else
                checkoutViewModel.ShowPlaceOrderButton = true;
        }

        // Process paypal express payment.
        protected virtual OrdersViewModel PayPalExpressPaymentProcess(SubmitOrderViewModel submitOrderViewModel, ShoppingCartModel cartModel, List<AddressModel> userAddresses)
        {
            try
            {
                submitOrderViewModel.CardType = "Paypal";

                submitOrderViewModel.PayPalReturnUrl = CheckQueryStringForAddressId(submitOrderViewModel.PayPalReturnUrl, userAddresses);
                if (submitOrderViewModel.PaymentApplicationSettingId == 0)
                {
                    PaymentSettingModel paymentSetting = _paymentAgent.GetPaymentSettingFromCache(submitOrderViewModel.PaymentSettingId, PortalAgent.CurrentPortal.PortalId);
                    submitOrderViewModel.PaymentApplicationSettingId = paymentSetting.PaymentApplicationSettingId;
                }
                ZnodeLogging.LogMessage($"PaymentApplicationSettingId - {submitOrderViewModel.PaymentApplicationSettingId}");
                cartModel.ShippingAddress = cartModel.ShippingAddress ?? userAddresses?.FirstOrDefault(w => w.IsDefaultShipping == true);
                if (IsNotNull(cartModel.ShippingAddress))
                {
                    SubmitPaymentModel model = PaymentViewModelMap.ToModel(cartModel, submitOrderViewModel);

                    GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                    //Call PayPal payment finalize method in Payment Application if payment initialization token is present.
                    gatewayResponse = !string.IsNullOrEmpty(submitOrderViewModel.PayPalToken) ? _paymentAgent.FinalizePayPalProcess(model)
                                            : _paymentAgent.ProcessPayPal(model);

                    if (gatewayResponse?.HasError ?? true)
                    {
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment);
                    }
                    return new OrdersViewModel() { PayPalExpressResponseText = gatewayResponse.ResponseText, PayPalExpressResponseToken = gatewayResponse.TransactionId, PaymentStatus = gatewayResponse.PaymentStatus.ToString() };
                }
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorProcessPayment);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in PayPalExpressPaymentProcess method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Process credit card payment.
        protected virtual OrdersViewModel ProcessCreditCardPayment(SubmitOrderViewModel submitOrderViewModel, ShoppingCartModel cartModel)
        {
            try
            {
                SetUsersPaymentDetails(submitOrderViewModel.PaymentSettingId, cartModel);
                submitOrderViewModel.PaymentType = cartModel?.Payment?.PaymentName;
                GatewayResponseModel gatewayResponse = GetPaymentResponse(cartModel, submitOrderViewModel);
                if (string.Equals(submitOrderViewModel.GatewayCode, ZnodeConstant.CyberSource, StringComparison.CurrentCultureIgnoreCase))
                {
                    SaveCustomerPaymentGuid(submitOrderViewModel.UserId, gatewayResponse.CustomerGUID, cartModel.UserDetails.CustomerPaymentGUID);
                }
                if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
                {
                    //RemoveInSession(WebStoreConstants.CartModelSessionKey);
                    return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? (string.Equals(gatewayResponse.ErrorMessage, WebStore_Resources.ErrorCardConnectGatewayResponse, StringComparison.InvariantCultureIgnoreCase) ? WebStore_Resources.ErrorProcessPayment : gatewayResponse.ErrorMessage) : WebStore_Resources.ErrorProcessPayment);
                }


                //Map payment token
                cartModel.Token = gatewayResponse.Token;
                cartModel.IsGatewayPreAuthorize = gatewayResponse.IsGatewayPreAuthorize;
                cartModel.TransactionDate = gatewayResponse.TransactionDate;
                cartModel.TransactionId = gatewayResponse.Token;
                cartModel.Payment.PaymentStatusId = (int)gatewayResponse.PaymentStatus;
                submitOrderViewModel.TransactionId = gatewayResponse.Token;
                if (IsNotNull(cartModel?.Payment?.PaymentSetting?.GatewayCode) && (string.Equals(cartModel.Payment.PaymentSetting.GatewayCode, ZnodeConstant.CyberSource, StringComparison.InvariantCultureIgnoreCase) || string.Equals(cartModel.Payment.PaymentSetting.GatewayCode, ZnodeConstant.AuthorizeNet, StringComparison.InvariantCultureIgnoreCase)))
                    cartModel.CreditCardNumber = gatewayResponse.CardNumber;
                return new OrdersViewModel();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in ProcessCreditCardPayment method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Create Shipping address of guest user.
        protected virtual AddressModel CreateGuestUserShippingAddress(int userId, ShoppingCartModel cartModel)
        {
            cartModel.ShippingAddress.UserId = userId;
            cartModel.ShippingAddress.IsGuest = true;
            //Create guest users addresses.
            return CreateAnonymousUserAddress(cartModel.ShippingAddress);
        }

        // Get userId from session.
        protected virtual int GetUserUserIdFromSession()
        {
            UserViewModel userViewModel = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
            int userId = IsNull(userViewModel) ? 0 : userViewModel.UserId;
            return userId > 0 ? userId : -1;
        }
        //Save Customer Payment Guid for Save Credit Card
        protected virtual void SaveCustomerPaymentGuid(int userId, string customerGuid, string customerPaymentGUID)
        {
            if (!string.IsNullOrEmpty(customerGuid) && string.IsNullOrEmpty(customerPaymentGUID))
            {
                UserModel userModel = _userClient.GetUserAccountData(userId);
                userModel.CustomerPaymentGUID = customerGuid;
                _userClient.UpdateCustomerAccount(userModel);
            }
        }
        //delete the upload zip file which is temporarily saved.
        protected virtual void DeletePurchaseOrderFile(string documentName)
        {
            if (!string.IsNullOrEmpty(documentName))
            {
                if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath($"{ZnodeWebstoreSettings.ZnodeApiRootUri}/{documentName.Replace("~", string.Empty)}"))))
                {
                    File.Delete(Path.Combine(HttpContext.Current.Server.MapPath($"{ZnodeWebstoreSettings.ZnodeApiRootUri}/{documentName.Replace("~", string.Empty)}")));
                }
            }
        }

        // Check inventory, min and max quantity.
        protected virtual OrdersViewModel CheckInventoryAndMinMaxQuantity(ShoppingCartModel cartModel)
        {
            try
            {
                _orderClient.SetProfileIdExplicitly(Helper.GetProfileId().GetValueOrDefault());
                OrderModel orderModel = _orderClient.CheckInventoryAndMinMaxQuantity(cartModel);
                return new OrdersViewModel() { IsInventoryAndMinMaxQuantityAvailable = true };
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Warning);
                //Set error message according to ErrorCode.
                switch (exception.ErrorCode)
                {
                    case ErrorCodes.ProcessingFailed:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
                    case ErrorCodes.ErrorSendResetPasswordLink:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel() { }, WebStore_Resources.ErrorOrderEmailNotSend);
                    case ErrorCodes.OutOfStockException:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel() { }, WebStore_Resources.OutOfStockException);
                    case ErrorCodes.MinAndMaxSelectedQuantityError:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), exception.ErrorMessage);
                    default:
                        return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (OrdersViewModel)GetViewModelWithErrorMessage(new OrdersViewModel(), WebStore_Resources.ProcessingFailedError);
            }
        }

        //Capture Payment
        protected virtual bool CapturePayment(int OmsOrderId, string paymentTransactionToken, OrdersViewModel orderViewModel)
        {
            try
            {
                Api.Models.BooleanModel booleanModel = _paymentClient.CapturePayment(paymentTransactionToken);
                if (!booleanModel?.HasError ?? true)
                {
                    return _orderClient.UpdateOrderPaymentStatus(OmsOrderId, ZnodeConstant.CAPTURED.ToString());
                }
                else
                    _orderClient.CreateOrderHistory(new OrderHistoryModel() { OmsOrderDetailsId = orderViewModel.OmsOrderDetailsId, Message = booleanModel?.ErrorMessage, TransactionId = orderViewModel.TransactionId, CreatedBy = orderViewModel.CreatedBy, ModifiedBy = orderViewModel.ModifiedBy });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return false;
        }

        //Capture Payment
        protected virtual bool AmazonCapturePayment(int OmsOrderId, string paymentTransactionToken)
        {
            try
            {
                Api.Models.BooleanModel booleanModel = _paymentClient.AmazonCapturePayment(paymentTransactionToken);
                if (!booleanModel?.HasError ?? true)
                {
                    return _orderClient.UpdateOrderPaymentStatus(OmsOrderId, ZnodeConstant.CAPTURED.ToString());
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return false;
        }

        protected virtual ExpandCollection SetExpandsForReceipt()
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

        protected virtual string GetTrackingUrlByShippingId(int shippingId)
        {
            return _shippingsClient.GetShipping(shippingId)?.TrackingUrl;
        }

        protected virtual void SetUserDetails(UserViewModel user, ShoppingCartModel cartModel)
        {
            user.Email = cartModel?.BillingAddress?.EmailAddress;
            user.FirstName = cartModel?.BillingAddress?.FirstName;
            user.LastName = cartModel?.BillingAddress?.LastName;
            _userClient.UpdateCustomer(user?.ToModel<UserModel>());
        }

        //Set tracking url.
        protected virtual string SetTrackingUrl(string trackingNo, string trackingUrl)
                     => string.IsNullOrEmpty(trackingUrl) ? trackingNo : "<a target=_blank href=" + trackingUrl + trackingNo + ">" + trackingNo + "</a>";

        //Get Address list for logged in user.
        protected virtual AddressListViewModel GetAddressListForUser(int addressId, int otherAddressId)
        {
            AddressListViewModel addressList;
            //Get Logged in user Address list
            AddressListViewModel loggedInUserAddressList = _userAgent.GetAddressList();
            //Set Address book availability flag
            loggedInUserAddressList?.AddressList?.ForEach(o => o.DontAddUpdateAddress = true);
            List<int> userAddressIds = loggedInUserAddressList?.AddressList?.Select(o => o.AddressId)?.ToList();
            //Get recently added address list (For one time use address)
            //Check if recently added address list is available in users associated address, If not then get it from address table and merge to users associated address 
            if (IsNotNull(userAddressIds) && userAddressIds.Contains(addressId) && userAddressIds.Contains(otherAddressId))
            {
                //Newly inserted address and previously inserted address is mapped to users address list
                addressList = loggedInUserAddressList;
            }
            else
            {
                //Get list from address table.
                AddressListViewModel availableAddressList = _addressAgent.GetAddressList(addressId, otherAddressId);
                if (loggedInUserAddressList?.AddressList?.Count > 0)
                {
                    addressList = loggedInUserAddressList;
                    //If  just inserted address are available in database then merge them to users address list.
                    if (availableAddressList?.AddressList?.Any(o => !userAddressIds.Contains(o.AddressId)) ?? false)
                    {
                        List<AddressViewModel> oneTimeAddress = availableAddressList.AddressList.Where(o => !userAddressIds.Contains(o.AddressId))?.ToList();
                        if (IsNotNull(oneTimeAddress))
                        {
                            oneTimeAddress.ForEach(o => o.DontAddUpdateAddress = false);

                            addressList.AddressList = addressList.AddressList.Union(oneTimeAddress)?.ToList();
                        }
                    }
                }
                else
                {
                    //Address not available in address-book for the user
                    //Set one time address flag.
                    availableAddressList?.AddressList
                                        ?.ToList()
                                        ?.ForEach(o => o.DontAddUpdateAddress = false);
                    //Logged in user has no address mapped to himself.
                    addressList = availableAddressList;
                }
            }
            return addressList;
        }


        //Get cart address list
        protected virtual AddressListViewModel GetCartAddressList(ShoppingCartModel cartModel)
        {
            return new AddressListViewModel()
            {
                ShippingAddress = cartModel?.ShippingAddress?.ToViewModel<AddressViewModel>(),
                BillingAddress = cartModel?.BillingAddress?.ToViewModel<AddressViewModel>(),
                AddressList = new List<AddressViewModel>() {
                    cartModel?.ShippingAddress?.ToViewModel<AddressViewModel>(),
                    cartModel?.BillingAddress?.ToViewModel<AddressViewModel>()
              }
            };
        }

        //Appends the available user address to the address list
        protected virtual void AppendUserAddress(string searchTerm, AddressListModel listModel, string addressType, int? pageIndex = null, int? recordPerPage = null)
        {
            if (IsNull(listModel.AddressList))
            {
                listModel.AddressList = new List<AddressModel>();
            }

            List<AddressModel> userAddresses;

            //In case of edit quote
            ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                         _cartAgent.GetCartFromCookie();

            //Get account/user address list
            UserModel userModel = (cartModel?.OmsQuoteId > 0)
                                            ? _userClient.GetUserAccountData(cartModel.UserId ?? 0)
                                            : _userClient.GetUserAccountData(GetUserUserIdFromSession());

            if (IsNotNull(userModel))
            {
                //SetFiltersForAddress(filters, userModel);
                FilterCollection filters = new FilterCollection();

                if (userModel.AccountId > 0)
                {
                    //Set filters for account id.
                    HelperMethods.SetAccountIdFilters(filters, Convert.ToInt32(userModel.AccountId));
                }
                else
                {
                    //Set filters for user id.
                    HelperMethods.SetUserIdFilters(filters, userModel.UserId);
                }

                //Get the sort collection for address id desc.
                SortCollection sortCollection = new SortCollection();
                sortCollection.Add(ZnodeAddressEnum.AddressId.ToString(), DynamicGridConstants.DESCKey);

                //expand for address.
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

                AddressListModel addressList = userModel.AccountId > 0
                    ? _accountClient.GetAddressList(expands, filters, sortCollection, pageIndex, recordPerPage)
                    : _customerClient.GetAddressList(expands, filters, sortCollection, pageIndex, recordPerPage);

                userAddresses = addressList?.AddressList?.ToList();

            }
            else
            {
                userAddresses = _userAgent.GetAddressList()
                                                  ?.AddressList
                                                  ?.ToModel<AddressModel>()?.ToList();
            }

            //Filter user address list
            List<AddressModel> filteredUserAddresses = userAddresses?.Where(o => ((o.FirstName != null) && o.FirstName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.LastName != null) && o.LastName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.DisplayName != null) && o.DisplayName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.CompanyName != null) && o.CompanyName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.Address1 != null) && o.Address1.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.Address2 != null) && o.Address2.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.Address3 != null) && o.Address3.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.CountryName != null) && o.CountryName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.CountryCode != null) && o.CountryCode.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.StateName != null) && o.StateName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.CityName != null) && o.CityName.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.PostalCode != null) && o.PostalCode.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.PhoneNumber != null) && o.PhoneNumber.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.Mobilenumber != null) && o.Mobilenumber.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.AlternateMobileNumber != null) && o.AlternateMobileNumber.ToLower().Contains(searchTerm.ToLower()))
                                                                              || ((o.FaxNumber != null) && o.FaxNumber.ToLower().Contains(searchTerm.ToLower())))
                                                                     ?.ToList();

            //Combine user address
            if (IsNotNull(filteredUserAddresses))
            {
                listModel.AddressList = listModel?.AddressList.Union(filteredUserAddresses)?.ToList();
            }
        }

        protected virtual bool AlreadyExist(List<AutoComplete> autoCompleteList, AutoComplete autoCompleteModel)
        {
            return autoCompleteList.Any(x => x.Id == autoCompleteModel.Id);
        }

        //Update the Session as per the new order total
        protected void UpdateUserDetailsInSession(ShoppingCartModel model, UserViewModel userViewModel)
        {
            if (IsNotNull(userViewModel) && IsNotNull(model))
            {
                userViewModel.FirstName = userViewModel.FirstName;
                userViewModel.LastName = userViewModel.LastName;
                userViewModel.PhoneNumber = userViewModel.PhoneNumber ?? model.BillingAddress?.PhoneNumber;
            }
            SaveInSession(WebStoreConstants.UserAccountKey, userViewModel);
        }

        //Perform calculation on shopping cart items
        public virtual ShoppingCartModel EnsureShoppingCartCalculations(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel)
        {
            try
            {
                //Check the order calculated correctly. i.e. total, tax cost and shipping cost calculated against order
                //This condition evaluates to be always true for COD
                //Taxcost gets zero incase of multitab scenario. ie. Checkout page and Cart page
                //Shipping and taxes do not get mapped incase the shoppingcartModel is fetched from cookie instead of session
                if (IsNull(submitOrderViewModel) || cartModel?.Total.GetValueOrDefault().ToPriceRoundOff() != submitOrderViewModel.Total.ToPriceRoundOff() || cartModel.TaxCost == 0 || ((cartModel.ShippingCost == 0) && !cartModel.FreeShipping))
                {
                    //If voucher is applied then calculate the voucher against order in calculation
                    cartModel.IsCalculateVoucher = (IsNotNull(cartModel?.Vouchers) && cartModel?.Vouchers.Count > 0) ? true : false;

                    //Perform calculation on cart item
                    ShoppingCartModel calculatedCartModel = GetClient<ShoppingCartClient>().Calculate(cartModel);

                    if (IsNotNull(calculatedCartModel))
                    {
                        //Map calculatation related properties from calculated shopping cart to shoppingcart which pass to create order
                        cartModel = MapCalculatedCartToShoppingCart(cartModel, calculatedCartModel);
                    }
                }
                return cartModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in EnsureShoppingCartCalculations method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map calculatation related properties from calculated shopping cart to shoppingcart which pass to create order
        protected virtual ShoppingCartModel MapCalculatedCartToShoppingCart(ShoppingCartModel cartModel, ShoppingCartModel calculatedCartModel)
        {
            try
            {
                calculatedCartModel.Shipping.AccountNumber = cartModel?.Shipping?.AccountNumber;
                calculatedCartModel.Shipping.ShippingMethod = cartModel?.Shipping?.ShippingMethod;
                //Gift card related properties
                cartModel.GiftCardAmount = calculatedCartModel.GiftCardAmount;
                cartModel.GiftCardBalance = calculatedCartModel.GiftCardBalance;

                //Discount related properties
                cartModel.Discount = calculatedCartModel.Discount;
                cartModel.OrderLevelDiscount = calculatedCartModel.OrderLevelDiscount;
                cartModel.OrderLevelShipping = calculatedCartModel.OrderLevelShipping;
                cartModel.OrderLevelTaxes = calculatedCartModel.OrderLevelTaxes;
                cartModel.CSRDiscountAmount = calculatedCartModel.CSRDiscountAmount;

                //Shipping related properties
                cartModel.ShippingCost = calculatedCartModel.ShippingCost;
                cartModel.ShippingDiscount = calculatedCartModel.ShippingDiscount;
                cartModel.ShippingHandlingCharges = calculatedCartModel.ShippingHandlingCharges;
                cartModel.ShippingDifference = calculatedCartModel.ShippingDifference;
                cartModel.CustomShippingCost = calculatedCartModel?.CustomShippingCost;
                cartModel.Shipping = IsNotNull(calculatedCartModel?.Shipping) ? calculatedCartModel.Shipping : null;

                //Tax related properties
                cartModel.TaxCost = calculatedCartModel.TaxCost;
                cartModel.TaxRate = calculatedCartModel.TaxRate;
                cartModel.SalesTax = calculatedCartModel.SalesTax;
                cartModel.Vat = calculatedCartModel?.Vat;
                cartModel.Gst = calculatedCartModel?.Gst;
                cartModel.Hst = calculatedCartModel?.Hst;
                cartModel.Pst = calculatedCartModel?.Pst;
                cartModel.CustomTaxCost = calculatedCartModel?.CustomTaxCost;
                cartModel.ImportDuty = calculatedCartModel.ImportDuty;
                cartModel.TaxSummaryList = calculatedCartModel.TaxSummaryList;

                //Vouchers properties mapping
                cartModel.Vouchers = IsNotNull(calculatedCartModel?.Vouchers) ? calculatedCartModel.Vouchers : new List<VoucherModel>();

                //Coupon properties mapping
                cartModel.Coupons = calculatedCartModel?.Coupons;

                //Cart line item wise mapping
                MapShoppingLineItemPricingProperties(cartModel, calculatedCartModel);

                //Other calculation properties
                cartModel.TotalAdditionalCost = calculatedCartModel?.TotalAdditionalCost;
                cartModel.SubTotal = calculatedCartModel?.SubTotal;
                cartModel.Total = calculatedCartModel?.Total;

                return cartModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapCalculatedCartToShoppingCart method for OrderNumber " + cartModel?.OrderNumber + " is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map line item calculatation related properties  from calculated shippingcart to cart  model
        protected virtual void MapShoppingLineItemPricingProperties(ShoppingCartModel cartModel, ShoppingCartModel calculatedCartModel)
        {
            if (IsNotNull(calculatedCartModel?.ShoppingCartItems))
            {
                //Map calculated shoppingcart line item properties to cartmodel line item properties
                foreach (ShoppingCartItemModel calculatedCartLineItem in calculatedCartModel?.ShoppingCartItems)
                {
                    if (IsNotNull(calculatedCartLineItem))
                    {
                        ShoppingCartItemModel shoppingCartItem = cartModel?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == calculatedCartLineItem.ExternalId);

                        if (IsNotNull(shoppingCartItem))
                        {
                            //Shipping related properties
                            shoppingCartItem.CustomShippingCost = calculatedCartLineItem.CustomShippingCost;

                            //Discount related properties
                            shoppingCartItem.ProductDiscountAmount = calculatedCartLineItem.ProductDiscountAmount;

                            //Tax related properties
                            shoppingCartItem.TaxCost = calculatedCartLineItem.TaxCost;

                            //Per quantity discount
                            shoppingCartItem.PerQuantityLineItemDiscount = calculatedCartLineItem.PerQuantityLineItemDiscount;
                            shoppingCartItem.PerQuantityCSRDiscount = calculatedCartLineItem.PerQuantityCSRDiscount;
                            shoppingCartItem.PerQuantityShippingCost = calculatedCartLineItem.PerQuantityShippingCost;
                            shoppingCartItem.PerQuantityShippingDiscount = calculatedCartLineItem.PerQuantityShippingDiscount;
                            shoppingCartItem.PerQuantityOrderLevelDiscountOnLineItem = calculatedCartLineItem.PerQuantityOrderLevelDiscountOnLineItem;
                            shoppingCartItem.PerQuantityVoucherAmount = calculatedCartLineItem.PerQuantityVoucherAmount;

                            //Price related properties
                            shoppingCartItem.AdditionalCost = IsNotNull(calculatedCartLineItem?.AdditionalCost) ? calculatedCartLineItem.AdditionalCost : null;
                            shoppingCartItem.ExtendedPrice = calculatedCartLineItem.ExtendedPrice;
                            shoppingCartItem.UnitPrice = calculatedCartLineItem.UnitPrice;

                            if (IsNotNull(shoppingCartItem.Product) && IsNotNull(calculatedCartLineItem.Product))
                            {
                                //Shipping related properties
                                shoppingCartItem.Product.ShippingCost = calculatedCartLineItem.Product?.ShippingCost > 0 ? calculatedCartLineItem.Product?.ShippingCost : calculatedCartLineItem.ShippingCost;

                                //Price related properties
                                shoppingCartItem.Product.PromotionalPrice = calculatedCartLineItem.Product?.PromotionalPrice;
                            }
                        }
                    }
                }
            }
        }

        protected virtual bool SkipOrderCalculations()
        {
            return true;
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

        #endregion
    }
}