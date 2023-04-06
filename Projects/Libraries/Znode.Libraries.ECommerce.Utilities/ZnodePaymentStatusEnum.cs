namespace ZNode.Libraries.ECommerce.Utilities
{
    public enum ZnodePaymentStatusEnum
    {
        /// <summary>
        /// Credit card Authorized
        /// </summary>
        CC_AUTHORIZED,

        /// <summary>
        /// Credir Card Captured.
        /// </summary>
        CC_CAPTURED,

        /// <summary>
        /// Card Card Declined
        /// </summary>
        CC_DECLINED,

        /// <summary>
        /// Card Card Refunded
        /// </summary>
        CC_REFUNDED,

        /// <summary>
        /// Credit Card Payment was Voided
        /// </summary>
        CC_VOIDED,

        /// <summary>
        /// Credit Card Payment Pending
        /// </summary>
        CC_PENDING,

        /// <summary>
        /// Purchase Order Pending
        /// </summary>
        PO_PENDING,

        /// <summary>
        /// Purchase Order Received
        /// </summary>
        PO_RECEIVED,

        /// <summary>
        /// COD Pending
        /// </summary>
        COD_PENDING,

        /// <summary>
        /// COD Received
        /// </summary>
        COD_RECEIVED,
    }
}
