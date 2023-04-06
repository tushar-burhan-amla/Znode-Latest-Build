using ZNode.Libraries.ECommerce.Utilities;

namespace ZNode.Libraries.ECommerce.Entities
{
    public class ZNodePaymentResponse 
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the Gateway Response code 
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the Gateway Response Text 
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets the Gateway redirect URL - which holds Google Post URL 
        /// </summary>
        public string RedirectURL { get; set; }

        /// <summary>
        /// Gets or sets the Gateway TransactionID 
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment response is success or failure.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        ///  Gets or sets the payment status
        /// </summary>
        public ZnodePaymentStatusEnum PaymentStatus { get; set; }

        /// <summary>
        /// Gets or sets the echo data.
        /// </summary>
        public string EchoData { get; set; }

        /// <summary>
        /// Gets or sets the issuer post data.
        /// </summary>
        public string IssuerPostData { get; set; }

        /// <summary>
        /// Gets or sets the WorldPay header cookie.
        /// </summary>
        public string WorldPayHeaderCookie { get; set; }

        /// <summary>
        /// Gets or sets the card authorization code or request token information from gateway
        /// </summary>
        public string CardAuthorizationCode { get; set; }

        /// <summary>
        /// Gets or sets the Subscription Id from the gateway response 
        /// </summary>
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the gateway response data.
        /// </summary>
        public string GatewayResponseData { get; set; }

        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the card type.
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the customer profile Id
        /// </summary>
        public string ProfileID { get; set; }

        /// <summary>
        /// Gets or sets the payment profile Id
        /// </summary>
        public string PaymentProfileID { get; set; }

        /// <summary>
        /// Gets or sets the shipping address Id
        /// </summary>
        public string ShippingAddressID { get; set; }
        #endregion
    }
}
