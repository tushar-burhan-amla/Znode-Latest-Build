using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IPaymentHelper
    {
        /// <summary>
        /// Process Payment
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        GatewayResponseModel ProcessPayment(ConvertQuoteToOrderModel convertToOrderModel, ShoppingCartModel cartModel);

        bool IsPaypalExpressPayment(string paymentType);

        bool IsCreditCardPayment(string paymentType);

        bool IsAmazonPayPayment(string paymentType);

        bool CapturePayment(string paymentTransactionToken);

        /// <summary>
        /// Delete expired token from payment api.
        /// </summary>
        /// <param></param>
        /// <returns>Return bool</returns>
        bool DeletePaymentToken();

        /// <summary>
        /// Process Payment
        /// </summary>
        /// <param name="payInvoiceModel"></param>
        /// <param name="cartModel"></param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessPayment(PayInvoiceModel payInvoiceModel, ShoppingCartModel cartModel);

        /// <summary>
        /// Check ACH payment type.
        /// </summary>
        /// <param name="paymentType"></param>
        /// <returns>Boolean returned whether its ach payment or not.</returns>
        bool IsACHPayment(string paymentType);
    }
}
