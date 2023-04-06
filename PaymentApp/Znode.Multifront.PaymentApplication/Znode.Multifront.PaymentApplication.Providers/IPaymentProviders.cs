using Znode.Multifront.PaymentFramework.Bussiness;
namespace Znode.Multifront.PaymentApplication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPaymentProviders 
    {
        PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentGatewayTokenModel);
        GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel);
        GatewayResponseModel Refund(PaymentModel paymentModel);
        GatewayResponseModel Void(PaymentModel paymentModel);
        GatewayResponseModel Subscription(PaymentModel paymentModel);
        TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel);
    }
}