using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Payment.Client;
using Znode.Engine.Services.Maps;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PaymentHelper : BaseHelper, IPaymentHelper
    {
        #region Private Variables
        private readonly IPaymentClient _paymentClient;
        #endregion

        public PaymentHelper(IPaymentClient paymentClient)
        {
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
        }

        #region Public Methods
        //Process for payment
        public virtual GatewayResponseModel ProcessPayment(ConvertQuoteToOrderModel convertToOrderModel, ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter ConvertQuoteToOrderModel having CustomerPaymentId and CustomerProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { convertToOrderModel.PaymentDetails.CustomerPaymentId, convertToOrderModel.PaymentDetails.CustomerProfileId });

            if (HelperUtility.IsNull(convertToOrderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingModelNotNull);

            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            gatewayResponseModel.IsSuccess = false;

            if (IsCODPaymentType(convertToOrderModel.PaymentDetails?.PaymentType) || IsPOPaymentType(convertToOrderModel?.PaymentDetails.PaymentType))
            {
               gatewayResponseModel.IsSuccess = ProcessCODAndPOPayment(convertToOrderModel, cartModel);
               return gatewayResponseModel;
            }

            SetUsersPaymentDetails(convertToOrderModel.PaymentDetails.PaymentSettingId, cartModel);
            convertToOrderModel.PaymentDetails.PaymentType = string.IsNullOrEmpty(cartModel?.Payment?.PaymentName) ? convertToOrderModel.PaymentDetails.PaymentType : cartModel?.Payment?.PaymentName;

            // Map shopping Cart model and submit Payment view model to Submit payment model 
            SubmitPaymentModel submitPaymentModel = PaymentViewModelMap.ToModel(cartModel, convertToOrderModel);

            // Condition for "Credit Card" payment.
            if (IsCreditCardPayment(convertToOrderModel.PaymentDetails?.PaymentType))
            {
                if (string.Equals(convertToOrderModel.PaymentDetails.GatewayCode, ZnodeConstant.CyberSource, StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.CustomerPaymentId) || (!string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.CustomerProfileId)))
                {
                    return ProcessCreditCardPayment(convertToOrderModel, cartModel, submitPaymentModel);
                }
                else if (string.Equals(convertToOrderModel.PaymentDetails.GatewayCode, ZnodeConstant.AuthorizeNet, StringComparison.OrdinalIgnoreCase))
                {
                    return ProcessCreditCardPayment(convertToOrderModel, cartModel, submitPaymentModel);
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorCustomerPaymentIdRequired);//profile id req message
                }
            }
            else if (IsACHPayment(convertToOrderModel.PaymentDetails?.PaymentType))
            {
                if (!string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.CustomerPaymentId) || (!string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.CustomerProfileId)))
                {
                    submitPaymentModel.IsACHPayment = true;
                    return ProcessCreditCardPayment(convertToOrderModel, cartModel, submitPaymentModel);
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorCustomerPaymentIdRequired);//profile id req message
                }
            }
            else if (IsPaypalExpressPayment(convertToOrderModel?.PaymentDetails?.PaymentType))
            {
                return ProcessPayPalExpressPayment(convertToOrderModel, cartModel);
            }
            else if (IsAmazonPayPayment(convertToOrderModel?.PaymentDetails?.PaymentType))
            {   
                //Amazon payment.
                return AmazonPaymentProcess(convertToOrderModel, submitPaymentModel);
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return gatewayResponseModel;
        }

        //Check payment type is Paypal Express
        public virtual bool IsPaypalExpressPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType.Replace("_", ""), ZnodeConstant.PayPalExpress, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
        }

        //Check payment type is COD payment method
        public virtual bool IsCODPaymentType(string paymentType)
        {
            return (!string.IsNullOrEmpty(paymentType) && (string.Equals(paymentType, ZnodeConstant.COD.ToString(), StringComparison.InvariantCultureIgnoreCase)));
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

        //Check payment type is Purchase Order payment method
        public virtual bool IsPOPaymentType(string paymentType)
        {
            return (!string.IsNullOrEmpty(paymentType) && (string.Equals(paymentType, ZnodeConstant.PurchaseOrder.ToString(), StringComparison.InvariantCultureIgnoreCase)));
        }

        //Check payment type is Amazon Pay
        public virtual bool IsAmazonPayPayment(string paymentType)
        {
            if (!string.IsNullOrEmpty(paymentType))
            {
                return string.Equals(paymentType, ZnodeConstant.AmazonPay, StringComparison.InvariantCultureIgnoreCase);

            }
            else return false;
        }

        #endregion

        #region Protected Methods
        //Process COD or PO payment
        protected virtual bool ProcessCODAndPOPayment(ConvertQuoteToOrderModel model, ShoppingCartModel cartModel)
        {
            if (IsPOPaymentType(model.PaymentDetails?.PaymentType))
            {
                cartModel.PurchaseOrderNumber = model.PaymentDetails.PurchaseOrderNumber;
                cartModel.PODocumentName = !string.IsNullOrEmpty(model.PaymentDetails.PODocumentName) ? $"{ZnodeConstant.PODocumentPath}/{model.PaymentDetails.PODocumentName}" : null;
            }
            return true;
        }

        // Process paypal express payment.
        protected virtual GatewayResponseModel ProcessPayPalExpressPayment(ConvertQuoteToOrderModel submitOrderViewModel, ShoppingCartModel cartModel)
        {
            submitOrderViewModel.PaymentDetails.CardType = "Paypal";
            submitOrderViewModel.PaymentDetails.PayPalReturnUrl = CheckQueryStringForAddressId(submitOrderViewModel.PaymentDetails.PayPalReturnUrl, cartModel.ShippingAddress.AddressId, cartModel.BillingAddress.AddressId);

            //PaymentApplicationSettingId
            //submitOrderViewModel.PaymentDetails.PaymentSettingId = submitOrderViewModel.PaymentDetails.PaymentSettingId > 0 ? submitOrderViewModel.PaymentDetails.PaymentSettingId : (_paymentClient.GetPaymentSetting(submitOrderViewModel.PaymentSettingId)?.PaymentApplicationSettingId).GetValueOrDefault();
            ZnodeLogging.LogMessage($"PaymentApplicationSettingId - {submitOrderViewModel.PaymentDetails.PaymentSettingId }");

            if (submitOrderViewModel.PaymentDetails.PaymentSettingId > 0)
            {
                SubmitPaymentModel model = PaymentViewModelMap.ToModel(cartModel, submitOrderViewModel);
                cartModel.Payment.PaymentSetting.PaymentTypeName = ZnodeConstant.PAYPAL_EXPRESS;

                GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                //Call PayPal payment finalize method in Payment Application if payment initialization token is present.
                gatewayResponse = !string.IsNullOrEmpty(submitOrderViewModel.PaymentDetails.PayPalToken) ? FinalizePayPalProcess(model)
                                        : ProcessPayPal(model);

                if (gatewayResponse?.HasError ?? true)
                {
                    return new GatewayResponseModel { IsSuccess = false , ResponseText = gatewayResponse.ResponseText};
                }
                return new GatewayResponseModel { IsSuccess = true, ResponseText = gatewayResponse.ResponseText, TransactionId = gatewayResponse.TransactionId, IsGatewayPreAuthorize = gatewayResponse.IsGatewayPreAuthorize };
             }
            return new GatewayResponseModel();
        }

        // Check if paypal express return url shipping and billing address id is "0".
        private string CheckQueryStringForAddressId(string payPalReturnUrl, int shippingAddressId, int billingAddressId)
        {
            bool isChange = false;
            if (!string.IsNullOrEmpty(payPalReturnUrl))
            {
                Uri uri = new Uri(payPalReturnUrl);
                NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);

                if (Equals(queryString.Get("ShippingAddressId"), "0"))
                {
                    queryString.Set("ShippingAddressId", shippingAddressId.ToString());
                    isChange = true;
                }
                if (Equals(queryString.Get("BillingAddressId"), "0"))
                {
                    queryString.Set("BillingAddressId", billingAddressId.ToString());
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


        // Process credit card payment.
        protected virtual GatewayResponseModel ProcessCreditCardPayment(ConvertQuoteToOrderModel convertToOrderModel, ShoppingCartModel cartModel, SubmitPaymentModel model)
        {
            GatewayResponseModel gatewayResponse = GetPaymentResponse(cartModel, convertToOrderModel, model);
            MapCustomerDetails(convertToOrderModel, gatewayResponse);
            SaveCustomerDetails(cartModel, convertToOrderModel);

            if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
            {
                return gatewayResponse;
            }
            else
            {
                gatewayResponse.IsSuccess = true;
            }

            //Map payment token
            cartModel.Token = gatewayResponse.Token;
            cartModel.IsGatewayPreAuthorize = gatewayResponse.IsGatewayPreAuthorize;

            if (!cartModel.IsGatewayPreAuthorize && !string.IsNullOrEmpty(cartModel.Token) && !IsACHPayment(convertToOrderModel?.PaymentDetails.PaymentType))
            {
                CapturePayment(cartModel.Token);
                
            }

            return gatewayResponse;
        }

        //Capture Payment
        public virtual bool CapturePayment(string paymentTransactionToken)
        {
            try
            {
                BooleanModel booleanModel = _paymentClient.CapturePayment(paymentTransactionToken);
                return !booleanModel.HasError;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return false;
        }


        // Get payment response.
        protected virtual GatewayResponseModel GetPaymentResponse(ShoppingCartModel cartModel, ConvertQuoteToOrderModel convertToOrderModel, SubmitPaymentModel model)
        {
            model.Email = model.Email ?? cartModel.UserDetails.Email;
            model.Total = ConvertTotalToLocale(Convert.ToString(cartModel.Total));

            return ProcessPayNow(model);
        }

        public void SaveCustomerDetails(ShoppingCartModel cartModel, ConvertQuoteToOrderModel convertToOrderModel)
        {
            if (!string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.CustomerGuid) && string.IsNullOrEmpty(cartModel.UserDetails.CustomerPaymentGUID))
            {
                IUserService userService = GetService<IUserService>();
                NameValueCollection expands = new NameValueCollection();
                UserModel userModel = userService.GetUserById(convertToOrderModel.UserId, expands);
                userModel.CustomerPaymentGUID = convertToOrderModel.PaymentDetails.CustomerGuid;
                userService.UpdateCustomer(userModel, false);
            }
        }

        public void MapCustomerDetails(ConvertQuoteToOrderModel convertToOrderModel, GatewayResponseModel gatewayResponse)
        {
            convertToOrderModel.PaymentDetails.CustomerGuid = convertToOrderModel.PaymentDetails.CustomerGuid ?? gatewayResponse.CustomerGUID;
        }

        //Process for payment
        public virtual GatewayResponseModel ProcessPayment(PayInvoiceModel payInvoiceModel, ShoppingCartModel cartModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter PayInvoiceModel having CustomerPaymentId and CustomerProfileId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { payInvoiceModel.PaymentDetails.CustomerPaymentId, payInvoiceModel.PaymentDetails.CustomerProfileId });

            if (HelperUtility.IsNull(payInvoiceModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PaymentSettingModelNotNull);

            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            gatewayResponseModel.IsSuccess = false;

            SetUsersPaymentDetails(payInvoiceModel.PaymentDetails.PaymentSettingId, cartModel);
            payInvoiceModel.PaymentDetails.PaymentType = string.IsNullOrEmpty(cartModel?.Payment?.PaymentName) ? payInvoiceModel.PaymentDetails.PaymentType : cartModel?.Payment?.PaymentName;

            // Map shopping Cart model and submit Payment view model to Submit payment model 
            SubmitPaymentModel submitPaymentModel = PaymentViewModelMap.ToModel(cartModel, payInvoiceModel);

            // Condition for "Credit Card" payment.
            if (IsCreditCardPayment(payInvoiceModel.PaymentDetails?.PaymentType))
            {
                if (!string.IsNullOrEmpty(payInvoiceModel.PaymentDetails.CustomerPaymentId) || (!string.IsNullOrEmpty(payInvoiceModel.PaymentDetails.CustomerProfileId) || payInvoiceModel.PaymentDetails.GatewayCode.Equals(ZnodeConstant.CardConnect, StringComparison.InvariantCultureIgnoreCase)) || payInvoiceModel.PaymentDetails.GatewayCode.Equals(ZnodeConstant.AuthorizeNet, StringComparison.InvariantCultureIgnoreCase)|| payInvoiceModel.PaymentDetails.GatewayCode.Equals(ZnodeConstant.CyberSource, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ProcessCreditCardPayment(payInvoiceModel, cartModel, submitPaymentModel);
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorCustomerPaymentIdRequired);//profile id req message
                }
            }

            // Condition for "ACH" payment.
            if (IsACHPayment(payInvoiceModel.PaymentDetails?.PaymentType))
            {
                if (!string.IsNullOrEmpty(payInvoiceModel.PaymentDetails.CustomerPaymentId) || (!string.IsNullOrEmpty(payInvoiceModel.PaymentDetails.CustomerProfileId)))
                {
                    submitPaymentModel.IsACHPayment = true;
                    return ProcessCreditCardPayment(payInvoiceModel, cartModel, submitPaymentModel);
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorCustomerPaymentIdRequired);//profile id req message
                }
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return gatewayResponseModel;
        }

        // Process credit card payment.
        protected virtual GatewayResponseModel ProcessCreditCardPayment(PayInvoiceModel payInvoiceModel, ShoppingCartModel cartModel, SubmitPaymentModel model)
        {
            GatewayResponseModel gatewayResponse = GetPaymentResponse(cartModel, payInvoiceModel, model);
            if (gatewayResponse?.HasError ?? true || string.IsNullOrEmpty(gatewayResponse?.Token))
            {
                return gatewayResponse;
            }
            else
            {
                gatewayResponse.IsSuccess = true;
            }

            //Map payment token
            cartModel.Token = gatewayResponse.Token;
            cartModel.IsGatewayPreAuthorize = gatewayResponse.IsGatewayPreAuthorize;
            payInvoiceModel.PaymentDetails.TransactionDate = gatewayResponse.TransactionDate;
            payInvoiceModel.PaymentDetails.TransactionStatus = gatewayResponse.PaymentStatus.ToString();
            payInvoiceModel.PaymentDetails.TransactionId = gatewayResponse.Token;
            if (!cartModel.IsGatewayPreAuthorize && !string.IsNullOrEmpty(cartModel.Token) && IsCreditCardPayment(payInvoiceModel.PaymentDetails?.PaymentType))
            {
                CapturePayment(cartModel.Token);
            }

            return gatewayResponse;
        }

        // Get payment response.
        protected virtual GatewayResponseModel GetPaymentResponse(ShoppingCartModel cartModel, PayInvoiceModel payInvoiceModel, SubmitPaymentModel model)
        {
            // Map Customer Payment Guid for Save Credit Card 
            if (!string.IsNullOrEmpty(payInvoiceModel.PaymentDetails.CustomerGuid) && string.IsNullOrEmpty(cartModel.UserDetails.CustomerPaymentGUID))
            {
                IUserService userService = GetService<IUserService>();
                NameValueCollection expands = new NameValueCollection();
                UserModel userModel = userService.GetUserById(payInvoiceModel.UserId, expands);
                userModel.CustomerPaymentGUID = payInvoiceModel.PaymentDetails.CustomerGuid;
                userService.UpdateCustomer(userModel, false);
            }

            model.Total = ConvertTotalToLocale(Convert.ToString(payInvoiceModel.PaymentDetails.PaymentAmount));

            return ProcessPayNow(model);
        }

        // Call PayNow method in Payment Application
        protected virtual GatewayResponseModel ProcessPayNow(SubmitPaymentModel model)
        {
            if (HelperUtility.IsNotNull(model))
                return _paymentClient.PayNow(model);
            return new GatewayResponseModel { HasError = true };
        }

        // Call PayNow method in Payment Application
        protected virtual GatewayResponseModel ProcessPayPal(SubmitPaymentModel model)
        {
            if (HelperUtility.IsNotNull(model))
                return _paymentClient.PayPal(model);
            return new GatewayResponseModel { HasError = true };
        }

        // Call PayPal payment finalize method in Payment Application
        protected virtual GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel)
        {
            if (HelperUtility.IsNotNull(submitPaymentModel.PaymentToken))
                return _paymentClient.FinalizePayPalProcess(submitPaymentModel);
            return new GatewayResponseModel { HasError = true, };
        }

        //AmazonPay payment process.
        protected virtual GatewayResponseModel AmazonPaymentProcess(ConvertQuoteToOrderModel convertToOrderModel, SubmitPaymentModel submitPaymentModel)
        {
            if (!string.IsNullOrEmpty(convertToOrderModel.PaymentDetails?.AmazonPayReturnUrl) && !string.IsNullOrEmpty(convertToOrderModel.PaymentDetails?.AmazonPayCancelUrl))
            {
                if (string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.AmazonOrderReferenceId))
                    return new GatewayResponseModel() { IsSuccess = false, ResponseText = WebStore_Resources.ErrorProcessPayment };

                GatewayResponseModel gatewayResponse = _paymentClient.PayNow(submitPaymentModel);
                if (gatewayResponse?.HasError ?? true)
                {
                    return new GatewayResponseModel() { IsSuccess = false, ResponseText = !string.IsNullOrEmpty(gatewayResponse?.ErrorMessage) ? gatewayResponse.ErrorMessage : WebStore_Resources.ErrorProcessPayment };
                }
                return new GatewayResponseModel() { Token = gatewayResponse.Token, IsSuccess = string.IsNullOrWhiteSpace(gatewayResponse.Token) ? false : true, ResponseText = gatewayResponse.ResponseText };
            }
            else if (!string.IsNullOrEmpty(convertToOrderModel?.PaymentDetails?.PaymentToken))
            {
                return new GatewayResponseModel() { IsSuccess = true };
            }
            return new GatewayResponseModel() { IsSuccess = false, ResponseText = WebStore_Resources.ErrorProcessPayment };
        }

        /// <summary>
        /// Delete expired token from payment api.
        /// </summary>
        /// <param></param>
        /// <returns>Return bool</returns>
        public virtual bool DeletePaymentToken()
        {
            try
            {
                TrueFalseResponse responseModel = _paymentClient.DeletePaymentToken();
                return responseModel.IsSuccess;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return false;
        }
        #endregion

        #region Private Methods
        //to convert total amount to locale wise
        private string ConvertTotalToLocale(string total)
            => total.Replace(",", ".");

        private void SetUsersPaymentDetails(int paymentSettingId, ShoppingCartModel model) //, bool isRequiredExpand = false)
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodePaymentSettingEnum.ZnodePaymentType.ToString().ToLower(), ZnodePaymentSettingEnum.ZnodePaymentType.ToString());

            IPaymentSettingService _paymentSettingService = GetService<IPaymentSettingService>();

            PaymentSettingModel paymentSetting = _paymentSettingService.GetPaymentSetting(paymentSettingId, expands, model.PortalId);

            string paymentName = string.Empty;
            if (HelperUtility.IsNotNull(paymentSetting))
            {
                paymentName = paymentSetting.PaymentTypeName;
            }

            model.Payment = PaymentViewModelMap.ToPaymentModel(model, paymentSetting, paymentName);

        }
       
        #endregion
    }
}
