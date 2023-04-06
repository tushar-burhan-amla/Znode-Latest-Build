using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Agents
{
    public class PaymentAgent : BaseAgent, IPaymentAgent
    {
        #region Private Variables
        protected readonly IPaymentClient _paymentClient;
        protected readonly IOrderClient _orderClient;
        protected readonly ICartAgent _cartAgent;
        protected readonly IQuoteClient _quoteClient;
        #endregion

        #region Public Constructor
        public PaymentAgent(IPaymentClient paymentClient, IOrderClient orderClient, IQuoteClient quoteClient)
        {
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _cartAgent = GetService<ICartAgent>();
            _orderClient = GetClient<IOrderClient>(orderClient);
            _quoteClient = GetClient<IQuoteClient>(quoteClient);
        }
        #endregion

        #region Public Methods

        public virtual PaymentDetailsViewModel GetPaymentDetails(int paymentSettingId, string quoteNumber = "", bool skipCalculations = false, bool isUsedForOfflinePayment = false, decimal remainingOrderAmount = 0)
        {
            string gatwayname = string.Empty;
            FilterCollection filters = new FilterCollection();

            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), FilterOperators.Equals, paymentSettingId.ToString()));
            PaymentSettingModel paymentSetting = null;
           
            List<PaymentSettingModel> paymentSettingList = GetPaymentSettingByUserDetailsFromCache(GetUserPaymentSettingDetails(), isUsedForOfflinePayment, HelperUtility.IsNotNull(quoteNumber) ? true : false)
                ?.Where(x => x.ProfileId == Helper.GetProfileId().GetValueOrDefault() || x.ProfileId == null)?.ToList();

            if (HelperUtility.IsNotNull(paymentSettingList))
               paymentSetting = paymentSettingList.FirstOrDefault(x => x.PaymentSettingId.Equals(paymentSettingId));


            PaymentDetailsViewModel model = new PaymentDetailsViewModel();

            model.IsOABRequired = Convert.ToBoolean(paymentSettingList?.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId)?.IsOABRequired);

            if (HelperUtility.IsNotNull(paymentSetting))
            {
                model.IsPoDocUploadEnable = paymentSetting.IsPoDocUploadEnable;
                model.IsPoDocRequire = paymentSetting.IsPoDocRequire;
                model.IsBillingAddressOptional = paymentSetting.IsBillingAddressOptional;
                bool isPendingOrderRequest = paymentSetting.IsOABRequired ? paymentSetting.IsOABRequired : false;
                string totalAmount = string.Empty;
                if(!string.IsNullOrEmpty(quoteNumber) || skipCalculations)
                {
                    totalAmount = GetQuoteTotal(quoteNumber);
                }
                else
                {
                    totalAmount = GetRecalculatedOrderTotal(model.IsOABRequired, isPendingOrderRequest, remainingOrderAmount);
                }
                if (string.Equals(paymentSetting.PaymentTypeName, ZnodeConstant.PAYPAL_EXPRESS.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                          string.Equals(paymentSetting.PaymentTypeName, ZnodeConstant.Amazon_Pay.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    totalAmount = ConvertTotalToLocale(totalAmount);
                }
                else if (!string.IsNullOrEmpty(paymentSetting?.GatewayCode))
                {
                    totalAmount = Encryption.EncryptPaymentToken(ConvertTotalToLocale(totalAmount));
                }
                else
                {
                    model.HasError = string.IsNullOrEmpty(paymentSetting?.GatewayCode);
                }
                model.GatewayCode = paymentSetting.GatewayCode;
                model.PaymentCode = paymentSetting.PaymentCode;
                model.PaymentProfileId = paymentSetting.ProfileId;
                model.Total = totalAmount;
                model.PaymentGatewayId = paymentSetting.PaymentGatewayId;
            }
            return model;
        }

        public virtual PaymentSettingViewModel GetPaymentSetting(int paymentSettingId, int portalId = 0)
        {
            try
            {
                PaymentSettingViewModel paymentSettingViewModel = null;
                PaymentSettingModel paymentSettingModel = GetPaymentSettingFromCache(paymentSettingId, portalId);
                int? profileid = paymentSettingModel.ProfileId;
                if (paymentSettingModel.IsCallToPaymentAPI)
                {
                    string paymentDisplayName = paymentSettingModel.PaymentDisplayName;
                    paymentSettingViewModel = GetPaymentSettingByPaymentCodeFromCache(paymentSettingModel.PaymentCode)?.ToViewModel<PaymentSettingViewModel>();

                    if (HelperUtility.IsNotNull(paymentSettingViewModel))
                    {
                        paymentSettingViewModel.PaymentSettingId = paymentSettingId;
                        paymentSettingViewModel.ProfileId = profileid;
                        paymentSettingViewModel.PaymentTypeName = paymentSettingModel.PaymentTypeName;
                        paymentSettingViewModel.PaymentDisplayName = paymentDisplayName;
                        paymentSettingViewModel.PaymentCode = paymentSettingModel.PaymentCode;
                    }
                    return paymentSettingViewModel;
                }
                return paymentSettingModel.ToViewModel<PaymentSettingViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }

        }

        // Call PayNow method in Payment Application
        public virtual GatewayResponseModel ProcessPayNow(SubmitPaymentModel model)
        {
            if (HelperUtility.IsNotNull(model))
                return _paymentClient.PayNow(model);
            return new GatewayResponseModel { HasError = true, };
        }

        // Call PayNow method in Payment Application
        public virtual GatewayResponseModel ProcessPayPal(SubmitPaymentModel model)
        {
            if (HelperUtility.IsNotNull(model))
                return _paymentClient.PayPal(model);
            return new GatewayResponseModel { HasError = true, };
        }

        public virtual string GetOrderTotal()
        {
            decimal? total = 0;
            //Get shopping Cart from Session or cookie
            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                           _cartAgent.GetCartFromCookie();

            if (HelperUtility.IsNotNull(shoppingCart))
                total = shoppingCart.Total;

            string strTotal = ConvertTotalToLocale(Convert.ToString(total));
            return strTotal;
        }

        //Get Quote Total
        public virtual string GetQuoteTotal(string quoteNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(quoteNumber))
                   return _quoteClient.GetQuoteTotal(quoteNumber);

                return string.Empty;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }        

        //Get saved credit card details by customer GUID
        public virtual PaymentMethodCCDetailsListModel GetPaymentCreditCardDetails(string customersGUID)
        {
            try
            {
                return _paymentClient.GetSavedCardDetailsByCustomerGUID(customersGUID);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return new PaymentMethodCCDetailsListModel { HasError = true };
            }
        }

        //Get Count of saved credit card by customers GUID.
        public virtual int GetSaveCreditCardCount(string customersGUID)
        {
            try
            {
                PaymentMethodCCDetailsListModel cards = _paymentClient.GetSavedCardDetailsByCustomerGUID(customersGUID);
                return cards?.PaymentMethodCCDetails?.Count ?? 0;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return 0;
            }
        }

        //Delete saved credit card details 
        public virtual bool DeleteSavedCreditCardDetail(string paymentGUID)
        {
            try
            {
                return _paymentClient.DeleteSavedCreditCardDetail(paymentGUID);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Call PayPal payment finalize method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        public virtual GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel)
        {
            if (HelperUtility.IsNotNull(submitPaymentModel.PaymentToken))
                return _paymentClient.FinalizePayPalProcess(submitPaymentModel);
            return new GatewayResponseModel { HasError = true, };
        }

        //Get the amount total in string formate as per locale
        public virtual string FormatOrderTotal(decimal? orderTotal)
        {
            string formattedOrderTotal = ConvertTotalToLocale(Convert.ToString(orderTotal.GetValueOrDefault()));
            return formattedOrderTotal;
        }

        #endregion

        #region AmazonPay

        //Get Amazon Pay address from amazon.
        public virtual SubmitPaymentModel GetAmazonPayAddressDetails(SubmitPaymentModel model)
           => _paymentClient.GetAmazonPayAddressDetails(model);

        //Call AmazonPay method in payment application.
        public virtual GatewayResponseModel ProcessAmazonPay(SubmitPaymentModel model)
        {
            if (HelperUtility.IsNotNull(model))
                return _paymentClient.AmazonPay(model);
            return new GatewayResponseModel { HasError = true, };
        }

        #endregion

        #region Private Method
        //to convert total amount to locale wise
        private string ConvertTotalToLocale(string total)
             => total.Replace(",", ".");
        #endregion

        #region Public Method

        public virtual List<PaymentSettingModel> GetPaymentSettingListFromCache(int portalId, int profileId, FilterCollection filters, SortCollection sort, bool isUsedForOfflinePayment = false)
        {
            string cacheKey = WebStoreConstants.PaymentSettingListCacheKey + "_" + Convert.ToString(PortalAgent.CurrentPortal.PortalId) + "_" + Convert.ToString(Helper.GetProfileId().GetValueOrDefault() + "_" + Convert.ToInt32(isUsedForOfflinePayment));
            List<PaymentSettingModel> model = null;
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                //Get payment option list.
                PaymentSettingListModel paymentOptionListModel = _paymentClient.GetPaymentSettings(null, filters, sort, null, null);
                model = paymentOptionListModel.PaymentSettings?.Where(x => profileId.ToString().Contains(x.ProfileId.ToString()) || x.ProfileId == null)?.ToList();
                if (HelperUtility.IsNotNull(model))
                    Helper.AddIntoCache(model, cacheKey, "CurrentPortalCacheDuration");
            }
            else
            {
                model = Helper.GetFromCache<List<PaymentSettingModel>>(cacheKey);
            }
            return model;
        }

        //Get Payment setting by portalId and userId
        public virtual List<PaymentSettingModel> GetPaymentSettingByUserDetailsFromCache(UserPaymentSettingModel userPaymentSettingModel, bool isUsedForOfflinePayment = false, bool isQuotes = false)
        {
            string cacheKey = WebStoreConstants.PaymentSettingListCacheKey + "_" + Convert.ToString(PortalAgent.CurrentPortal.PortalId) + "_" + Convert.ToString(Helper.GetProfileId().GetValueOrDefault()) + "_" + Convert.ToInt32(isUsedForOfflinePayment) + "_" + Convert.ToInt32(isQuotes);
            List<PaymentSettingModel> paymentSettingListModel = null;
            if (HelperUtility.IsNull(HttpRuntime.Cache[cacheKey]))
            {
                //Get payment option list.
                PaymentSettingListModel paymentOptions = _paymentClient.GetPaymentSettingByUserDetails(new UserPaymentSettingModel { UserId = userPaymentSettingModel.UserId, PortalId = PortalAgent.CurrentPortal.PortalId });

                //For Approval routing enable skip authorize.net,Braintree,Card Connect and cybersource. 
                if (PortalAgent.CurrentPortal.EnableApprovalManagement && !isQuotes)
                {
                    paymentOptions?.PaymentSettings.RemoveAll(x => x.GatewayCode != null && (x.GatewayCode.Equals(ZnodeConstant.CyberSource, StringComparison.InvariantCultureIgnoreCase) || x.GatewayCode.Equals(ZnodeConstant.AuthorizeNet, StringComparison.InvariantCultureIgnoreCase) || x.GatewayCode.Equals(ZnodeConstant.Braintree, StringComparison.InvariantCultureIgnoreCase) || x.GatewayCode.Equals(ZnodeConstant.CardConnect, StringComparison.InvariantCultureIgnoreCase)));
                }

                paymentSettingListModel = paymentOptions?.PaymentSettings;
                if (HelperUtility.IsNotNull(paymentSettingListModel))
                    Helper.AddIntoCache(paymentSettingListModel, cacheKey, WebStoreConstants.CurrentPortalCacheDuration);
            }
            else
            {
                paymentSettingListModel = Helper.GetFromCache<List<PaymentSettingModel>>(cacheKey);
            }
            return paymentSettingListModel;
        }

        // To get the populated UserPaymentSettingModel.
        public virtual UserPaymentSettingModel GetUserPaymentSettingDetails()
        {
            return new UserPaymentSettingModel()
            {
                UserId = HelperUtility.IsNotNull(Helper.GetUserDetails()) ? Helper.GetUserDetails().UserId : -1,
                PortalId = PortalAgent.CurrentPortal.PortalId
            };
        }

        public virtual PaymentSettingModel GetPaymentSettingFromCache(int paymentSettingId, int portalId, bool isPaymentApplication = false)
        {
            string cacheKey = (WebStoreConstants.PaymentSettingCacheKey + "_" + Convert.ToString(paymentSettingId) + "_" + Convert.ToString(portalId) + "_" + Convert.ToString(isPaymentApplication)).ToLower();
            PaymentSettingModel paymentSettingModel = null;
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                paymentSettingModel = _paymentClient.GetPaymentSetting(paymentSettingId, isPaymentApplication, new ExpandCollection { ZnodePaymentSettingEnum.ZnodePaymentType.ToString()}, portalId);
                Helper.AddIntoCache(paymentSettingModel, cacheKey, WebStoreConstants.CurrentPortalCacheDuration);
            }
            else
            {
                paymentSettingModel = Helper.GetFromCache<PaymentSettingModel>(cacheKey);
            }
            return paymentSettingModel;
        }

        public virtual PaymentSettingModel GetPaymentSettingByPaymentCodeFromCache(string paymentCode)
        {
            string cacheKey = (WebStoreConstants.PaymentSettingByCodeCacheKey + "_" + Convert.ToString(paymentCode)).ToLower();
            PaymentSettingModel paymentSettingViewModel = null;
            if (HelperUtility.IsNull(HttpContext.Current.Cache[cacheKey]))
            {
                paymentSettingViewModel = _paymentClient.GetPaymentSettingByPaymentCode(paymentCode);
                Helper.AddIntoCache(paymentSettingViewModel, cacheKey, WebStoreConstants.CurrentPortalCacheDuration);
            }
            else
            {
                paymentSettingViewModel = Helper.GetFromCache<PaymentSettingModel>(cacheKey);
            }
            return paymentSettingViewModel;
        }

        //Get generated payment token.
        public string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp)
        {
            try
            {

                string token = _paymentClient.GetPaymentAuthToken(userOrSessionId, fromAdminApp);
                return token;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return null;
            }
        }

        // This method will be used to get order total and recalculate if isOABRequired is true.
        public virtual string GetRecalculatedOrderTotal(bool isOABRequired, bool isPendingOrderRequest, decimal remainingOrderAmount = 0)
        {
            decimal? total = 0;
            //Get shopping Cart from Session or cookie
            ShoppingCartModel shoppingCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                           _cartAgent.GetCartFromCookie();
            if (HelperUtility.IsNotNull(shoppingCart))
                shoppingCart.IsPendingOrderRequest = (isOABRequired == true) ? true : false;
            GetService<ICheckoutAgent>().EnsureShoppingCartCalculations(shoppingCart, null);
            SaveInSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey, shoppingCart);

            if (HelperUtility.IsNotNull(shoppingCart))
                total = shoppingCart.Total;

            if (remainingOrderAmount > 0)
                total = remainingOrderAmount;

            string strTotal = ConvertTotalToLocale(Convert.ToString(total));
            return strTotal;
        }

        //Get cart view model.
        public virtual CartViewModel GetCartViewModel()
        {
            try

            {
                ShoppingCartModel calculatedCart = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey) ??
                   _cartAgent.GetCartFromCookie();

                CartViewModel cartViewModel = calculatedCart?.ToViewModel<CartViewModel>();

                return cartViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }

        public virtual PaymentTokenViewModel GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenViewModel)
        {
            try
            {
                PaymentSettingViewModel paymentSetting = GetPaymentSetting(paymentTokenViewModel.PaymentSettingId);

                if (HelperUtility.IsNotNull(paymentSetting))
                {
                    paymentTokenViewModel.GatewayLoginName = paymentSetting.GatewayUsername;
                    paymentTokenViewModel.GatewayTransactionKey = paymentSetting.TransactionKey;
                    if (string.Equals(paymentTokenViewModel.GatewayCode, ZnodeConstant.Braintree, StringComparison.InvariantCultureIgnoreCase))
                    {
                        paymentTokenViewModel.GatewayPassword = paymentSetting.GatewayPassword;
                        paymentTokenViewModel.GatewayTestMode = paymentSetting.TestMode;
                    }
                }
                PaymentGatewayTokenModel paymentTokenModel = paymentTokenViewModel?.ToModel<PaymentGatewayTokenModel>();

                paymentTokenModel.BillingAddress = new AddressModel();
                paymentTokenModel.ShippingAddress = new AddressModel();

                if (paymentTokenViewModel.GatewayCode == ZnodeConstant.AuthorizeNet)
                {

                    var BillingAddress1 = paymentTokenViewModel.AddressListViewModel.BillingAddress;
                    var ShippingAddress1 = paymentTokenViewModel.AddressListViewModel.ShippingAddress;

                    if (paymentTokenModel.BillingAddress != null)
                    {
                        paymentTokenModel.BillingAddress.FirstName = BillingAddress1.FirstName;
                        paymentTokenModel.BillingAddress.LastName = BillingAddress1.LastName;
                        paymentTokenModel.BillingAddress.StateName = BillingAddress1.StateName;
                        paymentTokenModel.BillingAddress.CountryName = BillingAddress1.CountryName;
                        paymentTokenModel.BillingAddress.CompanyName = BillingAddress1.CompanyName;
                        paymentTokenModel.BillingAddress.Address1 = BillingAddress1.Address1;
                        paymentTokenModel.BillingAddress.CityName = BillingAddress1.CityName;
                        paymentTokenModel.BillingAddress.PostalCode = BillingAddress1.PostalCode;
                        paymentTokenModel.BillingAddress.PhoneNumber = BillingAddress1.PhoneNumber;
                        paymentTokenModel.BillingAddress.FaxNumber = BillingAddress1.FaxNumber;
                        if (paymentTokenViewModel.UserId > 0)
                        {
                            paymentTokenModel.BillingAddress.EmailAddress = paymentTokenViewModel.AddressListViewModel.AddressList[0].EmailAddress;
                        }
                        else
                        {
                            paymentTokenModel.BillingAddress.EmailAddress = BillingAddress1.EmailAddress;
                        }
                        paymentTokenModel.BillingAddress.Address2 = BillingAddress1.Address2;
                    }

                    if (paymentTokenModel.ShippingAddress != null)
                    {
                        paymentTokenModel.ShippingAddress.FirstName = ShippingAddress1.FirstName;
                        paymentTokenModel.ShippingAddress.LastName = ShippingAddress1.LastName;
                        paymentTokenModel.ShippingAddress.StateName = ShippingAddress1.StateName;
                        paymentTokenModel.ShippingAddress.CountryName = ShippingAddress1.CountryName;
                        paymentTokenModel.ShippingAddress.CompanyName = ShippingAddress1.CompanyName;
                        paymentTokenModel.ShippingAddress.Address1 = ShippingAddress1.Address1;
                        paymentTokenModel.ShippingAddress.CityName = ShippingAddress1.CityName;
                        paymentTokenModel.ShippingAddress.PostalCode = ShippingAddress1.PostalCode;
                        paymentTokenModel.ShippingAddress.PhoneNumber = ShippingAddress1.PhoneNumber;
                        paymentTokenModel.ShippingAddress.FaxNumber = ShippingAddress1.FaxNumber;
                        paymentTokenModel.ShippingAddress.Address2 = ShippingAddress1.Address2;
                    }
                }

                paymentTokenViewModel = (_paymentClient.GetPaymentGatewayToken(paymentTokenModel))?.ToViewModel<PaymentTokenViewModel>();
                return paymentTokenViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return null;
            }
        }

        #endregion
    }
}