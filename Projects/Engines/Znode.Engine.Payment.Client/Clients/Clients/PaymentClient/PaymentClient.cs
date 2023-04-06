using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Payment.Client.Endpoints;
using System.Net;
using Znode.Engine.Exceptions;
using Newtonsoft.Json;

namespace Znode.Engine.Payment.Client
{
    public class PaymentClient : BaseClient, IPaymentClient
    {
        //Call PayNow method in Payment Application
        public virtual GatewayResponseModel PayNow(SubmitPaymentModel submitPaymentModel)
        {
            string endpoint = PaymentEndpoint.PayNow();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(submitPaymentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        //Capture Payment 
        public virtual BooleanModel CapturePayment(string paymentTransactionToken)
        {
            string endpoint = PaymentEndpoint.CapturePayment(paymentTransactionToken);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.booleanModel;
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

        //Call PayPal method in Payment Application
        public virtual GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel)
        {
            string endpoint = PaymentEndpoint.FinalizePayPalProcess();
            ApiStatus status = new ApiStatus();
            PaymentGatewayResponse response = PostResourceToEndpoint<PaymentGatewayResponse>(endpoint, JsonConvert.SerializeObject(submitPaymentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.GatewayResponse;
        }

        /// <summary>
        /// Delete expired payment auth token 
        /// </summary>
        /// <param></param>
        /// <returns>TrueFalseResponse</returns>
        public virtual TrueFalseResponse DeletePaymentToken()
        {
            string endpoint = PaymentEndpoint.DeletePaymentAuthToken();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response;
        }
    }
}
