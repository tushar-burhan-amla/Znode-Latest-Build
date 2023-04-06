namespace Znode.Engine.Payment.Client.Endpoints
{
    public class PaymentEndpoint : BaseEndpoint
    {
        // Process Payment operation
        public static string PayNow() => $"{PaymentApiRoot}/payment/paynow";

        //Capture Payment
        public static string CapturePayment(string token) => $"{PaymentApiRoot}/payment/capture/{token}";

        // Process Payment operation
        public static string PayPal() => $"{PaymentApiRoot}/payment/paypal";

        // Finalize paypal Payment operation
        public static string FinalizePayPalProcess() => $"{PaymentApiRoot}/payment/finalizepaypalprocess";

        #region Delete payment auth token in every one hour
        public static string DeletePaymentAuthToken() => $"{PaymentApiRoot}/authtoken/deleteexpiredpaymentaccesstoken";
        #endregion
    }
}
