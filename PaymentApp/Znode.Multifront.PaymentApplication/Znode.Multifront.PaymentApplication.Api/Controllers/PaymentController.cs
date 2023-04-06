using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Multifront.PaymentApplication.Api.Filters;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Models.Models;
using Znode.Multifront.PaymentApplication.Providers;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    [TokenBasedAuthorization]
    public class PaymentController : BaseController
    {
        GatewayConnector gatewayConnector = new GatewayConnector();
        PaypalExpressRestProvider paypalExpressRestProvider = new PaypalExpressRestProvider();

        /// <summary>
        /// Payment model submitted to process credit card data through gateway
        /// </summary>
        /// <param name="paymentModel">paymentModel</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        [HttpPost]
        public HttpResponseMessage PayNow(PaymentModel paymentModel)
        {
            HttpResponseMessage response;
            try
            {
                if (IsCustomerProfileIdExist(paymentModel))
                {
                    GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                    try
                    {
                        gatewayResponse = gatewayConnector.GetCustomerResponse(paymentModel);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                        LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                    }
                    return CreateOKResponse(new PaymentGatewayResponse { GatewayResponse = gatewayResponse });
                }
                PaymentGatewayResponse gatewayResponseOutput = new PaymentGatewayResponse { GatewayResponse = gatewayConnector.GetResponse(paymentModel) };

                response = CreateOKResponse(gatewayResponseOutput);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Payment Gateway Token
        /// </summary>
        /// <param name="gatewayTokenModel">PaymentGatewayTokenModel</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayTokenResponse))]
        [HttpPost]
        public HttpResponseMessage GenerateGatewayToken(PaymentGatewayTokenModel gatewayTokenModel)
        {
            HttpResponseMessage response;
            try
            {

                PaymentGatewayTokenModel gatewayTokenResponse = new PaymentGatewayTokenModel();
                try
                {
                    gatewayTokenResponse = gatewayConnector.GetGatewayToken(gatewayTokenModel);
                }
                catch (Exception ex)
                {
                    Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                    LoggingService.LogActivity(gatewayTokenModel.PaymentSettingId, ex.Message);
                }
                return CreateOKResponse(new PaymentGatewayTokenResponse { PaymentTokenModel = gatewayTokenResponse });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(gatewayTokenModel.PaymentSettingId, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Creates a Paypal token and redirects to Paypal website
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        [HttpPost]
        public HttpResponseMessage Paypal(PaymentModel paymentModel)
        {
            HttpResponseMessage response;
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                gatewayResponse = gatewayConnector.GetPaypalExpressResponse(paymentModel);
                if (!gatewayResponse.IsSuccess)
                    Logging.LogMessage($"{ gatewayResponse.ResponseCode}  { gatewayResponse.ResponseText}", Logging.Components.Payment.ToString(), TraceLevel.Error);
                return CreateOKResponse(new PaymentGatewayResponse { GatewayResponse = gatewayResponse });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                StringResponse data = new StringResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        /// <summary>
        /// finalizes the Paypal payment process
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        [HttpPost]
        public HttpResponseMessage FinalizePaypalProcess(PaymentModel paymentModel)
        {
            HttpResponseMessage response;
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                LoggingService.LogActivity(paymentModel?.PaymentApplicationSettingId, $"{paymentModel?.PaymentApplicationSettingId} FinalizePaypalProcess called");
                gatewayResponse = gatewayConnector.GetFinalizedPaypalOrderResponse(paymentModel);
                LoggingService.LogActivity(paymentModel?.PaymentApplicationSettingId, $"{paymentModel?.PaymentApplicationSettingId} after FinalizePaypalProcess called");
                return CreateOKResponse(new PaymentGatewayResponse { GatewayResponse = gatewayResponse });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                StringResponse data = new StringResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Capture transaction based on GUID given
        /// </summary>
        /// <param name="token"></param>       
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage Capture(string token)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = gatewayConnector.GetCaptureResponse(token) });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Refund the amount to user based on the given guid and amount
        /// </summary>
        /// <param name="model">RefundPaymentModel</param>
        /// <returns>Boolean result</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage Refund(RefundPaymentModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = gatewayConnector.GetRefundVoidResponse(model.Token, model.RefundAmount, model.IsCompleteOrderRefund, false,model.BillingAddress,model.ShippingAddress) });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Void the transaction for the given token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage Void(string token)
        {
            HttpResponseMessage response;
            try
            {
                AddressModel BillingAddress=new AddressModel();
                AddressModel ShippingAddress = new AddressModel();
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = gatewayConnector.GetRefundVoidResponse(token, 0.00m, false, true,BillingAddress,ShippingAddress) });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Add payment settings
        /// </summary>
        /// <param name="paymentSetting"></param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpPost]
        public HttpResponseMessage AddPaymentSettings([FromBody] PaymentSettingsModel paymentSetting)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService repository = new PaymentSettingsService();
                PaymentSettingsModel paymentSettingModel = repository.AddPaymentSetting(paymentSetting);
                response = paymentSettingModel?.PaymentSettingId > 0 ? CreateCreatedResponse(new PaymentSettingsResponse { PaymentSetting = paymentSettingModel })
                            : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        ///Update payment settings
        /// </summary>
        /// <param name="paymentSetting"> PaymentSettingsModel paymentSetting</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpPut]
        public HttpResponseMessage UpdatePaymentSettings([FromBody] PaymentSettingsModel paymentSetting)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService repository = new PaymentSettingsService();
                response = repository.UpdatePaymentSetting(paymentSetting)
                            ? CreateOKResponse(new PaymentSettingsResponse { PaymentSetting = paymentSetting })
                            : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete payment settings by Payment settings ID
        /// </summary>
        /// <param name="paymentSettingIds">payment Setting Ids</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeletePaymentSettings(ParameterModel paymentSettingIds)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService repository = new PaymentSettingsService();
                if (repository.IsPaymentSettingUsed(paymentSettingIds?.Ids))
                {
                    //4 is the Error Code of Association Delete Error
                    response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorCode = 4, ErrorMessage = "Payment Setting Already in use" });
                }
                else
                {
                    bool deleted = repository.DeletePaymentSetting(paymentSettingIds?.Ids);
                    response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
                }

            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get payment settings by Payment settings ID
        /// </summary>
        /// <param name="paymentSettingId">int paymentSettingId</param>
        /// <returns>returns complete details of payment setting</returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSettingDetails(int paymentSettingId)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService paymentrepository = new PaymentSettingsService();
                PaymentSettingsModel paymentSetting = paymentrepository.GetPaymentSettingWithCredentials(paymentSettingId);
                response = !Equals(paymentrepository, null) ? CreateOKResponse(new PaymentSettingsResponse { PaymentSetting = paymentSetting }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Payment Settings
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSettings()
        {
            HttpResponseMessage response;
            try
            {
                //_queryStringParser.Filters
                PaymentSettingsService paymentrepository = new PaymentSettingsService();
                PaymentSettingListModel paymentSettings = paymentrepository.GetPaymentSetting(Filters, Sorts, Page);
                response = !Equals(paymentrepository, null) ? CreateOKResponse(new PaymentSettingsResponse { PaymentSettings = paymentSettings }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Payment Setting Credentials
        /// </summary>
        /// <param name="paymentSettingId">Id of payment Setting.</param>
        /// <param name="isTestMode">true to get testmode credentials else set false.</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpGet]
        [Obsolete("This method is deprecated, please use 'GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode)' instead.")]
        public HttpResponseMessage GetPaymentSettingCredentials(int paymentSettingId, bool isTestMode)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingCredentialsService paymentrepository = new PaymentSettingCredentialsService();
                PaymentSettingCredentialsModel paymentSettingCredentials = paymentrepository.GetPaymentSettingCredentials(paymentSettingId, isTestMode);
                response = !Equals(paymentSettingCredentials, null) ? CreateOKResponse(new PaymentSettingsResponse { PaymentSettingCredentials = paymentSettingCredentials }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Payment Setting Credentials
        /// </summary>
        /// <param name="paymentCode">Code of payment Setting.</param>
        /// <param name="isTestMode">true to get testmode credentials else set false.</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingCredentialsService paymentrepository = new PaymentSettingCredentialsService();
                PaymentSettingCredentialsModel paymentSettingCredentials = paymentrepository.GetPaymentSettingCredentials(paymentCode, isTestMode);
                response = !Equals(paymentSettingCredentials, null) ? CreateOKResponse(new PaymentSettingsResponse { PaymentSettingCredentials = paymentSettingCredentials }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all available gateway names
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        public HttpResponseMessage GetGateways()
        {
            HttpResponseMessage response;
            try
            {
                GatewayService repository = new GatewayService();
                PaymentGatewayListModel paymentGateways = repository.GetAll();
                response = !Equals(paymentGateways, null) ? CreateOKResponse(new PaymentGatewayResponse { PaymentGatewayList = paymentGateways }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = $"{ ex.Message},{ex.StackTrace.ToString()}" };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all available gateway names
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaymentTypeResponse))]
        public HttpResponseMessage GetPaymentTypes()
        {
            HttpResponseMessage response;
            try
            {
                PaymentTypeService repository = new PaymentTypeService();
                PaymentTypeListModel PaymentTypes = repository.GetAll();
                response = !Equals(PaymentTypes, null) ? CreateOKResponse(new PaymentTypeResponse { PaymentTypes = PaymentTypes }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = $"{ ex.Message},{ex.StackTrace.ToString()}" };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all Saved Payment Credit Card Details
        /// </summary>
        /// <param name="paymentSettingId">int paymentSettingId</param>
        /// <param name="customersGUID">string customersGUID</param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentMethodResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentCreditCardDetails(int paymentSettingId, string customersGUID)
        {
            HttpResponseMessage response;
            try
            {
                PaymentMethodsService repository = new PaymentMethodsService();
                List<PaymentMethodCCDetailsModel> paymentMethodCCDetails = repository.GetPaymentCreditCardDetails(paymentSettingId, customersGUID);

                response = !Equals(paymentMethodCCDetails, null) ? CreateOKResponse(new PaymentMethodResponse { PaymentMethodCCDetails = paymentMethodCCDetails }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = $"{ ex.Message},{ex.StackTrace.ToString()}" };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all Saved Payment Credit Card Details
        /// </summary>
        /// <param name="paymentSettingId">int paymentSettingId</param>
        /// <param name="customerGUID">string customersGUID</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetPaymentCreditCardCount(int paymentSettingId, string customerGUID)
        {
            HttpResponseMessage response;
            try
            {
                PaymentMethodsService repository = new PaymentMethodsService();
                int countSavedCreditCard = repository.GetSaveCreditCardCount(paymentSettingId, customerGUID);
                response = !Equals(countSavedCreditCard, null) ? CreateOKResponse(new CountResponse { Count = countSavedCreditCard }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = $"{ ex.Message},{ex.StackTrace.ToString()}" };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete saved card details
        /// </summary>
        /// <param name="paymentGUID">paymentGUID</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet]
        public HttpResponseMessage DeleteSavedCreditCardDetail(string paymentGUID)
        {
            HttpResponseMessage response;
            try
            {
                PaymentMethodsService repository = new PaymentMethodsService();
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = repository.DeleteSavedCreditCard(paymentGUID) });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Payment transaction by transactionId
        /// </summary>
        /// <param name="transactionId">Id of transaction.</param>
        /// <returns>returns payment transaction</returns>
        [ResponseType(typeof(PaymentTransactionResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentTransaction(string transactionId)
        {
            HttpResponseMessage response;
            try
            {
                TransactionService repository = new TransactionService();
                PaymentTransactionModel transaction = repository.GetPaymentTransaction(transactionId.Trim());
                response = !Equals(transaction, null) ? CreateOKResponse(new PaymentTransactionResponse { PaymentTransaction = transaction }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get payment settings by paymentCode
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <returns>returns complete details of payment setting</returns>
        [ResponseType(typeof(PaymentSettingsResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentSettingByPaymentCode(string paymentCode)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService paymentrepository = new PaymentSettingsService();
                PaymentSettingsModel paymentSetting = paymentrepository.GetPaymentSettingsByPaymentCode(paymentCode);
                response = !Equals(paymentrepository, null) ? CreateOKResponse(new PaymentSettingsResponse { PaymentSetting = paymentSetting }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentSettingsResponse data = new PaymentSettingsResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        /// <summary>
        /// Delete payment settings by paymentCode
        /// </summary>
        /// <param name="paymentCodes">payment Code</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeletePaymentSettingByPaymentCode(ParameterModel paymentCodes)
        {
            HttpResponseMessage response;
            try
            {
                PaymentSettingsService repository = new PaymentSettingsService();
                if (repository.IsPaymentCodeUsed(paymentCodes?.Ids))
                {
                    //4 is the Error Code of Association Delete Error
                    response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorCode = 4, ErrorMessage = "Payment Setting Already in use" });
                }
                else
                {
                    bool deleted = repository.DeletePaymentSettingByPaymentCode(paymentCodes?.Ids);
                    response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
                }

            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get saved card details by customers GUID
        /// </summary>
        /// <param name="customersGUID">string customersGUID</param>
        /// <returns>card details</returns>
        [ResponseType(typeof(PaymentMethodResponse))]
        [HttpGet]
        public HttpResponseMessage GetSavedCardDetailsByCustomerGUID(string customersGUID)
        {
            HttpResponseMessage response;
            try
            {
                PaymentMethodsService repository = new PaymentMethodsService();
                List<PaymentMethodCCDetailsModel> paymentMethodCCDetails = repository.GetSavedCardDetailsByCustomerGUID(customersGUID);
                response = !Equals(paymentMethodCCDetails, null) ? CreateOKResponse(new PaymentMethodResponse { PaymentMethodCCDetails = paymentMethodCCDetails }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all available gateway names for ACH
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        public HttpResponseMessage GetACHGateways()
        {
            HttpResponseMessage response;
            try
            {
                GatewayService repository = new GatewayService();
                PaymentGatewayListModel paymentGateways = repository.GetACHGateways();
                response = !Equals(paymentGateways, null) ? CreateOKResponse(new PaymentGatewayResponse { PaymentGatewayList = paymentGateways }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = $"{ ex.Message},{ex.StackTrace.ToString()}" };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// To check Customer Profile Id is already Exist
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns true/false </returns>
        private bool IsCustomerProfileIdExist(PaymentModel paymentModel)
        => ((string.IsNullOrEmpty(paymentModel.CustomerProfileId)
                       || string.IsNullOrEmpty(paymentModel.CustomerPaymentProfileId))
                       && (!string.IsNullOrEmpty(paymentModel.GatewayType))
                       && (Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.AUTHORIZENET).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.STRIPE).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.WORLDPAY).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.CYBERSOURCE).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.PAYMENTECH).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.BRAINTREE).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.PAYFLOW).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.CARDCONNECT).ToLower())
                       || Equals(paymentModel.GatewayType.ToLower(), Convert.ToString(GatewayType.PAYPAL).ToLower())));

        /// <summary>
        /// Get transaction status details.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [ResponseType(typeof(TransactionDetailsResponse))]
        [HttpGet]
        public HttpResponseMessage GetTransactionStatusDetails(string transactionId)
        {
            HttpResponseMessage response;
            try
            {
                TransactionDetailsModel transactionDetailsModel = gatewayConnector.GetTransactionDetails(transactionId);
                response = Equals(transactionDetailsModel, null) ? CreateInternalServerErrorResponse()
                            : CreateOKResponse(new TransactionDetailsResponse { TransactionDetails = transactionDetailsModel });
            } catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                response = CreateInternalServerErrorResponse(new TransactionDetailsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get refund transaction id.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public HttpResponseMessage GetRefundtransactionId(string transactionId)
        {        
            HttpResponseMessage response;
            try
            {
                string refundTransactionId = gatewayConnector.GetRefundTransactionId(transactionId);
                response= Request.CreateResponse(HttpStatusCode.OK, new StringResponse { Response= refundTransactionId });
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, new StringResponse { Response = "Invalid transaction id." });
            }
            return response;
        }

        #region Amazon Pay

        /// <summary>
        ///Process amazon pay.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(PaymentGatewayResponse))]
        [HttpPost]
        public HttpResponseMessage GetAmazonPayAddress(PaymentModel paymentModel)
        {
            HttpResponseMessage response;
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                gatewayResponse = gatewayConnector.GetAmazonPayAddress(paymentModel);
                if (!gatewayResponse.IsSuccess)
                    Logging.LogMessage($"{ gatewayResponse.ResponseCode}  { gatewayResponse.ResponseText}", Logging.Components.Payment.ToString(), TraceLevel.Error);
                return CreateOKResponse(new PaymentGatewayResponse
                {
                    GatewayResponse = gatewayResponse,
                    PaymentModel = gatewayResponse.PaymentModel
                });
            }
            catch (Exception ex)
            {
                string returnUrl = string.Empty;
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                StringResponse data = new StringResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #endregion

    }
}
