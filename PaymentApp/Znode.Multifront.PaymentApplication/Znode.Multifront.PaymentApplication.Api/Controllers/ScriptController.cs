using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Api.Filters;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    /// <summary>
    /// This controller will only allow such methods which need not have to check for the 
    /// ZnodeDomain table and allow on that basis to access its members.
    /// </summary>
    [TokenBasedAuthorization(DoNotCount = true)]
    public class ScriptController : BaseController
    {
        /// <summary>
        /// This method will generate the script in the client UI and this will then give the call
        /// to the Znode Payment Application
        /// </summary>
        /// <param name="paymentCreditCardModel">payment credit card model</param>
        /// <returns>Returns the script which will be then call the Payment application function(s)</returns>
        [ResponseType(typeof(BaseResponse))]
        public HttpResponseMessage ZnodeApiJs(PaymentCreditCardModel paymentCreditCardModel)
        {
            HttpResponseMessage response;
            try
            {
                if (!string.IsNullOrEmpty(paymentCreditCardModel.gateway))
                {
                    switch (paymentCreditCardModel.gateway.ToLower())
                    {
                        case "highradius":
                            return GetHighRadiusScript(paymentCreditCardModel);
                        default:
                            return GetCreditCardScript(paymentCreditCardModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentCreditCardModel.paymentSettingId, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = ex.Message, GatewayResponse = new GatewayResponseModel() { GatewayResponseData = ex.Message } };
                response = CreateInternalServerErrorResponse(data);
            }
            return null;
        }

        /// <summary>
        /// This method will generate the script in the client UI and this will then give the call
        /// to the Znode Payment Application
        /// </summary>
        /// <param name="paymentCreditCardModel">payment credit card model</param>
        /// <returns>Returns the script which will be then call the Payment application function(s)</returns>
        [ResponseType(typeof(BaseResponse))]
        public HttpResponseMessage ZnodeApiJsForACH(PaymentCreditCardModel paymentCreditCardModel)
        {
            HttpResponseMessage response;
            try
            {
                if (!string.IsNullOrEmpty(paymentCreditCardModel.gateway))
                {
                    return GetACHScript(paymentCreditCardModel);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentCreditCardModel.paymentSettingId, ex.Message);
                PaymentGatewayResponse data = new PaymentGatewayResponse { HasError = true, ErrorMessage = ex.Message, GatewayResponse = new GatewayResponseModel() { GatewayResponseData = ex.Message } };
                response = CreateInternalServerErrorResponse(data);
            }
            return null;
        }

        #region Private Method
        //Enable Credit Card Types
        private string EnabledCreditCardTypes(PaymentSettingsModel paymentSetting)
        {
            string creditCardTypes = string.Empty;

            if (paymentSetting.EnableVisa.Value)
                creditCardTypes = string.Concat(creditCardTypes, ",", "VISA");

            if (paymentSetting.EnableMasterCard.Value)
                creditCardTypes = string.Concat(creditCardTypes, ",", "MASTERCARD");

            if (paymentSetting.EnableAmex.Value)
                creditCardTypes = string.Concat(creditCardTypes, ",", "AMEX");

            if (paymentSetting.EnableDiscover.Value)
                creditCardTypes = string.Concat(creditCardTypes, ",", "DISCOVER");

            if (creditCardTypes.StartsWith(","))
                creditCardTypes = creditCardTypes.Substring(1);

            else if (creditCardTypes.EndsWith(","))
                creditCardTypes = creditCardTypes.Substring(0, creditCardTypes.Length - 2);

            return creditCardTypes;
        }

        //to get credit card script
        private HttpResponseMessage GetCreditCardScript(PaymentCreditCardModel paymentCreditCardModel)
        {
            PaymentSettingsService repository = new PaymentSettingsService();
            PaymentSettingsModel paymentSetting = null;
            string savedCCDetails = string.Empty;
            string APIUrl = string.Empty;
            if (paymentCreditCardModel.paymentSettingId == 0 && !string.IsNullOrEmpty(paymentCreditCardModel.PaymentCode))
            {
                paymentSetting = repository.GetPaymentSettingWithCredentials(paymentCreditCardModel.PaymentCode, paymentCreditCardModel.gateway);
                APIUrl =  string.Equals(paymentCreditCardModel.gateway, GatewayType.CARDCONNECT.ToString(), StringComparison.InvariantCultureIgnoreCase) ? (paymentSetting.TestMode ? Convert.ToString(ConfigurationManager.AppSettings["CardConnectTestURL"]) : Convert.ToString(ConfigurationManager.AppSettings["CardConnectLiveURL"])) : string.Empty;
            }
            else
            {
                GatewayType gatewayType;
                Enum.TryParse(paymentCreditCardModel.gateway, out gatewayType);
                int gatewaytypeId = Convert.ToInt16(Enum.Parse(gatewayType.GetType(), paymentCreditCardModel.gateway.ToUpper()));
                paymentSetting = repository.GetPaymentSettingWithCredentials(gatewaytypeId, paymentCreditCardModel.profileId, paymentCreditCardModel.paymentSettingId);
            }

            if (!Equals(paymentSetting, null))
            {
                if (!string.IsNullOrEmpty(paymentCreditCardModel.customerGUID))
                {
                    PaymentMethodsService paymentMethodsRepository = new PaymentMethodsService();
                    List<PaymentMethodCCDetailsModel> paymentMethods = paymentMethodsRepository.GetPaymentMethods(paymentSetting.PaymentSettingId, paymentCreditCardModel.customerGUID);
                    if (paymentMethods.Count > 0)
                    {
                        savedCCDetails = JsonConvert.SerializeObject(paymentMethods, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    }
                }

                string enabledPaymentProviders = EnabledCreditCardTypes(paymentSetting);


                string script = @"var apiUrl = '" + Request.RequestUri.ToString().Replace(Request.RequestUri.PathAndQuery, "") + @"/payment/PayNow?callback=?" + @"';

                                       var paymentHeader = function(xhr) { xhr.setRequestHeader('Authorization', PaymentauthHeader); };
                                        var savedUserCCDetails = '" + savedCCDetails + @"';
                                        var enabledPaymentProviders = '" + enabledPaymentProviders + @"';
                                        var iframeUrl = '" + APIUrl + Convert.ToString(ConfigurationManager.AppSettings["CardConnectTokenizerURL"]) + @"';

                                        var postDataApi = function (pdata) {
                                        //pdata['PaymentApplicationSettingId'] = " + paymentSetting.PaymentSettingId + @";
                                        $.ajax({
                                                type: 'POST',
                                                beforeSend: paymentHeader,
                                                url: apiUrl,
                                                data: pdata,
                                                success: function(response) {
                                                successCalBack(response);
                                                }
                                            });

                                        };

                                        var errorCallback = function(data) {
                                            if (data.errorCode === 200)
                                            {
                                                tokenRequest();
                                            }
                                            else
                                            {
                                                var error_message = 'Error Code: ' + data.errorCode + '<br />Error Message: ' + data.errorMsg;
                                                successCalBack(error_message);
                                            }
                                        };

                                        var successCalBack;
                                        function submitCard(data, callBack)
                                        {
                                            successCalBack = callBack;
                                            postDataApi(data);
                                            return false;
                                        };";

                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(script);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                return response;
            }
            return null;
        }

        //to get ach script
        private HttpResponseMessage GetACHScript(PaymentCreditCardModel paymentCreditCardModel)
        {
            PaymentSettingsService repository = new PaymentSettingsService();
            PaymentSettingsModel paymentSetting = null;
            string APIUrl = string.Empty;
            string savedUserACHAccountDetails = string.Empty;
            if (paymentCreditCardModel.paymentSettingId == 0 && !string.IsNullOrEmpty(paymentCreditCardModel.PaymentCode))
            {
                paymentSetting = repository.GetPaymentSettingWithCredentials(paymentCreditCardModel.PaymentCode, paymentCreditCardModel.gateway);
                APIUrl = string.Equals(paymentCreditCardModel.gateway, GatewayType.CARDCONNECT.ToString(), StringComparison.InvariantCultureIgnoreCase) ? (paymentSetting.TestMode ? Convert.ToString(ConfigurationManager.AppSettings["CardConnectTestURL"]) : Convert.ToString(ConfigurationManager.AppSettings["CardConnectLiveURL"])) : string.Empty;
            }
            else
            {
                GatewayType gatewayType;
                Enum.TryParse(paymentCreditCardModel.gateway, out gatewayType);
                int gatewaytypeId = Convert.ToInt16(Enum.Parse(gatewayType.GetType(), paymentCreditCardModel.gateway.ToUpper()));
                paymentSetting = repository.GetPaymentSettingWithCredentials(gatewaytypeId, paymentCreditCardModel.profileId, paymentCreditCardModel.paymentSettingId);
            }

            if (!Equals(paymentSetting, null))
            {
                if (!string.IsNullOrEmpty(paymentCreditCardModel.customerGUID))
                {
                    PaymentMethodsService paymentMethodsRepository = new PaymentMethodsService();
                    List<PaymentMethodCCDetailsModel> paymentMethods = paymentMethodsRepository.GetPaymentMethodsACH(paymentSetting.PaymentSettingId, paymentCreditCardModel.customerGUID);
                    if (paymentMethods.Count > 0)
                    {
                        savedUserACHAccountDetails = JsonConvert.SerializeObject(paymentMethods, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    }
                }
                string script = @"var apiUrl = '" + Request.RequestUri.ToString().Replace(Request.RequestUri.PathAndQuery, "") + @"/payment/PayNow?callback=?" + @"';

                                       var paymentHeader = function(xhr) { xhr.setRequestHeader('Authorization', PaymentauthHeader); };
                                        var iframeUrl = '" + APIUrl + Convert.ToString(ConfigurationManager.AppSettings["CardConnectACHTokenizerURL"]) + @"';
                                        var savedUserACHAccountDetails = '" + savedUserACHAccountDetails + @"';

                                        var postDataApi = function (pdata) {
                                        //pdata['PaymentApplicationSettingId'] = " + paymentSetting.PaymentSettingId + @";
                                        $.ajax({
                                                type: 'POST',
                                                beforeSend: paymentHeader,
                                                url: apiUrl,
                                                data: pdata,
                                                success: function(response) {
                                                successCalBack(response);
                                                }
                                            });

                                        };

                                        var errorCallback = function(data) {
                                            if (data.errorCode === 200)
                                            {
                                                tokenRequest();
                                            }
                                            else
                                            {
                                                var error_message = 'Error Code: ' + data.errorCode + '<br />Error Message: ' + data.errorMsg;
                                                successCalBack(error_message);
                                            }
                                        };

                                        var successCalBack;
                                        function submitCard(data, callBack)
                                        {
                                            successCalBack = callBack;
                                            postDataApi(data);
                                            return false;
                                        };";

                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(script);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                return response;
            }
            return null;
        }

        //to get high radius script
        private HttpResponseMessage GetHighRadiusScript(PaymentCreditCardModel paymentCreditCardModel)
        {
            PaymentSettingsService repository = new PaymentSettingsService();
            PaymentSettingsModel paymentSetting = repository.GetPaymentSettingWithCredentials(paymentCreditCardModel.PaymentCode, paymentCreditCardModel.gateway);
            if (!Equals(paymentSetting, null))
            {
                HighRadiusTokenRequestModel diModel = new HighRadiusTokenRequestModel
                {
                    CLIENT_SYSTEM_ID = paymentSetting.GatewayPassword,
                    SECURITY_KEY = paymentSetting.TransactionKey,
                    MERCHANT_ID = paymentSetting.GatewayUsername,
                    REQUEST_ID = paymentSetting.Partner,
                    PROCESSOR = Convert.ToString(ConfigurationManager.AppSettings["PROCESSOR"]),
                    TRANSACTION_TYPE = "TOKENIZE",
                    REFERENCE_NUMBER = string.Empty,
                    DATA_LEVEL = Convert.ToString(ConfigurationManager.AppSettings["DATA_LEVEL"]),
                    POST_BACK_URL = string.Empty,
                    STRING_URL = GetHighRadiusUrl(paymentSetting.TestMode),
                    CALLING_APP = Convert.ToString(ConfigurationManager.AppSettings["EIPP"]),
                    CURRENCY_CODE = paymentCreditCardModel.CurrencyCode,
                    BILL_TO_AVSCHECK_REQUIRED = Convert.ToString(ConfigurationManager.AppSettings["BILL_TO_AVSCHECK_REQUIRED"]),
                    REQUESTOR = Convert.ToString(ConfigurationManager.AppSettings["TOKEN_REQUESTOR"]),
                    CALLING_APP_RESPONSE = string.Empty,
                    IS_CVV_MANDATORY = true
                };
                string script = new JavaScriptSerializer().Serialize(diModel);
                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(script);
                return response;
            }
            else
            {
                Logging.LogMessage($"The paymentsetting and credential  for the paymentSettingId {paymentCreditCardModel.paymentSettingId} not found.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            return null;
        }

        //to get high radius di url for test/live mode
        private string GetHighRadiusUrl(bool testMode)
        {
            string highRadiusUrl = testMode ? Convert.ToString(ConfigurationManager.AppSettings["HighRadiusTestURL"]) : Convert.ToString(ConfigurationManager.AppSettings["HighRadiusLiveURL"]);
            highRadiusUrl += highRadiusUrl.EndsWith("/") ? "PaymentsRadiusDI/displayLogin.do" : "/PaymentsRadiusDI/displayLogin.do";
            return highRadiusUrl;
        }
        #endregion
    }
}
