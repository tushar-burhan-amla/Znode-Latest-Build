using AmazonPay;
using AmazonPay.Responses;
using AmazonPay.StandardPaymentRequests;
using System;
using System.Collections.Generic;
using System.Configuration;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Providers.AuthorizeNetAPI;
using Newtonsoft.Json;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class AmazonPayProvider : BaseProvider, IPaymentProviders
    {
        #region Public Method
        //Method for refund payment .
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            // call to the API Refund
            Client client = new Client(MapAmazonPayCredentials());
            RefundRequest refundRequest = new RefundRequest();
            refundRequest.WithAmazonCaptureId(paymentModel.CardDataToken);
            refundRequest.WithAmount(Convert.ToDecimal(paymentModel.Total));
            refundRequest.WithCurrencyCode(Regions.currencyCode.USD);
            refundRequest.WithMerchantId(ConfigurationManager.AppSettings["MerchantId"].ToString());
            refundRequest.WithRefundReferenceId(paymentModel.RefundTransactionId.Replace('-', 'a') + "REF" + "0");
            refundRequest.WithSellerRefundNote("Refund");
            refundRequest.WithSoftDescriptor("Refund");

            //Call amazon refund method. 
            RefundResponse _refundResponse = client.Refund(refundRequest);

            GatewayResponseModel amazonResponse = new GatewayResponseModel();
            if (_refundResponse.GetSuccess())
            {
                amazonResponse.TransactionId = _refundResponse.GetAmazonRefundId();
                amazonResponse.IsSuccess = true;
                amazonResponse.ResponseCode = "0";
                amazonResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
            }
            else
            {
                amazonResponse.IsSuccess = false;
                amazonResponse.GatewayResponseData = _refundResponse.GetErrorMessage();
                amazonResponse.ResponseCode = _refundResponse.GetErrorCode();
            }
            return amazonResponse;
        }

        //Authorize and capture payment using amazon pay. 
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            //Set Configuration
            Client client = new Client(MapAmazonPayCredentials());

            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();

            //Call authorize AmazonPay.
            if (paymentModel.IsCapture)
            {
                CaptureResponse transactionResult = Capture(paymentModel, client, paymentModel.TransactionId);
                gatewayResponseModel.IsSuccess = transactionResult.GetSuccess();
                if (gatewayResponseModel.IsSuccess)
                {
                    gatewayResponseModel.TransactionId = paymentModel.TransactionId;
                    gatewayResponseModel.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                    gatewayResponseModel.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponseModel.Token = transactionResult.GetCaptureId();
                    paymentModel.ResponseText = transactionResult.GetReasonDescription();
                    paymentModel.CardDataToken = transactionResult.GetCaptureId();
                    paymentModel.CaptureTransactionId = transactionResult.GetCaptureId();
                    GetCaptureDetails(paymentModel, client, transactionResult);
                    TransactionService repository = new TransactionService();
                    repository.UpdatePayment(paymentModel);
                }
                else
                {
                    gatewayResponseModel.ResponseText = transactionResult.GetErrorMessage();
                    gatewayResponseModel.ResponseCode = transactionResult.GetErrorCode();
                }
                return gatewayResponseModel;
            }
            else
            {
                //Set Order reference details.
                SetOrderReferenceDetails(paymentModel, client);

                //Confirm Order reference details.
                ConfirmOrderReference(paymentModel, client);
                AuthorizeResponse transactionResult = Authorize(paymentModel, client);
                gatewayResponseModel.IsSuccess = transactionResult.GetSuccess();
                if (gatewayResponseModel.IsSuccess && transactionResult.GetAuthorizationState() == "Open")
                {
                    gatewayResponseModel.TransactionId = transactionResult.GetAuthorizationId();
                    gatewayResponseModel.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                    gatewayResponseModel.CustomerProfileId = paymentModel.CustomerProfileId;
                    gatewayResponseModel.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    gatewayResponseModel.ResponseText = transactionResult.GetReasonDescription();
                    gatewayResponseModel.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponseModel.Token = paymentModel.AmazonOrderReferenceId;
                    paymentModel.TransactionId = transactionResult.GetAuthorizationId();
                    paymentModel.ResponseText = transactionResult.GetReasonDescription();
                }
                else
                {
                    gatewayResponseModel.ResponseText = transactionResult.GetErrorMessage();
                    gatewayResponseModel.ResponseCode = transactionResult.GetErrorCode();
                }
                return gatewayResponseModel;
            }
        }

        //Cancel AmazonPay transaction.
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            // call to the API
            Client client = new Client(MapAmazonPayCredentials());
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            // Create the request object 
            CancelOrderReferenceRequest cancelOrderReferenceRequest = new CancelOrderReferenceRequest();
            cancelOrderReferenceRequest.WithAmazonOrderReferenceId(paymentModel.CardDataToken);
            cancelOrderReferenceRequest.WithCancelationReason("");
            cancelOrderReferenceRequest.WithMerchantId(ConfigurationManager.AppSettings["MerchantId"].ToString());

            CancelOrderReferenceResponse cancelOrderReferenceResponse = client.CancelOrderReference(cancelOrderReferenceRequest);
            if (cancelOrderReferenceResponse.GetSuccess())
            {
                gatewayResponseModel.TransactionId = cancelOrderReferenceResponse.GetRequestId();

                gatewayResponseModel.IsSuccess = true;
                gatewayResponseModel.ResponseCode = "0";
            }
            else
            {
                gatewayResponseModel.IsSuccess = false;
                gatewayResponseModel.GatewayResponseData = cancelOrderReferenceResponse.GetErrorMessage();
                gatewayResponseModel.ResponseCode = cancelOrderReferenceResponse.GetErrorCode();
            }

            return gatewayResponseModel;
        }

        //Get amazon address details.
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            //Set Configuration
            Client client = new Client(MapAmazonPayCredentials());

            if (!string.IsNullOrEmpty(paymentModel.Total) && Convert.ToDecimal(paymentModel.Total) > 0)
                SetOrderReferenceDetails(paymentModel, client);

            //Confirm Order reference details.
            ConfirmOrderReference(paymentModel, client);

            //Get order reference details.
            OrderReferenceDetailsResponse orderReferenceDetailsResponse = GetOrderReferenceDetails(paymentModel, client);

            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel() { PaymentModel = new PaymentModel() };
            gatewayResponseModel.IsSuccess = orderReferenceDetailsResponse.GetSuccess();
            if (gatewayResponseModel.IsSuccess)
            {
                gatewayResponseModel.PaymentModel.BillingCity = orderReferenceDetailsResponse.GetCity();
                gatewayResponseModel.PaymentModel.BillingCountryCode = orderReferenceDetailsResponse.GetCountryCode();
                gatewayResponseModel.PaymentModel.BillingStateCode = orderReferenceDetailsResponse.GetStateOrRegion();
                gatewayResponseModel.PaymentModel.BillingName = orderReferenceDetailsResponse.GetBuyerShippingName();
                gatewayResponseModel.PaymentModel.BillingPostalCode = orderReferenceDetailsResponse.GetPostalCode();
                gatewayResponseModel.PaymentModel.BillingStreetAddress1 = orderReferenceDetailsResponse.GetAddressLine1();
                gatewayResponseModel.PaymentModel.BillingStreetAddress2 = orderReferenceDetailsResponse.GetAddressLine2();
                gatewayResponseModel.PaymentModel.BillingEmailId = orderReferenceDetailsResponse.GetEmail();
                gatewayResponseModel.PaymentModel.BillingPhoneNumber = orderReferenceDetailsResponse.GetPhone();
                if (string.IsNullOrEmpty(gatewayResponseModel.PaymentModel.BillingEmailId) && !string.IsNullOrEmpty(paymentModel?.AccessToken))
                    gatewayResponseModel = SetUserDetails(gatewayResponseModel, paymentModel, client);

            }
            else
            {
                gatewayResponseModel.ResponseText = orderReferenceDetailsResponse.GetErrorMessage();
                gatewayResponseModel.ResponseCode = orderReferenceDetailsResponse.GetErrorCode();
            }
            return gatewayResponseModel;
        }

        //
        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            string transStatus = string.Empty;
            TransactionDetailsModel gatewayResponse = new TransactionDetailsModel();
            getTransactionDetailsRequest request = new getTransactionDetailsRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            request.transId = paymentModel.TransactionId;

            System.Xml.XmlDocument response_xml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out response_xml, paymentModel.GatewayTestMode);
            object response = null;
            getTransactionDetailsResponse api_response = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(response_xml, out response);
            if (!(response is getTransactionDetailsResponse))
            {
                var errorResponse = (ANetApiResponse)response;
                gatewayResponse.IsSuccess = false;
                if (!Equals(errorResponse, null))
                {
                    gatewayResponse.ResponseText = errorResponse.messages.message[0].text;
                }
                return gatewayResponse;
            }
            if (bResult)
                api_response = (getTransactionDetailsResponse)response;

            if (!Equals(api_response?.transaction?.transactionStatus, null))
            {
                gatewayResponse.TransactionStatus = api_response.transaction.transactionStatus;
                gatewayResponse.TransactionType = api_response.transaction.transactionType;
                gatewayResponse.TransactionId = api_response.transaction.transId;
                gatewayResponse.ResponseCode = api_response.transaction.responseReasonDescription;
                gatewayResponse.IsSuccess = true;
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = "Unable to get transaction details.";
            }

            return gatewayResponse;
        }

        //Get User details.
        public string GetUserInfo(PaymentModel paymentModel, Client client) => client.GetUserInfo(paymentModel.AccessToken);

        //Set User Details.
        public GatewayResponseModel SetUserDetails(GatewayResponseModel gatewayResponseModel, PaymentModel paymentModel, Client client)
        {
            //GetUserInfo
            var userInfo = GetUserInfo(paymentModel, client);

            PaymentGatewayUserDetailModel userInfoModel = JsonConvert.DeserializeObject<PaymentGatewayUserDetailModel>(userInfo);
            gatewayResponseModel.PaymentModel.BillingEmailId = userInfoModel.email;
            gatewayResponseModel.PaymentModel.BillingFirstName = userInfoModel.name;
            return gatewayResponseModel;
        }

        #endregion

        #region Private Method

        //Authorize amazon pay.
        private AuthorizeResponse Authorize(PaymentModel paymentModel, Client client)
        {
            AuthorizeRequest authRequest = new AuthorizeRequest();
            authRequest.WithAmazonOrderReferenceId(paymentModel.AmazonOrderReferenceId);
            authRequest.WithMerchantId(paymentModel.GatewayLoginName);
            authRequest.WithCurrencyCode((Regions.currencyCode)Enum.Parse(typeof(Regions.currencyCode),
                ConfigurationManager.AppSettings["CurrencyCode"].ToString()));
            authRequest.WithAmount(Convert.ToDecimal(paymentModel.Total));
            authRequest.WithCaptureNow(paymentModel.IsCapture);
            authRequest.WithTransactionTimeout(0);
            authRequest.WithAuthorizationReferenceId(paymentModel.AmazonOrderReferenceId.Replace('-', 'a') + "PAYRF" + "0");
            AuthorizeResponse transactionResult = client.Authorize(authRequest);
            return transactionResult;
        }

        //Capture payment.
        private CaptureResponse Capture(PaymentModel paymentModel, Client client, string authorizeResponse)
        {
            CaptureRequest captureRequest = new CaptureRequest();
            captureRequest.WithAmazonAuthorizationId(authorizeResponse);
            captureRequest.WithMerchantId(paymentModel.GatewayLoginName);
            captureRequest.WithCurrencyCode((Regions.currencyCode)Enum.Parse(typeof(Regions.currencyCode),
                ConfigurationManager.AppSettings["CurrencyCode"].ToString()));
            captureRequest.WithAmount(Convert.ToDecimal(paymentModel.Total));
            captureRequest.WithCaptureReferenceId(authorizeResponse.Replace('-', 'a') + "CRF" + "0");
            CaptureResponse transactionResult = client.Capture(captureRequest);
            return transactionResult;
        }

        //Get capture details.
        private CaptureResponse GetCaptureDetails(PaymentModel paymentModel, Client client, CaptureResponse captureResponse)
        {
            GetCaptureDetailsRequest captureDetailsRequest = new GetCaptureDetailsRequest();
            captureDetailsRequest.WithAmazonCaptureId(captureResponse.GetCaptureId());
            captureDetailsRequest.WithMerchantId(paymentModel.GatewayLoginName);
            return client.GetCaptureDetails(captureDetailsRequest);
        }

        //Get authorize details.
        private AuthorizeResponse GetAuthorizeDetails(PaymentModel paymentModel, Client client, AuthorizeResponse authorizeResponse)
        {
            GetAuthorizationDetailsRequest captureRequest = new GetAuthorizationDetailsRequest();
            captureRequest.WithAmazonAuthorizationId(authorizeResponse.GetAuthorizationId());
            captureRequest.WithMerchantId(paymentModel.GatewayLoginName);
            AuthorizeResponse authorizeDetailsResponse = client.GetAuthorizationDetails(captureRequest);
            return authorizeDetailsResponse;
        }

        //Get Order reference details.
        private OrderReferenceDetailsResponse GetOrderReferenceDetails(PaymentModel paymentModel, Client client)
        {
            //Call Order reference details.
            GetOrderReferenceDetailsRequest getOrderRequest = new GetOrderReferenceDetailsRequest();
            getOrderRequest.WithMerchantId(paymentModel.GatewayLoginName);
            getOrderRequest.WithAmazonOrderReferenceId(paymentModel.AmazonOrderReferenceId);
            return client.GetOrderReferenceDetails(getOrderRequest);
        }

        //Conform amazon order details.
        private void ConfirmOrderReference(PaymentModel paymentModel, Client client)
        {
            //Call ConfirmOrderReference
            ConfirmOrderReferenceRequest confirmOrderRequest = new ConfirmOrderReferenceRequest();
            confirmOrderRequest.WithMerchantId(paymentModel.GatewayLoginName);
            confirmOrderRequest.WithAmazonOrderReferenceId(paymentModel.AmazonOrderReferenceId);
            ConfirmOrderReferenceResponse confirmOrderReferenceResponse = client.ConfirmOrderReference(confirmOrderRequest);
        }

        //Set all order reference details.
        private void SetOrderReferenceDetails(PaymentModel paymentModel, Client client)
        {
            SetOrderReferenceDetailsRequest setOrderReferenceDetails = new SetOrderReferenceDetailsRequest();
            setOrderReferenceDetails.WithMerchantId(paymentModel.GatewayLoginName);
            setOrderReferenceDetails.WithAmazonOrderReferenceId(paymentModel.AmazonOrderReferenceId);
            setOrderReferenceDetails.WithCurrencyCode((Regions.currencyCode)Enum.Parse(typeof(Regions.currencyCode),
                ConfigurationManager.AppSettings["CurrencyCode"].ToString()));
            //Binds the order Id of payment model to Seller Order Id
            setOrderReferenceDetails.WithSellerOrderId(paymentModel.OrderId);
            setOrderReferenceDetails.WithAmount(Convert.ToDecimal(paymentModel.Total));
            OrderReferenceDetailsResponse orderReferenceDetailsResponse = client.SetOrderReferenceDetails(setOrderReferenceDetails);
        }

        //Map amazon pay Credentials.
        private AmazonPay.CommonRequests.Configuration MapAmazonPayCredentials()
        {
            AmazonPay.CommonRequests.Configuration clientConfig = new AmazonPay.CommonRequests.Configuration();
            clientConfig.WithMerchantId(ConfigurationManager.AppSettings["MerchantId"].ToString());
            clientConfig.WithAccessKey(ConfigurationManager.AppSettings["AccessKey"].ToString());
            clientConfig.WithSecretKey(ConfigurationManager.AppSettings["SecretKey"].ToString());
            clientConfig.WithCurrencyCode((Regions.currencyCode)Enum.Parse(typeof(Regions.currencyCode),
                ConfigurationManager.AppSettings["CurrencyCode"].ToString()));
            clientConfig.WithClientId(ConfigurationManager.AppSettings["ClientId"].ToString());
            clientConfig.WithRegion((Regions.supportedRegions)Enum.Parse(typeof(Regions.supportedRegions),
                ConfigurationManager.AppSettings["Region"].ToString()));
            clientConfig.WithSandbox(String.Equals(ConfigurationManager.AppSettings["Environment"].ToString(), "sand", StringComparison.OrdinalIgnoreCase)
                ? true : false);
            clientConfig.WithApplicationVersion(ConfigurationManager.AppSettings["ApplicationVersion"].ToString());
            clientConfig.WithProxyHost(ConfigurationManager.AppSettings["ProxyHost"].ToString());
            clientConfig.WithProxyUserPassword(ConfigurationManager.AppSettings["ProxyUserPassword"].ToString());

            return clientConfig;
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            //Not Implemented
            throw new NotImplementedException();
        }

        #endregion
    }
}
