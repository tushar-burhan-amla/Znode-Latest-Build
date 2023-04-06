using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Payment.Client
{
    public interface IPaymentClient
    {
        /// <summary>
        /// Call PayNow method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        GatewayResponseModel PayNow(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Capture payment
        /// </summary>
        /// <param name="paymentTransactionToken">Payment Transaction Token</param>
        /// <returns>Boolean response</returns>
        BooleanModel CapturePayment(string paymentTransactionToken);

        /// <summary>
        /// Call PayPal method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        GatewayResponseModel PayPal(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Call PayPal method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit paypal Model</param>
        /// <returns>Response from Gateway API</returns>
        GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Delete expired payment auth token 
        /// </summary>
        /// <param></param>
        /// <returns>TrueFalseResponse model</returns>
        TrueFalseResponse DeletePaymentToken();
    }
}
