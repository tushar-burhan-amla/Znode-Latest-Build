using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class PaymentClient : BaseClient, IPaymentClient
    {
        // Get all Payment Gateways
        public virtual PaymentGatewayListModel GetGateways()
        {
            string endpoint = PaymentEndpoint.GetGatewaysFromPaymentApplication();

            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = GetResourceFromEndpoint<PaymentGatewayResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentGatewayList;
        }

        // Get All Payment Types
        public virtual PaymentTypeListModel GetPaymentTypes()
        {
            string endpoint = PaymentEndpoint.GetPaymentTypesFromPaymentApplication();

            ApiStatus status = new ApiStatus();
            PaymentTypeResponse response = GetResourceFromEndpoint<PaymentTypeResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentTypes;
        }

        // Get Payment Setting
        public virtual PaymentSettingModel GetPaymentSetting(int paymentsettingId, bool isPaymentApplication = false, ExpandCollection expands = null, int portalId = 0)
        {
            string endpoint = isPaymentApplication ? PaymentEndpoint.GetFromPaymentApplication(paymentsettingId) : PaymentEndpoint.Get(paymentsettingId, portalId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = GetResourceFromEndpoint<PaymentSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentSetting;
        }

        //to get paymentsetting by paymentcode
        public virtual PaymentSettingModel GetPaymentSettingByPaymentCode(string paymentCode)
        {
            string endpoint = PaymentEndpoint.GetPaymentApplicationSettingByPaymentCode(paymentCode);

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = GetResourceFromEndpoint<PaymentSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentSetting;
        }

        // Get All Payment Settings
        public virtual PaymentSettingListModel GetPaymentSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PaymentEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PaymentSettingListResponse response = GetResourceFromEndpoint<PaymentSettingListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PaymentSettingListModel list = new PaymentSettingListModel { PaymentSettings = response?.PaymentSettings };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Create Payment setting
        public virtual PaymentSettingModel CreatePaymentSetting(PaymentSettingModel model, bool isPaymentApplication = false)
        {
            string endpoint = isPaymentApplication ? PaymentEndpoint.CreateOnPaymentApplication() : PaymentEndpoint.Create();

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = PostResourceToEndpoint<PaymentSettingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PaymentSetting;
        }

        // Update Payment setting
        public virtual PaymentSettingModel UpdatePaymentSetting(PaymentSettingModel model, bool isPaymentApplication = false)
        {
            string endpoint = isPaymentApplication ? PaymentEndpoint.UpdateOnPaymentApplication() : PaymentEndpoint.Update();

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = PutResourceToEndpoint<PaymentSettingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PaymentSetting;
        }

        //Delete payment setting from payment application
        public virtual bool DeletePaymentSetting(string paymentSettingIds, bool isPaymentApplication = false)
        {
            string endpoint = isPaymentApplication ? PaymentEndpoint.DeletePaymentSettingByPaymentCode() : PaymentEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = paymentSettingIds }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get Payment Setting Credentials
        public virtual PaymentSettingCredentialModel GetPaymentSettingCredentials(int paymentsettingId, bool isTestMode)
        {
            string endpoint = PaymentEndpoint.GetCredentialsFromPaymentApplication(paymentsettingId, isTestMode);

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = GetResourceFromEndpoint<PaymentSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentSettingCredentials;
        }

        //Check Whether Active payment seting present for given Profile and paymentType.
        public virtual bool IsActivePaymentSettingPresent(PaymentSettingModel paymentSettingModel)
        {
            string endpoint = PaymentEndpoint.IsActivePaymentSettingPresent();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(paymentSettingModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Check Whether Active payment seting present for given Profile and paymentType by paymentCode.
        public virtual bool IsActivePaymentSettingPresentByPaymentCode(PaymentSettingModel paymentSettingModel)
        {
            string endpoint = PaymentEndpoint.IsActivePaymentSettingPresentByPaymentCode();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(paymentSettingModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Call PayNow method in Payment Application
        public virtual GatewayResponseModel PayNow(SubmitPaymentModel submitPaymentModel)
        {
            string endpoint = PaymentEndpoint.PayNow();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(submitPaymentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        //Call PayPal method in Payment Application
        public virtual GatewayResponseModel PayPal(SubmitPaymentModel submitPaymentModel)
        {
            string endpoint = PaymentEndpoint.PayPal();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(submitPaymentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        //Call amazon pay in payment application
        public virtual GatewayResponseModel AmazonPay(SubmitPaymentModel model)
        {
            string endpoint = PaymentEndpoint.PayNow();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        //Call amazon pay in payment application.
        public virtual SubmitPaymentModel GetAmazonPayAddressDetails(SubmitPaymentModel model)
        {
            string endpoint = PaymentEndpoint.GetAmazonPayAddress();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PaymentModel;
        }

        //Capture Payment 
        public virtual BooleanModel CapturePayment(string paymentTransactionToken)
        {
            string endpoint = PaymentEndpoint.CapturePayment(paymentTransactionToken);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.booleanModel;
        }

        //Capture amazon payment.
        public virtual BooleanModel AmazonCapturePayment(string paymentTransactionToken)
        {
            string endpoint = PaymentEndpoint.AmazonCapturePayment(paymentTransactionToken);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.booleanModel;
        }

        //Void Payment
        public virtual BooleanModel VoidPayment(string paymentTransactionToken)
        {
            string endpoint = PaymentEndpoint.VoidPayment(paymentTransactionToken);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.booleanModel;
        }

        //AmazonPay void payment.
        public virtual BooleanModel AmazonVoidPayment(string paymentTransactionToken)
        {
            string endpoint = PaymentEndpoint.AmazonVoidPayment(paymentTransactionToken);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.booleanModel;
        }

        //Refund Payment
        public virtual BooleanModel RefundPayment(RefundPaymentModel model)
        {
            string endpoint = PaymentEndpoint.RefundPayment();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.booleanModel;
        }

        //Refund amazon pay.
        public virtual BooleanModel AmazonPayRefund(RefundPaymentModel model)
        {
            string endpoint = PaymentEndpoint.RefundPayment();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.booleanModel;
        }

        // Get saved credit card details.
        public virtual PaymentMethodCCDetailsListModel GetPaymentCreditCardDetails(int paymentSettingId, string customersGUID)
        {
            string endpoint = PaymentEndpoint.GetPaymentCreditCardDetails(paymentSettingId, customersGUID);

            ApiStatus status = new ApiStatus();
            PaymentMethodResponse response = GetResourceFromEndpoint<PaymentMethodResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PaymentMethodCCDetailsListModel list = new PaymentMethodCCDetailsListModel { PaymentMethodCCDetails = response?.PaymentMethodCCDetails };
            return list;
        }

        // Get saved credit card Count.
        public virtual int GetSaveCreditCardCount(int paymentSettingId, string customersGUID)
        {
            string endpoint = PaymentEndpoint.GetSaveCreditCardCount(paymentSettingId, customersGUID);

            ApiStatus status = new ApiStatus();
            CountResponse savedCreditCard = GetResourceFromEndpoint<CountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return savedCreditCard.Count;
        }

        // Get saved credit card Count.
        public virtual bool DeleteSavedCreditCardDetail(string paymentGUID)
        {
            string endpoint = PaymentEndpoint.DeleteSavedCreditCardDetail(paymentGUID);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse result = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return !result.HasError;
        }

        //Get payment transaction status details.
        public virtual TransactionDetailsModel GetTransactionStatusDetails(string transactionId)
        {
            string endpoint = PaymentEndpoint.GetTransactionStatusDetails(transactionId);

            ApiStatus status = new ApiStatus();
            TransactionDetailsResponse response = GetResourceFromEndpoint<TransactionDetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TransactionDetails;
        }

        //Call PayPal method in Payment Application
        public virtual GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel)
        {
            string endpoint = PaymentEndpoint.FinalizePayPalProcess();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(submitPaymentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        //Get captured payment details for sending it to ERP.
        public virtual bool GetCapturedPaymentDetails(int omsOrderId)
        {
            string endpoint = PaymentEndpoint.GetCapturedPaymentDetails(omsOrderId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Check whether to call payment API by paymentTypeCode.
        public virtual bool CallToPaymentAPI(string paymentTypeCode)
        {
            string endpoint = PaymentEndpoint.CallToPaymentAPI(paymentTypeCode);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Payment Setting Credentials by paymentCode
        public virtual PaymentSettingCredentialModel GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode)
        {
            string endpoint = PaymentEndpoint.GetCredentialsFromPaymentApplicationByPaymentCode(paymentCode, isTestMode);

            ApiStatus status = new ApiStatus();
            PaymentSettingResponse response = GetResourceFromEndpoint<PaymentSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentSettingCredentials;
        }

        // Get saved card details by customers GUID
        public virtual PaymentMethodCCDetailsListModel GetSavedCardDetailsByCustomerGUID(string customersGUID)
        {
            string endpoint = PaymentEndpoint.GetSavedCardDetailsByCustomerGUID(customersGUID);

            ApiStatus status = new ApiStatus();
            PaymentMethodResponse response = GetResourceFromEndpoint<PaymentMethodResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PaymentMethodCCDetailsListModel list = new PaymentMethodCCDetailsListModel { PaymentMethodCCDetails = response?.PaymentMethodCCDetails };
            return list;
        }
        #region Portal/Profile
        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettings(PaymentSettingAssociationModel model)
        {
            string endpoint = PaymentEndpoint.AssociatePaymentSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettingsForInvoice(PaymentSettingAssociationModel model)
        {
            string endpoint = PaymentEndpoint.AssociatePaymentSettingsForInvoice();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedPaymentSettings(PaymentSettingAssociationModel model)
        {
            string endpoint = PaymentEndpoint.RemoveAssociatedPaymentSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Update portal payment settings.
        public virtual bool UpdatePortalPaymentSettings(PaymentSettingPortalModel model)
        {
            string endpoint = PaymentEndpoint.UpdatePortalPaymentSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Update profile payment setting.
        public virtual bool UpdateProfilePaymentSetting(PaymentSettingAssociationModel model)
        {
            string endpoint = PaymentEndpoint.UpdateProfilePaymentSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }
        #endregion

        #region Get Payment AuthToken
        public virtual string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp = false)
        {
            string endpoint = PaymentEndpoint.GetPaymentAuthToken(HelperUtility.EncodeBase64(userOrSessionId), fromAdminApp);
            ApiStatus status = new ApiStatus();
            StringResponse responseToken = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return responseToken.Response;
        }
        #endregion

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="paymentSettingValidationModel"></param>
        /// <returns>Return True False Response</returns>
        public virtual bool IsPaymentDisplayNameExists(PaymentSettingValidationModel paymentSettingValidationModel)
        {
            string endpoint = PaymentEndpoint.IsPaymentDisplayNameExists();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(paymentSettingValidationModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.IsSuccess ?? false;
        }

        // Get all payment settings by portalId and userId using UserPaymentSettingModel.
        public virtual PaymentSettingListModel GetPaymentSettingByUserDetails(UserPaymentSettingModel userPaymentSettingModel)
        {
            string endpoint = PaymentEndpoint.GetPaymentSettingByUserDetails();

            ApiStatus status = new ApiStatus();
            PaymentSettingListResponse response = PostResourceToEndpoint<PaymentSettingListResponse>(endpoint, JsonConvert.SerializeObject(userPaymentSettingModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PaymentSettingListModel paymentSettingList = new PaymentSettingListModel { PaymentSettings = response?.PaymentSettings };
            paymentSettingList.MapPagingDataFromResponse(response);

            return paymentSettingList;
        }

        // Get all Payment Gateways for ACH.
        public virtual PaymentGatewayListModel GetGatewaysForACH()
        {
            string endpoint = PaymentEndpoint.GetGatewaysFromPaymentApplicationForACH();

            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = GetResourceFromEndpoint<PaymentGatewayResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentGatewayList;
        }

        public virtual PaymentGatewayTokenModel GetPaymentGatewayToken(PaymentGatewayTokenModel payment) 
        {
            string endpoint = PaymentEndpoint.GetPaymentGatewayToken();

            ApiStatus status = new ApiStatus();
            PaymentGatewayTokenResponse response = PostResourceToEndpoint<PaymentGatewayTokenResponse>(endpoint, JsonConvert.SerializeObject(payment), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PaymentTokenModel;
        }

        //Get the refund transaction id
        public string GetRefundTransactionId(string transactionId)
        {
            string endpoint = PaymentEndpoint.GetRefundTransactionId(transactionId);

            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint,status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.Response?.ToString();
        }
    }
}
